using System;
using System.Numerics;

namespace Neosmartpen.Net.Support.Encryption
{
    public class RSACipher
    {
        public static byte[] Decrypt(PrivateKey privateKey, byte[] dataToDecrypt)
        {
            if (privateKey == null || dataToDecrypt == null)
            {
                return null;
            }

            byte[] temp = MakePositive(dataToDecrypt);
            Array.Reverse(temp);

            BigInteger value = new BigInteger(temp);
            BigInteger e = privateKey.GetPrivateExponent();
            BigInteger n = privateKey.GetModulus();

            BigInteger result = Power(value, e, n);

            byte[] resultBytes = result.ToByteArray();
            Array.Reverse(resultBytes);

            // remove positive byte
            if (resultBytes.Length == 33 && resultBytes[0] == 0x00)
            {
                temp = new byte[32];
                Array.Copy(resultBytes, 1, temp, 0, 32);
                return temp;
            }
            else if (resultBytes.Length < 32)
            {
                temp = new byte[32];
                Array.Copy(resultBytes, 0, temp, 32 - resultBytes.Length, resultBytes.Length);
                return temp;
            }

            return resultBytes;
        }

        private static BigInteger Power(BigInteger value, BigInteger e, BigInteger n)
        {
            BigInteger zero = new BigInteger(0);
            if (value.CompareTo(zero) == 0 || n.CompareTo(zero) == 0)
                return new BigInteger(0);
            if (e.CompareTo(zero) == 0)
                return new BigInteger(1) % n;

            BigInteger one = new BigInteger(1);

            BigInteger currentMode = value % n;
            BigInteger currentValue = (e & one).CompareTo(zero) > 0 ? currentMode : one;

            for (e = e >> 1; e.CompareTo(zero) > 0; e = e >> 1)
            {
                currentMode = (currentMode * currentMode) % n;
                if ((e & one).CompareTo(zero) > 0)
                {
                    currentValue = (currentValue * currentMode) % n;
                }
            }

            return currentValue;
        }

        private static byte[] MakePositive(byte[] data)
        {
            if ((data[0] & 0x80) == 0x80)
            {
                byte[] temp = new byte[data.Length + 1];
                temp[0] = 0x00;
                Array.Copy(data, 0, temp, 1, data.Length);
                return temp;
            }
            else
            {
                return data;
            }
        }
    }
}
