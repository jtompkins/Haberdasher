using System;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Dapper;
using Haberdasher.QueryGenerators;
using Xunit;

namespace Haberdasher.Tests
{
	public class DbAccessFixture
	{
//		private class Manufacturer
//		{
//			[Key]
//			public int Id { get; set; }
//			public string Name { get; set; }
//		}

//		private class Product
//		{
//			[Key]
//			public int Id { get; set; }
//			public int ManufacturerId { get; set; }
//			public string Name { get; set; }
//		}

//		private EntityStore<Manufacturer, int> _manufacturerStore;
//		private EntityStore<Product, int> _productStore;

//		private string GetDbFile() {
//			return Environment.CurrentDirectory + "\\TestDb.sqlite";
//		}

//		private SQLiteConnection GetDbConnection() {
//			return new SQLiteConnection("Data Source=" + GetDbFile());
//		}

//		private void CreateDatabase() {
//			using (var cnn = GetDbConnection()) {
//				cnn.Open();

//				cnn.Execute(
//					@"create table Manufacturers
//					  (
//						 Id     integer primary key,
//						 Name	text not null
//					  )");

//				cnn.Execute(
//					@"create table Products
//					  (
//						 Id					integer primary key,
//						 ManufacturerId		integer not null,
//						 Name				text not null
//					  )");
//			}
//		}

//		public DbAccessFixture() {
//			var file = GetDbFile();

//			if (File.Exists(file))
//				File.Delete(file);

//			CreateDatabase();

//			var conn = GetDbConnection();

//			_manufacturerStore = new EntityStore<Manufacturer, int>(conn, new SqliteGenerator());
//			_productStore = new EntityStore<Product, int>(conn, new SqliteGenerator());
//		}

//		[Fact]
//		public void InsertsRecords() {
//			var newManufacturer = new Manufacturer() {
//				Name = "Test Manufacturer"
//			};

//			var manufacturerId = _manufacturerStore.InsertWithIdentity(newManufacturer);

//			Assert.Equal(1, manufacturerId);

//			var allManufacturers = _manufacturerStore.Get();

//			Assert.Equal(1, allManufacturers.Count());

//			var newProduct = new Product() {
//				ManufacturerId = manufacturerId,
//				Name = "Test Product"
//			};

//			var productId = _productStore.InsertWithIdentity(newProduct);

//			Assert.Equal(1, productId);
//		}

//		[Fact]
//		public void SelectsRecords() {
//			var newManufacturer = new Manufacturer() {
//				Name = "Test Manufacturer"
//			};

//			var manufacturerId = _manufacturerStore.InsertWithIdentity(newManufacturer);

//			var m = _manufacturerStore.Get(manufacturerId);

//			Assert.NotNull(m);
//			Assert.Equal(manufacturerId, m.Id);
//			Assert.Equal("Test Manufacturer", m.Name);

//			var newProduct = new Product() {
//				ManufacturerId = manufacturerId,
//				Name = "Test Product"
//			};

//			var productId = _productStore.InsertWithIdentity(newProduct);

//			var p = _productStore.FindOne("ManufacturerId = @id", new { id = manufacturerId });

//			Assert.NotNull(p);
//			Assert.Equal(productId, p.Id);
//			Assert.Equal("Test Product", p.Name);
//		}

//		[Fact]
//		public void UpdatesRecords() {
//			var newManufacturer = new Manufacturer() {
//				Name = "Test Manufacturer"
//			};

//			var manufacturerId = _manufacturerStore.InsertWithIdentity(newManufacturer);

//			newManufacturer.Id = manufacturerId;
//			newManufacturer.Name = "Updated Manufacturer";

//			_manufacturerStore.Update(newManufacturer);

//			var m = _manufacturerStore.Get(manufacturerId);

//			Assert.NotNull(m);
//			Assert.Equal("Updated Manufacturer", m.Name);
//		}

//		[Fact]
//		public void DeletesRecords() {
//			var newManufacturer = new Manufacturer() {
//				Name = "Test Manufacturer"
//			};

//			var manufacturerId = _manufacturerStore.InsertWithIdentity(newManufacturer);

//			var allManufacturers = _manufacturerStore.Get();

//			Assert.Equal(1, allManufacturers.Count());

//			var deleted = _manufacturerStore.Delete(manufacturerId);

//			Assert.Equal(1, deleted);

//			allManufacturers = _manufacturerStore.Get();

//			Assert.Equal(0, allManufacturers.Count());
//		}
	}
}
