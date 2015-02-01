using System;
using System.Collections.Generic;
using System.Linq;
using Haberdasher.Contracts;

namespace Haberdasher.QueryGenerators
{
	public class SqliteGenerator : SqlServerGenerator, IQueryGenerator
	{
		private const string SelectParamFormat = "{0} as {1}";

		private const string FindOneFormat = "select {0} from [{1}] where {2} order by rowid asc limit 1";

		private const string InsertFormat = "insert into [{0}] ({1}) values ({2})";
		private const string InsertWithIdentityFormat = "insert into [{0}] ({1}) values ({2}); {3}";

		private static string BuildColumns(IEnumerable<CachedProperty> properties) {
			if (!properties.Any()) return String.Empty;

			var clauses = properties.Select(property => property.IsAliased
															? String.Format(SelectParamFormat, property.Alias, property.Property)
															: property.Property);

			var columns = String.Join(", ", clauses);

			return columns;
		}

		/// <summary>
		/// Generates a SELECT statement with an arbitrary WHERE clause that only returns one row
		/// </summary>
		/// <param name="table">The name of the table</param>
		/// <param name="properties">An enumerable of properties to be included in the SELECT clause</param>
		/// <param name="whereClause">A WHERE clause which will be passed to the database</param>
		public new string FindOne(string table, IEnumerable<CachedProperty> properties, string whereClause) {
			return String.Format(FindOneFormat, BuildColumns(properties), table, whereClause);
		}

		/// <summary>
		/// Generates an INSERT statement using the passed-in properties.
		/// </summary>
		/// <param name="table">The name of the table</param>
		/// <param name="properties">A dictionary representing the properties to be inserted; the key is the parameterized name of the property and the value is the property itself</param>
		/// <param name="key">The primary key of the table</param>
		public new string Insert(string table, IDictionary<string, CachedProperty> properties, CachedProperty key) {
			var fields = new List<string>();
			var valueParams = new List<string>();

			foreach (var kvp in properties) {
				fields.Add(kvp.Value.Name);
				valueParams.Add(kvp.Key);
			}

			var format = key.IsIdentity ? InsertWithIdentityFormat : InsertFormat;
			var insertOptions = (key.IsIdentity) ? "select last_insert_rowid()" : "";

			return String.Format(format, table, String.Join(", ", fields), String.Join(", ", valueParams), insertOptions).Trim();
		}
	}
}
