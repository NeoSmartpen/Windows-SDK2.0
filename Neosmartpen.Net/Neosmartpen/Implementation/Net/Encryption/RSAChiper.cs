using System;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace Neosmartpen.Net.Encryption
{
	class RSAChiper
	{
		private PrivateKey PrivateKey { get; set; }
		private PublicKey PublicKey { get; set; }

		public RSAChiper()
		{
			PrivateKey = null;
			PublicKey = null;
		}

		public enum KeySize : uint
		{
			KS512 = 512,
			KS1024 = 1024,
			KS2048 = 2048,
			KS4096 = 4096
		}

		public void CreateKey(KeySize keySize = KeySize.KS2048)
		{
			AsymmetricKeyAlgorithmProvider asym = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.RsaPkcs1);
			CryptographicKey key = asym.CreateKeyPair((uint)keySize);

			var privateKey = key.Export(CryptographicPrivateKeyBlobType.Pkcs1RsaPrivateKey);
			var publicKey = key.ExportPublicKey(CryptographicPublicKeyBlobType.Pkcs1RsaPublicKey);

			//System.Diagnostics.Debug.WriteLine($"private : {BitConverter.ToString(privateKey.ToArray())}");
			//System.Diagnostics.Debug.WriteLine($"public : {BitConverter.ToString(publicKey.ToArray())}");

			PublicKey = new PublicKey(publicKey);
			PrivateKey = new PrivateKey(privateKey);
		}

		public byte[] Encrypt(string data)
		{
			if (PublicKey == null || string.IsNullOrEmpty(data))
				return null;

			var temp = Encoding.ASCII.GetBytes(data);
			MakePositive(ref temp);
			var value = new BigInteger(temp);

			temp = PublicKey.PublicExponent.ToArray();
			ReverseDataArray(ref temp);
			var e = new BigInteger(temp);

			temp = PublicKey.Modulus.ToArray();
			ReverseDataArray(ref temp);
			var n = new BigInteger(temp);

			var result = Power(value, e, n);

			temp = result.ToByteArray();
			Array.Reverse(temp);

			return temp;
		}

		// data is big endian
		public byte[] Decrypt(byte[] data)
		{
			if (PrivateKey == null || data == null)
				return null;

			ReverseDataArray(ref data);
			//MakePositive(ref data);
			var value = new BigInteger(data);

			var temp = PrivateKey.PrivateExponent.ToArray();
			ReverseDataArray(ref temp);
			var e = new BigInteger(temp);

			temp = PrivateKey.Modulus.ToArray();
			ReverseDataArray(ref temp);
			var n = new BigInteger(temp);

			var result = Power(value, e, n);

			temp = result.ToByteArray();
			Array.Reverse(temp);

			return temp;
			//return Encoding.UTF8.GetString(result.ToByteArray());
			//return result.ToByteArray();
		}

		private void ReverseDataArray(ref byte[] data)
		{
			Array.Reverse(data);
			MakePositive(ref data);
		}

		private void MakePositive(ref byte[] data)
		{
			Array.Resize(ref data, data.Length + 1);
		}

		private BigInteger Power(BigInteger value, BigInteger e, BigInteger n)
		{
			if (value == 0 || n == 0)
				return 0;
			if (e == 0) return 1 % n;

			BigInteger currentMode = value % n;
			BigInteger currentValue = (e & 1) > 0 ? currentMode : 1;
			for(e >>= 1; e > 0; e >>= 1)
			{
				currentMode = (currentMode * currentMode) % n;
				if ( (e & 1) > 0 )
				{
					currentValue = (currentValue * currentMode) % n;
				}
			}

			return currentValue;
		}

		public byte[] GetPublicKeyModulus()
		{
			if (PublicKey == null)
				return null;

			CryptographicBuffer.CopyToByteArray(PublicKey.Modulus, out byte[] encryptedBytes);
			return encryptedBytes;
		}

		public byte[] GetPublicExponent()
		{
			if (PublicKey == null)
				return null;

			CryptographicBuffer.CopyToByteArray(PublicKey.PublicExponent, out byte[] encryptedBytes);
			return encryptedBytes;
		}
		/*
		public byte[] Encrypt(string data)
		{
			if (PublicKey == null)
			{
				return null;
			}

			AsymmetricKeyAlgorithmProvider asym = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.RsaPkcs1);
			CryptographicKey key = asym.ImportPublicKey(PublicKey, CryptographicPublicKeyBlobType.Pkcs1RsaPublicKey);

			IBuffer plainBuffer = CryptographicBuffer.ConvertStringToBinary(data, BinaryStringEncoding.Utf8);
			IBuffer encryptedBuffer = CryptographicEngine.Encrypt(key, plainBuffer, null);

			CryptographicBuffer.CopyToByteArray(encryptedBuffer, out byte[] encryptedBytes);

			return encryptedBytes;
		}

		public string Decrypt(byte[] data)
		{
			if (PrivateKey == null)
			{
				return null;
			}

			AsymmetricKeyAlgorithmProvider asym = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.RsaPkcs1);
			CryptographicKey key = asym.ImportKeyPair(privateKey, CryptographicPrivateKeyBlobType.Pkcs1RsaPrivateKey);

			IBuffer plainBuffer = CryptographicEngine.Decrypt(key, data.AsBuffer(), null);

			CryptographicBuffer.CopyToByteArray(plainBuffer, out byte[] plainBytes);

			return Encoding.UTF8.GetString(plainBytes, 0, plainBytes.Length);
		}

		public byte[] GetPublicKey()
		{
			if (PublicKey == null)
				return null;

			CryptographicBuffer.CopyToByteArray(PublicKey, out byte[] publicKeyBytes);
			return publicKeyBytes; 
		}

		public string GetPublicKeyString()
		{
			if (PublicKey == null)
				return null;

			//IBuffer keyBuffer = CryptographicBuffer.DecodeFromBase64String(PublicKey);
			var bytes = GetPublicKey();
			var publicKeyString = Convert.ToBase64String(bytes);
			return publicKeyString;
		}
		*/
	}
}