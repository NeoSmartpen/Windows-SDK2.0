using Neosmartpen.Net.Usb.Demo.Util;
using System;
using System.IO;

namespace Neosmartpen.Net.Usb.Demo.OfflineData
{
    class StrokeRef
    {
        public static readonly int length = 32;

        public Int64 downTime;
        public int upTimeFromDownTime;      //a pen up time is relative value from down time.
        public int pageId;
        public byte status;                 //reserved
        public byte penTipType;             //0: normal, 1: remover
        public int penTipColor;             //Argb  (default: 0xff000000)
        public short codeTableFileNumber;   //reserved (0 or 1)
        public int codeTableFileOffset;     //Dot data start offset in a Dot Table file
        public short codeCount;             //Dot data count in a Dot Table file
        public byte successRate;           //reserved
        public byte checkSum;
        public bool isLittleEndian = false;

        public StrokeRef(bool isLittleEndian)
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

            downTime = ms.readInt64();
            upTimeFromDownTime = ms.readInt32();
            pageId = ms.readInt32();
            status = (byte)ms.ReadByte();
            penTipType = (byte)ms.ReadByte();
            penTipColor = ms.readInt32();
            codeTableFileNumber = ms.readInt16();
            codeTableFileOffset = ms.readInt32();
            codeCount = ms.readInt16();
            successRate = (byte)ms.ReadByte();
            checkSum = (byte)ms.ReadByte();

            byte sum = ms.getByteCheckSum(0, buffer.Length - 1);
            return (sum == checkSum);
        }
    }
}
