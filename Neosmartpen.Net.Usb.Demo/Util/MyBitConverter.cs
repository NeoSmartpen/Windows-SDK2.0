
namespace Neosmartpen.Net.Usb.Demo.Util
{
    class MyBitConverter
    {
        public static short bigEndian_toInt16(byte[] buf, int ofs)
        {
            int a = buf[ofs++];
            int b = buf[ofs] & 0xff;
            short n = (short)(((a << 8) | b) & 0xffff);
            return n;
        }

        public static int bigEndian_toInt32(byte[] buf, int ofs)
        {
            int s = 0;

            s |= ((int)buf[ofs++] & 0xff) << 24;
            s |= ((int)buf[ofs++] & 0xff) << 16;
            s |= ((int)buf[ofs++] & 0xff) << 8;
            s |= (int)buf[ofs++] & 0xff;

            return s;
        }

        public static long bigEndian_toInt64(byte[] buf, int ofs)
        {
            long s = 0;

            s |= ((long)buf[ofs++] & 0xff) << 56;
            s |= ((long)buf[ofs++] & 0xff) << 48;
            s |= ((long)buf[ofs++] & 0xff) << 40;
            s |= ((long)buf[ofs++] & 0xff) << 32;
            s |= ((long)buf[ofs++] & 0xff) << 24;
            s |= ((long)buf[ofs++] & 0xff) << 16;
            s |= ((long)buf[ofs++] & 0xff) << 8;
            s |= (long)buf[ofs++] & 0xff;

            return s;
        }
    }
}
