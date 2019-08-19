using Neosmartpen.Net.Usb.Demo.Util;
using System.IO;

namespace Neosmartpen.Net.Usb.Demo.OfflineData
{
    class StrokeFileHeader
    {
        public static readonly int length = 32;

        public byte fileType;
        public short version;
        public int sectionId, ownerId, noteId;  //Int64 notebookId
        public byte[] macAddress = new byte[6];
        public byte[] reserved = new byte[14];
        public byte checkSum;
        public bool isLittleEndian = false;
        public static int endianByteIndex = 17;

        public StrokeFileHeader()
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
            if ((char)fileType != 'T')
                return false;

            version = ms.readInt16();

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
