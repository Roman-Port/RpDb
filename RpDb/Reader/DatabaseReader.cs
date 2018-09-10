using RpDb.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RpDb.Reader
{
    class DatabaseReader
    {
        public static RpDbDatabase ReadDatabase(Stream s, Type[] types)
        {
            //Read "RpDb" at the beginning 
            char[] intro = ReaderTools.ReadChars(s, 4);
            //Read the version.
            UInt16 version = ReaderTools.ReadUInt16(s);
            //Skip 32 bytes of reserved space.
            s.Position += 32;
            //Read table count.
            UInt16 tableCount = ReaderTools.ReadUInt16(s);
            //Create the database object.
            RpDbDatabase db = new RpDbDatabase(s);
            //Create the tables array in the database.
            db.tables = new RpDbTable[tableCount];
            //Read the table offsets.
            UInt32[] tableOffsets = new UInt32[tableCount];
            string[] tableNames = new string[tableCount];
            //Read each of the offsets and table names.
            for (int i = 0; i<tableCount; i++)
            {
                tableOffsets[i] = ReaderTools.ReadUInt32(s);
                //Read the name for verification.
                tableNames[i] = ReaderTools.ReadString(s);
            }
            //Read each of the tables, starting at the offsets.
            UInt32 startingOffset = (UInt32)s.Position;
            for(int tableId = 0; tableId<tableCount; tableId++)
            {
                //Jump to the position offered.
                s.Position = startingOffset + tableOffsets[tableId];
                db.tables[tableId] = TableReader.ReadTable(s, types, db);
            }
            //Return the database.
            return db;
        }
    }
}
