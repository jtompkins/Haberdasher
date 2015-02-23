using System;
using Haberdasher.Support;
using Xunit;

namespace Haberdasher.Tests
{
	public class PropertyTestsFixture
	{
		//private class TestClass
		//{
		//	public int Id { get; set; }
		//	public string Name { get; set; }
		//}

		//private class NullableAndIgnorableClass
		//{
		//	public int Id { get; set; }

		//	[Nullable]
		//	[Ignore(IgnoreTypeEnum.Select)]
		//	public string Name { get; set; }

		//	[Ignore(IgnoreTypeEnum.Insert)]
		//	public string Address { get; set; }

		//	[Ignore(IgnoreTypeEnum.Update)]
		//	public string City { get; set; }

		//	[Ignore(IgnoreTypeEnum.Writes)]
		//	public string State { get; set; }

		//	[Ignore(IgnoreTypeEnum.All)]
		//	public string Zip { get; set; }
		//}

		//private class NullableKeyClass
		//{
		//	[Nullable]
		//	[Key]
		//	public int? Id { get; set; }

		//	[Nullable]
		//	public decimal Total { get; set; }
		//}

		

		//[Fact]
		//public void CachedPropertyDoesNotMarkNonKeyPropertiesAsKeys() {
		//	var type = typeof(TestClass);

		//	var nameProperty = new EntityProperty(type.GetProperty("Name"));

		//	Assert.Equal(false, nameProperty.IsKey);
		//}

		//[Fact]
		//public void CachedPropertyDoesNotMarkNonNumericPropertiesAsNumeric() {
		//	var type = typeof(TestClass);

		//	var nameProperty = new EntityProperty(type.GetProperty("Name"));

		//	Assert.Equal(false, nameProperty.IsNumeric);
		//}

		//[Fact]
		//public void CachedPropertyMarksPropertiesAsNullable() {
		//	var type = typeof(NullableAndIgnorableClass);

		//	var nameProperty = new EntityProperty(type.GetProperty("Name"));

		//	Assert.Equal(true, nameProperty.IsNullable);
		//}

		//[Fact]
		//public void CachedPropertyThrowsExceptionForNullableKey() {
		//	var type = typeof(NullableKeyClass);

		//	var ex = Assert.Throws<Exception>(() => new EntityProperty(type.GetProperty("Id")));

		//	Assert.Equal("Key properties may not be marked with the Nullable attribute: Id", ex.Message);
		//}

		//[Fact]
		//public void CachedPropertyThrowsExceptionForNullableOnNonNullableProperty() {
		//	var type = typeof(NullableKeyClass);

		//	var ex = Assert.Throws<Exception>(() => new EntityProperty(type.GetProperty("Total")));

		//	Assert.Equal("Non-Nullable value type properties may not be marked with the Nullable attribute: Total", ex.Message);
		//}

		//[Fact]
		//public void CachedPropertyMarksPropertiesAsSelectable() {
		//	var type = typeof(TestClass);

		//	var nameProperty = new EntityProperty(type.GetProperty("Name"));

		//	Assert.Equal(true, nameProperty.IsSelectable);
		//}

		//[Fact]
		//public void CachedPropertyMarksPropertiesAsInsertable() {
		//	var type = typeof(TestClass);

		//	var nameProperty = new EntityProperty(type.GetProperty("Name"));

		//	Assert.Equal(true, nameProperty.IsInsertable);
		//}

		//[Fact]
		//public void CachedPropertyMarksPropertiesAsUpdatable() {
		//	var type = typeof(TestClass);

		//	var nameProperty = new EntityProperty(type.GetProperty("Name"));

		//	Assert.Equal(true, nameProperty.IsUpdatable);
		//}

		//[Fact]
		//public void CachedPropertyCalculatesDefaultValue() {
		//	var type = typeof(TestClass);

		//	var nameProperty = new EntityProperty(type.GetProperty("Name"));

		//	Assert.Equal(string.Empty, nameProperty.DefaultValue);
		//}

		//[Fact]
		//public void CachedPropertyMarksPropertiesAsNotSelectable() {
		//	var type = typeof(NullableAndIgnorableClass);

		//	var nameProperty = new EntityProperty(type.GetProperty("Name"));
		//	var zipProperty = new EntityProperty(type.GetProperty("Zip"));

		//	Assert.Equal(false, nameProperty.IsSelectable);
		//	Assert.Equal(false, zipProperty.IsSelectable);
		//}

		//[Fact]
		//public void CachedPropertyMarksPropertiesAsNotInsertable() {
		//	var type = typeof(NullableAndIgnorableClass);

		//	var addressProperty = new EntityProperty(type.GetProperty("Address"));
		//	var stateProperty = new EntityProperty(type.GetProperty("State"));
		//	var zipProperty = new EntityProperty(type.GetProperty("Zip"));

		//	Assert.Equal(false, addressProperty.IsInsertable);
		//	Assert.Equal(false, stateProperty.IsInsertable);
		//	Assert.Equal(false, zipProperty.IsInsertable);
		//}

		//[Fact]
		//public void CachedPropertyMarksPropertiesAsNotUpdatable() {
		//	var type = typeof(NullableAndIgnorableClass);

		//	var cityProperty = new EntityProperty(type.GetProperty("City"));
		//	var stateProperty = new EntityProperty(type.GetProperty("State"));
		//	var zipProperty = new EntityProperty(type.GetProperty("Zip"));

		//	Assert.Equal(false, cityProperty.IsUpdatable);
		//	Assert.Equal(false, stateProperty.IsUpdatable);
		//	Assert.Equal(false, zipProperty.IsUpdatable);
		//}

		
	}
}
