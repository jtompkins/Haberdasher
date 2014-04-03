using System;
using System.Data;
using System.Diagnostics;
using Dapper;
using Haberdasher.Contrib.Oracle.Tailors;
using Haberdasher.SqlBuilders;
using Oracle.ManagedDataAccess.Client;

namespace Haberdasher.Contrib.Oracle
{
    /// <summary>
    /// the OracleHaberdashery is an abstract base class to retrieve data from an Oracle table using the Oracle Managed Provider by default
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TKey">The type of the t key.</typeparam>
    public abstract class OracleHaberdashery<TEntity, TKey> : Haberdashery<TEntity, TKey> where TEntity : class, new()
    {

        #region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Haberdashery{TEntity, TKey}"/> class.
		/// </summary>
		/// <param name="connectionString">The connection string (not the configuration name).</param>
		/// <param name="sqlBuilder">The tailor.</param>
		/// <exception cref="System.ArgumentException">A connection string must be specified.</exception>
		protected OracleHaberdashery(string connectionString, ISqlBuilder sqlBuilder) {
			_sqlBuilder = sqlBuilder;

			if (!String.IsNullOrEmpty(connectionString)) {
				_connectionString = connectionString;
			}
			else {
				throw new ArgumentException("A connection string must be specified.");
			}
		}

        protected OracleHaberdashery(string name, string connectionStringConfigName = null)
            : base(name, connectionStringConfigName, new OracleSqlBuilder(name))
        {

        }

        #endregion

        public OracleSqlBuilder SqlBuilder
        {
            get { return _sqlBuilder as OracleSqlBuilder; }
        }

        /// <summary>
        /// Gets or sets the name of the Oracle sequence to use when assigning the primary key 
        /// </summary>
        /// <value>The name of the sequence.</value>
        /// <remarks>used only when identity is set to false [Key(false)]</remarks>
        public string SequenceName { get; set; }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <returns>IDbConnection.</returns>
        protected override IDbConnection GetConnection()
        {
            // don't use the connection passed in through constructor until issue is fixed with connection being closed

            // always return an Oracle connection using the connectionstring from the constructor
            return new OracleConnection(_connectionString);
        }


        /// <summary>
        /// Inserts the specified entity into the Oracle DB using the configured method to determine the primary key.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>`1.</returns>
        /// <exception cref="System.ArgumentException">Entity must not be null.</exception>
        public override TKey Insert(TEntity entity)
        {
            TKey newId;

            if (entity == null)
                throw new ArgumentException("Entity must not be null.");

            if (this._key.IsIdentity)
            {
                newId = InsertWithIdentityKey(entity);
            }
            else
            {
                // the primary key value will not be set in DB trigger
                // either set it from a configured sequence
                // or set it manually before calling insert
                bool hasSequence = !string.IsNullOrEmpty(SequenceName);
                if (hasSequence)
                {
                    SetIdFromSequence(entity);
                }

                newId = InsertWithNoIdentityKey(entity);
            }

            return newId;
        }

        /// <summary>
        /// Inserts a new record using the primary key value set in the database
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>TKey.</returns>
        protected TKey InsertWithIdentityKey(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentException("Entity must not be null.");

            if (!this._key.IsIdentity)
            {
                throw new InvalidOperationException("This entity has no primary key set from an identity. Use the InsertWithNoIdentityKey instead.");
            }

            var properties = BuildPropertyList(_insertFields);
            var parameters = BuildParameterList(_insertFields, entity);

            // add an output parameter for the id
            var paramNameForIdentity = _sqlBuilder.FormatSqlParamName(_key.Name);
            parameters.Add(name: paramNameForIdentity, dbType: DbType.Decimal, direction: ParameterDirection.Output);

            using (var connection = GetConnection())
            {
                string sql = this.SqlBuilder.InsertWithIdentity(properties, _key);
                Debug.WriteLine(String.Format("InsertWithIdentityKey :: sql = {0}", sql));

                connection.Execute(sql, parameters);

                // read new id from output parameter
                decimal newIdNum = parameters.Get<decimal>(paramNameForIdentity);
                TKey newId = (TKey)Convert.ChangeType(newIdNum, typeof(TKey));
                return newId;
            }
        }

        /// <summary>
        /// Inserts a new record using a value supplied by the entity for the primary key value
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>TKey.</returns>
        /// <remarks>be sure the property is marked with Key(false)</remarks>
        protected TKey InsertWithNoIdentityKey(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentException("Entity must not be null.");

            var properties = BuildPropertyList(_insertFields);
            var parameters = BuildParameterList(_insertFields, entity);

            using (var connection = GetConnection())
            {
                string sql = _sqlBuilder.Insert(properties, _key);
                Debug.WriteLine(String.Format("InsertWithNoIdentityKey :: sql = {0}", sql));
                connection.Execute(sql, parameters);

                // since was already set in the entity, return the new id from the entity itself
                TKey newId = (TKey)_key.Getter(entity);
                return newId;
            }
        }

        /// <summary>
        /// Sets the identifier value from the sequence.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <exception cref="System.ArgumentException">Entity must not be null.</exception>
        /// <exception cref="System.InvalidOperationException">the SequenceName must be set to use InsertWithSequence</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        protected void SetIdFromSequence(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentException("Entity must not be null.");

            if (String.IsNullOrEmpty(SequenceName))
            {
                throw new InvalidOperationException("the SequenceName must be set to use InsertWithSequence");
            }

            using (var connection = GetConnection())
            {
                connection.Open();

                // lookup the id from the sequence first
                var cmd = connection.CreateCommand();
                string sequenceSql = String.Format("select {0}.nextval from dual", this.SequenceName); // suppress CA2100 since sequence name is not from user
                Debug.WriteLine(String.Format("InsertWithSequence :: sql = {0}", sequenceSql));
                cmd.CommandText = sequenceSql;

                var value = cmd.ExecuteScalar();

                TKey newId = (TKey)Convert.ChangeType(value, typeof(TKey));
                _key.Setter(entity, newId);
            }
        }

    }
}
