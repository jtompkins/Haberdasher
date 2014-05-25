using System;

namespace Haberdasher.Attributes
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
	public class AliasAttribute : Attribute
	{
		public string Alias { get; private set; }

		public AliasAttribute(string alias) {
			Alias = alias;
		}
	}
}
