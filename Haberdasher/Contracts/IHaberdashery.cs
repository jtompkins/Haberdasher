using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haberdasher.Contracts
{
	public interface IHaberdashery<TEntity, TKey> where TEntity : class, new()
	{
		IEnumerable<TEntity> Query(string sql, object param = null, SqlTransaction transaction = null, bool buffered = true);
		IEnumerable<T> Query<T>(string sql, object param = null, SqlTransaction transaction = null, bool buffered = true);
		int Execute(string sql, object param = null, SqlTransaction transaction = null);
		TEntity Get(TKey key);
		IEnumerable<TEntity> Get(IEnumerable<TKey> keys);
		IEnumerable<TEntity> Find(string whereClause, object param = null);
		TEntity First(string whereClause, object param = null);
		IEnumerable<TEntity> All();
		TKey Insert(TEntity entity);
		IEnumerable<TKey> Insert(IEnumerable<TEntity> entities);
		int Update(TEntity entity);
		int Update(IEnumerable<TEntity> entities);
		int Delete(TKey key);
		int Delete(IEnumerable<TKey> keys);
	}
}
