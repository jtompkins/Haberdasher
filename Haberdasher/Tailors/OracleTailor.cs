using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Haberdasher.Support;

namespace Haberdasher.Tailors
{
	public class OracleTailor : ITailor
	{
		#region Constants
        // Oracle uses double quotes for literal table names 
		private const string SELECT_ALL_FORMAT = @"select {0} from ""{1}""";   
		private const string SELECT_FORMAT = @"select {0} from ""{1}"" where {2} = {3}";
		private const string SELECT_MANY_FORMAT = @"select {0} from ""{1}"" where {2} in {3}";

		private const string SELECT_PARAM_FORMAT = "{0} as {1}";

		private const string ALL_FORMAT = "{0} order by {1}";
		private const string FIND_FORMAT = "{0} where {1}";

		private const string INSERT_FORMAT = @"insert into ""{0}"" ({1}) values ({2}) {3}";

		private const string UPDATE_FORMAT = @"update ""{0}"" set {1} where {2} = {3}";
		private const string UPDATE_MANY_FORMAT = @"update ""{0}"" set {1} where {2} in {3}";
		
		private const string UPDATE_PARAM_FORMAT = "{0} = {1}";

		private const string DELETE_ALL_FORMAT = @"truncate table ""{0}""";
		private const string DELETE_FORMAT = @"delete from ""{0}"" where {1} = {2}";
		private const string DELETE_MANY_FORMAT = @"delete from ""{0}"" where {1} in {2}";


        public const string STR_SqlParamIndicator = ":";

		#endregion
        // name of the table
		private readonly string _name;

        public OracleTailor(string name)
        {
			_name = name;
		}

		#region Private Methods

		private static string BuildColumns(IEnumerable<CachedProperty> properties) {
			var columns = "*";

			var cachedProperties = properties as IList<CachedProperty> ?? properties.ToList();

			if (cachedProperties.Count > 0) {
				var clauses = cachedProperties.Select(property => property.IsAliased ? String.Format(SELECT_PARAM_FORMAT, property.Alias, property.Property) : property.Property);

				columns = String.Join(", ", clauses);
			}

			return columns;
		}

		#endregion

		public string SelectAll(IEnumerable<CachedProperty> properties) {
			return String.Format(SELECT_ALL_FORMAT, BuildColumns(properties), _name);
		}

		public string Select(IEnumerable<CachedProperty> properties, CachedProperty key, string keyParam) {
            string sql = String.Format(SELECT_FORMAT, BuildColumns(properties), _name, key.Name, keyParam);
            Debug.WriteLine(String.Format("Select :: sql = {0}", sql));
            return sql;
		}

		public string SelectMany(IEnumerable<CachedProperty> properties, CachedProperty key, string keysParam) {
            string sql = String.Format(SELECT_MANY_FORMAT, BuildColumns(properties), _name, key.Name, keysParam);
            Debug.WriteLine(String.Format("SelectMany :: sql = {0}", sql));
            return sql;
		}

        public string All(IEnumerable<CachedProperty> properties, CachedProperty key)
        {
            string sql = String.Format("{0} order by {1}", SelectAll(properties), key.Name);
            Debug.WriteLine(String.Format("All :: sql = {0}", sql));

            return sql;
        }

        public string Find(IEnumerable<CachedProperty> properties, string whereClause)
        {
            string sql = String.Format("{0} where {1}", SelectAll(properties), whereClause);
            Debug.WriteLine(String.Format("Find :: sql = {0}", sql));

            return sql;
        }

        public string Insert(IDictionary<string, CachedProperty> properties, CachedProperty key)
        {
            var fields = new List<string>();
            var valueParams = new List<string>();

            foreach (var kvp in properties)
            {
                fields.Add(kvp.Value.Name);
                valueParams.Add(kvp.Key);
            }

            var insertOptions = "";

            string sql = String.Format(INSERT_FORMAT, _name, String.Join(", ", fields), String.Join(", ", valueParams), insertOptions).Trim();

            // if key is set by a trigger, retrieve the value using plsql block 
            if (key.IsIdentity)
            {
                string plsql = String.Format(@"declare
                                   newId number;
                                 begin
                                   {0} returning {1} into newId;
                                 end;", sql, key.Name);

                sql = plsql;
            }

            Debug.WriteLine(String.Format("Insert :: sql = {0}", sql));

            return sql;
        }

        public string Update(IDictionary<string, CachedProperty> properties, CachedProperty key, string keyParam)
        {
            var clauses = properties.Select(kvp => String.Format(UPDATE_PARAM_FORMAT, kvp.Value.Name, kvp.Key));

            string sql = String.Format(UPDATE_FORMAT, _name, String.Join(", ", clauses), key.Name, keyParam);
            Debug.WriteLine(String.Format("Update :: sql = {0}", sql));

            return sql;
        }

        public string UpdateMany(IDictionary<string, CachedProperty> properties, CachedProperty key, string keysParam)
        {
            var clauses = properties.Select(kvp => String.Format(UPDATE_PARAM_FORMAT, kvp.Value.Name, kvp.Key));

            string sql = String.Format(UPDATE_MANY_FORMAT, _name, String.Join(", ", clauses), key.Name, keysParam);
            Debug.WriteLine(String.Format("UpdateMany :: sql = {0}", sql));

            return sql;
        }

        public string DeleteAll()
        {
            string sql = String.Format(DELETE_ALL_FORMAT, _name);
            Debug.WriteLine(String.Format("DeleteAll :: sql = {0}", sql));

            return sql;
        }

        public string Delete(CachedProperty key, string keyParam)
        {
            string sql = String.Format(DELETE_FORMAT, _name, key.Name, keyParam);
            Debug.WriteLine(String.Format("Delete :: sql = {0}", sql));

            return sql;
        }

        public string DeleteMany(CachedProperty key, string keysParam)
        {
            string sql = String.Format(DELETE_MANY_FORMAT, _name, key.Name, keysParam);
            Debug.WriteLine(String.Format("DeleteMany :: sql = {0}", sql));

            return sql;
        }

        /// <summary>
        /// Formats the name of the SQL parameter such as including the : before the param name.
        /// Passing in "Id" returns ":Id"
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <returns>System.String.</returns>
        public string FormatSqlParamName(string paramName)
        {
            if (string.IsNullOrWhiteSpace(paramName))
            {
                return string.Empty;
            }

            string cleanName = paramName.RemoveParamIdentifier();
            string nameWithIdentifier = String.Format("{0}{1}",STR_SqlParamIndicator, cleanName);
            return nameWithIdentifier;
        }
	}
}