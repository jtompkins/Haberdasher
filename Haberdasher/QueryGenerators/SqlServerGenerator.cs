using System;
using System.Collections.Generic;
using System.Linq;
using Haberdasher.Contracts;

namespace Haberdasher.QueryGenerators
{
	public class SqlServerGenerator : IQueryGenerator
	{
		private const string SelectAllFormat = "select {0} from [{1}] order by {2}";
		private const string SelectFormat = "select {0} from [{1}] where {2} = {3}";
		private const string SelectManyFormat = "select {0} from [{1}] where {2} in {3}";
		private const string SelectParamFormat = "{0} as {1}";

		private const string FindFormat = "select {0} from [{1}] where {2}";
		private const string FindOneFormat = "select top 1 {0} from [{1}] where {2}";

		private const string InsertFormat = "insert into [{0}] ({1}) values ({2})";
		private const string InsertWithIdentityFormat = "set nocount on insert into [{0}] ({1}) values ({2}) {3}";

		private const string UpdateFormat = "update [{0}] set {1} where {2} = {3}";
		private const string UpdateManyFormat = "update [{0}] set {1} where {2} in {3}";
		private const string UpdateParamFormat = "{0} = {1}";

		private const string DeleteAllFormat = "truncate table [{0}]";
		private const string DeleteFormat = "delete from [{0}] where {1} = {2}";
		private const string DeleteManyFormat = "delete from [{0}] where {1} in {2}";

        public const string ParameterFormat = "@";

		private static string BuildColumns(IEnumerable<CachedProperty> properties) {
			if (!properties.Any()) return String.Empty;

			var clauses = properties.Select(property => property.IsAliased
															? String.Format(SelectParamFormat, property.Alias, property.Property)
															: property.Property);

			var columns = String.Join(", ", clauses);

			return columns;
		}

		/// <summary>
		/// Generates a complete SELECT statement with an ORDER BY clause that returns all rows in a SQL table, ordered by the given key property.
		/// </summary>
		/// <param name="table">The name of the table</param>
		/// <param name="properties">An enumerable of properties to be included in the SELECT clause</param>
		/// <param name="key">The property that represents the table's primary key</param>
		public string SelectAll(string table, IEnumerable<CachedProperty> properties, CachedProperty key) {
			return String.Format(SelectAllFormat, BuildColumns(properties), table, key.Name);
		}

		/// <summary>
		/// Generates a complete SELECT statement with a WHERE clause that selects a single row based on the given key property.
		/// </summary>
		/// <param name="table">The name of the table</param>
		/// <param name="properties">An enumerable of properties to be included in the SELECT clause</param>
		/// <param name="key">The property that represents the table's primary key</param>
		/// <param name="value">The value which will be passed to the database</param>
		public string Select(string table, IEnumerable<CachedProperty> properties, CachedProperty key, string value) {
			return String.Format(SelectFormat, BuildColumns(properties), table, key.Name, value);
		}

		/// <summary>
		/// Generates a SELECT statement with a WHERE IN clause which selects multiple rows based on one or more primary key values. 
		/// </summary>
		/// <param name="table">The name of the table</param>
		/// <param name="properties">An enumerable of properties to be included in the SELECT clause</param>
		/// <param name="key">The property that represents the table's primary key</param>
		/// <param name="values">The primary key values that will be passed to the database</param>
		public string SelectMany(string table, IEnumerable<CachedProperty> properties, CachedProperty key, string values) {
			return String.Format(SelectManyFormat, BuildColumns(properties), table, key.Name, values);
		}

		/// <summary>
		/// Generates a SELECT statement with an arbitrary WHERE clause.
		/// </summary>
		/// <param name="table">The name of the table</param>
		/// <param name="properties">An enumerable of properties to be included in the SELECT clause</param>
		/// <param name="whereClause">A WHERE clause which will be passed to the database</param>
		public string Find(string table, IEnumerable<CachedProperty> properties, string whereClause) {
			return String.Format(FindFormat, BuildColumns(properties), table, whereClause);
		}

		/// <summary>
		/// Generates a SELECT statement with an arbitrary WHERE clause that only returns one row
		/// </summary>
		/// <param name="table">The name of the table</param>
		/// <param name="properties">An enumerable of properties to be included in the SELECT clause</param>
		/// <param name="whereClause">A WHERE clause which will be passed to the database</param>
		public string FindOne(string table, IEnumerable<CachedProperty> properties, string whereClause) {
			return String.Format(FindOneFormat, BuildColumns(properties), table, whereClause);
		}

		/// <summary>
		/// Generates an INSERT statement using the passed-in properties.
		/// </summary>
		/// <param name="table">The name of the table</param>
		/// <param name="properties">A dictionary representing the properties to be inserted; the key is the parameterized name of the property and the value is the property itself</param>
		/// <param name="key">The primary key of the table</param>
		public string Insert(string table, IDictionary<string, CachedProperty> properties, CachedProperty key) {
			var fields = new List<string>();
			var valueParams = new List<string>();

			foreach (var kvp in properties) {
				fields.Add(kvp.Value.Name);
				valueParams.Add(kvp.Key);
			}

			return String.Format(InsertFormat, table, String.Join(", ", fields), String.Join(", ", valueParams)).Trim();
		}

		public string InsertWithIdentity(string table, IDictionary<string, CachedProperty> properties, CachedProperty key) {
			var fields = new List<string>();
			var valueParams = new List<string>();

			foreach (var kvp in properties) {
				fields.Add(kvp.Value.Name);
				valueParams.Add(kvp.Key);
			}

			var insertOptions = (key.IsIdentity) ? "select SCOPE_IDENTITY()" : "";

			return String.Format(InsertWithIdentityFormat, table, String.Join(", ", fields), String.Join(", ", valueParams), insertOptions).Trim();
		}

		/// <summary>
		/// Generates an UPDATE statement for a single key using the passed-in properties.
		/// </summary>
		/// <param name="table">The name of the table</param>
		/// <param name="properties">A dictionary representing the properties to be inserted; the key is the parameterized name of the property and the value is the property itself</param>
		/// <param name="key">The primary key of the table</param>
		/// <param name="value">The key to be updated</param>
		public string Update(string table, IDictionary<string, CachedProperty> properties, CachedProperty key, string value) {
			var clauses = properties.Select(kvp => String.Format(UpdateParamFormat, kvp.Value.Name, kvp.Key));

			return String.Format(UpdateFormat, table, String.Join(", ", clauses), key.Name, value);
		}

		/// <summary>
		/// Generates an UPDATE statement for one or more keys using the passed-in properties.
		/// </summary>
		/// <param name="table">The name of the table</param>
		/// <param name="properties">A dictionary representing the properties to be inserted; the key is the parameterized name of the property and the value is the property itself</param>
		/// <param name="key">The primary key of the table</param>
		/// <param name="values">The keys to be updated</param>
		public string UpdateMany(string table, IDictionary<string, CachedProperty> properties, CachedProperty key, string values) {
			var clauses = properties.Select(kvp => String.Format(UpdateParamFormat, kvp.Value.Name, kvp.Key));

			return String.Format(UpdateManyFormat, table, String.Join(", ", clauses), key.Name, values);
		}

		/// <summary>
		/// Generates a DELETE statement for all rows in a table.
		/// </summary>
		/// <param name="table">The name of the table</param>
		public string DeleteAll(string table) {
			return String.Format(DeleteAllFormat, table);
		}

		/// <summary>
		/// Generates a DELETE statement for a single key.
		/// </summary>
		/// <param name="table">The name of the table</param>
		/// <param name="key">The primary key of the table</param>
		/// <param name="value">The key to be deleted</param>
		public string Delete(string table, CachedProperty key, string value) {
			return String.Format(DeleteFormat, table, key.Name, value);
		}

		/// <summary>
		/// Generates a DELETE statement for one or more keys.
		/// </summary>
		/// <param name="table">The name of the table</param>
		/// <param name="key">The primary key of the table</param>
		/// <param name="values">The keys to be deleted</param>
		public string DeleteMany(string table, CachedProperty key, string values) {
			return String.Format(DeleteManyFormat, table, key.Name, values);
		}

		/// <summary>
		/// Formats a SQL parameter for use with a specific SQL dialect or database.
		/// </summary>
		/// <example>TSQL: parameters should be prefixed with "@".</example>
		/// <param name="param">The unformatted SQL parameter</param>
        public string FormatSqlParameter(string param) {
			if (param.Substring(0, ParameterFormat.Length) == ParameterFormat)
				param = RemoveSqlParameterFormatting(param);

			return String.IsNullOrWhiteSpace(param)
				? String.Empty 
				: String.Format("{0}{1}", ParameterFormat, param);
		}

		/// <summary>
		/// Removes formatting added to a parameter by the FormatSqlParameter method.
		/// </summary>
		/// <param name="param">The formatted SQL parameter</param>
		public string RemoveSqlParameterFormatting(string param) {
			return param.Length < ParameterFormat.Length 
				? param 
				: param.Remove(0, ParameterFormat.Length);
		}
	}
}