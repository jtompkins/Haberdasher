using System;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Reflection;
using Haberdasher.Attributes;

namespace Haberdasher.Support.Helpers
{
	public static class NameHelper
	{
		public static string GetEntityTableName<T>() {
			return GetEntityTableName(typeof (T));
		}

		public static string GetEntityTableName(Type type) {
			// there are three cases here:
			//	1. The type name needs to be pluralized (the most common case)
			//	2. The type name is aliased (look for the AliasAttribute on the type)
			//	3. The type name should be used as-is (look for the SingularAttribute on the type)
			// we'll check in reverse order.

			if (type.GetCustomAttribute<SingularAttribute>() != null)
				return type.Name;

			var alias = type.GetCustomAttribute<AliasAttribute>();

			if (alias != null && !String.IsNullOrEmpty(alias.Alias))
				return alias.Alias;

			return PluralizationHelper.Pluralize(type.Name);
		}
	}
}
