using Haberdasher.Attributes;
using Haberdasher.Support;

namespace Haberdasher.Tests.TestClasses
{
	public class IgnoredColumnsClass
	{
		[Ignore(IgnoreTypeEnum.Select)]
		public string IgnoredForSelect { get; set; }

		[Ignore(IgnoreTypeEnum.Insert)]
		public string IgnoredForInsert { get; set; }

		[Ignore(IgnoreTypeEnum.Update)]
		public string IgnoredForUpdate { get; set; }

		[Ignore(IgnoreTypeEnum.Writes)]
		public string IgnoredForWrites { get; set; }

		[Ignore(IgnoreTypeEnum.All)]
		public string IgnoredAlways { get; set; }
	}
}
