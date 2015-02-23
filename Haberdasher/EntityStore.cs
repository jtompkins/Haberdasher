using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using Dapper;

using Haberdasher.Contracts;
using Haberdasher.QueryGenerators;
using Haberdasher.Support.Helpers;

namespace Haberdasher
{
	public class EntityStore<TEntity, TKey> : IEntityStore<TEntity, TKey> where TEntity : class, new()
	{
		private readonly IQueryGenerator _queryGenerator;

		private readonly string _connectionString;
		private readonly IDbConnection _connection;
		private readonly bool _useProvidedConnection;

		private string Table {
			get { return EntityTypes.Get<TEntity>().Table; }
		}

		private EntityProperty Key {
			get { return EntityTypes.Get<TEntity>().KeyField; }
		}

		private IEnumerable<EntityProperty> SelectFields {
			get { return EntityTypes.Get<TEntity>().SelectFields; }
		}

		private IEnumerable<EntityProperty> InsertFields {
			get { return EntityTypes.Get<TEntity>().InsertFields; }
		}

		private IEnumerable<EntityProperty> UpdateFields {
			get { return EntityTypes.Get<TEntity>().UpdateFields; }
		}

		#region Constructors

		public EntityStore() {
			EntityTypes.Register<TEntity>();

			_connectionString = ConnectionStringHelper.FindFirst();
			_queryGenerator = new SqlServerGenerator();
		}

		public EntityStore(IQueryGenerator generator) : this() {
			_queryGenerator = generator;
		} 

		public EntityStore(string connectionString) : this() {
			_connectionString = ConnectionStringHelper.FindByName(connectionString);

			if (_connectionString == null)
				throw new ArgumentException("A connection string must be specified, or there must be at least one connection string set in your configuration file.");
		}

		public EntityStore(IDbConnection connection)
			: this() {
			_connection = connection;
			_useProvidedConnection = true;
		}

		public EntityStore(string connectionString, IQueryGenerator generator) : this() {
			_connectionString = ConnectionStringHelper.FindByName(connectionString);

			if (_connectionString == null)
				throw new ArgumentException("A connection string must be specified, or there must be at least one connection string set in your configuration file.");

			if (generator != null)
				_queryGenerator = generator;
		}

		
		public EntityStore(IDbConnection connection, IQueryGenerator generator) : this() {
			_connection = connection;
			_useProvidedConnection = true;

			if (generator != null)
				_queryGenerator = generator;
		}

		#endregion

		#region Dapper Wrappers

		public IEnumerable<TEntity> Query(string sql, object param = null, SqlTransaction transaction = null, bool buffered = true, 
			int? commandTimeout = null, CommandType? commandType = null) {
			var connection = GetConnection();

			try {
				return connection.Query<TEntity>(sql, param, transaction, buffered, commandTimeout, commandType);
			}
			finally {
				if (!_useProvidedConnection)
					connection.Dispose();
			}
		}

		public IEnumerable<T> Query<T>(string sql, object param = null, SqlTransaction transaction = null, bool buffered = true, 
			int? commandTimeout = null, CommandType? commandType = null) {
			var connection = GetConnection();

			try {
				return connection.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType);
			}
			finally {
				if (!_useProvidedConnection)
					connection.Dispose();
			}
		}

		public int Execute(string sql, object param = null, SqlTransaction transaction = null, int? commandTimeout = null,
			CommandType? commandType = null) {
			var connection = GetConnection();

			try {
				return connection.Execute(sql, param, transaction, commandTimeout, commandType);
			}
			finally {
				if (!_useProvidedConnection)
					connection.Dispose();
			}
		}

		#endregion

		#region CRUD Methods

		/// <summary>
		/// Gets all entities from the database.
		/// </summary>
		public IEnumerable<TEntity> Get() {
			var query = _queryGenerator.SelectAll(Table, SelectFields, Key);

			IEnumerable<TEntity> result;

			var connection = GetConnection();

			try {
				result = connection.Query<TEntity>(query);
			}
			finally {
				if (!_useProvidedConnection)
					connection.Dispose();
			}

			return result;
		}

		/// <summary>
		/// Gets an entity from the database by key.
		/// </summary>
		/// <param name="key">The primary key of the entity</param>
		public TEntity Get(TKey key) {
			var parameters = new DynamicParameters();

			parameters.Add("id", key);

			var connection = GetConnection();

			try {
				var keyParamName = _queryGenerator.FormatSqlParameter("id");
				var entity = connection.Query<TEntity>(_queryGenerator.Select(Table, SelectFields, Key, keyParamName), parameters).FirstOrDefault();

				return entity;
			}
			finally {
				if (!_useProvidedConnection)
					connection.Dispose();
			}
		}

		/// <summary>
		/// Gets one or more entities from the database by key.
		/// </summary>
		/// <param name="keys">An enumerable of key values</param>
		public IEnumerable<TEntity> Get(IEnumerable<TKey> keys) {
			if (keys == null || !keys.Any())
				throw new ArgumentException("Keys must not be null or an empty enumerable.");

			var results = new List<TEntity>();
			var parameters = new DynamicParameters();

			parameters.Add("keys", keys);

			IEnumerable<TEntity> entities;

			var connection = GetConnection();

			try {
				var keyParamName = _queryGenerator.FormatSqlParameter("keys");
				entities = connection.Query<TEntity>(_queryGenerator.SelectMany(Table, SelectFields, Key, keyParamName), parameters).ToList();
			}
			finally {
				if (!_useProvidedConnection)
					connection.Dispose();
			}

			if (entities.Any())
				results.AddRange(entities);

			return results;
		}

		/// <summary>
		/// Gets entities from the database, filtered using a WHERE clause.
		/// </summary>
		/// <param name="whereClause">A string containing the WHERE clause's predicate</param>
		/// <param name="param">Parameters to be passed to the WHERE clause</param>
		public IEnumerable<TEntity> Find(string whereClause, object param = null) {
			var sql = _queryGenerator.Find(Table, SelectFields, whereClause);
			var connection = GetConnection();

			IEnumerable<TEntity> entities;

			try {
				entities = connection.Query<TEntity>(sql, param);
			}
			finally {
				if (!_useProvidedConnection)
					connection.Dispose();
			}

			return entities ?? new List<TEntity>();
		}

		/// <summary>
		/// Gets a single entity from the database, filtered using a WHERE clause.
		/// </summary>
		/// <param name="whereClause">A string containing the WHERE clause's predicate</param>
		/// <param name="param">Parameters to be passed to the WHERE clause</param>
		public TEntity FindOne(string whereClause, object param = null) {
			var sql = _queryGenerator.FindOne(Table, SelectFields, whereClause);
			var connection = GetConnection();

			try {
				var entity = connection.Query<TEntity>(sql, param).FirstOrDefault();

				return entity;
			}
			finally {
				if (!_useProvidedConnection)
					connection.Dispose();
			}	
		}

		/// <summary>
		/// Inserts an entity into the database.
		/// </summary>
		/// <param name="entity">The entity to be inserted</param>
		/// <returns>The number of rows inserted</returns>
		public int Insert(TEntity entity) {
			if (entity == null)
				throw new ArgumentException("Entity must not be null.");

			var properties = BuildPropertyList(InsertFields);
			var parameters = BuildParameterList(InsertFields, entity);

			var connection = GetConnection();
			int result;

			try {
				var sql = _queryGenerator.Insert(Table, properties, Key);
				
				result = connection.Execute(sql, parameters);
			}
			finally {
				if (!_useProvidedConnection)
					connection.Dispose();
			}

			return result;
		}

		/// <summary>
		/// Inserts an entity into the database.
		/// </summary>
		/// <param name="entity">The entity to be inserted</param>
		/// <returns>The primary key of the inserted entity</returns>
		public TKey InsertWithIdentity(TEntity entity) {
			if (entity == null)
				throw new ArgumentException("Entity must not be null.");

			var properties = BuildPropertyList(InsertFields);
			var parameters = BuildParameterList(InsertFields, entity);

			var identity = default(TKey);
			var connection = GetConnection();

			try {
				var sql = _queryGenerator.Insert(Table, properties, Key);

				if (Key.IsIdentity)
					identity = connection.Query<TKey>(sql, parameters).Single();
				else
					connection.Execute(sql, parameters);
			}
			finally {
				if (!_useProvidedConnection)
					connection.Dispose();
			}

			return Key.IsIdentity ? identity : (TKey)Key.Getter(entity);
		}

		/// <summary>
		/// Inserts multiple entities into the database.
		/// </summary>
		/// <param name="entities">The entities to be inserted</param>
		/// <returns>The primary key of the inserted entity</returns>
		public int Insert(IEnumerable<TEntity> entities) {
			var properties = BuildPropertyList(InsertFields);

			var connection = GetConnection();
			var records = 0;

			try {
				foreach (var entity in entities) {
					var parameters = BuildParameterList(InsertFields, entity);
					var sql = _queryGenerator.Insert(Table, properties, Key);

					records += connection.Execute(sql, parameters);
				}
			}
			finally {
				if (!_useProvidedConnection)
					connection.Dispose();
			}

			return records;
		}

		/// <summary>
		/// Updates an entity in the database.
		/// </summary>
		/// <param name="entity">The entity to be updated</param>
		/// <returns>The number of updated records</returns>
		public int Update(TEntity entity) {
			if (entity == null)
				throw new ArgumentException("Entity must not be null.");

			var properties = BuildPropertyList(UpdateFields);
			var parameters = BuildParameterList(UpdateFields, entity);

			parameters.Add(Key.Property, (TKey)Key.Getter(entity));

			var result = 0;

			if (properties.Count <= 0) return result;

			var connection = GetConnection();

			try {
				var keyParamName = _queryGenerator.FormatSqlParameter(Key.Property);
				result = connection.Execute(_queryGenerator.Update(Table, properties, Key, keyParamName), parameters);
			}
			finally {
				if (!_useProvidedConnection)
					connection.Dispose();
			}

			return result;
		}

		/// <summary>
		/// Updates multiple entities in the database.
		/// </summary>
		/// <param name="entities">The entities to be updated</param>
		/// <returns>The number of updated records</returns>
		public int Update(IEnumerable<TEntity> entities) {
			var properties = BuildPropertyList(UpdateFields);
			var result = 0;

			if (properties.Count <= 0) return result;

			var connection = GetConnection();

			try {
				foreach (var entity in entities) {
					var parameters = BuildParameterList(UpdateFields, entity);

					parameters.Add(Key.Property, (TKey)Key.Getter(entity));

					var keyParamName = _queryGenerator.FormatSqlParameter(Key.Property);

					result += connection.Execute(_queryGenerator.Update(Table, properties, Key, keyParamName), parameters);
				}
			}
			finally {
				if (!_useProvidedConnection)
					connection.Dispose();
			}

			return result;
		}

		/// <summary>
		/// Deletes all rows in the database table.
		/// </summary>
		public int Delete() {
			var query = _queryGenerator.DeleteAll(Table);

			int result;

			var connection = GetConnection();

			try {
				result = connection.Execute(query);
			}
			finally {
				if (!_useProvidedConnection)
					connection.Dispose();
			}

			return result;
		}

		/// <summary>
		/// Deletes a row in the database table by key.
		/// </summary>
		/// <param name="key">The key of the row to be deleted</param>
		/// <returns>The number of deleted rows</returns>
		public int Delete(TKey key) {
			var parameters = new DynamicParameters();

			parameters.Add(Key.Property, key);

			int result;

			var connection = GetConnection();

			try {
				var keyParamName = _queryGenerator.FormatSqlParameter(Key.Property);
				result = connection.Execute(_queryGenerator.Delete(Table, Key, keyParamName), parameters);
			}
			finally {
				if (!_useProvidedConnection)
					connection.Dispose();
			}

			return result;
		}

		/// <summary>
		/// Deletes multiple rows in the database.
		/// </summary>
		/// <param name="keys">The keys of the rows to be deleted</param>
		/// <returns>The number of deleted rows</returns>
		public int Delete(IEnumerable<TKey> keys) {
			if (keys == null || !keys.Any())
				throw new ArgumentException("Keys must not be null or an empty enumerable.");

			var parameters = new DynamicParameters();

			parameters.Add(Key.Property, keys);

			int result;

			var connection = GetConnection();

			try {
				var keyParamName = _queryGenerator.FormatSqlParameter(Key.Property);
				result = connection.Execute(_queryGenerator.DeleteMany(Table, Key, keyParamName), parameters);
			}
			finally {
				if (!_useProvidedConnection)
					connection.Dispose();
			}

			return result;
		}

		#endregion

		#region Utilities

		protected IDbConnection GetConnection() {
			if (_useProvidedConnection && _connection != null)
				return _connection;

			if (String.IsNullOrEmpty(_connectionString))
				throw new ArgumentException("No connection string defined.");

			return new SqlConnection(_connectionString);
		}

		public Dictionary<string, EntityProperty> BuildPropertyList(IEnumerable<EntityProperty> properties) {
			var propertyList = new Dictionary<string, EntityProperty>();

			foreach (var property in properties) {
				if (property == null || String.IsNullOrEmpty(property.Name))
					continue;

				var key = _queryGenerator.FormatSqlParameter(property.Name);

				if (!propertyList.ContainsKey(key))
					propertyList.Add(key, property);
			}

			return propertyList;
		}

		public DynamicParameters BuildParameterList(IEnumerable<EntityProperty> properties, TEntity entity) {
			var parameterList = new DynamicParameters();

			foreach (var property in properties) {
				if (property == null || String.IsNullOrEmpty(property.Name) || property.Getter == null)
					continue;

				parameterList.Add(property.Name, property.Getter(entity));
			}

			return parameterList;
		}

		#endregion
	}
}