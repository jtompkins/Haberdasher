using Haberdasher.Attributes;

namespace Haberdasher.Tests.TestClasses
{
	public class NullableColumnsClass
	{
		[Key]
		[Nullable]
		public int? Id { get; set; }

		[Nullable]
		public string Name { get; set; }

		[Nullable]
		public int NonNullableProperty { get; set; }
	}
}
