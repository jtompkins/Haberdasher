using System.Linq;
using Xunit;

namespace Haberdasher.Tests
{
	public class EntityStoreTests
	{
		private class TestClass
		{
			public int Id { get; set; }
			public string Name { get; set; }
		}

		private class AliasedColumnsClass
		{
			public int Id { get; set; }
			public string Name { get; set; }
			public string Description { get; set; }
		}

		private readonly EntityType<TestClass> _simpleType;
		private readonly EntityType<AliasedColumnsClass> _aliasedType;

		private readonly EntityStore<TestClass, int> _simpleEntityStore;
		private readonly EntityStore<AliasedColumnsClass, int> _aliasedEntityStore;

		public EntityStoreTests() {
			_simpleType = EntityTypes.Register<TestClass>();
			_aliasedType = EntityTypes.Register<AliasedColumnsClass>(et => {
				  et.Alias(t => t.Name, "ADifferentName")
					.Alias(t => t.Description, null);
			});

			_simpleEntityStore = new EntityStore<TestClass, int>();
			_aliasedEntityStore = new EntityStore<AliasedColumnsClass, int>();
		}

		[Fact]
		public void BuildsSimpleParameterListCorrectly() {
			var simple = new TestClass { Id = 1, Name = "Simple Class" };

			var parameters = _simpleEntityStore.BuildParameterList(_simpleType.SelectFields, simple);

			Assert.Equal(2, parameters.ParameterNames.Count());
			Assert.Equal(true, parameters.ParameterNames.Contains("Id"));
			Assert.Equal(true, parameters.ParameterNames.Contains("Name"));
		}

		[Fact]
		public void BuildsAliasedParameterListCorrectly() {
			var aliased = new AliasedColumnsClass() { Id = 1, Name = "Aliased Class", Description = "Aliased Description" };

			var parameters = _aliasedEntityStore.BuildParameterList(_aliasedType.SelectFields, aliased);

			Assert.Equal(3, parameters.ParameterNames.Count());
			Assert.True(parameters.ParameterNames.Contains("Id"));
			Assert.True(parameters.ParameterNames.Contains("ADifferentName"));
			Assert.True(parameters.ParameterNames.Contains("Description"));
		}

		[Fact]
		public void BuildsSimplePropertyListCorrectly() {
			var properties = _simpleEntityStore.BuildPropertyList(_simpleType.SelectFields);

			Assert.Equal(2, properties.Count());
			Assert.True(properties.ContainsKey("@Id"));
			Assert.True(properties.ContainsKey("@Name"));
		}

		[Fact]
		public void BuildsAliasedPropertyListCorrectly() {
			var properties = _aliasedEntityStore.BuildPropertyList(_aliasedType.SelectFields);

			Assert.Equal(3, properties.Count());
			Assert.True(properties.ContainsKey("@Id"));
			Assert.True(properties.ContainsKey("@ADifferentName"));
			Assert.True(properties.ContainsKey("@Description"));
		}
	}
}
