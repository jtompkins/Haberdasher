using System;
using System.Configuration;

namespace Haberdasher.Support.Helpers
{
	public static class ConnectionStringHelper
	{
		public static string FindByName(string name) {
			if (String.IsNullOrEmpty(name))
				return FindFirst();

			var connectionString = ConfigurationManager.ConnectionStrings[name];

			return connectionString == null 
				? FindFirst() 
				: connectionString.ConnectionString;
		}

		public static string FindFirst() {
			return ConfigurationManager.ConnectionStrings.Count < 1 
				? null 
				: ConfigurationManager.ConnectionStrings[0].ConnectionString;
		}
	}
}
