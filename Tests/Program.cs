using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RpDb;
using RpDb.Entities;

namespace Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            /*RpDbDatabase database = new RpDbDatabase();
            database.tables = new RpDb.Entities.RpDbTable[2];

            database.tables[0] = new RpDb.Entities.RpDbTable("TableTest0", typeof(TestingClass));
            database.tables[0].AddNewEntry(new TestingClass());
            database.tables[0].AddNewEntry(new TestingClass());
            database.tables[0].AddNewEntry(new TestingClass());
            database.tables[0].AddNewEntry(new TestingClass());

            database.tables[1] = new RpDb.Entities.RpDbTable("TableTest0", typeof(TestingClass));
            database.tables[1].AddNewEntry(new TestingClass());
            database.tables[1].AddNewEntry(new TestingClass());
            database.tables[1].AddNewEntry(new TestingClass());
            database.tables[1].AddNewEntry(new TestingClass());

            database.SaveNow(@"E:\testdb.bin");

            Console.WriteLine("done");
            Console.ReadLine();*/

            RpDbDatabase readDb = RpDbDatabase.LoadDatabase(@"E:\testdb.bin", new Type[] {typeof(TestingClass) });

            var table = readDb.tables[0];
            foreach(var uuid in table.uuidLookup.Keys)
            {
                RpDbTableEntry entry = table.ReadResultByUuid(uuid);
                Console.WriteLine(((TestingClass)entry.data).testingInt);
            }

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
