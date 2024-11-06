using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Neosmartpen.Net
{
    public class ChunkEx
    {
        // chunk size is 0.5k
        private int mSize = 1024;

        private int mRows;

        private byte[] mDatas;

        private int mFileSize = 0;

        public ChunkEx(int chunksize)
        {
            mSize = chunksize;
        }

        public async Task<bool> Load(string filepath)
        {
            return await Load(await StorageFile.GetFileFromPathAsync(filepath));
        }

        public async Task<bool> Load(StorageFile filepath)
        {
            byte[] datas = null;

            try
            {
                IBuffer buffer = await FileIO.ReadBufferAsync(filepath);
                datas = System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeBufferExtensions.ToArray(buffer);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }

            if (datas == null)
            {
                return false;
            }

            mFileSize = datas.Length;

            double filesize = datas.Length / mSize;

            mRows = (int)Math.Ceiling(filesize) + 1;

            mDatas = datas;

            return true;
        }

        public byte[] Get(int offset, int size)
        {
            return mDatas.Skip(offset).Take(size).ToArray();
        }

        public byte[] Get(int offset)
        {
            return Get(offset, mSize);
        }

        public int GetFileSize()
        {
            return mFileSize;
        }

        public int GetChunkLength()
        {
            return mRows;
        }

        public int GetChunksize()
        {
            return mSize;
        }

        public byte GetTotalChecksum()
        {
            return CalcChecksum(mDatas);
        }

        public static byte CalcChecksum(byte[] bytes)
        {
            int CheckSum = 0;

            for (int i = 0; i < bytes.Length; i++)
            {
                CheckSum += (int)(bytes[i] & 0xFF);
            }

            return (byte)CheckSum;
        }
    }
}
