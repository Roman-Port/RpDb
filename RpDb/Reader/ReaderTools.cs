using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RpDb.Reader
{
    class ReaderTools
    {
        public static string ReadString(Stream s)
        {
            //Read bytes.
            byte[] buf = new byte[2048 * 4];
            //Kepe reading until null.
            int pos = 0;
            while(true)
            {
                s.Read(buf, pos, 1);
                if (buf[pos] == 0x00)
                    break;
                pos++;
            }
            //Convert
            return Encoding.UTF8.GetString(buf,0,pos);
        }

        public static char[] ReadChars(Stream s, int size)
        {
            //Read that many.
            byte[] buf = new byte[size];
            //Read
            s.Read(buf, 0, size);
            //Return encoded
            return Encoding.ASCII.GetChars(buf);
        }

        public static byte[] ReadBytesRespectEndian(Stream s, int size)
        {
            //Read
            byte[] buf = new byte[size];
            //Read
            s.Read(buf, 0, size);
            //Respect endian
            if (BitConverter.IsLittleEndian != Writer.WriterTools.DST_LITTLE_ENDIAN)
                Array.Reverse(buf);
            //Return
            return buf;
        }

        public static UInt32 ReadUInt32(Stream s)
        {
            return BitConverter.ToUInt32(ReadBytesRespectEndian(s, 4), 0);
        }

        public static UInt16 ReadUInt16(Stream s)
        {
            return BitConverter.ToUInt16(ReadBytesRespectEndian(s, 2), 0);
        }
    }
}
