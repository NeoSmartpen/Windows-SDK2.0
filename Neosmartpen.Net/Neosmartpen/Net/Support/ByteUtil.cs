using System;

namespace Neosmartpen.Net.Support
{
    /// <summary>
    /// This class functions as a ByteBuffer 
    /// </summary>
    public class ByteUtil
    {
        public const int DEF_LIMIT = 1000, DEF_GROWTH = 1000;

        private int mPosWrite = 0, mPosRead = 0;

        private byte[] mBuffer;

        public delegate byte[] EscapeDelegate( byte input );

        private EscapeDelegate mEscDele;

        public int Size
        {
            get { return mBuffer != null ? mBuffer.Length : 0; }
        }

        public int Max
        {
            get { return mBuffer != null ? mBuffer.Length - 1 : 0; }
        }

        public ByteUtil( int length, EscapeDelegate escape = null )
        {
            mBuffer = new byte[length];
            mEscDele = escape;
        }

        public ByteUtil( EscapeDelegate escape = null )
            : this( DEF_LIMIT, escape )
        {
        }

        public ByteUtil( byte[] data ) : this( data.Length, null )
        {
            //mBuffer = new byte[data.Length];
            Array.Copy( data, mBuffer, data.Length );
        }

        public void Clear()
        {
            mPosWrite = 0;
            mPosRead = 0;

            Array.Clear( mBuffer, 0, mBuffer.Length );
        }

        public ByteUtil Put( byte[] inputs )
        {
            foreach ( byte b in inputs )
            {
                Put( b );
            }

            return this;
        }

        public ByteUtil Put( byte[] inputs, int length )
        {
            int alength = inputs.Length < length ? inputs.Length : length;
            
            byte[] result = new byte[length];

            Array.Clear( result, 0, length );
            Array.Copy( inputs, 0, result, 0, alength );

            return Put( result );
        }

        public ByteUtil Put( byte input, bool escapeIfExist = true )
        {
            if ( mEscDele != null && escapeIfExist )
            {
                byte[] escDatas = mEscDele( input );

                foreach ( byte item in escDatas )
                {
                    PutByte( item );
                }
            }
            else
            {
                PutByte( input );
            }

            return this;
        }

        private ByteUtil PutByte( byte input )
        {
            if ( mPosWrite > Max )
            {
                Expand( DEF_GROWTH );    
            }

            mBuffer[mPosWrite++] = input;
            //AddSingleByte( input );

            return this;
        }

        public ByteUtil PutNull( int length )
        {
            for ( int i=0;i < length; i++ )
            {
                Put( 0x00 );
            }

            return this;
        }

        public ByteUtil PutInt(int input)
        {
            return Put( ByteConverter.IntToByte( input ) );
        }

        public ByteUtil PutLong( long input )
        {
            return Put( ByteConverter.LongToByte( input ) );
        }

        public ByteUtil PutShort( short input )
        {
            return Put( ByteConverter.ShortToByte( input ) );
        }

        /*
        private void AddSingleByte( byte input )
        {
            mBuffer[mPosWrite++] = input;
        }
        */

        private void Expand( int increase )
        {
            Array.Resize<byte>( ref mBuffer, mBuffer.Length + increase );
        }

        #region get

        public int GetInt()
        {
            return BitConverter.ToInt32( GetBytes( 4 ), 0 );
        }

        public short GetShort()
        {
            return BitConverter.ToInt16( GetBytes( 2 ), 0 );
        }
        public ushort GetUShort()
        {
            return BitConverter.ToUInt16( GetBytes( 2 ), 0 );
        }

        public long GetLong()
        {
            return BitConverter.ToInt64( GetBytes( 8 ), 0 );
        }

        public byte[] GetBytes()
        {
            return GetBytes( mPosWrite - mPosRead );
        }

        public byte[] GetBytes( int size )
        {
            byte[] result = new byte[size];

            Array.Copy( mBuffer, mPosRead, result, 0, size );

            mPosRead += size;

            return result;
        }

        public byte GetByte()
        {
            return GetBytes( 1 )[0];
        }

        public int GetByteToInt()
        {
            return (int)( GetByte() & 0xFF );
        }

        public string GetString( int length )
        {
            return System.Text.Encoding.Default.GetString( GetBytes( length ) ).Trim( '\0' );
        }

        public byte GetChecksum( int length )
        {
            byte[] bytes = new byte[length];

            Array.Copy( mBuffer, mPosRead, bytes, 0, length );

            int CheckSum = 0;

            for ( int i = 0; i < bytes.Length; i++ )
            {
                CheckSum += (int)( bytes[i] & 0xFF );
            }

            return (byte)CheckSum;
        }

        public byte[] ToArray()
        {
            byte[] result = new byte[mPosWrite];

            Array.Copy( mBuffer, result, mPosWrite);

            return result;
        }

        #endregion
    }
}
