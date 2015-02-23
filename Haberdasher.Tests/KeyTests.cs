using System;
using System.Data;
using Haberdasher.Support;
using Xunit;

namespace Haberdasher.Tests
{
	public class KeyTestsFixure
	{
		private class KeyClass
		{
			public int Id { get; set; }
		}

		private class MissingKeyClass
		{
			public string Name { get; set; }
		}

		[Fact]
		public void FindsRegisteredKey() {
			var type = EntityTypes.Register<KeyClass>(t => {
				t.Key(c => c.Id);
			});

			var property = type.GetProperty<KeyClass>(c => c.Id);

			Assert.Equal(true, property.IsKey);
		}

		[Fact]
		public void AutodetectsKey()
		{
			var type = EntityTypes.Register<KeyClass>();

			var property = type.GetProperty<KeyClass>(c => c.Id);

			Assert.Equal(true, property.IsKey);
		}

		[Fact]
		public void MarksAutodetectedKeyAsIdentity() {
			var type = EntityTypes.Register<KeyClass>();

			var property = type.GetProperty<KeyClass>(c => c.Id);

			Assert.Equal(true, property.IsKey);
			Assert.Equal(true, property.IsIdentity);
		}

		[Fact]
		public void MarksNumericKeys() {
			var type = EntityTypes.Register<KeyClass>();

			var property = type.GetProperty<KeyClass>(c => c.Id);

			Assert.Equal(true, property.IsNumeric);
		}

		[Fact]
		public void MarksRegisteredKeysAsIdentity() {
			var type = EntityTypes.Register<KeyClass>(t => {
				t.Key(c => c.Id);
			});

			var property = type.GetProperty<KeyClass>(c => c.Id);

			Assert.Equal(true, property.IsIdentity);
		}

		[Fact]
		public void DoesNotMarkNonIdentityRegisteredKeysAsIdentity() {
			var type = EntityTypes.Register<KeyClass>(t => {
				t.Key(c => c.Id, false);
			});

			var property = type.GetProperty<KeyClass>(c => c.Id);

			Assert.Equal(false, property.IsIdentity);
		}

		[Fact]
		public void MarksKeyAsSelectable() {
			var type = EntityTypes.Register<KeyClass>();

			var property = type.GetProperty<KeyClass>(c => c.Id);

			Assert.Equal(true, property.IsSelectable);
		}

		[Fact]
		public void MarksIdentityKeyAsNotInsertable() {
			var type = EntityTypes.Register<KeyClass>();

			var property = type.GetProperty<KeyClass>(c => c.Id);

			Assert.Equal(false, property.IsInsertable);
		}

		[Fact]
		public void MarksIdentityKeyAsNotUpdatable() {
			var type = EntityTypes.Register<KeyClass>();

			var property = type.GetProperty<KeyClass>(c => c.Id);

			Assert.Equal(false, property.IsUpdatable);
		}

		[Fact]
		public void ThrowsForMissingKey() {
			Assert.Throws<MissingPrimaryKeyException>(() => EntityTypes.Register<MissingKeyClass>());
		}

		[Fact]
		public void ThrowsForNullableKey() {
			var ex = Assert.Throws<Exception>(() => {
				var type = EntityTypes.Register<KeyClass>(t => {
					t.Key(c => c.Id);
					t.Ignore(c => c.Id, IgnoreTypeEnum.Select);
				});
			});

			Assert.Equal("Cannot override ignore properties of primary key: Id", ex.Message);
		}

		[Fact]
		public void ThrowsWhenTryingToIgnoreKey() {
			var ex = Assert.Throws<Exception>(() => {
				var type = EntityTypes.Register<KeyClass>(t => {
					t.Key(c => c.Id);
					t.Nullable(c => c.Id);
				});
			});

			Assert.Equal("Cannot set primary key to be nullable: Id", ex.Message);
		}
	}
}