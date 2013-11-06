using System.Collections.Generic;

namespace Haberdasher.Tailors
{
	public interface ITailor
	{
		string SelectAll(IEnumerable<CachedProperty> properties);
		string Select(IEnumerable<CachedProperty> properties, CachedProperty key, string keyParam);
		string SelectMany(IEnumerable<CachedProperty> properties, CachedProperty key, string keysParam);

		string Insert(IDictionary<string, CachedProperty> properties, CachedProperty key);

		string Update(IDictionary<string, CachedProperty> properties, CachedProperty key, string keyParam);
		string UpdateMany(IDictionary<string, CachedProperty> properties, CachedProperty key, string keysParam);

		string DeleteAll();
		string Delete(CachedProperty key, string keyParam);
		string DeleteMany(CachedProperty key, string keysParam);
	}
}
