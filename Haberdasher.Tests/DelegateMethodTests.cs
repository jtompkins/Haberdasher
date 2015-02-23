using Xunit;

namespace Haberdasher.Tests
{
	public class DelegateMethodTests
	{
		private class TestClass
		{
			public int Id { get; set; }
		}

		public class GetterTests
		{
			[Fact]
			public void GetterReturnsProperty() {
				var type = EntityTypes.Register<TestClass>();
				var idProperty = type.GetProperty<TestClass>(t => t.Id);
				var test = new TestClass { Id = 1 };

				Assert.Equal(idProperty.Getter(test), 1);
			}
		}

		public class SetterTests
		{
			[Fact]
			public void SetterSetsProperty() {
				var type = EntityTypes.Register<TestClass>();
				var idProperty = type.GetProperty<TestClass>(t => t.Id);
				var test = new TestClass();

				idProperty.Setter(test, 1);

				Assert.Equal(test.Id, 1);
			}
		}

		public class ComparerTests
		{
			[Fact]
			public void ComparerComparesProperty() {
				var type = EntityTypes.Register<TestClass>();
				var idProperty = type.GetProperty<TestClass>(t => t.Id);
				var a = new TestClass() { Id = 1 };
				var b = new TestClass() { Id = 1 };

				Assert.True(idProperty.Comparer(a, b));

				b.Id = 2;

				Assert.False(idProperty.Comparer(a, b));
			}
		}
	}
}
