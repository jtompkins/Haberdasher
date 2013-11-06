using System;
using Haberdasher.Tests.TestClasses;
using Xunit;

namespace Haberdasher.Tests
{
	public class CachedPropertyFixture
	{
		public class SimpleClassTests
		{
			private readonly CachedProperty _idProperty;
			private readonly CachedProperty _nameProperty;

			public SimpleClassTests() {
				var simpleClassType = typeof(SimpleClass);

				_idProperty = new CachedProperty(simpleClassType.GetProperty("Id"));
				_nameProperty = new CachedProperty(simpleClassType.GetProperty("Name"));
			}

			[Fact]
			public void IdIsKey() {
				Assert.Equal(_idProperty.IsKey, true);
			}

			[Fact]
			public void IdIsIdentity() {
				Assert.Equal(_idProperty.IsIdentity, true);
			}

			[Fact]
			public void IdUsesScopeIdentity() {
				Assert.Equal(_idProperty.UseScopeIdentity, true);
			}

			[Fact]
			public void IdIsNotAliased() {
				Assert.Equal(_idProperty.IsAliased, false);
			}

			[Fact]
			public void IdIsNumeric() {
				Assert.Equal(_idProperty.IsNumeric, true);
			}

			[Fact]
			public void IdIsNotNullable() {
				Assert.Equal(_idProperty.IsNullable, false);
			}

			[Fact]
			public void IdIsSelectable() {
				Assert.Equal(_idProperty.IsSelectable, true);
			}

			[Fact]
			public void IdIsNotInsertable() {
				Assert.Equal(_idProperty.IsInsertable, false);
			}

			[Fact]
			public void IdIsNotUpdatable() {
				Assert.Equal(_idProperty.IsUpdatable, false);
			}

			[Fact]
			public void NameIsNotAKey() {
				Assert.Equal(_nameProperty.IsKey, false);
			}

			[Fact]
			public void NameIsNotIdentity() {
				Assert.Equal(_nameProperty.IsIdentity, false);
			}

			[Fact]
			public void NameDoesNotUseScopeIdentity() {
				Assert.Equal(_nameProperty.UseScopeIdentity, false);
			}

			[Fact]
			public void NameIsNotNumeric() {
				Assert.Equal(_nameProperty.IsNumeric, false);
			}

			[Fact]
			public void NameIsNotNullable() {
				Assert.Equal(_nameProperty.IsNullable, false);
			}

			[Fact]
			public void NameIsSelectable() {
				Assert.Equal(_nameProperty.IsSelectable, true);
			}

			[Fact]
			public void NameIsInsertable() {
				Assert.Equal(_nameProperty.IsInsertable, true);
			}
			
			[Fact]
			public void NameIsUpdatable() {
				Assert.Equal(_nameProperty.IsUpdatable, true);
			}

			[Fact]
			public void NameDefaultValueIsEmptyString() {
				Assert.Equal(_nameProperty.DefaultValue, string.Empty);
			}
		}

		public class NonIdentityKeyClassTests
		{
			private readonly CachedProperty _idProperty;

			public NonIdentityKeyClassTests() {
				var type = typeof(NonIdentityKeyClass);

				_idProperty = new CachedProperty(type.GetProperty("Id"));
			}

			[Fact]
			public void IdIsKey() {
				Assert.Equal(_idProperty.IsKey, true);
			}

			[Fact]
			public void IdIsNotIdentity() {
				Assert.Equal(_idProperty.IsIdentity, false);
			}

			[Fact]
			public void IdIsInsertable() {
				Assert.Equal(_idProperty.IsInsertable, true);
			}
		}

		public class NonScopeIdentityKeyClassTests
		{
			private readonly CachedProperty _idProperty;

			public NonScopeIdentityKeyClassTests() {
				var type = typeof(NonScopeIdentityKeyClass);

				_idProperty = new CachedProperty(type.GetProperty("Id"));
			}

			[Fact]
			public void IdIsKey() {
				Assert.Equal(_idProperty.IsKey, true);
			}

			[Fact]
			public void IdIsIdentity() {
				Assert.Equal(_idProperty.IsIdentity, true);
			}

			[Fact]
			public void IdDoesNotUseScopeIdentity() {
				Assert.Equal(_idProperty.UseScopeIdentity, false);
			}
		}

		public class AliasedColumnsClassTests
		{
			private readonly CachedProperty _nameProperty;
			private readonly CachedProperty _descriptionProperty;

			public AliasedColumnsClassTests() {
				var type = typeof(AliasedColumnsClass);

				_nameProperty = new CachedProperty(type.GetProperty("Name"));
				_descriptionProperty = new CachedProperty(type.GetProperty("Description"));
			}

			[Fact]
			public void NameIsAliased() {
				Assert.Equal(_nameProperty.IsAliased, true);
			}

			[Fact]
			public void NameIsAliasedToProperValue() {
				Assert.Equal(_nameProperty.Alias, "ADifferentName");
			}

			[Fact]
			public void NameAliasedToNullIsNotAliased() {
				Assert.Equal(_descriptionProperty.IsAliased, false);
			}
		}

		public class NullableColumnsClassTests
		{
			private readonly Type _type;

			public NullableColumnsClassTests() {
				_type = typeof(NullableColumnsClass);
			}

			[Fact]
			public void NameIsNullable() {
				var nameProperty = new CachedProperty(_type.GetProperty("Name"));

				Assert.Equal(nameProperty.IsNullable, true);
			}

			[Fact]
			public void ThrowsExceptionForNullableKey() {
				var ex = Assert.Throws<Exception>(() => new CachedProperty(_type.GetProperty("Id")));

				Assert.Equal(ex.Message, "Key properties may not be marked with the Nullable attribute: Id");
			}

			[Fact]
			public void ThrowsExceptionForNullableOnNonNullableProperty() {
				var ex = Assert.Throws<Exception>(() => new CachedProperty(_type.GetProperty("NonNullableProperty")));

				Assert.Equal(ex.Message, "Non-Nullable properties may not be marked with the Nullable attribute: NonNullableProperty");
			}
		}

		public class IgnoredColumnsClassTests
		{
			private readonly CachedProperty _selectProperty;
			private readonly CachedProperty _insertProperty;
			private readonly CachedProperty _updateProperty;
			private readonly CachedProperty _writeProperty;
			private readonly CachedProperty _allProperty;

			public IgnoredColumnsClassTests() {
				var type = typeof(IgnoredColumnsClass);

				_selectProperty = new CachedProperty(type.GetProperty("IgnoredForSelect"));
				_insertProperty = new CachedProperty(type.GetProperty("IgnoredForInsert"));
				_updateProperty = new CachedProperty(type.GetProperty("IgnoredForUpdate"));
				_writeProperty = new CachedProperty(type.GetProperty("IgnoredForWrites"));
				_allProperty = new CachedProperty(type.GetProperty("IgnoredAlways"));
			}

			[Fact]
			public void SelectPropertyIsProperlyIgnored() {
				Assert.Equal(_selectProperty.IsSelectable, false);
				Assert.Equal(_selectProperty.IsInsertable, true);
				Assert.Equal(_selectProperty.IsUpdatable, true);
			}

			[Fact]
			public void InsertPropertyIsProperlyIgnored() {
				Assert.Equal(_insertProperty.IsSelectable, true);
				Assert.Equal(_insertProperty.IsInsertable, false);
				Assert.Equal(_insertProperty.IsUpdatable, true);
			}

			[Fact]
			public void UpdatePropertyIsProperlyIgnored() {
				Assert.Equal(_updateProperty.IsSelectable, true);
				Assert.Equal(_updateProperty.IsInsertable, true);
				Assert.Equal(_updateProperty.IsUpdatable, false);
			}

			[Fact]
			public void WritePropertyIsProperlyIgnored() {
				Assert.Equal(_writeProperty.IsSelectable, true);
				Assert.Equal(_writeProperty.IsInsertable, false);
				Assert.Equal(_writeProperty.IsUpdatable, false);
			}

			[Fact]
			public void AllPropertyIsProperlyIgnored() {
				Assert.Equal(_allProperty.IsSelectable, false);
				Assert.Equal(_allProperty.IsInsertable, false);
				Assert.Equal(_allProperty.IsUpdatable, false);
			}
		}
	}
}