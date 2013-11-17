using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haberdasher.Tests.TestClasses
{
	public class SimpleClassHaberdashery : Haberdashery<SimpleClass, int>
	{
		public SimpleClassHaberdashery() : base("SimpleClasses", "") { }
	}
}
