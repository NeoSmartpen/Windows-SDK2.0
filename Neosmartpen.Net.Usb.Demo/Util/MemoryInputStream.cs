using System;
using System.IO;

namespace Neosmartpen.Net.Usb.Demo.Util
{
    class MemoryInputStream : MemoryStream
    {
        public bool littleEndian = true;
        private byte[] mArray = null;

        public MemoryInputStream(byte[] buffer, bool isLittleEndian = true)
            : base(buffer)
        {
            littleEndian = isLittleEndian;
            mArray = buffer;
        }

        public byte getByte(int index)
        {
            return mArray[index];
        }

        public byte getByteCheckSum(int startOfs, int length)
        {
            byte sum = 0;
            int endOfs = startOfs + length - 1;

            for (int i = startOfs; i <= endOfs; i++)
            {
                sum += mArray[i];
            }

            return sum;
        }

        public short readInt16()
        {
            byte[] buf = new byte[2];

            if (this.Read(buf, 0, 2) != 2)
                throw new Exception("MemoryInputStream: Could not read int16 from a stream");

            if (littleEndian)
                return BitConverter.ToInt16(buf, 0);
            else
                return MyBitConverter.bigEndian_toInt16(buf, 0);
        }

        public int readInt32()
        {
            byte[] buf = new byte[4];

            if (this.Read(buf, 0, 4) != 4)
                throw new Exception("MemoryInputStream: Could not read int32 from a stream");

            if (littleEndian)
                return BitConverter.ToInt32(buf, 0);
            else
                return MyBitConverter.bigEndian_toInt32(buf, 0);
        }

        public Int64 readInt64()
        {
            byte[] buf = new byte[8];

            if (this.Read(buf, 0, 8) != 8)
                throw new Exception("MemoryInputStream: Could not read int64 from a stream");

            if (littleEndian)
                return BitConverter.ToInt64(buf, 0);
            else
                return MyBitConverter.bigEndian_toInt64(buf, 0);
        }

        public string readString( int len )
        {
            byte[] buf = new byte[len];

            if (this.Read(buf, 0, len) != len)
                throw new Exception("MemoryInputStream: Could not read a string from a stream");

            return System.Text.Encoding.ASCII.GetString(buf, 0, buf.Length);
        }
    }
}
