using Neosmartpen.Net.Support;
using Neosmartpen.Net.Support.Encryption;
using System;

namespace Neosmartpen.Net.Support.Encryption
{
    /// <summary>
    /// Class to read and process private key file
    /// </summary>
    public class PrivateKeyFileReader
    {
        private static string PKCS1_BEGIN_STRING = "-----BEGIN RSA PRIVATE KEY-----";
	    private static string PKCS1_END_STRING = "-----END RSA PRIVATE KEY-----";
	    private static string PKCS8_BEGIN_STRING = "-----BEGIN PRIVATE KEY-----";
	    private static string PKCS8_END_STRING = "-----END PRIVATE KEY-----";

	    private enum PemFormat
	    {
		    NONE,
		    PKCS1,
		    PKCS8
	    }

        /// <summary>
        /// Create PrivateKey class instance by inputting file path where private key is located
        /// </summary>
        /// <param name="privateKeyFilePath">Private key file location</param>
        /// <returns>The generated PrivateKey class instance</returns>
        public static PrivateKey GetPrivateKeyFromFile(string privateKeyFilePath)
        {
            return GetPrivateKeyFromString(System.IO.File.ReadAllText(privateKeyFilePath));
        }

        /// <summary>
        /// Create PrivateKey class instance by receiving private key string
        /// </summary>
        /// <param name="keyData">Private key string</param>
        /// <returns>The generated PrivateKey class instance</returns>
        public static PrivateKey GetPrivateKeyFromString(string keyData)
	    {
		    if (string.IsNullOrEmpty(keyData))
		    {
			    return null;
		    }

            string privateKeyPEM = keyData;

		    PemFormat format = PemFormat.NONE;
		    if (privateKeyPEM.StartsWith(PKCS1_BEGIN_STRING))
		    {
			    privateKeyPEM = privateKeyPEM.Replace(PKCS1_BEGIN_STRING, "");
			    privateKeyPEM = privateKeyPEM.Replace(PKCS1_END_STRING, "");
			    format = PemFormat.PKCS1;
		    }
		    else if (privateKeyPEM.StartsWith(PKCS8_BEGIN_STRING))
		    {
			    privateKeyPEM = privateKeyPEM.Replace(PKCS8_BEGIN_STRING, "");
			    privateKeyPEM = privateKeyPEM.Replace(PKCS8_END_STRING, "");
			    format = PemFormat.PKCS8;
		    }

            privateKeyPEM = privateKeyPEM.Replace("\r\n", "");
            //byte[] encoded = Base64.decode(privateKeyPEM, Base64.DEFAULT);
            byte[] encoded = System.Convert.FromBase64String(privateKeyPEM);

            try
            {
			    switch (format)
			    {
				    case PemFormat.PKCS1:
					    return ParsePemFromPKCS1(encoded);
				    case PemFormat.PKCS8:
					    return ParsePemFromPKCS8(encoded);
				    case PemFormat.NONE:
					    throw new FormatException("Not Supported Format Exception");
			    }
		    }
            catch (Exception e)
            {
		    }

		    return null;
	    }

	    private static PrivateKey ParsePemFromPKCS1(byte[] data)
        {
            ByteUtil byteBuffer = new ByteUtil(data);
            //ByteBuffer byteBuffer = ByteBuffer.allocate(data.Length).put(data);
            //byteBuffer.rewind();
            byte tag = byteBuffer.GetByte();
		    if (tag != 0x30)
            {
			    throw new Exception("Invalid tag Exception");
		    }

		    PrivateKey keys = new PrivateKey();

		    int sequenceSize = GetLength(byteBuffer);

		    // version, modulus, publicExponent, privateExponents, prime1, prime2, exponent1, exponent2, coefficient
		    byte[] version = ParseData(byteBuffer);
		    if ( version == null)
			    return null;
		    keys.SetVersion(version);

		    byte[] modulus = ParseData(byteBuffer);
		    if ( modulus == null)
			    return null;
		    keys.SetModulus(modulus);

		    byte[] publicExponent = ParseData(byteBuffer);
		    if ( publicExponent == null)
			    return null;
		    keys.SetPublicExponent(publicExponent);

		    byte[] privateExponent = ParseData(byteBuffer);
		    if ( privateExponent == null)
			    return null;
		    keys.SetPrivateExponent(privateExponent);

		    return keys;
	    }

	    private static PrivateKey ParsePemFromPKCS8(byte[] data)
        {
            ByteUtil byteBuffer = new ByteUtil(data);

            //ByteBuffer byteBuffer = ByteBuffer.allocate(data.Length).put(data);
		    //byteBuffer.rewind();
		    byte[] privateKeyInfo = ParseData(byteBuffer);
		    if (privateKeyInfo == null) return null;

            ByteUtil privateKeyInfoBuffer = new ByteUtil(privateKeyInfo);
            //ByteBuffer privateKeyInfoBuffer = ByteBuffer.allocate(privateKeyInfo.Length).put(privateKeyInfo);
		    //privateKeyInfoBuffer.rewind();

		    // privatekeyinfo version
		    byte[] version = ParseData(privateKeyInfoBuffer);
		    if (version == null) return null;

		    // algorithm data
		    byte[] algorithm = ParseData(privateKeyInfoBuffer);
		    if ( algorithm == null ) return null;

		    // private key octet string
		    byte[] privateKey = ParseData(privateKeyInfoBuffer);
		    if (privateKey == null) return null;

		    // sequence
		    // same to PKCS1
		    return ParsePemFromPKCS1(privateKey);
	    }

	    private static byte[] ParseData(ByteUtil byteBuffer)
	    {
            byte tag = byteBuffer.GetByte();
		    int length = GetLength(byteBuffer);
		    switch (tag)
		    {
			    case 0x02:  // Integer
				    return ReadIntegerType(byteBuffer, length);
			    case 0x04:  // Octet string
			    case 0x30:  // sequence
				    return ReadBytes(byteBuffer, length);
			    default:
				    return null;
		    }
	    }

	    private static byte[] ReadIntegerType(ByteUtil byteBuffer, int length)
	    {
            byte[] array = byteBuffer.GetBytes(length);
		    return array;
	    }

	    private static byte[] ReadBytes(ByteUtil byteBuffer, int length)
        {
            byte[] array = byteBuffer.GetBytes(length);
            return array;
        }

	    private static int GetLength(ByteUtil byteBuffer)
        {
            byte dataByte = byteBuffer.GetByte();
		    if ((dataByte & 0x80) != 0x80)
            {
			    return dataByte;
		    }
            else
            {
			    int count = (dataByte & 0x7f);
                byte[] lengthBytes = byteBuffer.GetBytes(count);
			    return ByteArrayToInt(lengthBytes);
		    }
	    }

	    private static int ByteArrayToInt(byte[] array)
	    {
		    int count = array.Length;
		    int length = 0;
		    for(int i = 0; i < count; ++i)
		    {
			    length += array[count - (i + 1)] << (8 * i);
		    }
		    return length;
	    }
    }
}
