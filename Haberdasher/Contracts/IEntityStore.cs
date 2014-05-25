using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Haberdasher.Contracts
{
	public interface IEntityStore<TEntity, TKey> where TEntity : class, new()
	{
		#region Dapper Methods

		IEnumerable<TEntity> Query(string sql, object param = null, SqlTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);
		IEnumerable<T> Query<T>(string sql, object param = null, SqlTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);
		int Execute(string sql, object param = null, SqlTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

		#endregion

		#region Sync Methods

		/// <summary>
		/// Gets all entities from the database.
		/// </summary>
		IEnumerable<TEntity> Get();

		/// <summary>
		/// Gets an entity from the database by key.
		/// </summary>
		/// <param name="key">The primary key of the entity</param>
		TEntity Get(TKey key);
		
		/// <summary>
		/// Gets one or more entities from the database by key.
		/// </summary>
		/// <param name="keys">An enumerable of key values</param>
		IEnumerable<TEntity> Get(IEnumerable<TKey> keys);

		/// <summary>
		/// Gets entities from the database, filtered using a WHERE clause.
		/// </summary>
		/// <param name="whereClause">A string containing the WHERE clause's predicate</param>
		/// <param name="param">Parameters to be passed to the WHERE clause</param>
		IEnumerable<TEntity> Find(string whereClause, object param = null);

		/// <summary>
		/// Gets a single entity from the database, filtered using a WHERE clause.
		/// </summary>
		/// <param name="whereClause">A string containing the WHERE clause's predicate</param>
		/// <param name="param">Parameters to be passed to the WHERE clause</param>
		TEntity FindOne(string whereClause, object param = null);

		/// <summary>
		/// Inserts an entity into the database.
		/// </summary>
		/// <param name="entity">The entity to be inserted</param>
		/// <returns>The primary key of the inserted entity</returns>
		TKey Insert(TEntity entity);

		/// <summary>
		/// Inserts multiple entities into the database.
		/// </summary>
		/// <param name="entities">The entities to be inserted</param>
		/// <returns>The primary key of the inserted entity</returns>
		IEnumerable<TKey> Insert(IEnumerable<TEntity> entities);

		/// <summary>
		/// Updates an entity in the database.
		/// </summary>
		/// <param name="entity">The entity to be updated</param>
		/// <returns>The number of updated records</returns>
		int Update(TEntity entity);

		/// <summary>
		/// Updates multiple entities in the database.
		/// </summary>
		/// <param name="entities">The entities to be updated</param>
		/// <returns>The number of updated records</returns>
		int Update(IEnumerable<TEntity> entities);

		/// <summary>
		/// Deletes all rows in the database table.
		/// </summary>
		int Delete();

		/// <summary>
		/// Deletes a row in the database table by key.
		/// </summary>
		/// <param name="key">The key of the row to be deleted</param>
		/// <returns>The number of deleted rows</returns>
		int Delete(TKey key);

		/// <summary>
		/// Deletes multiple rows in the database.
		/// </summary>
		/// <param name="keys">The keys of the rows to be deleted</param>
		/// <returns>The number of deleted rows</returns>
		int Delete(IEnumerable<TKey> keys);

		#endregion
	}
}
