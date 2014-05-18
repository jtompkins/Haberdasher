﻿using System.Collections.Generic;
using Haberdasher.Support;

namespace Haberdasher.SqlBuilders
{
	public interface ISqlGenerator
	{
		/// <summary>
		/// Generates a complete SELECT statement with an ORDER BY clause that returns all rows in a SQL table, ordered by the given key property.
		/// </summary>
		/// <param name="properties">An enumerable of properties to be included in the SELECT clause</param>
		/// <param name="key">The property that represents the table's primary key</param>
		string SelectAll(IEnumerable<CachedProperty> properties, CachedProperty key);

		/// <summary>
		/// Generates a complete SELECT statement with a WHERE clause that selects a single row based on the given key property.
		/// </summary>
		/// <param name="properties">An enumerable of properties to be included in the SELECT clause</param>
		/// <param name="key">The property that represents the table's primary key</param>
		/// <param name="value">The value which will be passed to the database</param>
		string Select(IEnumerable<CachedProperty> properties, CachedProperty key, string value);

		/// <summary>
		/// Generates a SELECT statement with a WHERE IN clause which selects multiple rows based on one or more primary key values. 
		/// </summary>
		/// <param name="properties">An enumerable of properties to be included in the SELECT clause</param>
		/// <param name="key">The property that represents the table's primary key</param>
		/// <param name="values">The primary key values that will be passed to the database</param>
		string SelectMany(IEnumerable<CachedProperty> properties, CachedProperty key, string values);

		/// <summary>
		/// Generates a SELECT statement with an arbitrary WHERE clause.
		/// </summary>
		/// <param name="properties">An enumerable of properties to be included in the SELECT clause</param>
		/// <param name="whereClause">A WHERE clause which will be passed to the database</param>
		string Find(IEnumerable<CachedProperty> properties, string whereClause);

		/// <summary>
		/// Generates a SELECT statement with an arbitrary WHERE clause that only returns one row
		/// </summary>
		/// <param name="properties">An enumerable of properties to be included in the SELECT clause</param>
		/// <param name="whereClause">A WHERE clause which will be passed to the database</param>
		string FindOne(IEnumerable<CachedProperty> properties, string whereClause);

		/// <summary>
		/// Generates an INSERT statement using the passed-in properties.
		/// </summary>
		/// <param name="properties">A dictionary representing the properties to be inserted; the key is the parameterized name of the property and the value is the property itself</param>
		/// <param name="key">The primary key of the table</param>
		string Insert(IDictionary<string, CachedProperty> properties, CachedProperty key);

		/// <summary>
		/// Generates an UPDATE statement for a single key using the passed-in properties.
		/// </summary>
		/// <param name="properties">A dictionary representing the properties to be inserted; the key is the parameterized name of the property and the value is the property itself</param>
		/// <param name="key">The primary key of the table</param>
		/// <param name="value">The key to be updated</param>
		string Update(IDictionary<string, CachedProperty> properties, CachedProperty key, string value);

		/// <summary>
		/// Generates an UPDATE statement for one or more keys using the passed-in properties.
		/// </summary>
		/// <param name="properties">A dictionary representing the properties to be inserted; the key is the parameterized name of the property and the value is the property itself</param>
		/// <param name="key">The primary key of the table</param>
		/// <param name="values">The keys to be updated</param>
		string UpdateMany(IDictionary<string, CachedProperty> properties, CachedProperty key, string values);

		/// <summary>
		/// Generates a DELETE statement for all rows in a table.
		/// </summary>
		string DeleteAll();

		/// <summary>
		/// Generates a DELETE statement for a single key.
		/// </summary>
		/// <param name="key">The primary key of the table</param>
		/// <param name="value">The key to be deleted</param>
		string Delete(CachedProperty key, string value);

		/// <summary>
		/// Generates a DELETE statement for one or more keys.
		/// </summary>
		/// <param name="key">The primary key of the table</param>
		/// <param name="values">The keys to be deleted</param>
		string DeleteMany(CachedProperty key, string values);

		/// <summary>
		/// Formats a SQL parameter for use with a specific SQL dialect or database.
		/// </summary>
		/// <example>TSQL: parameters should be prefixed with "@".</example>
		/// <param name="param">The unformatted SQL parameter</param>
		string FormatSqlParameter(string param);

		/// <summary>
		/// Removes formatting added to a parameter by the FormatSqlParameter method.
		/// </summary>
		/// <param name="param">The formatted SQL parameter</param>
		string RemoveSqlParameterFormatting(string param);
	}
}
