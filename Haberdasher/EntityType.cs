﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Haberdasher.Support;
using Haberdasher.Support.Helpers;

namespace Haberdasher
{
	public class EntityType<TEntity> where TEntity : class, new()
	{
		public string Name { get; private set; }

		public EntityProperty KeyField { get; private set; }

		public IList<EntityProperty> SelectFields { get; private set; }
		public IList<EntityProperty> InsertFields { get; private set; }
		public IList<EntityProperty> UpdateFields { get; private set; }

		public EntityType() {
			var type = typeof (TEntity);

			Name = PluralizationHelper.Pluralize(type.Name);

			SelectFields = new List<EntityProperty>();
			InsertFields = new List<EntityProperty>();
			UpdateFields = new List<EntityProperty>();

			foreach (var property in type.GetProperties())
				Add(new EntityProperty(property));
		}

		private void Add(EntityProperty property) {
			if (property.IsKey)
				KeyField = property;

			if (property.IsSelectable)
				SelectFields.Add(property);

			if (property.IsInsertable)
				InsertFields.Add(property);

			if (property.IsUpdatable)
				UpdateFields.Add(property);
		}

		#region Fluent Interface Support

		private EntityProperty GetMemberByName(string memberName) {
			var property = SelectFields.FirstOrDefault(p => p.Property.Equals(memberName));

			if (property != null)
				return property;

			property = InsertFields.FirstOrDefault(p => p.Property.Equals(memberName));

			return property ?? UpdateFields.FirstOrDefault(p => p.Property.Equals(memberName));
		}

		private string GetMemberNameFromExpression<T>(Expression<Func<T, object>> property) {
			PropertyInfo propertyInfo;

			if (property.Body is MemberExpression) {
				propertyInfo = (property.Body as MemberExpression).Member as PropertyInfo;
			}
			else {
				var body = (UnaryExpression)property.Body;
				var member = body.Operand as MemberExpression;

				if (member == null)
					throw new InvalidOperationException("Couldn't find property in type.");

				propertyInfo = member.Member as PropertyInfo;
			}

			if (propertyInfo == null)
				throw new InvalidOperationException("Couldn't find property in type.");

			return propertyInfo.Name;
		}

		public EntityProperty GetProperty<T>(Expression<Func<T, object>> property) {
			var name = GetMemberNameFromExpression(property);
			var cachedProperty = GetMemberByName(name);

			if (cachedProperty == null)
				throw new InvalidOperationException("Couldn't find property in property cache.");

			return cachedProperty;
		}

		private void RemoveFromSelect(EntityProperty property) {
			if (SelectFields.Contains(property))
				SelectFields.Remove(property);
		}

		private void RemoveFromInsert(EntityProperty property) {
			if (InsertFields.Contains(property))
				InsertFields.Remove(property);
		}

		private void RemoveFromUpdate(EntityProperty property) {
			if (UpdateFields.Contains(property))
				UpdateFields.Remove(property);
		}

		private void RemoveFromFieldCache(EntityProperty property, IgnoreTypeEnum? type)
		{
			if (!type.HasValue)
				return;

			switch (type)
			{
				case IgnoreTypeEnum.Select:
					RemoveFromSelect(property);

					break;

				case IgnoreTypeEnum.Insert:
					RemoveFromInsert(property);

					break;

				case IgnoreTypeEnum.Update:
					RemoveFromUpdate(property);

					break;

				case IgnoreTypeEnum.Writes:
					RemoveFromInsert(property);
					RemoveFromUpdate(property);

					break;

				case IgnoreTypeEnum.All:
					RemoveFromSelect(property);
					RemoveFromInsert(property);
					RemoveFromUpdate(property);

					break;
			}
		}

		#endregion

		#region Fluent Interface Methods

		public EntityType<TEntity> Singular() {
			var type = typeof(TEntity);

			Name = type.Name;

			return this;
		}

		public EntityType<TEntity> AliasTable(string alias){
			if (!String.IsNullOrEmpty(alias))
				Name = alias;

			return this;
		} 

		public EntityType<TEntity> Key(Expression<Func<TEntity, object>> property, bool? isIdentity = null) {
			var cachedProperty = GetProperty(property);

			cachedProperty.SetKey(isIdentity);

			return this;
		}

		public EntityType<TEntity> Ignore(Expression<Func<TEntity, object>> property, IgnoreTypeEnum? type) {
			var cachedProperty = GetProperty(property);

			cachedProperty.SetIgnore(type);
			RemoveFromFieldCache(cachedProperty, type);

			return this;
		}

		public EntityType<TEntity> Alias(Expression<Func<TEntity, object>> property, string alias) {
			var cachedProperty = GetProperty(property);

			cachedProperty.SetAlias(alias);

			return this;
		}

		public EntityType<TEntity> Nullable(Expression<Func<TEntity, object>> property) {
			var cachedProperty = GetProperty(property);

			cachedProperty.SetNullable();

			return this;
		}

		#endregion
	}
}
