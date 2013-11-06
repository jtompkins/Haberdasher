using System;
using Haberdasher.Support;

namespace Haberdasher.Attributes
{
	public class IgnoreAttribute : Attribute
	{
		public IgnoreTypeEnum Type { get; private set; }

		public IgnoreAttribute(IgnoreTypeEnum type) {
			Type = type;
		}
	}
}
