using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RpDb
{
    public class ClassAttrib : Attribute
    {
        public ClassAttrib(UInt16 _uuid)
        {
            uuid = _uuid;
        }

        public UInt16 uuid;
    }
}
