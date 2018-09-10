using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RpDb.Writer
{
    static class WriterTools
    {
        public const bool DST_LITTLE_ENDIAN = true;

        public static int WriteString(Stream s, string text)
        {
            //Store length
            int len = 0;
            //Convert to UTF-8
            byte[] data = Encoding.UTF8.GetBytes(text);
            //Write to stream
            len += data.Length;
            s.Write(data, 0, data.Length);
            //If this doesn't end will null, add it.
            if(data[data.Length-1] != 0x00)
            {
                //Write a null to terminate the string.
                s.Write(new byte[] { 0x00 }, 0, 1);
                len += 1;
            }
            return len;
        }

        public static int WriteChars(Stream s, char[] ch)
        {
            //Store length
            int len = 0;
            //Convert to UTF-8
            byte[] data = Encoding.ASCII.GetBytes(ch);
            //Write to stream
            len += data.Length;
            s.Write(data, 0, data.Length);
            return len;
        }

        private static void ApplyEndian(ref byte[] input)
        {
            if (BitConverter.IsLittleEndian != DST_LITTLE_ENDIAN)
                Array.Reverse(input);
        } 

        public static int WriteUInt32(Stream s, UInt32 input)
        {
            byte[] data = BitConverter.GetBytes(input);
            ApplyEndian(ref data);
            s.Write(data, 0, data.Length);
            return data.Length;
        }

        public static int WriteUInt16(Stream s, UInt16 input)
        {
            byte[] data = BitConverter.GetBytes(input);
            ApplyEndian(ref data);
            s.Write(data, 0, data.Length);
            return data.Length;
        }
    }
}
