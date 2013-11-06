using Haberdasher.Attributes;

namespace Haberdasher.Tests.TestClasses
{
	public class NonIdentityKeyClass
	{
		[Key(false)]
		public long Id { get; set; }
	}
}
