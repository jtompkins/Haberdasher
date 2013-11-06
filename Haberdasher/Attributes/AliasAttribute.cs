using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haberdasher.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public class AliasAttribute : Attribute
	{
		public string Alias { get; private set; }

		public AliasAttribute(string alias) {
			this.Alias = alias;
		}
	}
}
