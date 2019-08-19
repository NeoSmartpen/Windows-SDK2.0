using Neosmartpen.Net.Usb.Demo.Util;
using System.IO;

namespace Neosmartpen.Net.Usb.Demo.OfflineData
{
    class DotData
    {
        public static readonly int length = 16;

        public byte timestampDelta;
        public short force;                     //0~1023
        public int x, y, fx, fy;                //x 3byte, y 3byte
        public byte xTilt, yTilt;               //reserved
        public short twist;                     //reserved
        public byte labelCount;                 //reserved
        public byte brightnessAndProcessTime;   //reserved
        public byte checkSum;
        public bool isLittleEndian = false;

        public DotData(bool isLittleEndian)
        {
            this.isLittleEndian = isLittleEndian;
        }

        public bool readFromStream(FileStream stream)
        {
            byte[] buffer = new byte[length];
            int result = stream.Read(buffer, 0, buffer.Length);

            if (result != buffer.Length)
                return false;

            MemoryInputStream ms = new MemoryInputStream(buffer, isLittleEndian);

            timestampDelta = (byte)ms.ReadByte();
            force = ms.readInt16();

            x = ms.readInt16() & 0xffff;
            y = ms.readInt16() & 0xffff;
            fx = ms.ReadByte() & 0xff;
            fy = ms.ReadByte() & 0xff;

            xTilt = (byte)ms.ReadByte();
            yTilt = (byte)ms.ReadByte();
            twist = ms.readInt16();
            labelCount = (byte)ms.ReadByte();
            brightnessAndProcessTime = (byte)ms.ReadByte();
            checkSum = (byte)ms.ReadByte();

            byte sum = ms.getByteCheckSum(0, buffer.Length - 1);
            return (sum == checkSum);
        }
    }
}
