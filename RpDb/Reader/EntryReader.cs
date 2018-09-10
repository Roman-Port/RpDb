using RpDb.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RpDb.Reader
{
    class EntryReader
    {
        public static RpDbTableEntry ReadEntry(Stream s, RpDbTable table)
        {
            //Create object.
            RpDbTableEntry entry = new RpDbTableEntry(table, null);
            entry.offset = (UInt32)s.Position;
            //First, read in the UUID.
            UInt32 uuid = ReaderTools.ReadUInt32(s);
            entry.uuid = uuid;
            //Read in the lengths.
            UInt16[] lengths = new UInt16[table.typeOrder.Length];
            for (int i = 0; i < table.typeOrder.Length; i++)
            {
                lengths[i] = ReaderTools.ReadUInt16(s);
            }
            //Now, read in the data
            object obj = Activator.CreateInstance(table.type);
            for (int i = 0; i < table.typeOrder.Length; i++)
            {
                Writer.PropIdPair pair = table.typeOrder[i];
                //Skip this if the length is zero. (it's null)
                UInt16 length = lengths[i];
                if(length != 0)
                {
                    //Read in the data.
                    byte[] buf = new byte[length];
                    s.Read(buf, 0, length);
                    //Todo: Actually get the data from it.
                    //For now, just read it in as a string
                    object d = Encoding.UTF8.GetString(buf);
                    //Now, set it inside the object.
                    pair.prop.SetValue(obj, d, null);
                }
            }
            //Set the entry's object to this.
            entry.data = obj;
            //Return the entry.
            return entry;
        }
    }
}
