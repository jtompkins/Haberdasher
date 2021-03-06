﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

			IsSelectable = true;
			IsInsertable = true;
			IsUpdatable = true;

			if (_isNullableType)
				SetNullable();
			else
				DefaultValue = property.PropertyType.GetDefaultValue();

			var autoDetectedKeyName = AutodetectableKeyNames.Any(n => n.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase));

			if (autoDetectedKeyName)
				SetKey();
		}

		#region Fluent Interface Support

		internal EntityProperty SetKey(bool? isIdentity = null) {
			if (IsNullable)
				throw new Exception("Cannot set primary key to be nullable: " + Property);

			IsKey = true;
			IsIdentity = isIdentity.HasValue ? isIdentity.GetValueOrDefault() && IsNumeric : IsNumeric;

			IsSelectable = true;
			IsInsertable = !IsIdentity;
			IsUpdatable = false;

			return this;
		}

		internal EntityProperty SetIgnore(IgnoreTypeEnum? type = null) {
			if (IsKey)
				throw new Exception("Cannot override ignore properties of primary key: " + Property);

			if (!type.HasValue) return this;

			IsSelectable = type != IgnoreTypeEnum.All && type != IgnoreTypeEnum.Select;
			IsInsertable = type != IgnoreTypeEnum.All && type != IgnoreTypeEnum.Writes && type != IgnoreTypeEnum.Insert;
			IsUpdatable = type != IgnoreTypeEnum.All && type != IgnoreTypeEnum.Writes && type != IgnoreTypeEnum.Update;

			return this;
		}

		internal EntityProperty SetAlias(string alias) {
			if (String.IsNullOrEmpty(alias))
				return this;

			IsAliased = true;
			Alias = alias;

			return this;
		}

		internal EntityProperty SetNullable() {
			if (IsKey)
				throw new Exception("Cannot set primary key to be nullable: " + Property);

			if (_isValueType && !_isNullableType)
				throw new Exception("Cannot set non-nullable property to be nullable: " + Property);

			IsNullable = true;

			return this;
		}

		#endregion
	}
}
