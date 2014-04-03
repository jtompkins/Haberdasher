using Haberdasher.Contrib.Oracle;
using Haberdasher.Tests.TestClasses;

namespace Haberdasher.Contrib.Tests.Oracle
{

    /// <summary>
    /// the SimpleClassOracleHaberdashery is an Oracle Haberdashery to store SimpleClass entities 
    /// in the SIMPLE_CLASSES table using the connection string named "OracleTest"
    /// </summary>
	public class SimpleClassOracleSqlTable : OracleSqlTable<SimpleClass, int>
	{
        public SimpleClassOracleSqlTable() : base("SIMPLE_CLASSES", "OracleTest") { }
	}

    /// <summary>
    /// the NonIdentityKeyOracleHaberdashery is an Oracle Haberdashery to store NonIdentityKeyClass entities 
    /// in the SIMPLE_CLASSES table using the connection string named "OracleTest"
    /// </summary>
    /// <remarks>SimpleClass and NonIdentityKeyClass have the same definition</remarks>
    public class NonIdentityKeyOracleSqlTable : OracleSqlTable<NonIdentityKeyClass, long>
    {
        public NonIdentityKeyOracleSqlTable() : base("SIMPLE_CLASSES", "OracleTest") { }
    }

}
