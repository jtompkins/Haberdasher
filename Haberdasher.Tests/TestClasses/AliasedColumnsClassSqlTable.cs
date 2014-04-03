using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haberdasher.Tests.TestClasses
{
	public class AliasedColumnsClassSqlTable : SqlTable<AliasedColumnsClass, int>
	{
		public AliasedColumnsClassSqlTable() : base("AliasedClasses", "") {}
	}
}
