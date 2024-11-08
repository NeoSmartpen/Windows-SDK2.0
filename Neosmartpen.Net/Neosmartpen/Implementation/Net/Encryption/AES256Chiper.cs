using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace Neosmartpen.Net.Encryption
{
    /// <exclude />
    class AES256Chiper
	{
		/*
		public string AES_Encrypt(string input, string pass)
		{
			SymmetricKeyAlgorithmProvider SAP = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcbPkcs7);
			CryptographicKey AES;
			HashAlgorithmProvider HAP = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
			CryptographicHash Hash_AES = HAP.CreateHash();

			try
			{
				byte[] hash = new byte[32];
				Hash_AES.Append(CryptographicBuffer.CreateFromByteArray(Encoding.UTF8.GetBytes(pass)));
				CryptographicBuffer.CopyToByteArray(Hash_AES.GetValueAndReset(), out byte[] temp);

				Array.Copy(temp, 0, hash, 0, 16);
				Array.Copy(temp, 0, hash, 15, 16);

				AES = SAP.CreateSymmetricKey(CryptographicBuffer.CreateFromByteArray(hash));

				IBuffer Buffer = CryptographicBuffer.CreateFromByteArray(Encoding.UTF8.GetBytes(input));
				string encrypted = CryptographicBuffer.EncodeToBase64String(CryptographicEngine.Encrypt(AES, Buffer, null));

				return encrypted;
			}
			catch (Exception ex)
			{
				return null;
			}
		}


		public string AES_Decrypt(string input, string pass)
		{
			SymmetricKeyAlgorithmProvider SAP = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcbPkcs7);
			CryptographicKey AES;
			HashAlgorithmProvider HAP = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
			CryptographicHash Hash_AES = HAP.CreateHash();

			try
			{
				byte[] hash = new byte[32];
				Hash_AES.Append(CryptographicBuffer.CreateFromByteArray(Encoding.UTF8.GetBytes(pass)));
				CryptographicBuffer.CopyToByteArray(Hash_AES.GetValueAndReset(), out byte[] temp);

				Array.Copy(temp, 0, hash, 0, 16);
				Array.Copy(temp, 0, hash, 15, 16);

				AES = SAP.CreateSymmetricKey(CryptographicBuffer.CreateFromByteArray(hash));

				IBuffer Buffer = CryptographicBuffer.DecodeFromBase64String(input);
				CryptographicBuffer.CopyToByteArray(CryptographicEngine.Decrypt(AES, Buffer, null), out byte[] Decrypted);
				string decrypted = Encoding.UTF8.GetString(Decrypted, 0, Decrypted.Length);

				return decrypted;
			}
			catch (Exception ex)
			{
				return null;
			}
		}
		*/

		public AES256Chiper(byte[] key)
		{
			var iv = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
			AesEnDecryption(key);
		}

		// Key with 256 and IV with 16 length
		private CryptographicKey m_key;

		public void AesEnDecryption(byte[] k)
		{
			SymmetricKeyAlgorithmProvider provider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcb);
			m_key = provider.CreateSymmetricKey(k.AsBuffer());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="input">Input buffer length must be Multiples of 16</param>
		/// <returns></returns>
		public byte[] Encrypt(byte[] input)
		{
			IBuffer bufferMsg = CryptographicBuffer.ConvertStringToBinary(Encoding.ASCII.GetString(input), BinaryStringEncoding.Utf8);
			try
			{
				IBuffer bufferEncrypt = CryptographicEngine.Encrypt(m_key, bufferMsg, null);
				return bufferEncrypt.ToArray();
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="input">Input buffer length must be Multiples of 16</param>
		/// <returns></returns>
		public byte[] Decrypt(byte[] input)
		{
			try
			{
				IBuffer bufferDecrypt = CryptographicEngine.Decrypt(m_key, input.AsBuffer(), null);
				return bufferDecrypt.ToArray();
			}
			catch
			{
				return null;
			}
		}
	}
}
