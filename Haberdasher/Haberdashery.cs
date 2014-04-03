using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Haberdasher.Contracts;
using Haberdasher.SqlBuilders;

namespace Haberdasher
{
	public class Haberdashery<TEntity, TKey> : IHaberdashery<TEntity, TKey> where TEntity : class, new()
	{
		#region Static Properties

		protected static IDictionary<Type, CachedType> CachedTypes { get; set; }

		#endregion

		#region Properties

		private readonly Type _entityType;
		protected ISqlBuilder _sqlBuilder;
		protected string _connectionString;
		protected readonly IDbConnection _connection;
		protected readonly bool _useProvidedConnection;

		protected readonly IDictionary<TKey, TEntity> _entityCache;

		protected CachedProperty _key {
			get { return CachedTypes[_entityType].Key; }
		}

		protected IEnumerable<CachedProperty> _selectFields {
			get { return CachedTypes[_entityType].SelectFields; }
		}

		protected IEnumerable<CachedProperty> _insertFields {
			get { return CachedTypes[_entityType].InsertFields; }
		}

		protected IEnumerable<CachedProperty> _updateFields {
			get { return CachedTypes[_entityType].UpdateFields; }
		}

		#endregion

		#region Constructors

		static Haberdashery() {
			CachedTypes = new Dictionary<Type, CachedType>();
		}

		protected Haberdashery(string name, string connectionString = null, ISqlBuilder sqlBuilder = null)
			: this() {
			_sqlBuilder = sqlBuilder ?? new SqlServerSqlBuilder(name);

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

		protected Haberdashery(string name, IDbConnection connection, ISqlBuilder sqlBuilder = null)
			: this() {
			if (connection == null)
				throw new ArgumentException("A valid IDbConnection must be given.");

			_connection = connection;
			_useProvidedConnection = true;
			_sqlBuilder = sqlBuilder ?? new SqlServerSqlBuilder(name);
		}

		protected Haberdashery() {
			_entityType = typeof(TEntity);
			_entityCache = new Dictionary<TKey, TEntity>();

			if (!CachedTypes.ContainsKey(_entityType))
				CachedTypes.Add(_entityType, new CachedType(_entityType));
		}

		#endregion

		#region Dapper Wrappers

		public IEnumerable<TEntity> Query(string sql, object param = null, SqlTransaction transaction = null, bool buffered = true) {
			var connection = GetConnection();

			try {
				return connection.Query<TEntity>(sql, param, transaction, buffered);
			}
			finally {
				if (!_useProvidedConnection)
					connection.Dispose();
			}
		}

		public IEnumerable<T> Query<T>(string sql, object param = null, SqlTransaction transaction = null, bool buffered = true) {
			var connection = GetConnection();

			try {
				return connection.Query<T>(sql, param, transaction, buffered);
			}
			finally {
				if (!_useProvidedConnection)
					connection.Dispose();
			}
		}

		public int Execute(string sql, object param = null, SqlTransaction transaction = null) {
			var connection = GetConnection();

			try {
				return connection.Execute(sql, param, transaction);
			}
			finally {
				if (!_useProvidedConnection)
					connection.Dispose();
			}
		}

		#endregion

		#region CRUD Methods

		public virtual TEntity Get(TKey key) {
			var parameters = new DynamicParameters();

			parameters.Add("id", key);

			var connection = GetConnection();

			try {
				var keyParamName = _sqlBuilder.FormatSqlParamName("id");
				var entity = connection.Query<TEntity>(_sqlBuilder.Select(_selectFields, _key, keyParamName), parameters).FirstOrDefault();

				return entity;
			}
			finally {
				if (!_useProvidedConnection)
					connection.Dispose();
			}
		}

		public virtual IEnumerable<TEntity> Get(IEnumerable<TKey> keys) {
			if (keys == null || !keys.Any())
				throw new ArgumentException("Keys must not be null or an empty enumerable.");

			var results = new List<TEntity>();
			var parameters = new DynamicParameters();

			parameters.Add("keys", keys);

			IEnumerable<TEntity> entities;

			var connection = GetConnection();

			try {
				var keyParamName = _sqlBuilder.FormatSqlParamName("keys");
				entities = connection.Query<TEntity>(_sqlBuilder.SelectMany(_selectFields, _key, keyParamName), parameters).ToList();
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
			var sql = _sqlBuilder.Find(_selectFields, whereClause);

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

		public TEntity First(string whereClause, object param = null) {
			var entities = Find(whereClause, param);

			var entity = entities.FirstOrDefault();

			return entity;
		}

		public virtual IEnumerable<TEntity> All() {
			var query = _sqlBuilder.All(_selectFields, _key);

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

		public virtual TKey Insert(TEntity entity) {
			if (entity == null)
				throw new ArgumentException("Entity must not be null.");

			var properties = BuildPropertyList(_insertFields);
			var parameters = BuildParameterList(_insertFields, entity);

			decimal identity;

			var connection = GetConnection();

			try {
				var sql = _sqlBuilder.Insert(properties, _key);
				identity = connection.Query<decimal>(sql, parameters).Single();
			}
			finally {
				if (!_useProvidedConnection)
					connection.Dispose();
			}

			return _key.IsIdentity ? (TKey)Convert.ChangeType(identity, typeof(TKey)) : (TKey)_key.Getter(entity);
		}

		public virtual IEnumerable<TKey> Insert(IEnumerable<TEntity> entities) {
			if (entities == null || !entities.Any())
				throw new ArgumentException("Entities must not be null or an empty enumerable.");

			return entities.Select(Insert).ToList();
		}

		public virtual int Update(TEntity entity) {
			if (entity == null)
				throw new ArgumentException("Entity must not be null.");

			var properties = BuildPropertyList(_updateFields);
			var parameters = BuildParameterList(_updateFields, entity);

			parameters.Add(_key.Property, (TKey)_key.Getter(entity));

			var result = 0;

			if (properties.Count <= 0) return result;

			var connection = GetConnection();

			try {
				var keyParamName = _sqlBuilder.FormatSqlParamName(_key.Property);
				result = connection.Execute(_sqlBuilder.Update(properties, _key, keyParamName), parameters);
			}
			finally {
				if (!_useProvidedConnection)
					connection.Dispose();
			}

			return result;
		}

		public virtual int Update(IEnumerable<TEntity> entities) {
			if (entities == null || !entities.Any())
				throw new ArgumentException("Entities must not be null or an empty enumerable.");

			return entities.Sum(entity => Update(entity));
		}

		public virtual int Delete(TKey key) {
			var parameters = new DynamicParameters();

			parameters.Add(_key.Property, key);

			int result;

			var connection = GetConnection();

			try {
				var keyParamName = _sqlBuilder.FormatSqlParamName(_key.Property);
				result = connection.Execute(_sqlBuilder.Delete(_key, keyParamName), parameters);
			}
			finally {
				if (!_useProvidedConnection)
					connection.Dispose();
			}

			return result;
		}

		public virtual int Delete(IEnumerable<TKey> keys) {
			if (keys == null || !keys.Any())
				throw new ArgumentException("Keys must not be null or an empty enumerable.");

			var parameters = new DynamicParameters();

			parameters.Add(_key.Property, keys);

			int result;

			var connection = GetConnection();

			try {
				var keyParamName = _sqlBuilder.FormatSqlParamName(_key.Property);
				result = connection.Execute(_sqlBuilder.DeleteMany(_key, keyParamName), parameters);
			}
			finally {
				if (!_useProvidedConnection)
					connection.Dispose();
			}

			return result;
		}

		#endregion

		#region Utilities
		protected virtual IDbConnection GetConnection() {
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

				var key = _sqlBuilder.FormatSqlParamName(property.Name);

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