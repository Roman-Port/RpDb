using RpDb.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RpDb.Writer
{
    static class TableWriter
    {
        public static ClassAttrib GetUuidFromProperty(System.Reflection.PropertyInfo prop )
        {
            //Check if this has a UUID attached.
            ClassAttrib propAttr = null;

            foreach (var attr in prop.GetCustomAttributes(true))
            {
                if (attr.GetType() == typeof(ClassAttrib))
                {
                    //Correct.
                    propAttr = (ClassAttrib)attr;
                }
            }

            return propAttr;
        }

        public static void WriteTable(Stream s, RpDbTable table)
        {
            //First, write the table name.
            WriterTools.WriteString(s, table.name);
            //Write the type name.
            WriterTools.WriteString(s, table.type.ToString());
            //Now, write the number of entries.
            WriterTools.WriteUInt32(s, (UInt32)table.GetNumberOfEntries());
            //Write the type table now. Go through each attribute in the type.
            //Determine the order.
            List<PropIdPair> order = new List<PropIdPair>();
            foreach (var prop in table.type.GetProperties())
            {
                //Check the UUID.
                ClassAttrib propAttr = GetUuidFromProperty(prop);
                //If there is no UUID, skip this.
                if (propAttr == null)
                {
                    Console.WriteLine("(debug) Skipping because no attr.");
                    continue;
                }
                //Add this to the order.
                order.Add(new PropIdPair(prop,propAttr.uuid));
            }
            //Now, write number of types.
            WriterTools.WriteUInt16(s, (UInt16)order.Count);
            //Write the type table itself.
            foreach(PropIdPair o in order)
            {
                WriterTools.WriteUInt16(s, o.typeid);
            }
            //Skip 32 bytes of reserved space.
            s.Position += 32;
            //Now, we'll find all the entries and put them in a stream.
            //We'll create a stream for the actual data and copy it here later.
            MemoryStream dataStream = new MemoryStream();
            //We'll now find and include data.

            //First, find new entries.

            List<UInt32> uuidsToSkip = new List<uint>(); //These are the UUIDs that we will skip when we copy directly from the unmodified enteries.

            foreach(RpDbTableEntry entry in table.modifyBuffer.Values)
            {
                //Check if this key exists in the uuid lookup. If it does, this is a UPDATED entry. If it doesn't, this is a NEW entry.
                bool existsBefore = table.uuidLookup.ContainsKey(entry.uuid);
                //Add this now.
                UInt32 offset = ValueWriter.WriteEntry(dataStream, s, entry, order);
                //Add this to the written dictonary, but also add it to the skipped list so we don't rewrite it. 
                uuidsToSkip.Add(entry.uuid);
                entry.offset = offset;
                //Make sure we know the offset.
                if (!existsBefore)
                {
                    //If this wasn't already in the lookup table, add it.
                    table.uuidLookup.Add(entry.uuid, offset);
                } else
                {
                    //Update the offset now.
                    table.uuidLookup[entry.uuid] = offset;
                }
            }

            //Now, we'll add existing, untouched, files.
            foreach(var entry in table.uuidLookup)
            {
                //Check if this is in the "uuidstoskip" list.
                if(!uuidsToSkip.Contains(entry.Key))
                {
                    //We'll copy this to the new one.
                    //TODO: COPY!!!!!!!!!!!
                    throw new NotImplementedException();
                }
            }

            //Now that that is done, we can copy the data stream to the main stream.
            dataStream.Position = 0;
            dataStream.CopyTo(s);
        }

        
    }

    public struct PropIdPair
    {
        public System.Reflection.PropertyInfo prop;
        public UInt16 typeid;

        public PropIdPair(System.Reflection.PropertyInfo _prop, UInt16 _typeid)
        {
            typeid = _typeid;
            prop = _prop;
        }
    }
}
