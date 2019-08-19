using Neosmartpen.Net.Support;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neosmartpen.Net.Usb
{
    /// <summary>
    /// 오프라인 데이터 조각을 받아 파일로 완성
    /// </summary>
    public class FileBuilder
    {
        private ByteUtil byteUtil;
	
        public string FilePath { private set; get; }
        public int FileSize { private set; get; }
        public int PacketSize { private set; get; }

        public FileBuilder( string filePath, int fileSize, int packetSize )
	    {
            byteUtil = new ByteUtil();
            FilePath = filePath;
            FileSize = fileSize;
            PacketSize = packetSize > fileSize ? fileSize : packetSize;
	    }

        public int GetPacketSize(int index)
        {
            if (index < FileSize - 1)
            {
                if ((FileSize - 1 - index) > PacketSize)
                {
                    return PacketSize;
                }
                else
                {
                    return PacketSize - (FileSize - 1 - index);
                }
            }
            else
            {
                return -1;
            }
        }

	    public bool MakeFile() 
	    {
            return ByteToFile(byteUtil.ToArray(), FilePath);
        }

	    public bool Put(byte[] data, int offset) 
	    {
            if (byteUtil.WritePosition == offset)
            {
                byteUtil.Put(data);
                return true;
            }
            else
                return false;
        }

        public int GetNextOffset()
        {
            return byteUtil.WritePosition;
        }

        private bool ByteToFile( byte[] bytes, String filepath ) 
	    {
            try
            {
                System.IO.FileStream fs = new System.IO.FileStream( filepath, System.IO.FileMode.Create, System.IO.FileAccess.Write );
                fs.Write( bytes, 0, bytes.Length );
                fs.Close();

                return true;
            }
            catch ( Exception e )
            {
                Console.WriteLine( "[FileSerializer] Exception caught in process: {0}", e.StackTrace );
            }

            // error occured, return false
            return false;
	    }
    }
}
