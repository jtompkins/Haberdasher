using System;
using System.Data;
using System.Diagnostics;
using Dapper;
using Haberdasher.Contrib.Oracle.SqlBuilders;
using Haberdasher.SqlBuilders;
using Oracle.ManagedDataAccess.Client;

namespace Haberdasher.Contrib.Oracle
{
    /// <summary>
    /// the OracleHaberdashery is an abstract base class to retrieve data from an Oracle table using the Oracle Managed Provider by default
    /// </summary>
    /// <typeparam table="TEntity">The type of the t entity.</typeparam>
    /// <typeparam table="TKey">The type of the t key.</typeparam>
    public abstract class OracleSqlTable<TEntity, TKey> : SqlTable<TEntity, TKey> where TEntity : class, new()
    {

        #region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlTable{TEntity,TKey}"/> class.
		/// </summary>
		/// <param table="connectionString">The connection string (not the configuration table).</param>
		/// <param table="sqlGenerator">The tailor.</param>
		/// <exception cref="System.ArgumentException">A connection string must be specified.</exception>
		protected OracleSqlTable(string connectionString, ISqlGenerator sqlGenerator) {
			_sqlGenerator = sqlGenerator;

			if (!String.IsNullOrEmpty(connectionString)) {
				_connectionString = connectionString;
			}
			else {
				throw new ArgumentException("A connection string must be specified.");
			}
		}

        protected OracleSqlTable(string name, string connectionStringConfigName = null)
            : base(name, connectionStringConfigName, new OracleGenerator(name))
        {

        }

        #endregion

        public OracleGenerator SqlGenerator
        {
            get { return _sqlGenerator as OracleGenerator; }
        }

        /// <summary>
        /// Gets or sets the table of the Oracle sequence to use when assigning the primary key 
        /// </summary>
        /// <value>The table of the sequence.</value>
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
        /// <param table="entity">The entity.</param>
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
        /// <param table="entity">The entity.</param>
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
            var paramNameForIdentity = _sqlGenerator.FormatSqlParameter(_key.Name);
            parameters.Add(name: paramNameForIdentity, dbType: DbType.Decimal, direction: ParameterDirection.Output);

            using (var connection = GetConnection())
            {
                string sql = this.SqlGenerator.InsertWithIdentity(properties, _key);
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
        /// <param table="entity">The entity.</param>
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
                string sql = _sqlGenerator.Insert(properties, _key);
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
        /// <param table="entity">The entity.</param>
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
                string sequenceSql = String.Format("select {0}.nextval from dual", this.SequenceName); // suppress CA2100 since sequence table is not from user
                Debug.WriteLine(String.Format("InsertWithSequence :: sql = {0}", sequenceSql));
                cmd.CommandText = sequenceSql;

                var value = cmd.ExecuteScalar();

                TKey newId = (TKey)Convert.ChangeType(value, typeof(TKey));
                _key.Setter(entity, newId);
            }
        }

    }
}
