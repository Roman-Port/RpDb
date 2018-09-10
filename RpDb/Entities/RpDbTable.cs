using RpDb.Reader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RpDb.Entities
{
    public class RpDbTable
    {
        public string name;
        public Type type;

        public readonly RpDbDatabase database;

        public Dictionary<UInt32, RpDbTableEntry> modifyBuffer = new Dictionary<uint, RpDbTableEntry>(); //Changes pending saving to the disk. Key is the uuid.

        public Dictionary<UInt32, UInt32> uuidLookup = new Dictionary<uint, uint>(); //Lookup on the image. Key is the UUID, value is the offset starting from 0.

        public long tableDataOffset; //Byte in the file where the data starts.

        public Writer.PropIdPair[] typeOrder;

        public int GetNumberOfEntries()
        {
            //Look for new entries in the modify buffer.
            int newItems = 0;

            foreach(uint entry in modifyBuffer.Keys)
            {
                if (!uuidLookup.ContainsKey(entry))
                {
                    //New.
                    newItems++;
                }
            }

            return newItems + uuidLookup.Count;
        }

        private UInt32 GenerateId(Random r)
        {
            byte[] buf = new byte[4];
            r.NextBytes(buf);
            UInt32 uuid = BitConverter.ToUInt32(buf, 0);
            return uuid;
        }

        public void AddNewEntry(object data)
        {
            //Add a whole new entry with a new ID.
            //Generate an ID
            Random r = new Random();
            UInt32 uuid = GenerateId(r);
            while(modifyBuffer.ContainsKey(uuid) || uuidLookup.ContainsKey(uuid))
            {
                //Generate new
                uuid = GenerateId(r);
            }
            RpDb.Entities.RpDbTableEntry e = new RpDb.Entities.RpDbTableEntry(this, data);
            e.uuid = uuid;
            //Add
            modifyBuffer.Add(uuid, e);
        }

        public RpDbTableEntry ReadResultByUuid(UInt32 uuid)
        {
            //Note: THIS IS NOT THREAD SAFE!!!!!!!!!!!

            RpDbTableEntry entry = null;

            //Check where we want to seek. First, check if this exists on the file and not loaded into memory.
            if(uuidLookup.ContainsKey(uuid))
            {
                //Check the modify buffer to see if we have an updated version loaded into memory.
                if(modifyBuffer.ContainsKey(uuid))
                {
                    //We have an updated version inside of the memory buffer.
                    entry = modifyBuffer[uuid];
                } else
                {
                    //This must be loaded from disk into memory.
                    //Jump to the position in the stream we need.
                    database.stream.Position = tableDataOffset + uuidLookup[uuid];
                    //Read it in.
                    entry = EntryReader.ReadEntry(database.stream, this);
                }
            } else
            {
                //This UUID might exist inside of the modify buffer.
                if (modifyBuffer.ContainsKey(uuid))
                {
                    //We have an updated version inside of the memory buffer.
                    entry = modifyBuffer[uuid];
                } else
                {
                    //This doesn't exist.
                    entry = null;
                }
            }

            //Return entry.
            return entry;
        }

        public RpDbTable(string _name, Type _type, RpDbDatabase _db)
        {
            name = _name;
            type = _type;
            database = _db;
        }
    }
}
