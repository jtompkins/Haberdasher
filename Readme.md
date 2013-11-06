# Haberdasher - A lightweight CRUD wrapper for Dapper

Haberdasher is a light-weight wrapper for the [Dapper Micro-ORM](https://github.com/SamSaffron/dapper-dot-net). It's designed to simplify simple CRUD operations without limiting access to Dapper's low level database access primitives. Dapper is a very fast and lightweight way to communicate to your database, but it relies on you to write SQL for all of your database operations, which can be pretty tedious even for small-ish entity classes. Haberdasher tries to reduce the boring parts of database communication by generating SQL for `SELECT`, `INSERT`, and `DELETE` statements for you.

## Installation

Haberdasher is available via [Nuget](http://www.nuget.org/). Install from the Package Manager Console:

	> Install-Package Haberdasher

## Preparing your app for Haberdasher

Even though Haberdasher isn't an ORM in the traditional sense, some of the same concepts apply. Haberdasher conceptualizes your database as a series of entity types mapped one-to-one to tables in your database. Your entities are basic POCO classes plus a few simple annotations. Actual communication with your database is done though a context-ish base class called a `Haberdashery`, which you'll extend for each of your entity types.

### Annotate your entities

Haberdasher needs to know how your entities are structured in order to automate reading and writing them to the database. You provide that information by annotating your entity classes with simple attributes.

#### The Key Attribute

The only required Haberdasher attribute is `KeyAttribute`, which tells Haberdasher which property on your model represents the primary key of the database table to which it's mapped. *Currently, Haberdasher doesn't support entity models with composite keys.*

The simplest possible Haberdasher entity type looks like this:

	public class SimpleClass {
		[Key]
		public int Id { get; set; }
	}

That's it! Haberdasher doesn't require your entity classes to extend a base class or implement a Haberdasher-specific interface.

#### Aliasing Columns

By default, Haberdasher will automatically "map" all of the properties in your entity to database columns with the same name. If your column names don't match the names of your database columns, you can use the `AliasAttribute` to tell Haberdasher the actual name of the column in your database.

	public class SimpleClass {
		[Key]
		public int Id { get; set; }
		
		[Alias("ClientName")]
		public string Name { get; set; }
	}

When Haberdasher generates SQL for this entity, it will properly alias the columns to match your entity.

#### Preventing Reads and Writes

When generating SQL for database reads and writes, Haberdasher will include all of the properties in your entity. You can tell Haberdasher to ignore certain properties using the `IgnoreAttribute`.

`IgnoreAttribute` takes one argument, an enum of type `IgnoreTypeEnum` that allows you to specify which operations should ignore the property. `Find Name` has the following possible values: `All`, `Write`, `Select`, `Insert`, `Update`.

	public class SimpleClass {
		[Key]
		public int Id { get; set; }
		
		public string FirstName { get; set; }
		public string LastName { get; set; }
		
		[Ignore(IgnoreTypeEnum.All)]
		public string Name {
			get {
				return FirstName + " " + LastName;
			}
		}
	}

### Extend the Haberdashery class

You'll talk to your database using classes that extend the `Haberdashery` base class, which provides methods for database access. You'll need to extend the `Haberdashery` class for each of your entity types.

	public class ProductsHaberdashery : Haberdashery<Product, int> {
		public ProductsHaberdashery("Products")() {
		}
	}

`Haberdashery` is a generic type with two arguments - the type of your entity, and the type of the primary key.

The `Haberdashery` class has no public constructors - you are intended to define your own constructor and call the protected constructors with the appropriate arguments.

The protected constructor takes three arguments:

Argument | Type | Description
--- | --- | ---
name | `String` | The name of the table to which this Haberdashery is mapped.
connectionString | `String` | The name of the connection string in your config file. If you don't provide this argument, Haberdasher will choose the first connection string it finds.
tailor | `ITailor` | An instance of a class that implements the `ITailor` interface (*defaults to an instance of the `SqlServerTailor`* - see *Extending Haberdasher*, below, for more info).

## Using Haberdasher

Haberdasher has a small API surface. For this walkthrough, we'll be using the following types:

	public class Product 
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public decimal Price { get; set; }
		public int AvailableQuantity { get; set; }
	}
	
	public class ProductsHaberdashery : Haberdashery<Product, int> {
		public ProductsHaberdashery("Products")() {
		}
	}

### Getting entities by key

	var context = new ProductsHaberdashery();
	var product = context.Get(1);
	var products = context.Get(new [1, 2, 3]);

### Getting all entities

	var context = new ProductsHaberdashery();	
	var allProducts = context.All();

### Querying Entities

Haberdasher gives two simple query methods, `Find` and `First`. Both take a `String` argument containing the SQL `WHERE` clause:

	var context = new ProductsHaberdashery();
	
	var productsOverTenDollars = context.Find("Price > 10.0");
	var firstProductOverTen = context.First("Price > 10.0");
	
You also have access to the base Dapper query methods, `Query` (returns an `IEnumerable` of your entity type), `Query<T>`, and `Execute` (used for SQL commands that return no results):
	
	var complexQuery = context.Query("select * from Products as p 
										join Orders as o on o.ProductId = p.Id
										where p.Price > 10.0");

### Inserting an entity into the database

Calling `Insert` on the Haberdashery returns the new primary key:

	var product = new Product() { 
									Name = "Test", 
									Price = 10.0m, 
									AvailableQuantity = 1 
								};

	var context = new ProductsHaberdashery();
	var newId = context.Insert(product);

### Updating an existing entity

Calling `Update` with a single entity or list of entities returns the number of updated rows:

	product.Name = "Test Again";
	
	var context = new ProductsHaberdashery();
	var updated = context.Update(product);

### Deleting an entity

Calling `Delete` with a single key or list of keys returns the number of deleted rows:
	
	var context = new ProductsHaberdashery();
	var deleted = context.Delete(product.Id);

## Extending Haberdasher

All of the methods on the `Haberdashery` class are `virtual` and can be overriden in your code:

	public class ProductsHaberdashery : Haberdashery<Product, int> {
		public ProductsHaberdashery("Products")() {
		}
		
		// make sure the .All method only returns available products.
		// why would you do this? who knows?
		public override IEnumerable<Product> All() {
			return base.All().Where(p => p.AvailableQuantity > 0);
		}
	}

### Adding support for additional databases

Since Haberdasher interacts with the database (via Dapper, of course) by generating SQL, it can be used on any database that Dapper and ADO.NET supports. Unfortunately, not every database uses SQL in quite the same way. Haberdasher allows you to override its built-in SQL generation (which produces SQL Server-supported SQL) by creating types that implement the `ITailor` interface:

	public interface ITailor
    {
    	string SelectAll(IEnumerable<CachedProperty> properties);
        string Select(IEnumerable<CachedProperty> properties, CachedProperty key, string keyParam);
        string SelectMany(IEnumerable<CachedProperty> properties, CachedProperty key, string keysParam);

        string Insert(IDictionary<string, CachedProperty> properties, CachedProperty key);

        string Update(IDictionary<string, CachedProperty> properties, CachedProperty key, string keyParam);
        string UpdateMany(IDictionary<string, CachedProperty> properties, CachedProperty key, string keysParam);

        string DeleteAll();
        string Delete(CachedProperty key, string keyParam);
        string DeleteMany(CachedProperty key, string keysParam);
	}
	
Haberdasher comes with one implementation of `ITailor`, the `SqlServerTailor` class. Check out the [source](https://github.com/jtompkins/envelopes-api/blob/master/src/Haberdasher/Tailors/SqlServerTailor.cs) for more information on implementing the `ITailor` methods.

## Contributing to Haberdasher

If you'd like to contribute to Haberdasher, I'd love your help! 

Building the project should be simple - it's a pretty basic VS solution. Clone this repo to your machine, and be sure to enable Nuget Package Restore to pull in the dependencies the first time you build the project. 

Please open issues with any problems you find, or send me pull requests with new code. There are a few [XUnit](http://xunit.codeplex.com/) tests in place now (not enough!), so make sure they all pass before you send a PR.

## Thanks for checking out Haberdasher!

Haberdasher has been pretty useful in my projects - hopefully you'll find it useful, too. I'm available on Twitter - [@jtompkinsx](http://www.twitter.com/jtompkinsx) - so hit me up if you find something broken, have questions, or need a pull request merged. 

I'd love to hear about anyone using Haberdasher. Thanks for checking it out.