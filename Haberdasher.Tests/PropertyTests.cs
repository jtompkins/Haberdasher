using System;
using Haberdasher.Support;
using Xunit;

namespace Haberdasher.Tests
{
	public class PropertyTests
	{
		private class TestClass
		{
			public int Id { get; set; }

			public string Name { get; set; }
			public string Address { get; set; }
			public string City { get; set; }
			public string State { get; set; }
			public string Zip { get; set; }

			public decimal Total { get; set; }
		}

		private readonly EntityType<TestClass> _testClassType;

		public PropertyTests() {
			_testClassType = EntityTypes.Register<TestClass>(te => {
				  te.Key(t => t.Id)
					.Nullable(t => t.Name)
					.Ignore(t => t.Total, IgnoreTypeEnum.Select)
					.Ignore(t => t.Address, IgnoreTypeEnum.Insert)
					.Ignore(t => t.City, IgnoreTypeEnum.Update)
					.Ignore(t => t.State, IgnoreTypeEnum.Writes)
					.Ignore(t => t.Zip, IgnoreTypeEnum.All);
			});
		}

		[Fact]
		public void DoesNotMarkNonKeyPropertiesAsKeys() {
			var type = EntityTypes.Register<TestClass>();

			var property = type.GetProperty<TestClass>(c => c.Name);

			Assert.Equal(false, property.IsKey);
		}

		[Fact]
		public void DoesNotMarkNonNumericPropertiesAsNumeric() {
			var type = EntityTypes.Register<TestClass>();

			var property = type.GetProperty<TestClass>(c => c.Name);

			Assert.Equal(false, property.IsNumeric);
		}

		[Fact]
		public void MarksPropertiesAsNullable() {
			var type = _testClassType;

			var property = type.GetProperty<TestClass>(c => c.Name);

			Assert.Equal(true, property.IsNullable);
		}

		[Fact]
		public void ThrowsForNullableOnNonNullableProperty() {
			var ex = Assert.Throws<Exception>(() => {
				EntityTypes.Register<TestClass>(te => {
					te.Nullable(t => t.Total);
				});
			});

			Assert.Equal("Cannot set non-nullable property to be nullable: Total", ex.Message);
		}

		[Fact]
		public void MarksPropertiesAsSelectable() {
			var nameProperty = _testClassType.GetProperty<TestClass>(t => t.Name);

			Assert.Equal(true, nameProperty.IsSelectable);
		}

		[Fact]
		public void MarksPropertiesAsInsertable() {
			var nameProperty = _testClassType.GetProperty<TestClass>(t => t.Name);

			Assert.Equal(true, nameProperty.IsInsertable);
		}

		[Fact]
		public void MarksPropertiesAsUpdatable() {
			var nameProperty = _testClassType.GetProperty<TestClass>(t => t.Name);

			Assert.Equal(true, nameProperty.IsUpdatable);
		}

		[Fact]
		public void CalculatesDefaultValue() {
			var nameProperty = _testClassType.GetProperty<TestClass>(t => t.Name);

			Assert.Equal(string.Empty, nameProperty.DefaultValue);
		}

		[Fact]
		public void MarksPropertiesAsNotSelectable() {
			var totalProperty = _testClassType.GetProperty<TestClass>(t => t.Total);
			var zipProperty = _testClassType.GetProperty<TestClass>(t => t.Zip);

			Assert.Equal(false, totalProperty.IsSelectable);
			Assert.Equal(false, zipProperty.IsSelectable);
		}

		[Fact]
		public void MarksPropertiesAsNotInsertable() {
			var addressProperty = _testClassType.GetProperty<TestClass>(t => t.Address);
			var stateProperty = _testClassType.GetProperty<TestClass>(t => t.State);
			var zipProperty = _testClassType.GetProperty<TestClass>(t => t.Zip);

			Assert.Equal(false, addressProperty.IsInsertable);
			Assert.Equal(false, stateProperty.IsInsertable);
			Assert.Equal(false, zipProperty.IsInsertable);
		}

		[Fact]
		public void MarksPropertiesAsNotUpdatable() {
			var cityProperty = _testClassType.GetProperty<TestClass>(t => t.City);
			var stateProperty = _testClassType.GetProperty<TestClass>(t => t.State);
			var zipProperty = _testClassType.GetProperty<TestClass>(t => t.Zip);

			Assert.Equal(false, cityProperty.IsUpdatable);
			Assert.Equal(false, stateProperty.IsUpdatable);
			Assert.Equal(false, zipProperty.IsUpdatable);
		}
	}
}
