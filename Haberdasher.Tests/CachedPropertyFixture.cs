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
				Assert.Equal(true, _idProperty.IsKey);
			}

			[Fact]
			public void IdIsIdentity() {
				Assert.Equal(true, _idProperty.IsIdentity);
			}

			[Fact]
			public void IdUsesScopeIdentity() {
				Assert.Equal(true, _idProperty.UseScopeIdentity);
			}

			[Fact]
			public void IdIsNotAliased() {
				Assert.Equal(false, _idProperty.IsAliased);
			}

			[Fact]
			public void IdIsNumeric() {
				Assert.Equal(true, _idProperty.IsNumeric);
			}

			[Fact]
			public void IdIsNotNullable() {
				Assert.Equal(false, _idProperty.IsNullable);
			}

			[Fact]
			public void IdIsSelectable() {
				Assert.Equal(true, _idProperty.IsSelectable);
			}

			[Fact]
			public void IdIsNotInsertable() {
				Assert.Equal(false, _idProperty.IsInsertable);
			}

			[Fact]
			public void IdIsNotUpdatable() {
				Assert.Equal(false, _idProperty.IsUpdatable);
			}

			[Fact]
			public void NameIsNotAKey() {
				Assert.Equal(false, _nameProperty.IsKey);
			}

			[Fact]
			public void NameIsNotIdentity() {
				Assert.Equal(false, _nameProperty.IsIdentity);
			}

			[Fact]
			public void NameDoesNotUseScopeIdentity() {
				Assert.Equal(false, _nameProperty.UseScopeIdentity);
			}

			[Fact]
			public void NameIsNotNumeric() {
				Assert.Equal(false, _nameProperty.IsNumeric);
			}

			[Fact]
			public void NameIsNotNullable() {
				Assert.Equal(false, _nameProperty.IsNullable);
			}

			[Fact]
			public void NameIsSelectable() {
				Assert.Equal(true, _nameProperty.IsSelectable);
			}

			[Fact]
			public void NameIsInsertable() {
				Assert.Equal(true, _nameProperty.IsInsertable);
			}
			
			[Fact]
			public void NameIsUpdatable() {
				Assert.Equal(true, _nameProperty.IsUpdatable);
			}

			[Fact]
			public void NameDefaultValueIsEmptyString() {
				Assert.Equal(string.Empty, _nameProperty.DefaultValue);
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
				Assert.Equal(true, _idProperty.IsKey);
			}

			[Fact]
			public void IdIsNotIdentity() {
				Assert.Equal(false, _idProperty.IsIdentity);
			}

			[Fact]
			public void IdIsInsertable() {
				Assert.Equal(true, _idProperty.IsInsertable);
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
				Assert.Equal(true, _idProperty.IsKey);
			}

			[Fact]
			public void IdIsIdentity() {
				Assert.Equal(true, _idProperty.IsIdentity);
			}

			[Fact]
			public void IdDoesNotUseScopeIdentity() {
				Assert.Equal(false, _idProperty.UseScopeIdentity);
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
				Assert.Equal(true, _nameProperty.IsAliased);
			}

			[Fact]
			public void NameIsAliasedToProperValue() {
				Assert.Equal("ADifferentName", _nameProperty.Alias);
			}

			[Fact]
			public void NameAliasedToNullIsNotAliased() {
				Assert.Equal(false, _descriptionProperty.IsAliased);
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

				Assert.Equal(true, nameProperty.IsNullable);
			}

			[Fact]
			public void ThrowsExceptionForNullableKey() {
				var ex = Assert.Throws<Exception>(() => new CachedProperty(_type.GetProperty("Id")));

				Assert.Equal("Key properties may not be marked with the Nullable attribute: Id", ex.Message);
			}

			[Fact]
			public void ThrowsExceptionForNullableOnNonNullableProperty() {
				var ex = Assert.Throws<Exception>(() => new CachedProperty(_type.GetProperty("NonNullableProperty")));

				Assert.Equal("Non-Nullable properties may not be marked with the Nullable attribute: NonNullableProperty", ex.Message);
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
				Assert.Equal(false, _selectProperty.IsSelectable);
				Assert.Equal(true, _selectProperty.IsInsertable);
				Assert.Equal(true, _selectProperty.IsUpdatable);
			}

			[Fact]
			public void InsertPropertyIsProperlyIgnored() {
				Assert.Equal(true, _insertProperty.IsSelectable);
				Assert.Equal(false, _insertProperty.IsInsertable);
				Assert.Equal(true, _insertProperty.IsUpdatable);
			}

			[Fact]
			public void UpdatePropertyIsProperlyIgnored() {
				Assert.Equal(true, _updateProperty.IsSelectable);
				Assert.Equal(true, _updateProperty.IsInsertable);
				Assert.Equal(false, _updateProperty.IsUpdatable);
			}

			[Fact]
			public void WritePropertyIsProperlyIgnored() {
				Assert.Equal(true, _writeProperty.IsSelectable);
				Assert.Equal(false, _writeProperty.IsInsertable);
				Assert.Equal(false, _writeProperty.IsUpdatable);
			}

			[Fact]
			public void AllPropertyIsProperlyIgnored() {
				Assert.Equal(false, _allProperty.IsSelectable);
				Assert.Equal(false, _allProperty.IsInsertable);
				Assert.Equal(false, _allProperty.IsUpdatable);
			}
		}
	}
}