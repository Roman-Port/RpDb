using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RpDb.Entities;

namespace RpDb.Reader
{
    class TableReader
    {
        public static RpDbTable ReadTable(Stream s, Type[] types, RpDbDatabase database)
        {
            //Read the table name.
            string name = ReaderTools.ReadString(s);
            //Read the type
            string typeName = ReaderTools.ReadString(s);
            //Determine the type.
            Type t = null;
            //Loop through types and find match.
            foreach(Type tt in types)
            {
                if (tt.ToString() == typeName)
                    t = tt;
            }
            //Check if it was found.
            if (t == null)
                throw new Exception("Failed to find type '" + typeName + "'.");
            //Create the object.
            RpDbTable table = new RpDbTable(name, t, database);
            //Read the number of entries.
            UInt32 entryCount = ReaderTools.ReadUInt32(s);
            //Read number of types.
            UInt16 typeCount = ReaderTools.ReadUInt16(s);
            //Read the type table in. This'll give us the order.
            Writer.PropIdPair[] order = new Writer.PropIdPair[typeCount];
            for(int i = 0; i<typeCount; i++)
            {
                //Read in.
                UInt16 typeId = ReaderTools.ReadUInt16(s);
                ClassAttrib attr = null;
                System.Reflection.PropertyInfo chosenProp = null;
                //Find this in the object.
                foreach (var prop in table.type.GetProperties())
                {
                    //Check the UUID.
                    ClassAttrib propAttr = Writer.TableWriter.GetUuidFromProperty(prop);
                    //If there is no UUID, skip this.
                    if (propAttr == null)
                    {
                        continue;
                    }
                    //Add this to the order if it matches
                    if (propAttr.uuid == typeId)
                    {
                        attr = propAttr;
                        chosenProp = prop;
                    }

                }
                if(attr != null && chosenProp != null)
                {
                    order[i] = new Writer.PropIdPair(chosenProp, typeId);
                } else
                {
                    Console.WriteLine("No attr found for ID "+typeId.ToString()+".");
                }
            }
            table.typeOrder = order;
            //Now that we know the order of the types, we can read it in.
            //Skip 32 bytes of reserved data.
            s.Position += 32;
            //Read each of the offsets in. Add them to the dict as we go.
            for (int i = 0; i<entryCount; i++)
            {
                //Read offset
                UInt32 offset = ReaderTools.ReadUInt32(s);
                //Read UUID
                UInt32 uuid = ReaderTools.ReadUInt32(s);
                //Insert.
                table.uuidLookup.Add(uuid, offset);
            }
            //Now, the data begins. Add the flag.
            table.tableDataOffset = s.Position;
            //We can now continue to the next table.
            return table;
        }
    }
}
