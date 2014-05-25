using Haberdasher.Attributes;
using Xunit;

namespace Haberdasher.Tests
{
	public class NameTestsFixture
	{
		private class Student
		{
			public int Id { get; set; }
		}

		private class Tests
		{
			public int Id { get; set; }
		}

		[Alias("Professors")]
		private class Teachers
		{
			public int Id { get; set; }
		}

		[Fact]
		public void PluralizesSingularNames() {
			var type = new CachedType(typeof(Student));

			Assert.Equal("Students", type.Name);
		}

		[Fact]
		public void IgnoresAlreadyPluralizedNames() {
			var type = new CachedType(typeof(Tests));

			Assert.Equal("Tests", type.Name);
		}

		[Fact]
		public void PassesThroughAliasedClasses() {
			var type = new CachedType(typeof(Teachers));

			Assert.Equal("Professors", type.Name);
		}
	}
}
