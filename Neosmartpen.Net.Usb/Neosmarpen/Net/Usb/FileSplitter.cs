using System;
using System.Collections.Generic;
using System.Linq;

namespace Neosmartpen.Net.Usb
{
    public class FileSplitter
    {
        private Dictionary<int, byte[]> chunks;

        public int PacketCount { private set; get; }
        public string FilePath { private set; get; }
        public int FileSize { private set; get; }
        public int PacketSize { private set; get; }

        private int leftByte = 0;

        private byte[] datas;

        public FileSplitter()
        {
        }

        public bool Load(string filePath)
        {
            try
            {
                FilePath = filePath;
                datas = System.IO.File.ReadAllBytes(FilePath);
                FileSize = datas.Length;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Split(int packetSize)
        {
            if (datas == null || datas.Length <= 0)
                return false;

            try
            {
                PacketSize = packetSize > FileSize ? FileSize : packetSize;
                chunks = new Dictionary<int, byte[]>();
                if ((leftByte = FileSize % PacketSize) == 0)
                    PacketCount = FileSize / PacketSize;
                else
                    PacketCount = (FileSize / PacketSize) + 1;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public byte[] GetBytes(int offset)
        {
            return datas.Skip(offset).Take(PacketSize).ToArray();
        }
    }
}
