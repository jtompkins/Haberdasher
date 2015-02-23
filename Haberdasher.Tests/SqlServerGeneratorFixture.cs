using System.Collections.Generic;
using Haberdasher.QueryGenerators;
using Haberdasher.Support.Helpers;
using Xunit;

namespace Haberdasher.Tests
{
	public class SqlServerGeneratorFixture
	{
		//private class TestClass
		//{
		//	public int Id { get; set; }
		//	public string Name { get; set; }
		//}

		//[Alias("TestClasses")]
		//private class AliasedTestClass
		//{
		//	public int Id { get; set; }
		//	public string Name { get; set; }
		//}

		//private class NonIdentityKeyClass
		//{
		//	[Key(false)]
		//	public int Id { get; set; }
		//}

		//private class AliasedColumnsClass
		//{
		//	public int Id { get; set; }

		//	[Alias("ADifferentName")]
		//	public string Name { get; set; }

		//	[Alias(null)]
		//	public string Description { get; set; }
		//}

		//public class TestClassTests
		//{
		//	private const string Table = "TestClasses";

		//	private readonly EntityProperty _idProperty;
		//	private readonly EntityProperty _nameProperty;

		//	private readonly SqlServerGenerator _queryGenerator;

		//	public TestClassTests() {
		//		var simpleClassType = typeof(TestClass);

		//		_idProperty = new EntityProperty(simpleClassType.GetProperty("Id"));
		//		_nameProperty = new EntityProperty(simpleClassType.GetProperty("Name"));

		//		_queryGenerator = new SqlServerGenerator();
		//	}

		//	[Fact]
		//	public void CreatesWellFormedSelect() {
		//		var sql = _queryGenerator.Select(Table, new List<EntityProperty>() { _idProperty, _nameProperty }, _idProperty, "@id");
		//		var expectedSql = "select Id, Name from [TestClasses] where Id = @id";

		//		Assert.Equal(expectedSql, sql);
		//	}

		//	[Fact]
		//	public void CreatesWellFormedSelectMany() {
		//		var sql = _queryGenerator.SelectMany(Table, new List<EntityProperty>() { _idProperty, _nameProperty }, _idProperty, "@ids");
		//		var expectedSql = "select Id, Name from [TestClasses] where Id in @ids";

		//		Assert.Equal(expectedSql, sql);
		//	}

		//	[Fact]
		//	public void CreatesWellFormedInsert() {
		//		var properties = new Dictionary<string, EntityProperty>();

		//		properties.Add("@name", _nameProperty);

		//		var sql = _queryGenerator.Insert(Table, properties, _idProperty);
		//		var expectedSql = "set nocount on insert into [TestClasses] (Name) values (@name) select SCOPE_IDENTITY()";

		//		Assert.Equal(expectedSql, sql);
		//	}

		//	[Fact]
		//	public void CreatesWellFormedUpdate() {
		//		var properties = new Dictionary<string, EntityProperty>();

		//		properties.Add("@name", _nameProperty);

		//		var sql = _queryGenerator.Update(Table, properties, _idProperty, "@id");
		//		var expectedSql = "update [TestClasses] set Name = @name where Id = @id";

		//		Assert.Equal(expectedSql, sql);
		//	}

		//	[Fact]
		//	public void CreatesWellFormedUpdateMany() {
		//		var properties = new Dictionary<string, EntityProperty>();

		//		properties.Add("@name", _nameProperty);

		//		var sql = _queryGenerator.UpdateMany(Table, properties, _idProperty, "@ids");
		//		var expectedSql = "update [TestClasses] set Name = @name where Id in @ids";

		//		Assert.Equal(expectedSql, sql);
		//	}

		//	[Fact]
		//	public void CreatesWellFormedDeleteAll() {
		//		var sql = _queryGenerator.DeleteAll(Table);
		//		var expectedSql = "truncate table [TestClasses]";

		//		Assert.Equal(expectedSql, sql);
		//	}

		//	[Fact]
		//	public void CreatesWellFormedDelete() {
		//		var sql = _queryGenerator.Delete(Table, _idProperty, "@id");
		//		var expectedSql = "delete from [TestClasses] where Id = @id";

		//		Assert.Equal(expectedSql, sql);
		//	}

		//	[Fact]
		//	public void CreatesWellFormedDeleteMany() {
		//		var sql = _queryGenerator.DeleteMany(Table, _idProperty, "@ids");
		//		var expectedSql = "delete from [TestClasses] where Id in @ids";

		//		Assert.Equal(expectedSql, sql);
		//	}

		//	[Fact]
		//	public void CreatesWellFormedSelectAll() {
		//		var properties = new List<EntityProperty>() { _idProperty, _nameProperty };

		//		var sql = _queryGenerator.SelectAll(Table, properties, _idProperty);
		//		var expectedSql = "select Id, Name from [TestClasses] order by Id";

		//		Assert.Equal(expectedSql, sql);
		//	}

		//	[Fact]
		//	public void CreatesWellFormedFind() {
		//		var properties = new List<EntityProperty>() { _idProperty, _nameProperty };
		//		var where = "Name like '%test%'";

		//		var sql = _queryGenerator.Find(Table, properties, where);
		//		var expectedSql = "select Id, Name from [TestClasses] where " + where;

		//		Assert.Equal(expectedSql, sql);
		//	}

		//	[Fact]
		//	public void CreatesWellFormedFindOne() {
		//		var properties = new List<EntityProperty>() { _idProperty, _nameProperty };
		//		var where = "Name like '%test%'";

		//		var sql = _queryGenerator.FindOne(Table, properties, where);
		//		var expectedSql = "select top 1 Id, Name from [TestClasses] where " + where;

		//		Assert.Equal(expectedSql, sql);
		//	}
		//}

		//public class NonIdentityKeyClassTests
		//{
		//	private const string Table = "NonIdentityKeyClasses";

		//	private readonly EntityProperty _idProperty;

		//	private readonly SqlServerGenerator _queryGenerator;

		//	public NonIdentityKeyClassTests() {
		//		var type = typeof(NonIdentityKeyClass);

		//		_idProperty = new EntityProperty(type.GetProperty("Id"));

		//		_queryGenerator = new SqlServerGenerator();
		//	}

		//	[Fact]
		//	public void CreatesWellFormedInsert() {
		//		var properties = new Dictionary<string, EntityProperty>();

		//		properties.Add("@id", _idProperty);

		//		var sql = _queryGenerator.Insert(Table, properties, _idProperty);
		//		var expectedSql = "insert into [NonIdentityKeyClasses] (Id) values (@id)";

		//		Assert.Equal(expectedSql, sql);
		//	}
		//}

		//public class AliasedTestClassTests
		//{
		//	private readonly string _table;

		//	private readonly EntityProperty _idProperty;
		//	private readonly EntityProperty _nameProperty;

		//	private readonly SqlServerGenerator _queryGenerator;

		//	public AliasedTestClassTests() {
		//		var aliasedClassType = typeof(AliasedTestClass);
		//		_table = NameHelper.GetEntityTableName(aliasedClassType);


		//		_idProperty = new EntityProperty(aliasedClassType.GetProperty("Id"));
		//		_nameProperty = new EntityProperty(aliasedClassType.GetProperty("Name"));

		//		_queryGenerator = new SqlServerGenerator();
		//	}

		//	[Fact]
		//	public void CreatesWellFormedSelect() {
		//		var sql = _queryGenerator.Select(_table, new List<EntityProperty>() { _idProperty, _nameProperty }, _idProperty, "@id");
		//		var expectedSql = "select Id, Name from [TestClasses] where Id = @id";

		//		Assert.Equal(expectedSql, sql);
		//	}

		//	[Fact]
		//	public void CreatesWellFormedSelectMany() {
		//		var sql = _queryGenerator.SelectMany(_table, new List<EntityProperty>() { _idProperty, _nameProperty }, _idProperty, "@ids");
		//		var expectedSql = "select Id, Name from [TestClasses] where Id in @ids";

		//		Assert.Equal(expectedSql, sql);
		//	}

		//	[Fact]
		//	public void CreatesWellFormedInsert() {
		//		var properties = new Dictionary<string, EntityProperty>();

		//		properties.Add("@name", _nameProperty);

		//		var sql = _queryGenerator.Insert(_table, properties, _idProperty);
		//		var expectedSql = "set nocount on insert into [TestClasses] (Name) values (@name) select SCOPE_IDENTITY()";

		//		Assert.Equal(expectedSql, sql);
		//	}

		//	[Fact]
		//	public void CreatesWellFormedUpdate() {
		//		var properties = new Dictionary<string, EntityProperty>();

		//		properties.Add("@name", _nameProperty);

		//		var sql = _queryGenerator.Update(_table, properties, _idProperty, "@id");
		//		var expectedSql = "update [TestClasses] set Name = @name where Id = @id";

		//		Assert.Equal(expectedSql, sql);
		//	}

		//	[Fact]
		//	public void CreatesWellFormedUpdateMany() {
		//		var properties = new Dictionary<string, EntityProperty>();

		//		properties.Add("@name", _nameProperty);

		//		var sql = _queryGenerator.UpdateMany(_table, properties, _idProperty, "@ids");
		//		var expectedSql = "update [TestClasses] set Name = @name where Id in @ids";

		//		Assert.Equal(expectedSql, sql);
		//	}

		//	[Fact]
		//	public void CreatesWellFormedDeleteAll() {
		//		var sql = _queryGenerator.DeleteAll(_table);
		//		var expectedSql = "truncate table [TestClasses]";

		//		Assert.Equal(expectedSql, sql);
		//	}

		//	[Fact]
		//	public void CreatesWellFormedDelete() {
		//		var sql = _queryGenerator.Delete(_table, _idProperty, "@id");
		//		var expectedSql = "delete from [TestClasses] where Id = @id";

		//		Assert.Equal(expectedSql, sql);
		//	}

		//	[Fact]
		//	public void CreatesWellFormedDeleteMany() {
		//		var sql = _queryGenerator.DeleteMany(_table, _idProperty, "@ids");
		//		var expectedSql = "delete from [TestClasses] where Id in @ids";

		//		Assert.Equal(expectedSql, sql);
		//	}

		//	[Fact]
		//	public void CreatesWellFormedSelectAll() {
		//		var properties = new List<EntityProperty>() { _idProperty, _nameProperty };

		//		var sql = _queryGenerator.SelectAll(_table, properties, _idProperty);
		//		var expectedSql = "select Id, Name from [TestClasses] order by Id";

		//		Assert.Equal(expectedSql, sql);
		//	}

		//	[Fact]
		//	public void CreatesWellFormedFind() {
		//		var properties = new List<EntityProperty>() { _idProperty, _nameProperty };
		//		var where = "Name like '%test%'";

		//		var sql = _queryGenerator.Find(_table, properties, where);
		//		var expectedSql = "select Id, Name from [TestClasses] where " + where;

		//		Assert.Equal(expectedSql, sql);
		//	}

		//	[Fact]
		//	public void CreatesWellFormedFindOne() {
		//		var properties = new List<EntityProperty>() { _idProperty, _nameProperty };
		//		var where = "Name like '%test%'";

		//		var sql = _queryGenerator.FindOne(_table, properties, where);
		//		var expectedSql = "select top 1 Id, Name from [TestClasses] where " + where;

		//		Assert.Equal(expectedSql, sql);
		//	}
		//}

		//public class AliasedColumnsClassTests
		//{
		//	private const string Table = "AliasedColumnsClasses";
		//	private readonly EntityProperty _idProperty;
		//	private readonly EntityProperty _nameProperty;
		//	private readonly EntityProperty _descriptionProperty;

		//	private readonly SqlServerGenerator _queryGenerator;

		//	public AliasedColumnsClassTests() {
		//		var type = typeof(AliasedColumnsClass);

		//		_idProperty = new EntityProperty(type.GetProperty("Id"));
		//		_nameProperty = new EntityProperty(type.GetProperty("Name"));
		//		_descriptionProperty = new EntityProperty(type.GetProperty("Description"));

		//		_queryGenerator = new SqlServerGenerator();
		//	}

		//	[Fact]
		//	public void CreatesWellFormedSelect() {
		//		var sql = _queryGenerator.Select(Table, new List<EntityProperty>() { _idProperty, _nameProperty, _descriptionProperty }, _idProperty, "@id");
		//		var expectedSql = "select Id, ADifferentName as Name, Description from [AliasedColumnsClasses] where Id = @id";

		//		Assert.Equal(expectedSql, sql);
		//	}

		//	[Fact]
		//	public void CreatesWellFormedSelectMany() {
		//		var sql = _queryGenerator.SelectMany(Table, new List<EntityProperty>() { _idProperty, _nameProperty, _descriptionProperty }, _idProperty, "@ids");
		//		var expectedSql = "select Id, ADifferentName as Name, Description from [AliasedColumnsClasses] where Id in @ids";

		//		Assert.Equal(expectedSql, sql);
		//	}

		//	[Fact]
		//	public void CreatesWellFormedInsert() {
		//		var properties = new Dictionary<string, EntityProperty>();

		//		properties.Add("@name", _nameProperty);
		//		properties.Add("@description", _descriptionProperty);

		//		var sql = _queryGenerator.Insert(Table, properties, _idProperty);
		//		var expectedSql = "set nocount on insert into [AliasedColumnsClasses] (ADifferentName, Description) values (@name, @description) select SCOPE_IDENTITY()";

		//		Assert.Equal(expectedSql, sql);
		//	}

		//	[Fact]
		//	public void CreatesWellFormedUpdate() {
		//		var properties = new Dictionary<string, EntityProperty>();

		//		properties.Add("@name", _nameProperty);
		//		properties.Add("@description", _descriptionProperty);

		//		var sql = _queryGenerator.Update(Table, properties, _idProperty, "@id");
		//		var expectedSql = "update [AliasedColumnsClasses] set ADifferentName = @name, Description = @description where Id = @id";

		//		Assert.Equal(expectedSql, sql);
		//	}

		//	[Fact]
		//	public void CreatesWellFormedUpdateMany() {
		//		var properties = new Dictionary<string, EntityProperty>();

		//		properties.Add("@name", _nameProperty);
		//		properties.Add("@description", _descriptionProperty);

		//		var sql = _queryGenerator.UpdateMany(Table, properties, _idProperty, "@ids");
		//		var expectedSql = "update [AliasedColumnsClasses] set ADifferentName = @name, Description = @description where Id in @ids";

		//		Assert.Equal(expectedSql, sql);
		//	}

		//	[Fact]
		//	public void CreatesWellFormedSelectAll() {
		//		var properties = new List<EntityProperty>() { _idProperty, _nameProperty, _descriptionProperty };

		//		var sql = _queryGenerator.SelectAll(Table, properties, _idProperty);
		//		var expectedSql = "select Id, ADifferentName as Name, Description from [AliasedColumnsClasses] order by Id";

		//		Assert.Equal(expectedSql, sql);
		//	}

		//	[Fact]
		//	public void CreatesWellFormedFind() {
		//		var properties = new List<EntityProperty>() { _idProperty, _nameProperty, _descriptionProperty };
		//		var where = "ADifferentName like '%test%'";

		//		var sql = _queryGenerator.Find(Table, properties, where);
		//		var expectedSql = "select Id, ADifferentName as Name, Description from [AliasedColumnsClasses] where " + where;

		//		Assert.Equal(expectedSql, sql);
		//	}

		//	[Fact]
		//	public void FormatSqlParamNameAddsIdentifier() {
		//		var paramName = _queryGenerator.FormatSqlParameter("id");

		//		Assert.Equal("@id", paramName);
		//	}

		//	[Fact]
		//	public void FormatSqlParamNameRemovesAndAddsIdentifier() {
		//		var paramName = _queryGenerator.FormatSqlParameter("@id");

		//		Assert.Equal("@id", paramName);
		//	}

		//	[Fact]
		//	public void RemoveSqlParameterFormattingRemovesFormatting() {
		//		var paramName = _queryGenerator.RemoveSqlParameterFormatting("@id");

		//		Assert.Equal("id", paramName);
		//	}
		//}
	}
}
