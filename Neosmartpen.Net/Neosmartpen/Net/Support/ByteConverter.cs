using System;

namespace Neosmartpen.Net.Support
{
    /// <summary>
    ///Data type converter between byte[] and short, int, long
    /// </summary>
    public class ByteConverter
    {
        public static long ByteToLong(byte[] data)
        {
            //long result = data[0] + ( data[1] << 8 ) + ( data[2] << 16 ) + ( data[3] << 24 )
                    //+ ( data[4] << 32 ) + ( data[5] << 40 ) + ( data[6] << 48 ) + ( data[7] << 56 );
            //return result;

            return BitConverter.ToInt64( data, 0 );
        }

        public static int ByteToInt(byte[] data)
        {
            int result = data[0] + ( data[1] << 8 ) + ( data[2] << 16 ) + ( data[3] << 24 );
            return result;
        }

        public static int SingleByteToInt(byte data)
        {
            return (int)( data & 0xFF );
        }

        public static short ByteToShort(byte[] data)
        {
            int result = data[0] + ( data[1] << 8 );
            return (short)result;
        }

        public static byte[] ShortToByte(short value)
        {
            byte[] b = BitConverter.GetBytes( value );
            return b;
        }

        public static byte[] LongToByte( long value )
        {
            byte[] b = BitConverter.GetBytes( value );
            return b;
        }

        public static byte[] IntToByte( int value )
        {
            byte[] b = BitConverter.GetBytes( value );
            return b;
        }
    }
}
