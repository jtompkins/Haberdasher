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
	}
}
