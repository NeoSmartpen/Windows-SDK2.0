using System;

namespace Neosmartpen.Net
{
    /// <exclude />
    public class Packet : IPacket
    {
        public int Cmd { protected set; get; }

        public int Result { protected set; get; }

        public byte[] Data { protected set; get; }

        private int mIndex = 0;

        public byte GetChecksum( int length )
        {
            byte[] bytes = new byte[length];

            Array.Copy( Data, mIndex, bytes, 0, length );

            int CheckSum = 0;

            for ( int i = 0; i < bytes.Length; i++ )
            {
                CheckSum += (int)( bytes[i] & 0xFF );
            }

            return (byte)CheckSum;
        }

        public byte GetChecksum()
        {
            return GetChecksum( Data.Length - mIndex );
        }

		public bool CheckMoreData()
		{
			return Data.Length > mIndex;
		}

        public int GetInt()
        {
            return BitConverter.ToInt32( GetBytes(4), 0 );
        }
        public uint GetUInt()
        {
            return BitConverter.ToUInt32( GetBytes(4), 0 );
        }

        public short GetShort()
        {
            return BitConverter.ToInt16( GetBytes(2), 0 );
        }

		public ushort GetUShort()
		{
			return BitConverter.ToUInt16(GetBytes(2), 0);
		}

        public long GetLong()
        {
            return BitConverter.ToInt64( GetBytes(8), 0 );
        }
        public ulong GetULong()
        {
            return BitConverter.ToUInt64( GetBytes(8), 0 );
        }

        public int GetByteToInt()
        {
            return (int)( GetByte() & 0xFF );
        }

        public byte[] GetBytes()
        {
            return GetBytes( Data.Length - mIndex );
        }

        public byte[] GetBytes( int size )
        {
            byte[] result = new byte[size];

            Array.Copy( Data, mIndex, result, 0, size );

            Move( size );

            return result;
        }

        public IPacket Move( int size )
        {
            mIndex += size;
            return this;
        }

        public IPacket Reset()
        {
            mIndex = 0;
            return this;
        }

        public byte GetByte()
        {
            return GetBytes(1)[0];
        }

        public string GetString( int length )
        {
            return System.Text.Encoding.UTF8.GetString( GetBytes( length ) ).Trim( '\0' );
        }

        public override string ToString()
        {
            return BitConverter.ToString( Data, 0 );
        }

        public class Builder
        {
            private Packet mPacket;

            public Builder()
            {
                mPacket = new Packet();
            }

            public Builder cmd( int cmd )
            {
                mPacket.Cmd = cmd;
                return this;
            }

            public Builder result( int code )
            {
                mPacket.Result = code;
                return this;
            }

            public Builder data( byte[] data )
            {
                mPacket.Data = data;
                return this;
            }

            public Packet Build()
            {
                return mPacket;
            }
        }
    }
}
