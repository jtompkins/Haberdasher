using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haberdasher.Tests.TestClasses
{
	public class AliasedColumnsClassEntityStore : EntityStore<AliasedColumnsClass, int>
	{
		public AliasedColumnsClassEntityStore() : base("AliasedClasses", "") {}
	}
}
