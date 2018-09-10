using RpDb.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RpDb.Writer
{
    static class ValueWriter
    {
        public static UInt32 WriteEntry(Stream dataStream, Stream tableStream, RpDbTableEntry entry, List<PropIdPair> order)
        {
            //Add the data and return the offset.
            UInt32 offset = (UInt32)dataStream.Position;
            //Write the offset and uuid to the table.
            WriterTools.WriteUInt32(tableStream, offset);
            WriterTools.WriteUInt32(tableStream, entry.uuid);
            //Now, we'll write the data.
            //Get the serialized data for this class, following the order offered.
            byte[][] serData = new byte[order.Count][]; //Data to be serialized.
            for(int i = 0; i<order.Count; i++)
            {
                //This is run for each property in this. Serialize it.
                var pair = order[i];
                var prop = pair.prop;
                //Get the value from this using the property found earlier.
                object value = prop.GetValue(entry.data, null);
                Type type = value.GetType();
                //Convert this to bytes.
                byte[] data = GetSerializedData(value, type);
                //Add it.
                serData[i] = data;
            }
            //Write UUID
            WriterTools.WriteUInt32(dataStream, entry.uuid);
            //Write the lengths
            foreach(byte[] d in serData)
            {
                WriterTools.WriteUInt16(dataStream, (ushort)d.Length);
            }
            //Now, write all the data.
            foreach (byte[] d in serData)
            {
                dataStream.Write(d, 0, d.Length);
            }

            //Return offset.
            return offset;
        }

        private static byte[] GetSerializedData(object obj, Type t)
        {
            return Encoding.ASCII.GetBytes("This is some testing data!");
        }
    }
}
