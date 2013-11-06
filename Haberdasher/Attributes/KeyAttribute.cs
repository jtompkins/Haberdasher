using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
