using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haberdasher.Tests.TestClasses;
using Xunit;

namespace Haberdasher.Tests
{
	public class CachedTypeFixture
	{
		public class MissingKeyPropertyTests
		{
			[Fact]
			public void NoKeyThrowsException() {
				Assert.Throws<MissingPrimaryKeyException>(() => new CachedType(typeof (NoKeyClass)));
			}

			[Fact]
			public void KeyDoesNotThrowException() {
				Assert.DoesNotThrow(() => new CachedType(typeof(SimpleClass)));
			}
		}

		public class PropertyScanningTests
		{
			[Fact]
			public void FindsPrimaryKey() {
				var type = new CachedType(typeof (SimpleClass));

				Assert.NotNull(type.Key);
			}

			[Fact]
			public void FindsSelectProperties() {
				var type = new CachedType(typeof (IgnoredColumnsClass));

				Assert.Equal(type.SelectFields.Count(), 4);
			}

			[Fact]
			public void FindsUpdateProperties() {
				var type = new CachedType(typeof(IgnoredColumnsClass));

				Assert.Equal(type.UpdateFields.Count(), 2);
			}

			[Fact]
			public void FindsInsertProperties() {
				var type = new CachedType(typeof(IgnoredColumnsClass));

				Assert.Equal(type.InsertFields.Count(), 2);
			}
		}
	}
}
