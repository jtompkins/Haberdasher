using System;
using System.Collections.Generic;
using System.Linq;

namespace Haberdasher.Support.Helpers
{
	public static class KeyHelper
	{
		public static readonly IEnumerable<string> AutodetectableKeyNames = new List<string> { "id", "guid" }; 

		public static CachedProperty FindKey(IEnumerable<CachedProperty> properties) {
			foreach (var keyName in AutodetectableKeyNames) {
				var property = properties.FirstOrDefault(p => p.Name.Equals(keyName, StringComparison.InvariantCultureIgnoreCase));

				if (property != null)
					return property;
			}

			return null;
		}
	}
}
