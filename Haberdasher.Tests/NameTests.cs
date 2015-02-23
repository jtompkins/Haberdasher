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

		private class Teachers
		{
			public int Id { get; set; }
		}

		public class FluentAliasedColumnsClass
		{
			public int Id { get; set; }
			public string Name { get; set; }
			public string Description { get; set; }
		}

		[Fact]
		public void PluralizesSingularNames() {
			var type = EntityTypes.Register<Student>();

			Assert.Equal("Students", type.Name);
		}

		[Fact]
		public void IgnoresAlreadyPluralizedNames() {
			var type = EntityTypes.Register<Tests>();

			Assert.Equal("Tests", type.Name);
		}

		[Fact]
		public void AliasesTableNames() {
			var type = EntityTypes.Register<Teachers>(et => {
				et.AliasTable("Professors");
			});

			Assert.Equal("Professors", type.Name);
		}

		[Fact]
		public void SetsSingularNamesProperly() {
			var type = EntityTypes.Register<Student>(et => {
				et.Singular();
			});

			Assert.Equal("Student", type.Name);
		}

		[Fact]
		public void MarksPropertiesAsAliased() {
			var type = EntityTypes.Register<FluentAliasedColumnsClass>(t => {
				t.Alias(c => c.Name, "ADifferentName");
			});

			var property = type.GetProperty<FluentAliasedColumnsClass>(c => c.Name);

			Assert.Equal(true, property.IsAliased);
		}

		[Fact]
		public void AliasesPropertiesToProperValue() {
			var type = EntityTypes.Register<FluentAliasedColumnsClass>(t => {
				t.Alias(c => c.Name, "ADifferentName");
			});

			var property = type.GetProperty<FluentAliasedColumnsClass>(c => c.Name);

			Assert.Equal("ADifferentName", property.Name);
		}

		[Fact]
		public void DoesNotAliasPropertiesThatAreAliasedToNull() {
			var type = EntityTypes.Register<FluentAliasedColumnsClass>(t => {
				t.Alias(c => c.Name, null);
			});

			var property = type.GetProperty<FluentAliasedColumnsClass>(c => c.Name);

			Assert.Equal(false, property.IsAliased);
			Assert.Equal("Name", property.Name);
		}
	}
}
