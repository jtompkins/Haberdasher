namespace Haberdasher.Support.Helpers
{
	public static class NameHelper
	{
		public static string GetEntityTableName<T>() where T : class, new() {
			var entityType = EntityTypes.Get<T>();

			if (entityType == null)
				return null;

			// there are three cases here:
			//	1. The type name needs to be pluralized (the most common case)
			//	2. The type name is aliased (look for the IsAlias property on the type)
			//	3. The type name should be used as-is (look for the IsSingular property on the type)
			// we'll check in reverse order.

			if (entityType.IsSingular)
				return entityType.Table;

			if (entityType.IsAliased)
				return entityType.TableAlias;

			return PluralizationHelper.Pluralize(entityType.Table);
		}
	}
}
