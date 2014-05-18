using System;

namespace Haberdasher.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public class KeyAttribute : Attribute
	{
		public bool IsIdentity { get; private set; }

		public KeyAttribute(bool isIdentity = true) {
			IsIdentity = isIdentity;
		}
	}
}
