using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq;
using System.IO;
using RpDb.Writer;
using RpDb.Entities;
using RpDb.Reader;

namespace RpDb
{
    public class RpDbDatabase
    {
        public const UInt16 RPDB_VERSION = 1;

        public RpDbTable[] tables = new RpDbTable[4];

        public readonly Stream stream; //Stream is always open and is never closed.

        public void SaveNow(string filename)
        {
            File.Delete(filename);
            using(FileStream fs = new FileStream(filename, FileMode.Create))
            {
                DatabaseWriter.WriteDatabase(fs, tables);
            }
        }

        public static RpDbDatabase LoadDatabase(string path, Type[] types)
        {
            RpDbDatabase db;
            FileStream fs = new FileStream(path, FileMode.Open);
            db = DatabaseReader.ReadDatabase(fs, types);
            return db;
        }

        public RpDbDatabase(Stream s)
        {
            stream = s;
        }
    }
}
