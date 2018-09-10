using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RpDb.Entities
{
    public class RpDbTableEntry
    {
        public UInt32 uuid;
        public UInt32 offset;

        public readonly RpDbTable table;

        public object data;

        public RpDbTableEntry(RpDbTable _table, object _data)
        {
            table = _table;
            data = _data;
        }
    }
}
