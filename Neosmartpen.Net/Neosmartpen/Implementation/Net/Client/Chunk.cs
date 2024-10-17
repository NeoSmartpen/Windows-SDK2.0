using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Neosmartpen.Net
{
    public class Chunk
    {
        // chunk size is 0.5k
        private int mSize = 512;

        private int mRows;

        private List<byte[]> mBuffer;

        private int mFileSize = 0;

        private byte mCheckSum;

        public Chunk( int chunksize )
        {
            mSize = chunksize;
        }

        public Chunk()  
        {
        }

        public static List<byte[]> SplitByteArray( byte[] bytes, int range )
        {
            List<byte> chunk = new List<byte>();
            List<byte[]> result = new List<byte[]>();

            int i = 1;
            int c = 1;

            foreach ( byte b in bytes )
            {
                // Put the byte into the bytes array
                chunk.Add( b );

                // If we match the range, add the byte array and create new one
                if ( i == range || c == bytes.Length )
                {
                    // Add as array
                    result.Add( chunk.ToArray() );

                    // Create again
                    chunk = new List<byte>();
                    i = 0;
                }

                c++;
                i++;
            }

            return result;
        }

        

        public async Task<bool> Load( StorageFile filepath )
        {
            byte[] datas = null;

            try
            {
                IBuffer buffer = await FileIO.ReadBufferAsync(filepath);
                datas = System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeBufferExtensions.ToArray(buffer);

                mCheckSum = CalcChecksum( datas );
            }
            catch ( Exception ex )
            {
                Debug.WriteLine(ex.Message);
                return false;
            }

            if ( datas == null )
            {
                return false;
            }

            mFileSize = datas.Length;

            double filesize = datas.Length / mSize;

            mRows = (int)Math.Ceiling( filesize ) + 1;

            mBuffer = SplitByteArray( datas, mSize );

            return true;
        }

        public bool Load( byte[] datas )
        {
            try
            {
                mCheckSum = CalcChecksum(datas);
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

            mBuffer = SplitByteArray(datas, mSize);

            return true;
        }

        public byte[] Get( int number ) 
        {
            return mBuffer != null &&  mBuffer.Count > number ? mBuffer[number] : null;
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

        public byte GetChecksum( int number )
        {
            return Get( number ) != null ? CalcChecksum( Get( number ) ) : (byte)0x00;
        }

        public byte GetTotalChecksum()
        {
            return mCheckSum;
        }

        public static byte CalcChecksum(byte[] bytes) 
        {
            int CheckSum = 0;
        
            for( int i = 0; i < bytes.Length; i++)
            {
                 CheckSum += (int)(bytes[i] & 0xFF);
            }

            return (byte)CheckSum;
        }
    }
}
