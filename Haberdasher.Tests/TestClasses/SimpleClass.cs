using Haberdasher.Attributes;

namespace Haberdasher.Tests.TestClasses
{
	public class SimpleClass
	{
		[Key]
		public int Id { get; set; }

		public string Name { get; set; }
	}
}
