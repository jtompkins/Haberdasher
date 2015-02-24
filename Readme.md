# Haberdasher - A lightweight CRUD wrapper for Dapper

Haberdasher is a light-weight wrapper for the [Dapper Micro-ORM](https://github.com/SamSaffron/dapper-dot-net). It's designed to simplify simple CRUD operations without limiting access to Dapper's low level database access primitives. Dapper is a very fast and lightweight way to communicate to your database, but it relies on you to write SQL for all of your database operations, which can be pretty tedious even for small-ish entity classes. Haberdasher tries to reduce the boring parts of database communication by generating SQL for `SELECT`, `INSERT`, and `DELETE` statements for you.

## Installation

Haberdasher is available via [Nuget](http://www.nuget.org/). Install from the Package Manager Console:

	> Install-Package Haberdasher

## Preparing your app for Haberdasher

Even though Haberdasher isn't an ORM in the traditional sense, some of the same concepts apply. Haberdasher conceptualizes your database as a series of entity types mapped one-to-one to tables in your database. Your entities are basic POCO classes. Before accessing your database, you'll need to register your entity types with the `EntityTypes` class. Actual communication with your database is done though a context-ish base class called a `EntityStore`.

### Registering your entity types

Haberdasher needs to know how your entities are structured in order to automate reading and writing them to the database. You provide that information registering your types with the `EntityTypes` class, which provides a cache of type information for use by the `EntityStore`.

For simple types, just call `Register<T>` and Haberdasher wil take care of the rest:

    EntityTypes.Register<Product>();

If you need to customize how Haberdasher handles your entity, just define your customizations in a `RegisterAction` hook:

    EntityTypes.Register<Product>(te => {
       // read further to find out about your options!
    });

#### Naming Tables

Haberdasher assumes a conventional Singular/Plural naming scheme for your entities and tables. For example, if you had a type called `Product`, Haberdasher will assume the table is named `Products`. You can override this assumption in two ways. If your table name is singular, just call `Singular` in the `RegisterAction`:

    EntityTypes.Register<Product>(te => {
       te.Singular();
    });

If your table name doesn't match your entity name at all, you can alias the name of the entity for SQL generation:

    EntityTypes.Register<Product>(te => {
       te.AliasTable("ProductItems");
    });

#### The Key Method

The only thing you're required to define for your entity is the property that represents the primary key. Haberdasher provides a `Key` method in the `RegisterAction` for this purpose:

    public class Product {
        public int Id { get; set; }
    }

    EntityTypes.Register<Product>(t => {
       t.Key(p => p.Id);
    });

That's it! Haberdasher doesn't require your entity classes to extend a base class or implement a Haberdasher-specific interface.

*Currently, Haberdasher doesn't support entity models with composite keys.*

##### Automatic Key Detection

Starting with version 0.3, Haberdasher can automatically identify key properties, as long as they are named with one of the following names (not case-sensitive):

* Id
* Guid

##### Identity Columns

By default, if your key property is numeric, Haberdasher assumes that it is also an IDENTITY column. If that's not the case, pass `false` to the `Key` method:

	EntityTypes.Register<Product>(e => {
       e.Key(p => p.Id, false);
    });

#### Aliasing Columns

By default, Haberdasher will automatically "map" all of the properties in your entity to database columns with the same name. If your column names don't match the names of your database columns, you can use the `Alias` method in the `RegisterAction` to tell Haberdasher the actual name of the column in your database:

    public class Product {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    EntityTypes.Register<Product>(e => {
       e.Key(p => p.Id)
        .Alias(p => p.Name, "ProductName");
    });

When Haberdasher generates SQL for this entity, it will properly alias the columns to match your entity.

This is probably a good time to point out that all of the methods available in the `RegisterAction` can be chained together into one fluent call.

#### Preventing Reads and Writes

When generating SQL for database reads and writes, Haberdasher will include all of the properties in your entity. You can tell Haberdasher to ignore certain properties using the `Ignore` method.

`Ignore` takes two arguments: the property to be ignored, and an enum of type `IgnoreTypeEnum` that allows you to specify which operations should ignore the property. `IgnoreTypeEnum` has the following possible values:

* `All`
* `Write`
* `Select`
* `Insert`
* `Update`

Key columns always `SELECT`-able. Identity keys are always ignored for `INSERT`s and `UPDATE`s.

    public class Customer {
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Name {
            get {
                return FirstName + " " + LastName;
            }
        }
    }

    EntityTypes.Register<Customer>(e => {
        //the key is auto-detected!
        e.Ignore(c => c.Name, IgnoreTypeEnum.All);
    });

## Using Haberdasher

Haberdasher has a small API surface centered around the `EntityStore` class. For this walkthrough, we'll be using the following type:

	public class Product
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public decimal Price { get; set; }
		public int AvailableQuantity { get; set; }
	}

### Getting entities by key

	var store = new EntityStore<Product, int>();
	var product = store.Get(1);
	var products = store.Get(new [1, 2, 3]);

### Getting all entities

	var store = new EntityStore<Product, int>();
	var allProducts = store.All();

### Querying Entities

Haberdasher gives two simple query methods, `Find` and `FindOne`. Both take a `String` argument containing the SQL `WHERE` clause:

	var store = new EntityStore<Product, int>();

	var productsOverTenDollars = store.Find("Price > 10.0");
	var firstProductOverTen = store.First("Price > 10.0");

For safety, you can parameterize your queries:

    var productsOverTenDollars = store.Find("Price > @price", new { price = 10.0m });

You also have access to the base Dapper query methods, `Query` (returns an `IEnumerable` of your entity type), `Query<T>`, and `Execute` (used for SQL commands that return no results):

	var complexQuery = store.Query("select * from Products as p
										join Orders as o on o.ProductId = p.Id
										where p.Price > 10.0");

### Inserting an entity into the database

Calling `InsertWithIdentity` on the `EntityStore` returns the new primary key:

	var product = new Product() {
									Name = "Test",
									Price = 10.0m,
									AvailableQuantity = 1
								};

	var store = new EntityStore<Product, int>();
	var newId = store.InsertWithIdentity(product);

If you don't need the new primary key, you can also call `Insert`:

	var product = new Product() {
									Name = "Test",
									Price = 10.0m,
									AvailableQuantity = 1
								};

	var store = new EntityStore<Product, int>();
	var numberOfRecordsInserted = store.Insert(product);

### Updating an existing entity

Calling `Update` with a single entity or list of entities returns the number of updated rows:

	product.Name = "Test Again";

	var store = new EntityStore<Product, int>();
	var updated = store.Update(product);

### Deleting an entity

Calling `Delete` with a single key or list of keys returns the number of deleted rows:

	var store = new EntityStore<Product, int>();
	var deleted = store.Delete(product.Id);

## Adding support for additional databases

Since Haberdasher interacts with the database (via Dapper, of course) by generating SQL, it can be used on any database that Dapper and ADO.NET supports. Unfortunately, not every database uses SQL in quite the same way. Haberdasher allows you to override its built-in SQL generation (which produces SQL Server-supported SQL) by creating types that implement the `IQueryGenerator` interface:

	public interface IQueryGenerator
    {
    	string SelectAll(string table, IEnumerable<CachedProperty> properties, CachedProperty key);
		string Select(string table, IEnumerable<CachedProperty> properties, CachedProperty key, string value);
		string SelectMany(string table, IEnumerable<CachedProperty> properties, CachedProperty key, string values);

		string Find(string table, IEnumerable<CachedProperty> properties, string whereClause);
		string FindOne(string table, IEnumerable<CachedProperty> properties, string whereClause);

		string Insert(string table, IDictionary<string, CachedProperty> properties, CachedProperty key);

		string Update(string table, IDictionary<string, CachedProperty> properties, CachedProperty key, string value);
		string UpdateMany(string table, IDictionary<string, CachedProperty> properties, CachedProperty key, string values);

		string DeleteAll(string table);
		string Delete(string table, CachedProperty key, string value);
		string DeleteMany(string table, CachedProperty key, string values);

		string FormatSqlParameter(string param);
		string RemoveSqlParameterFormatting(string param);
	}

Haberdasher comes with one implementation of `IQueryGenerator`, the `SqlServerGenerator` class. Check out the [source](https://github.com/jtompkins/Haberdasher/blob/master/Haberdasher/QueryGenerators/SqlServerGenerator.cs) for more information on implementing the `IQueryGenerator` methods.

## Contributing to Haberdasher

If you'd like to contribute to Haberdasher, I'd love your help!

Building the project should be simple - it's a pretty basic VS solution. Clone this repo to your machine, and be sure to enable Nuget Package Restore to pull in the dependencies the first time you build the project.

Please open issues with any problems you find, or send me pull requests with new code. There are a few [XUnit](http://xunit.codeplex.com/) tests in place now (not enough!), so make sure they all pass before you send a PR.

## Thanks for checking out Haberdasher!

Haberdasher has been pretty useful in my projects - hopefully you'll find it useful, too. I'm available on Twitter - [@jtompkinsx](http://www.twitter.com/jtompkinsx) - so hit me up if you find something broken, have questions, or need a pull request merged.

I'd love to hear about anyone using Haberdasher. Thanks for checking it out.
