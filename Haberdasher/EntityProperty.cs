using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Haberdasher.Attributes;
using Haberdasher.Support;
using Haberdasher.Support.ExpressionBuilders;

namespace Haberdasher
{
	public class EntityProperty
	{
		public static readonly IEnumerable<string> AutodetectableKeyNames = new List<string> { "id", "guid" };

		public string Property { get; private set; }
		public string Alias { get; private set; }

		public string Name {
			get { return IsAliased ? Alias : Property; }
		}

		#region Access Delegates

		public Func<object, object> Getter { get; private set; }
		public Action<object, object> Setter { get; private set; }
		public Func<object, object, bool> Comparer { get; private set; }

		#endregion

		#region Metadata

		private readonly bool _isValueType;
		private readonly bool _isNullableType;

		public bool IsNullable { get; private set; }
		public bool IsNumeric { get; private set; }
		public bool IsAliased { get; private set; }
		public bool IsIdentity { get; private set; }

		public bool IsKey { get; private set; }
		public bool IsSelectable { get; private set; }
		public bool IsInsertable { get; private set; }
		public bool IsUpdatable { get; private set; }

		public object DefaultValue { get; private set; }

		#endregion

		public EntityProperty(PropertyInfo property) {
			Property = property.Name;
			IsNumeric = property.PropertyType.IsNumber();
			Getter = GetterBuilder.Build(property);
			Setter = SetterBuilder.Build(property.DeclaringType, property);
			Comparer = ComparerBuilder.Build(property.DeclaringType, property);

			_isValueType = property.PropertyType.IsValueType;
			_isNullableType = property.PropertyType.IsNullableValueType();

			var nullableAttr = property.GetCustomAttribute<NullableAttribute>();

			if (nullableAttr != null || _isNullableType) {
				SetNullable();
			}
			else {
				DefaultValue = property.PropertyType.GetDefaultValue();
			}

			var aliasAttr = property.GetCustomAttribute<AliasAttribute>();

			if (aliasAttr != null)
				SetAlias(aliasAttr.Alias);

			var autoDetectedKeyName = AutodetectableKeyNames.Any(n => n.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase));
			var keyAttribute = property.GetCustomAttribute<KeyAttribute>();
			var ignoreAttribute = property.GetCustomAttribute<IgnoreAttribute>();

			if (keyAttribute != null || autoDetectedKeyName) {
				SetKey(keyAttribute != null ? keyAttribute.IsIdentity : (bool?)null);
			}
			else if (ignoreAttribute != null) {
				SetIgnore(ignoreAttribute.Type);
			}
		}

		#region Fluent Interface Support

		public EntityProperty SetKey(bool? isIdentity) {
			if (IsNullable)
				throw new Exception("Key properties may not be marked with the Nullable attribute: " + Property);

			IsKey = true;
			IsIdentity = isIdentity.HasValue ? isIdentity.GetValueOrDefault() && IsNumeric : IsNumeric;

			IsSelectable = true;
			IsInsertable = !IsIdentity;
			IsUpdatable = false;

			return this;
		}

		public EntityProperty SetIgnore(IgnoreTypeEnum? type) {
			if (type.HasValue) {
				IsSelectable = type != IgnoreTypeEnum.All && type != IgnoreTypeEnum.Select;
				IsInsertable = type != IgnoreTypeEnum.All && type != IgnoreTypeEnum.Writes && type != IgnoreTypeEnum.Insert;
				IsUpdatable = type != IgnoreTypeEnum.All && type != IgnoreTypeEnum.Writes && type != IgnoreTypeEnum.Update;
			}
			else {
				IsSelectable = true;
				IsInsertable = true;
				IsUpdatable = true;
			}

			return this;
		}

		public EntityProperty SetAlias(string alias) {
			if (String.IsNullOrEmpty(alias))
				return this;

			IsAliased = true;
			Alias = alias;

			return this;
		}

		public EntityProperty SetNullable() {
			if (_isValueType && !_isNullableType)
				throw new Exception("Non-Nullable value type properties may not be marked Nullable: " + Property);

			IsNullable = true;

			return this;
		}

		#endregion
	}
}
