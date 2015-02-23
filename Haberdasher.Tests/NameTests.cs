using Haberdasher.Attributes;
using Xunit;

namespace Haberdasher.Tests
{
	public class NameTests
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

		public class AliasedColumnsClass
		{
			public int Id { get; set; }

			[Alias("ADifferentName")]
			public string Name { get; set; }

			[Alias(null)]
			public string Description { get; set; }
		}

		public class FluentAliasedColumnsClass
		{
			public int Id { get; set; }
			public string Name { get; set; }
		}

		[Fact]
		public void PluralizesSingularNames() {
			var type = new EntityType<Student>();

			Assert.Equal("Students", type.Name);
		}

		[Fact]
		public void IgnoresAlreadyPluralizedNames() {
			var type = new EntityType<Tests>();

			Assert.Equal("Tests", type.Name);
		}

		[Fact]
		public void PassesThroughAliasedClasses() {
			var type = new EntityType<Teachers>();

			Assert.Equal("Professors", type.Name);
		}

		[Fact]
		public void AttributeMarksPropertiesAsAliased() {
			var type = typeof(AliasedColumnsClass);

			var nameProperty = new EntityProperty(type.GetProperty("Name"));

			Assert.Equal(true, nameProperty.IsAliased);
		}

		public void FluentApiMarksPropertiesAsAliased() {
			EntityTypeCache.Register<FluentAliasedColumnsClass>(t => {
				t.Alias(c => c.Name, "ADifferentName");
			});
		}

		[Fact]
		public void CachedPropertyAliasesPropertiesToProperValue() {
			var type = typeof(AliasedColumnsClass);

			var nameProperty = new EntityProperty(type.GetProperty("Name"));

			Assert.Equal("ADifferentName", nameProperty.Alias);
		}

		[Fact]
		public void CachedPropertyDoesNotAliasPropertiesThatAreAliasedToNull() {
			var type = typeof(AliasedColumnsClass);

			var descProperty = new EntityProperty(type.GetProperty("Description"));

			Assert.Equal(false, descProperty.IsAliased);
		}
	}
}
