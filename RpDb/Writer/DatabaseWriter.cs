using RpDb.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpDb.Writer
{
    static class DatabaseWriter
    {
        public static void WriteDatabase(Stream s, RpDbTable[] tables)
        {
            //We'll rewind to the beginning of the stream and begin writing.  https://docs.google.com/document/d/1PVuW3CshGg0BvQHSyrGBQhFceaTk92H3Fv8uKOkhpX8/edit
            //Write the name of this.
            WriterTools.WriteChars(s, new char[] { 'R', 'p', 'D', 'b' });
            //Write the version.
            WriterTools.WriteUInt16(s, RpDbDatabase.RPDB_VERSION);
            //Skip 32 bytes. These'll be used later.
            s.Position += 32;

            //We're going to write each table's data now.
            MemoryStream[] streams = new MemoryStream[tables.Length];
            Parallel.For(0,tables.Length, delegate (int i)
            {
                //Open MemoryStream for this table.
                MemoryStream stream = new MemoryStream();
                RpDbTable table = tables[i];
                //Write table.
                TableWriter.WriteTable(stream, table);
                //Save this stream.
                streams[i] = stream;
            });

            //Now, we'll write the table TOC.
            //Write the number of entries.
            WriterTools.WriteUInt16(s, (UInt16)tables.Length);
            //Now, write all of the entries.
            UInt32 offset = 0;
            for(int i = 0; i<tables.Length; i++)
            {
                RpDbTable table = tables[i];
                MemoryStream stream = streams[i];
                //Write the offset and the name.
                WriterTools.WriteUInt32(s, offset);
                //Now, the name.
                WriterTools.WriteString(s, table.name);
                //Add the offset.
                offset += (UInt32)stream.Length;
            }
            //Now, write all of the streams in order.
            for(int i = 0; i<streams.Length; i++)
            {
                MemoryStream stream = streams[i];
                stream.Position = 0;
                stream.CopyTo(s);
                //Clear
                stream.Close();
                stream.Dispose();
            }
        }
    }
}
