﻿using System.Collections.Generic;
using Haberdasher.Tailors;
using Haberdasher.Tests.TestClasses;
using Xunit;

namespace Haberdasher.Tests
{
	public class SqlServerTailorFixture
	{
		public class SimpleClassTests
		{
			private readonly CachedProperty _idProperty;
			private readonly CachedProperty _nameProperty;

			private readonly SqlServerTailor _tailor;

			public SimpleClassTests() {
				var simpleClassType = typeof(SimpleClass);

				_idProperty = new CachedProperty(simpleClassType.GetProperty("Id"));
				_nameProperty = new CachedProperty(simpleClassType.GetProperty("Name"));

				_tailor = new SqlServerTailor("SimpleClasses");
			}

			[Fact]
			public void CreatesWellFormedSelectAll() {
				var sql = _tailor.SelectAll(new List<CachedProperty>() { _idProperty, _nameProperty });
				var expectedSql = "select Id, Name from [SimpleClasses]";

				Assert.Equal(sql, expectedSql);
			}

			[Fact]
			public void CreatesWellFormedSelect() {
				var sql = _tailor.Select(new List<CachedProperty>() { _idProperty, _nameProperty }, _idProperty, "@id");
				var expectedSql = "select Id, Name from [SimpleClasses] where Id = @id";

				Assert.Equal(sql, expectedSql);
			}

			[Fact]
			public void CreatesWellFormedSelectMany() {
				var sql = _tailor.SelectMany(new List<CachedProperty>() { _idProperty, _nameProperty }, _idProperty, "@ids");
				var expectedSql = "select Id, Name from [SimpleClasses] where Id in @ids";

				Assert.Equal(sql, expectedSql);
			}

			[Fact]
			public void CreatesWellFormedInsert() {
				var properties = new Dictionary<string, CachedProperty>();

				properties.Add("@name", _nameProperty);

				var sql = _tailor.Insert(properties, _idProperty);
				var expectedSql = "set nocount on insert into [SimpleClasses] (Name) values (@name) select SCOPE_IDENTITY()";

				Assert.Equal(sql, expectedSql);
			}

			[Fact]
			public void CreatesWellFormedUpdate() {
				var properties = new Dictionary<string, CachedProperty>();

				properties.Add("@name", _nameProperty);

				var sql = _tailor.Update(properties, _idProperty, "@id");
				var expectedSql = "update [SimpleClasses] set Name = @name where Id = @id";

				Assert.Equal(sql, expectedSql);
			}

			[Fact]
			public void CreatesWellFormedUpdateMany() {
				var properties = new Dictionary<string, CachedProperty>();

				properties.Add("@name", _nameProperty);

				var sql = _tailor.UpdateMany(properties, _idProperty, "@ids");
				var expectedSql = "update [SimpleClasses] set Name = @name where Id in @ids";

				Assert.Equal(sql, expectedSql);
			}

			[Fact]
			public void CreatesWellFormedDeleteAll() {
				var sql = _tailor.DeleteAll();
				var expectedSql = "truncate table [SimpleClasses]";

				Assert.Equal(sql, expectedSql);
			}

			[Fact]
			public void CreatesWellFormedDelete() {
				var sql = _tailor.Delete(_idProperty, "@id");
				var expectedSql = "delete from [SimpleClasses] where Id = @id";

				Assert.Equal(sql, expectedSql);
			}

			[Fact]
			public void CreatesWellFormedDeleteMany() {
				var sql = _tailor.DeleteMany(_idProperty, "@ids");
				var expectedSql = "delete from [SimpleClasses] where Id in @ids";

				Assert.Equal(sql, expectedSql);
			}
		}

		public class NonIdentityKeyClassTests
		{
			private readonly CachedProperty _idProperty;

			private readonly SqlServerTailor _tailor;

			public NonIdentityKeyClassTests() {
				var type = typeof (NonIdentityKeyClass);

				_idProperty = new CachedProperty(type.GetProperty("Id"));

				_tailor = new SqlServerTailor("NonIdentityKeyClasses");
			}

			[Fact]
			public void CreatesWellFormedInsert() {
				var properties = new Dictionary<string, CachedProperty>();

				properties.Add("@id", _idProperty);

				var sql = _tailor.Insert(properties, _idProperty);
				var expectedSql = "set nocount on insert into [NonIdentityKeyClasses] (Id) values (@id)";

				Assert.Equal(sql, expectedSql);
			}
		}

		public class NonScopeIdentityKeyClassTests
		{
			private readonly CachedProperty _idProperty;
			private readonly SqlServerTailor _tailor;

			public NonScopeIdentityKeyClassTests() {
				var type = typeof (NonScopeIdentityKeyClass);

				_idProperty = new CachedProperty(type.GetProperty("Id"));
				_tailor = new SqlServerTailor("NonScopeIdentityKeyClasses");
			}

			[Fact]
			public void CreatesWellFormedInsert() {
				var properties = new Dictionary<string, CachedProperty>();

				properties.Add("@id", _idProperty);

				var sql = _tailor.Insert(properties, _idProperty);
				var expectedSql = "set nocount on insert into [NonScopeIdentityKeyClasses] (Id) values (@id) select @@IDENTITY";

				Assert.Equal(sql, expectedSql);
			}
		}

		public class AliasedColumnsClassTests
		{
			private readonly CachedProperty _idProperty;
			private readonly CachedProperty _nameProperty;
			private readonly CachedProperty _descriptionProperty;

			private readonly SqlServerTailor _tailor;

			public AliasedColumnsClassTests() {
				var type = typeof (AliasedColumnsClass);

				_idProperty = new CachedProperty(type.GetProperty("Id"));
				_nameProperty = new CachedProperty(type.GetProperty("Name"));
				_descriptionProperty = new CachedProperty(type.GetProperty("Description"));

				_tailor = new SqlServerTailor("AliasedColumnsClasses");
			}

			[Fact]
			public void CreatesWellFormedSelectAll() {
				var sql = _tailor.SelectAll(new List<CachedProperty>() { _idProperty, _nameProperty, _descriptionProperty });
				var expectedSql = "select Id, ADifferentName as Name, Description from [AliasedColumnsClasses]";

				Assert.Equal(sql, expectedSql);
			}

			[Fact]
			public void CreatesWellFormedSelect() {
				var sql = _tailor.Select(new List<CachedProperty>() { _idProperty, _nameProperty, _descriptionProperty }, _idProperty, "@id");
				var expectedSql = "select Id, ADifferentName as Name, Description from [AliasedColumnsClasses] where Id = @id";

				Assert.Equal(sql, expectedSql);
			}

			[Fact]
			public void CreatesWellFormedSelectMany() {
				var sql = _tailor.SelectMany(new List<CachedProperty>() { _idProperty, _nameProperty, _descriptionProperty }, _idProperty, "@ids");
				var expectedSql = "select Id, ADifferentName as Name, Description from [AliasedColumnsClasses] where Id in @ids";

				Assert.Equal(sql, expectedSql);
			}

			[Fact]
			public void CreatesWellFormedInsert() {
				var properties = new Dictionary<string, CachedProperty>();

				properties.Add("@name", _nameProperty);
				properties.Add("@description", _descriptionProperty);

				var sql = _tailor.Insert(properties, _idProperty);
				var expectedSql = "set nocount on insert into [AliasedColumnsClasses] (ADifferentName, Description) values (@name, @description) select SCOPE_IDENTITY()";

				Assert.Equal(sql, expectedSql);
			}

			[Fact]
			public void CreatesWellFormedUpdate() {
				var properties = new Dictionary<string, CachedProperty>();

				properties.Add("@name", _nameProperty);
				properties.Add("@description", _descriptionProperty);

				var sql = _tailor.Update(properties, _idProperty, "@id");
				var expectedSql = "update [AliasedColumnsClasses] set ADifferentName = @name, Description = @description where Id = @id";

				Assert.Equal(sql, expectedSql);
			}

			[Fact]
			public void CreatesWellFormedUpdateMany() {
				var properties = new Dictionary<string, CachedProperty>();

				properties.Add("@name", _nameProperty);
				properties.Add("@description", _descriptionProperty);

				var sql = _tailor.UpdateMany(properties, _idProperty, "@ids");
				var expectedSql = "update [AliasedColumnsClasses] set ADifferentName = @name, Description = @description where Id in @ids";

				Assert.Equal(sql, expectedSql);
			}
		}
	}
}
