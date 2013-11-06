using System;
using System.Collections.Generic;

namespace Haberdasher
{
	public class CachedType
	{
		public CachedProperty Key { get; set; }

		public IList<CachedProperty> SelectFields { get; private set; }
		public IList<CachedProperty> InsertFields { get; private set; }
		public IList<CachedProperty> UpdateFields { get; private set; }

		public CachedType(Type type) {
			SelectFields = new List<CachedProperty>();
			InsertFields = new List<CachedProperty>();
			UpdateFields = new List<CachedProperty>();

			foreach (var property in type.GetProperties())
				Add(new CachedProperty(property));
		}

		public void Add(CachedProperty property) {
			if (property.IsKey)
				Key = property;

			if (property.IsSelectable)
				SelectFields.Add(property);

			if (property.IsInsertable)
				InsertFields.Add(property);

			if (property.IsUpdatable)
				UpdateFields.Add(property);
		}
	}
}
