using Neosmartpen.Net.Usb.Demo.Util;
using System.IO;

namespace Neosmartpen.Net.Usb.Demo.OfflineData
{
    class DotDataHeader
    {
        public static readonly int length = 32;

        public byte fileType;
        public short version;
        public short fileNumber;                //reserved
        public int sectionId, ownerId, noteId;  //Int64 notebookId
        public byte[] macAddress = new byte[6];
        public byte[] reserved = new byte[12];
        public byte checkSum;
        public bool isLittleEndian = false;
        public static int endianByteIndex = 19;

        public DotDataHeader()
        {
        }

        public bool readFromStream(FileStream stream)
        {
            byte[] buffer = new byte[length];
            int result = stream.Read(buffer, 0, buffer.Length);

            if (result != buffer.Length)
                return false;

            isLittleEndian = buffer[endianByteIndex] != 0;

            MemoryInputStream ms = new MemoryInputStream(buffer, isLittleEndian);

            fileType = (byte)ms.ReadByte();
            if ((char)fileType != 'C')
                return false;

            version = ms.readInt16();
            fileNumber = ms.readInt16();

            long n = ms.readInt64();
            sectionId = (int)(n >> 56);
            ownerId = (int)(n >> 32) & 0x00ffffff;
            noteId = (int)(n & 0xffffffff);

            ms.Read(macAddress, 0, macAddress.Length);
            ms.Read(reserved, 0, reserved.Length);
            checkSum = (byte)ms.ReadByte();

            byte sum = ms.getByteCheckSum(0, buffer.Length - 1);
            return (sum == checkSum);
        }
    }
}
