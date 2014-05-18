﻿using System.Collections.Generic;
using Haberdasher.QueryGenerators;
using Haberdasher.Tests.TestClasses;
using Xunit;

namespace Haberdasher.Tests
{
	public class SqlServerSqlBuilderFixture
	{
		public class SimpleClassTests
		{
			private readonly CachedProperty _idProperty;
			private readonly CachedProperty _nameProperty;

			private readonly SqlServerGenerator _queryGenerator;

			public SimpleClassTests() {
				var simpleClassType = typeof(SimpleClass);

				_idProperty = new CachedProperty(simpleClassType.GetProperty("Id"));
				_nameProperty = new CachedProperty(simpleClassType.GetProperty("Name"));

				_queryGenerator = new SqlServerGenerator("SimpleClasses");
			}

			[Fact]
			public void CreatesWellFormedSelect() {
				var sql = _queryGenerator.Select(new List<CachedProperty>() { _idProperty, _nameProperty }, _idProperty, "@id");
				var expectedSql = "select Id, Name from [SimpleClasses] where Id = @id";

				Assert.Equal(expectedSql, sql);
			}

			[Fact]
			public void CreatesWellFormedSelectMany() {
				var sql = _queryGenerator.SelectMany(new List<CachedProperty>() { _idProperty, _nameProperty }, _idProperty, "@ids");
				var expectedSql = "select Id, Name from [SimpleClasses] where Id in @ids";

				Assert.Equal(expectedSql, sql);
			}

			[Fact]
			public void CreatesWellFormedInsert() {
				var properties = new Dictionary<string, CachedProperty>();

				properties.Add("@name", _nameProperty);

				var sql = _queryGenerator.Insert(properties, _idProperty);
				var expectedSql = "set nocount on insert into [SimpleClasses] (Name) values (@name) select SCOPE_IDENTITY()";

				Assert.Equal(expectedSql, sql);
			}

			[Fact]
			public void CreatesWellFormedUpdate() {
				var properties = new Dictionary<string, CachedProperty>();

				properties.Add("@name", _nameProperty);

				var sql = _queryGenerator.Update(properties, _idProperty, "@id");
				var expectedSql = "update [SimpleClasses] set Name = @name where Id = @id";

				Assert.Equal(expectedSql, sql);
			}

			[Fact]
			public void CreatesWellFormedUpdateMany() {
				var properties = new Dictionary<string, CachedProperty>();

				properties.Add("@name", _nameProperty);

				var sql = _queryGenerator.UpdateMany(properties, _idProperty, "@ids");
				var expectedSql = "update [SimpleClasses] set Name = @name where Id in @ids";

				Assert.Equal(expectedSql, sql);
			}

			[Fact]
			public void CreatesWellFormedDeleteAll() {
				var sql = _queryGenerator.DeleteAll();
				var expectedSql = "truncate table [SimpleClasses]";

				Assert.Equal(expectedSql, sql);
			}

			[Fact]
			public void CreatesWellFormedDelete() {
				var sql = _queryGenerator.Delete(_idProperty, "@id");
				var expectedSql = "delete from [SimpleClasses] where Id = @id";

				Assert.Equal(expectedSql, sql);
			}

			[Fact]
			public void CreatesWellFormedDeleteMany() {
				var sql = _queryGenerator.DeleteMany(_idProperty, "@ids");
				var expectedSql = "delete from [SimpleClasses] where Id in @ids";

				Assert.Equal(expectedSql, sql);
			}

			[Fact]
			public void CreatesWellFormedSelectAll() {
				var properties = new List<CachedProperty>() {_idProperty, _nameProperty};

				var sql = _queryGenerator.SelectAll(properties, _idProperty);
				var expectedSql = "select Id, Name from [SimpleClasses] order by Id";

				Assert.Equal(expectedSql, sql);
			}

			[Fact]
			public void CreatesWellFormedFind() {
				var properties = new List<CachedProperty>() { _idProperty, _nameProperty };
				var where = "Name like '%test%'";

				var sql = _queryGenerator.Find(properties, where);
				var expectedSql = "select Id, Name from [SimpleClasses] where " + where;

				Assert.Equal(expectedSql, sql);
			}
		}

		public class NonIdentityKeyClassTests
		{
			private readonly CachedProperty _idProperty;

			private readonly SqlServerGenerator _queryGenerator;

			public NonIdentityKeyClassTests() {
				var type = typeof (NonIdentityKeyClass);

				_idProperty = new CachedProperty(type.GetProperty("Id"));

				_queryGenerator = new SqlServerGenerator("NonIdentityKeyClasses");
			}

			[Fact]
			public void CreatesWellFormedInsert() {
				var properties = new Dictionary<string, CachedProperty>();

				properties.Add("@id", _idProperty);

				var sql = _queryGenerator.Insert(properties, _idProperty);
				var expectedSql = "set nocount on insert into [NonIdentityKeyClasses] (Id) values (@id)";

				Assert.Equal(expectedSql, sql);
			}
		}

		public class NonScopeIdentityKeyClassTests
		{
			private readonly CachedProperty _idProperty;
			private readonly SqlServerGenerator _queryGenerator;

			public NonScopeIdentityKeyClassTests() {
				var type = typeof (NonScopeIdentityKeyClass);

				_idProperty = new CachedProperty(type.GetProperty("Id"));
				_queryGenerator = new SqlServerGenerator("NonScopeIdentityKeyClasses");
			}

			[Fact]
			public void CreatesWellFormedInsert() {
				var properties = new Dictionary<string, CachedProperty>();

				properties.Add("@id", _idProperty);

				var sql = _queryGenerator.Insert(properties, _idProperty);
				var expectedSql = "set nocount on insert into [NonScopeIdentityKeyClasses] (Id) values (@id) select @@IDENTITY";

				Assert.Equal(expectedSql, sql);
			}
		}

		public class AliasedColumnsClassTests
		{
			private readonly CachedProperty _idProperty;
			private readonly CachedProperty _nameProperty;
			private readonly CachedProperty _descriptionProperty;

			private readonly SqlServerGenerator _queryGenerator;

			public AliasedColumnsClassTests() {
				var type = typeof (AliasedColumnsClass);

				_idProperty = new CachedProperty(type.GetProperty("Id"));
				_nameProperty = new CachedProperty(type.GetProperty("Name"));
				_descriptionProperty = new CachedProperty(type.GetProperty("Description"));

				_queryGenerator = new SqlServerGenerator("AliasedColumnsClasses");
			}

			[Fact]
			public void CreatesWellFormedSelect() {
				var sql = _queryGenerator.Select(new List<CachedProperty>() { _idProperty, _nameProperty, _descriptionProperty }, _idProperty, "@id");
				var expectedSql = "select Id, ADifferentName as Name, Description from [AliasedColumnsClasses] where Id = @id";

				Assert.Equal(expectedSql, sql);
			}

			[Fact]
			public void CreatesWellFormedSelectMany() {
				var sql = _queryGenerator.SelectMany(new List<CachedProperty>() { _idProperty, _nameProperty, _descriptionProperty }, _idProperty, "@ids");
				var expectedSql = "select Id, ADifferentName as Name, Description from [AliasedColumnsClasses] where Id in @ids";

				Assert.Equal(expectedSql, sql);
			}

			[Fact]
			public void CreatesWellFormedInsert() {
				var properties = new Dictionary<string, CachedProperty>();

				properties.Add("@name", _nameProperty);
				properties.Add("@description", _descriptionProperty);

				var sql = _queryGenerator.Insert(properties, _idProperty);
				var expectedSql = "set nocount on insert into [AliasedColumnsClasses] (ADifferentName, Description) values (@name, @description) select SCOPE_IDENTITY()";

				Assert.Equal(expectedSql, sql);
			}

			[Fact]
			public void CreatesWellFormedUpdate() {
				var properties = new Dictionary<string, CachedProperty>();

				properties.Add("@name", _nameProperty);
				properties.Add("@description", _descriptionProperty);

				var sql = _queryGenerator.Update(properties, _idProperty, "@id");
				var expectedSql = "update [AliasedColumnsClasses] set ADifferentName = @name, Description = @description where Id = @id";

				Assert.Equal(expectedSql, sql);
			}

			[Fact]
			public void CreatesWellFormedUpdateMany() {
				var properties = new Dictionary<string, CachedProperty>();

				properties.Add("@name", _nameProperty);
				properties.Add("@description", _descriptionProperty);

				var sql = _queryGenerator.UpdateMany(properties, _idProperty, "@ids");
				var expectedSql = "update [AliasedColumnsClasses] set ADifferentName = @name, Description = @description where Id in @ids";

				Assert.Equal(expectedSql, sql);
			}

			[Fact]
			public void CreatesWellFormedSelectAll() {
				var properties = new List<CachedProperty>() { _idProperty, _nameProperty, _descriptionProperty };

				var sql = _queryGenerator.SelectAll(properties, _idProperty);
				var expectedSql = "select Id, ADifferentName as Name, Description from [AliasedColumnsClasses] order by Id";

				Assert.Equal(expectedSql, sql);
			}

			[Fact]
			public void CreatesWellFormedFind() {
				var properties = new List<CachedProperty>() { _idProperty, _nameProperty, _descriptionProperty };
				var where = "ADifferentName like '%test%'";

				var sql = _queryGenerator.Find(properties, where);
				var expectedSql = "select Id, ADifferentName as Name, Description from [AliasedColumnsClasses] where " + where;

				Assert.Equal(expectedSql, sql);
			}

            [Fact]
            public void FormatSqlParamNameAddsIdentifier()
            {
                string paramName = _queryGenerator.FormatSqlParameter("id");
                Assert.Equal("@id", paramName);
            }

            [Fact]
            public void FormatSqlParamNameRemovesAndAddsIdentifier()
            {
                string paramName = _queryGenerator.FormatSqlParameter(":id");
                Assert.Equal("@id", paramName);

                paramName = _queryGenerator.FormatSqlParameter("@id");
                Assert.Equal("@id", paramName);

                paramName = _queryGenerator.FormatSqlParameter("?id");
                Assert.Equal("@id", paramName);

            }
		}
	}
}
