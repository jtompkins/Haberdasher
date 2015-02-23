﻿using System;
using System.Collections.Generic;
using System.Data;

namespace Haberdasher
{
	public class EntityTypes
	{
		private static IDictionary<Type, EntityType<object>> Types { get; set; }

		static EntityTypes() {
			Types = new Dictionary<Type, EntityType<object>>();
		}

		public static EntityType<T> Register<T>(Action<EntityType<T>> registerAction = null) where T : class, new() {
			var entityType = new EntityType<T>();

			if (registerAction != null)
				registerAction(entityType);

			if (entityType.KeyField == null)
				throw new MissingPrimaryKeyException("No primary key defined for type: " + entityType.Name);

			var type = typeof(T);

			Types[type] = entityType as EntityType<object>;

			return entityType;
		}

		public static EntityType<T> Get<T>() where T : class, new() {
			var type = typeof(T);

			if (!Types.ContainsKey(type))
				return null;

			return Types[type] as EntityType<T>;
		}
	}
}