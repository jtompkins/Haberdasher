using System;
using System.Reflection;
using Haberdasher.Attributes;
using Haberdasher.Support;
using Haberdasher.Support.ExpressionBuilders;

namespace Haberdasher
{
	public class CachedProperty
	{
		public string Property { get; set; }
		public string Alias { get; set; }

		public string Name {
			get { return IsAliased ? Alias : Property; }
		}

		#region Access Delegates

		public Func<object, object> Getter { get; set; }
		public Action<object, object> Setter { get; set; }
		public Func<object, object, bool> Comparer { get; set; }

		#endregion

		#region Metadata

		public bool IsNullable { get; private set; }
		public bool IsNumeric { get; private set; }
		public bool IsAliased { get; private set; }
		public bool IsIdentity { get; private set; }
		public bool UseScopeIdentity { get; private set; }

		public bool IsKey { get; private set; }
		public bool IsSelectable { get; private set; }
		public bool IsInsertable { get; private set; }
		public bool IsUpdatable { get; private set; }

		public object DefaultValue { get; private set; }

		#endregion

		public CachedProperty(PropertyInfo property) {
			Property = property.Name;

			var nullableAttr = property.GetCustomAttribute<NullableAttribute>();

			if (nullableAttr != null) {
				if (!property.PropertyType.IsNullable())
					throw new Exception("Non-Nullable properties may not be marked with the Nullable attribute: " + Property);

				IsNullable = true;
			}
			else
				DefaultValue = property.PropertyType.GetDefaultValue();

			var aliasAttr = property.GetCustomAttribute<AliasAttribute>();

			if (aliasAttr != null && !String.IsNullOrEmpty(aliasAttr.Alias)) {
				IsAliased = true;
				Alias = aliasAttr.Alias;
			}

			IsNumeric = property.PropertyType.IsNumber();

			var keyAttribute = property.GetCustomAttribute<KeyAttribute>();

			if (keyAttribute != null) {
				if (IsNullable)
					throw new Exception("Key properties may not be marked with the Nullable attribute: " + Property);

				IsKey = true;
				IsIdentity = keyAttribute.IsIdentity && IsNumeric; //non-numeric columns cannot be identity columns

				IsSelectable = true;
				IsInsertable = !IsIdentity;
				IsUpdatable = false;

				UseScopeIdentity = property.PropertyType == typeof(int) || property.PropertyType == typeof(long);
			}
			else {
				var ignoreAttribute = property.GetCustomAttribute<IgnoreAttribute>();

				if (ignoreAttribute != null) {
					var type = ignoreAttribute.Type;

					IsSelectable = type != IgnoreTypeEnum.All && type != IgnoreTypeEnum.Select;
					IsInsertable = type != IgnoreTypeEnum.All && type != IgnoreTypeEnum.Writes && type != IgnoreTypeEnum.Insert;
					IsUpdatable = type != IgnoreTypeEnum.All && type != IgnoreTypeEnum.Writes && type != IgnoreTypeEnum.Update;
				}
				else {
					IsSelectable = true;
					IsInsertable = true;
					IsUpdatable = true;
				}
			}

			Getter = GetterBuilder.Build(property);
			Setter = SetterBuilder.Build(property.DeclaringType, property);
			Comparer = ComparerBuilder.Build(property.DeclaringType, property);
		}
	}
}
