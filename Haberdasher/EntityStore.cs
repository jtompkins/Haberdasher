using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Haberdasher.Contracts;
using Haberdasher.QueryGenerators;

namespace Haberdasher
{
	public class EntityStore<TEntity, TKey> : IEntityStore<TEntity, TKey> where TEntity : class, new()
	{
		protected static IDictionary<Type, CachedType> CachedTypes { get; set; }

		private readonly Type _entityType;

		private readonly IQueryGenerator _queryGenerator;

		private readonly string _connectionString;
		private readonly IDbConnection _connection;
		private readonly bool _useProvidedConnection;

		private CachedProperty Key {
			get { return CachedTypes[_entityType].Key; }
		}

		private IEnumerable<CachedProperty> SelectFields {
			get { return CachedTypes[_entityType].SelectFields; }
		}

		private IEnumerable<CachedProperty> InsertFields {
			get { return CachedTypes[_entityType].InsertFields; }
		}

		private IEnumerable<CachedProperty> UpdateFields {
			get { return CachedTypes[_entityType].UpdateFields; }
		}

		#region Constructors

		static EntityStore() {
			CachedTypes = new Dictionary<Type, CachedType>();
		}

		protected EntityStore(string name, string connectionString = null, IQueryGenerator queryGenerator = null)
			: this() {
			_queryGenerator = queryGenerator ?? new SqlServerGenerator(name);

			if (!String.IsNullOrEmpty(connectionString)) {
				_connectionString = ConfigurationManager.ConnectionStrings[connectionString].ConnectionString;
			}
			else if (ConfigurationManager.ConnectionStrings.Count > 0) {
				_connectionString = ConfigurationManager.ConnectionStrings[0].ConnectionString;
			}
			else {
				throw new ArgumentException("A connection string must be specified, or there must be at least one connection string set in your configuration file.");
			}
		}

		protected EntityStore(string name, IDbConnection connection, IQueryGenerator queryGenerator = null)
			: this() {
			if (connection == null)
				throw new ArgumentException("A valid IDbConnection must be given.");

			_connection = connection;
			_useProvidedConnection = true;
			_queryGenerator = queryGenerator ?? new SqlServerGenerator(name);
		}

		protected EntityStore() {
			_entityType = typeof(TEntity);

			if (!CachedTypes.ContainsKey(_entityType))
				CachedTypes.Add(_entityType, new CachedType(_entityType));
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

		public Task<IEnumerable<TEntity>> QueryAsync(string sql, object param = null, SqlTransaction transaction = null, 
			int? commandTimeout = null, CommandType? commandType = null) {
			var connection = GetConnection();

			try {
				return connection.QueryAsync<TEntity>(sql, param, transaction, commandTimeout, commandType);
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

		public Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, SqlTransaction transaction = null, 
			int? commandTimeout = null, CommandType? commandType = null) {
			var connection = GetConnection();

			try {
				return connection.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType);
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

		public IEnumerable<TEntity> Get() {
			var query = _queryGenerator.SelectAll(SelectFields, Key);

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

		public TEntity Get(TKey key) {
			var parameters = new DynamicParameters();

			parameters.Add("id", key);

			var connection = GetConnection();

			try {
				var keyParamName = _queryGenerator.FormatSqlParameter("id");
				var entity = connection.Query<TEntity>(_queryGenerator.Select(SelectFields, Key, keyParamName), parameters).FirstOrDefault();

				return entity;
			}
			finally {
				if (!_useProvidedConnection)
					connection.Dispose();
			}
		}

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
				entities = connection.Query<TEntity>(_queryGenerator.SelectMany(SelectFields, Key, keyParamName), parameters).ToList();
			}
			finally {
				if (!_useProvidedConnection)
					connection.Dispose();
			}

			if (entities.Any())
				results.AddRange(entities);

			return results;
		}

		public IEnumerable<TEntity> Find(string whereClause, object param = null) {
			var sql = _queryGenerator.FindOne(SelectFields, whereClause);

			IEnumerable<TEntity> entities;

			var connection = GetConnection();

			try {
				entities = connection.Query<TEntity>(sql, param);
			}
			finally {
				if (!_useProvidedConnection)
					connection.Dispose();
			}

			return entities ?? new List<TEntity>();
		}

		public TEntity FindOne(string whereClause, object param = null) {
			var sql = _queryGenerator.Find(SelectFields, whereClause);
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

		public TKey Insert(TEntity entity) {
			if (entity == null)
				throw new ArgumentException("Entity must not be null.");

			var properties = BuildPropertyList(InsertFields);
			var parameters = BuildParameterList(InsertFields, entity);

			decimal identity;

			var connection = GetConnection();

			try {
				var sql = _queryGenerator.Insert(properties, Key);
				identity = connection.Query<decimal>(sql, parameters).Single();
			}
			finally {
				if (!_useProvidedConnection)
					connection.Dispose();
			}

			return Key.IsIdentity ? (TKey)Convert.ChangeType(identity, typeof(TKey)) : (TKey)Key.Getter(entity);
		}

		public IEnumerable<TKey> Insert(IEnumerable<TEntity> entities) {
			if (entities == null || !entities.Any())
				throw new ArgumentException("Entities must not be null or an empty enumerable.");

			return entities.Select(Insert).ToList();
		}

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
				result = connection.Execute(_queryGenerator.Update(properties, Key, keyParamName), parameters);
			}
			finally {
				if (!_useProvidedConnection)
					connection.Dispose();
			}

			return result;
		}

		public int Update(IEnumerable<TEntity> entities) {
			if (entities == null || !entities.Any())
				throw new ArgumentException("Entities must not be null or an empty enumerable.");

			return entities.Sum(entity => Update(entity));
		}

		public void Delete() {
			throw new NotImplementedException();
		}

		public int Delete(TKey key) {
			var parameters = new DynamicParameters();

			parameters.Add(Key.Property, key);

			int result;

			var connection = GetConnection();

			try {
				var keyParamName = _queryGenerator.FormatSqlParameter(Key.Property);
				result = connection.Execute(_queryGenerator.Delete(Key, keyParamName), parameters);
			}
			finally {
				if (!_useProvidedConnection)
					connection.Dispose();
			}

			return result;
		}

		public int Delete(IEnumerable<TKey> keys) {
			if (keys == null || !keys.Any())
				throw new ArgumentException("Keys must not be null or an empty enumerable.");

			var parameters = new DynamicParameters();

			parameters.Add(Key.Property, keys);

			int result;

			var connection = GetConnection();

			try {
				var keyParamName = _queryGenerator.FormatSqlParameter(Key.Property);
				result = connection.Execute(_queryGenerator.DeleteMany(Key, keyParamName), parameters);
			}
			finally {
				if (!_useProvidedConnection)
					connection.Dispose();
			}

			return result;
		}

		#endregion

		#region Async CRUD Methods

		public Task<IEnumerable<TEntity>> GetAsync() {
			throw new NotImplementedException();
		}

		public Task<TEntity> GetAsync(TKey key) {
			throw new NotImplementedException();
		}

		public Task<IEnumerable<TEntity>> GetAsync(IEnumerable<TKey> keys) {
			throw new NotImplementedException();
		}

		public Task<IEnumerable<TEntity>> FindAsync(string whereClause, object param = null) {
			throw new NotImplementedException();
		}

		public Task<TEntity> FindOneAsync(string whereClause, object param = null) {
			throw new NotImplementedException();
		}

		public Task<TKey> InsertAsync(TEntity entity) {
			throw new NotImplementedException();
		}

		public Task<IEnumerable<TKey>> InsertAsync(IEnumerable<TEntity> entities) {
			throw new NotImplementedException();
		}

		public Task<int> UpdateAsync(TEntity entity) {
			throw new NotImplementedException();
		}

		public Task<int> UpdateAsync(IEnumerable<TEntity> entities) {
			throw new NotImplementedException();
		}

		public Task DeleteAsync() {
			throw new NotImplementedException();
		}

		public Task<int> DeleteAsync(TKey key) {
			throw new NotImplementedException();
		}

		public Task<int> DeleteAsync(IEnumerable<TKey> keys) {
			throw new NotImplementedException();
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

		public Dictionary<string, CachedProperty> BuildPropertyList(IEnumerable<CachedProperty> properties) {
			var propertyList = new Dictionary<string, CachedProperty>();

			foreach (var property in properties) {
				if (property == null || String.IsNullOrEmpty(property.Name))
					continue;

				var key = _queryGenerator.FormatSqlParameter(property.Name);

				if (!propertyList.ContainsKey(key))
					propertyList.Add(key, property);
			}

			return propertyList;
		}

		public DynamicParameters BuildParameterList(IEnumerable<CachedProperty> properties, TEntity entity) {
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