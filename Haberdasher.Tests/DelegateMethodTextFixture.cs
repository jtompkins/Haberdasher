using Haberdasher.Tests.TestClasses;
using Xunit;

namespace Haberdasher.Tests
{
	public class DelegateMethodTextFixture
	{
		public class GetterTests
		{
			private readonly CachedProperty _idProperty;

			public GetterTests() {
				var simpleClassType = typeof(SimpleClass);

				_idProperty = new CachedProperty(simpleClassType.GetProperty("Id"));
			}

			[Fact]
			public void GetterReturnsProperty() {
				var s = new SimpleClass { Id = 1 };

				Assert.Equal(_idProperty.Getter(s), 1);
			}
		}

		public class SetterTests
		{
			private readonly CachedProperty _idProperty;

			public SetterTests() {
				var simpleClassType = typeof(SimpleClass);

				_idProperty = new CachedProperty(simpleClassType.GetProperty("Id"));
			}

			[Fact]
			public void SetterSetsProperty() {
				var s = new SimpleClass();

				_idProperty.Setter(s, 1);

				Assert.Equal(s.Id, 1);
			}
		}

		public class ComparerTests
		{
			private readonly CachedProperty _idProperty;

			public ComparerTests() {
				var simpleClassType = typeof(SimpleClass);

				_idProperty = new CachedProperty(simpleClassType.GetProperty("Id"));
			}

			[Fact]
			public void ComparerComparesProperty() {
				var a = new SimpleClass() { Id = 1 };
				var b = new SimpleClass() { Id = 1 };

				Assert.True(_idProperty.Comparer(a, b));

				b.Id = 2;

				Assert.False(_idProperty.Comparer(a, b));
			}
		}
	}
}
