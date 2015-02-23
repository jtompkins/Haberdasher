using Xunit;

namespace Haberdasher.Tests
{
	public class DelegateMethodTextFixture
	{
		private class TestClass
		{
			public int Id { get; set; }
		}

		public class GetterTests
		{
			private readonly EntityProperty _idProperty;

			public GetterTests() {
				var simpleClassType = typeof(TestClass);

				_idProperty = new EntityProperty(simpleClassType.GetProperty("Id"));
			}

			[Fact]
			public void GetterReturnsProperty() {
				var s = new TestClass { Id = 1 };

				Assert.Equal(_idProperty.Getter(s), 1);
			}
		}

		public class SetterTests
		{
			private readonly EntityProperty _idProperty;

			public SetterTests() {
				var simpleClassType = typeof(TestClass);

				_idProperty = new EntityProperty(simpleClassType.GetProperty("Id"));
			}

			[Fact]
			public void SetterSetsProperty() {
				var s = new TestClass();

				_idProperty.Setter(s, 1);

				Assert.Equal(s.Id, 1);
			}
		}

		public class ComparerTests
		{
			private readonly EntityProperty _idProperty;

			public ComparerTests() {
				var simpleClassType = typeof(TestClass);

				_idProperty = new EntityProperty(simpleClassType.GetProperty("Id"));
			}

			[Fact]
			public void ComparerComparesProperty() {
				var a = new TestClass() { Id = 1 };
				var b = new TestClass() { Id = 1 };

				Assert.True(_idProperty.Comparer(a, b));

				b.Id = 2;

				Assert.False(_idProperty.Comparer(a, b));
			}
		}
	}
}
