using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Haberdasher.Tests.TestClasses;
using Xunit;
using System.IO;
using System.Diagnostics;

namespace Haberdasher.Tests
{
	public class OracleHaberdasheryFixture
	{

		/// <summary>
		/// Initializes a new instance of the OracleHaberdasheryFixture class.
		/// </summary>
		public OracleHaberdasheryFixture() {
			// re-create db
			List<string> statements = new List<string>();
			statements.Add("DROP SEQUENCE SIMPLE_CLASSES_ID_SEQ");
			statements.Add("CREATE SEQUENCE SIMPLE_CLASSES_ID_SEQ INCREMENT BY 1 START WITH 1");
			statements.Add("DROP TABLE SIMPLE_CLASSES CASCADE CONSTRAINTS");
			statements.Add(@"CREATE TABLE SIMPLE_CLASSES 
                            (
                              ID INTEGER NOT NULL 
                            , NAME VARCHAR2(50) 
                            , CONSTRAINT SIMPLE_CLASSES_PK PRIMARY KEY 
                              (
                                ID 
                              )
                              ENABLE 
                            )");

			// trigger to update the id if not set
			statements.Add(@"create or replace trigger ON_SIMPLE_CLASSES_INSERT
	                            before insert on SIMPLE_CLASSES
	                            for each row
	                            begin
	                            if :new.ID is null then
		                            select SIMPLE_CLASSES_ID_SEQ.nextval into :new.ID from dual;
	                            end if;
	                            end;");

			SimpleClassOracleHaberdashery db = new SimpleClassOracleHaberdashery();

			foreach (string ddl in statements) {
				try {
					db.Execute(ddl);
					Debug.WriteLine(String.Format("Ran sql: {0}", ddl));
				}
				catch (Exception ex) {
					// log and continue
					Debug.WriteLine(String.Format("Error running sql {0} {1}", ddl, ex.Message));
				}
			}
		}


		[Fact(Skip = "No Oracle DB available.")]
		public void InsertNewWithIdentityReturnsNewId() {
			SimpleClassOracleHaberdashery db = new SimpleClassOracleHaberdashery();

			var simple = new SimpleClass { Name = "New Simple Class" };

			int newId = db.Insert(simple);

			Assert.True(newId > 0);

			// retrieve from db and check values
			var newOne = db.Get(newId);
			Assert.Equal("New Simple Class", newOne.Name);
		}

		[Fact(Skip = "No Oracle DB available.")]
		public void InsertNewWithNoIdentityReturnsAssignedId() {
			NonIdentityKeyOracleHaberdashery db = new NonIdentityKeyOracleHaberdashery();

			// 10 is an arbitrary key above the last sequence so should be unique
			// inserting a duplicate id value will cause error in DB since violates primary key
			var entityToInsert = new NonIdentityKeyClass { Id = 10, Name = "Has Assigned Key" };

			long newId = db.Insert(entityToInsert);

			Assert.Equal(10, newId);

			// retrieve from db and check values
			var newOne = db.Get(newId);
			Assert.Equal(10, newOne.Id);
			Assert.Equal("Has Assigned Key", newOne.Name);
		}

		[Fact(Skip = "No Oracle DB available.")]
		public void InsertNewWithSequenceAndNoIdentityReturnsAssignedId() {
			NonIdentityKeyOracleHaberdashery db = new NonIdentityKeyOracleHaberdashery();
			db.SequenceName = "SIMPLE_CLASSES_ID_SEQ";

			var entityToInsert = new NonIdentityKeyClass { Name = "Has Key from Sequence" };

			long newId = db.Insert(entityToInsert);

			Assert.True(newId > 0);

			// retrieve from db and check values
			var newOne = db.Get(newId);
			Assert.Equal("Has Key from Sequence", newOne.Name);
		}

		[Fact(Skip = "No Oracle DB available.")]
		public void CanCRUD() {
			SimpleClassOracleHaberdashery db = new SimpleClassOracleHaberdashery();

			// value of new name
			const string STR_NewName = "I am new";
			var simple = new SimpleClass { Name = STR_NewName };

			int newId = db.Insert(simple);

			Assert.True(newId > 0);

			// retrieve from db and check values
			var newOne = db.Get(newId);
			Assert.Equal(STR_NewName, newOne.Name);

			// update
			// value of updated name
			const string STR_UpdatedName = "I am updated";
			newOne.Name = STR_UpdatedName;
			db.Update(newOne);

			// retrieve from db and check values
			var updated = db.Get(newId);
			Assert.Equal(STR_UpdatedName, updated.Name);

			// delete it
			db.Delete(newId);
			var deleted = db.Get(newId);
			Assert.Null(deleted);
		}

	}
}
