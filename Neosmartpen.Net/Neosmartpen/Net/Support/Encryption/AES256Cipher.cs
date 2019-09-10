using System.IO;
using System.Security.Cryptography;

namespace Neosmartpen.Net.Support.Encryption
{
    public class AES256Cipher
    {
        private byte[] secretKey;

        public AES256Cipher(byte[] key)
        {
            secretKey = key;
        }

        public byte[] Decode(byte[] dataToDecrypt)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;
            aes.Key = secretKey;
            aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            var decrypt = aes.CreateDecryptor();
            byte[] xBuff = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                {
                    cs.Write(dataToDecrypt, 0, dataToDecrypt.Length);
                }

                xBuff = ms.ToArray();
            }

            return xBuff;
        }
    }
}
