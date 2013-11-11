using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Haberdasher.Contracts;
using Haberdasher.Tailors;

namespace Haberdasher
{
	public class Haberdashery<TEntity, TKey> : IHaberdashery<TEntity, TKey> where TEntity : class, new()
	{
		#region Static Properties

		protected static IDictionary<Type, CachedType> CachedTypes { get; set; }

		#endregion

		#region Properties

		private readonly Type _entityType;
		private readonly ITailor _tailor;
		private readonly string _connectionString;

		protected readonly IDictionary<TKey, TEntity> _entityCache;

		private IDbConnection _connection {
			get { return new SqlConnection(_connectionString); }
		}

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

		protected Haberdashery(string name, string connectionString = null, ITailor tailor = null)
			: this() {
			_tailor = tailor ?? new SqlServerTailor(name);

			if (!String.IsNullOrEmpty(connectionString)) {
				_connectionString = ConfigurationManager.ConnectionStrings[connectionString].ConnectionString;
			}
			else if (ConfigurationManager.ConnectionStrings.Count > 0) {
				_connectionString = ConfigurationManager.ConnectionStrings[0].ConnectionString;
			}
			else {
				throw new Exception("A connection string must be specified, or there must be at least one connection string set in your configuration file.");
			}
		}

		private Haberdashery() {
			_entityType = typeof(TEntity);
			_entityCache = new Dictionary<TKey, TEntity>();

			if (!CachedTypes.ContainsKey(_entityType))
				CachedTypes.Add(_entityType, new CachedType(_entityType));

			if (_key == null)
				throw new MissingPrimaryKeyException("Entity type does not define a primary key.");
		}

		#endregion

		#region Dapper Wrappers

		public IEnumerable<TEntity> Query(string sql, object param = null, SqlTransaction transaction = null, bool buffered = true) {
			using (var connection = _connection) {
				return connection.Query<TEntity>(sql, param, transaction, buffered);
			}
		}

		public IEnumerable<T> Query<T>(string sql, object param = null, SqlTransaction transaction = null, bool buffered = true) {
			using (var connection = _connection) {
				return connection.Query<T>(sql, param, transaction, buffered);
			}
		}

		public int Execute(string sql, object param = null, SqlTransaction transaction = null) {
			using (var connection = _connection) {
				return connection.Execute(sql, param, transaction);
			}
		}

		#endregion

		#region CRUD Methods

		public virtual TEntity Get(TKey key) {
			var parameters = new DynamicParameters();

			parameters.Add("id", key);

			TEntity entity;

			using (var connection = _connection) {
				entity = connection.Query<TEntity>(_tailor.Select(_selectFields, _key, "@id"), parameters).FirstOrDefault();
			}

			return entity;
		}

		public virtual IEnumerable<TEntity> Get(IEnumerable<TKey> keys) {
			var results = new List<TEntity>();
			var parameters = new DynamicParameters();

			parameters.Add("keys", keys);

			IEnumerable<TEntity> entities;

			using (var connection = _connection) {
				entities = connection.Query<TEntity>(_tailor.SelectMany(_selectFields, _key, "@keys"), parameters).ToList();
			}

			if (entities.Any())
				results.AddRange(entities);

			return results;
		}

		public IEnumerable<TEntity> Find(string whereClause, object param = null) {
			var sql = _tailor.Find(_selectFields, whereClause);

			IEnumerable<TEntity> entities;

			using (var connection = _connection) {
				entities = connection.Query<TEntity>(sql, param);
			}

			return entities ?? new List<TEntity>();
		}

		public TEntity First(string whereClause, object param = null) {
			var entities = Find(whereClause, param);

			var entity = entities.FirstOrDefault();

			if (entity == null)
				return null;

			return entity;
		}

		public virtual IEnumerable<TEntity> All() {
			var query = _tailor.All(_selectFields, _key);

			IEnumerable<TEntity> result;

			using (var connection = _connection) {
				result = connection.Query<TEntity>(query);
			}

			return result;
		}

		public virtual TKey Insert(TEntity entity) {
			var properties = BuildPropertiesList(_insertFields);
			var parameters = BuildParametersList(_insertFields, entity);

			decimal identity;

			using (var connection = _connection) {
				identity = connection.Query<decimal>(_tailor.Insert(properties, _key), parameters).Single();
			}

			return _key.IsIdentity ? (TKey)Convert.ChangeType(identity, typeof(TKey)) : (TKey)_key.Getter(entity);
		}

		public virtual IEnumerable<TKey> Insert(IEnumerable<TEntity> entities) {
			return entities.Select(Insert).ToList();
		}

		public virtual int Update(TEntity entity) {
			var properties = BuildPropertiesList(_updateFields);
			var parameters = BuildParametersList(_updateFields, entity);

			parameters.Add(_key.Property, (TKey)_key.Getter(entity));

			var result = 0;

			if (properties.Count <= 0) return result;

			using (var connection = _connection) {
				result = connection.Execute(_tailor.Update(properties, _key, "@" + _key.Property), parameters);
			}

			return result;
		}

		public virtual int Update(IEnumerable<TEntity> entities) {
			return entities.Sum(entity => Update(entity));
		}

		public virtual int Delete(TKey key) {
			var parameters = new DynamicParameters();

			parameters.Add(_key.Property, key);

			int result;

			using (var connection = _connection) {
				result = connection.Execute(_tailor.Delete(_key, "@" + _key.Property), parameters);
			}

			return result;
		}

		public virtual int Delete(IEnumerable<TKey> keys) {
			var parameters = new DynamicParameters();

			parameters.Add(_key.Property, keys);

			int result;

			using (var connection = _connection) {
				result = connection.Execute(_tailor.DeleteMany(_key, "@" + _key.Property), parameters);
			}

			return result;
		}

		#endregion

		#region Utilities

		public Dictionary<string, CachedProperty> BuildPropertiesList(IEnumerable<CachedProperty> properties) {
			var results = new Dictionary<string, CachedProperty>();

			foreach (var property in properties) {
				if (property == null || String.IsNullOrEmpty(property.Name))
					continue;

				var key = "@" + property.Name;

				if (!results.ContainsKey(key))
					results.Add(key, property);
			}

			return results;
		}

		public DynamicParameters BuildParametersList(IEnumerable<CachedProperty> properties, TEntity entity) {
			var results = new DynamicParameters();

			foreach (var property in properties) {
				if (property == null || String.IsNullOrEmpty(property.Name) || property.Getter == null)
					continue;

				results.Add(property.Name, property.Getter(entity));
			}

			return results;
		}

		#endregion
	}
}