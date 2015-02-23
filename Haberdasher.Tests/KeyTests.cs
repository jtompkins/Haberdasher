using System.Data;
using Xunit;

namespace Haberdasher.Tests
{
	public class KeyTestsFixure
	{
		//private class KeyClass
		//{
		//	public int Id { get; set; }
		//}

		//private class MissingKeyClass
		//{
		//	public string Name { get; set; }
		//}

		//private class MarkedKeyClass
		//{
		//	[Key]
		//	public int Id { get; set; }
		//}

		//private class NonIdentityKeyClass
		//{
		//	[Key(false)]
		//	public int Id { get; set; }
		//}

		//[Fact]
		//public void CachedPropertyFindsMarkedKey() {
		//	var type = typeof(MarkedKeyClass);

		//	var idProperty = new EntityProperty(type.GetProperty("Id"));

		//	Assert.Equal(true, idProperty.IsKey);
		//}

		//[Fact]
		//public void CachedPropertyFindsUnmarkedKey() {
		//	var type = typeof(KeyClass);

		//	var idProperty = new EntityProperty(type.GetProperty("Id"));

		//	Assert.Equal(true, idProperty.IsKey);
		//}

		//[Fact]
		//public void CachedPropertyMarksUnmarkedKeyAsIdentity() {
		//	var type = typeof(KeyClass);

		//	var idProperty = new EntityProperty(type.GetProperty("Id"));

		//	Assert.Equal(true, idProperty.IsKey);
		//	Assert.Equal(true, idProperty.IsIdentity);
		//}

		//[Fact]
		//public void CachedPropertyMarksNumericKeys() {
		//	var type = typeof(MarkedKeyClass);

		//	var idProperty = new EntityProperty(type.GetProperty("Id"));

		//	Assert.Equal(true, idProperty.IsNumeric);
		//}

		//[Fact]
		//public void CachedPropertyMarksIdentityKeys() {
		//	var type = typeof(MarkedKeyClass);

		//	var idProperty = new EntityProperty(type.GetProperty("Id"));

		//	Assert.Equal(true, idProperty.IsIdentity);
		//}

		//[Fact]
		//public void CachedPropertyMarksNonIdentityKeys() {
		//	var type = typeof(NonIdentityKeyClass);

		//	var idProperty = new EntityProperty(type.GetProperty("Id"));

		//	Assert.Equal(false, idProperty.IsIdentity);
		//}

		//[Fact]
		//public void CachedPropertyMarksIdAsSelectable() {
		//	var type = typeof(MarkedKeyClass);

		//	var idProperty = new EntityProperty(type.GetProperty("Id"));

		//	Assert.Equal(true, idProperty.IsSelectable);
		//}

		//[Fact]
		//public void CachedPropertyMarksIdAsNotInsertable() {
		//	var type = typeof(MarkedKeyClass);

		//	var idProperty = new EntityProperty(type.GetProperty("Id"));

		//	Assert.Equal(false, idProperty.IsInsertable);
		//}

		//[Fact]
		//public void CachedPropertyMarksIdAsNotUpdatable() {
		//	var type = typeof(MarkedKeyClass);

		//	var idProperty = new EntityProperty(type.GetProperty("Id"));

		//	Assert.Equal(false, idProperty.IsUpdatable);
		//}

		//[Fact]
		//public void CachedTypeThrowsForMissingKey() {
		//	Assert.Throws<MissingPrimaryKeyException>(() => new EntityType<MissingKeyClass>());
		//}
	}
}
