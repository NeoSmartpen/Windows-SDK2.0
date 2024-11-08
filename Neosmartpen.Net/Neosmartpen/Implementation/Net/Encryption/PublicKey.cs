using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;

namespace Neosmartpen.Net.Encryption
{
    /// <exclude />
    public class PublicKey
	{
		public IBuffer Modulus { get; private set; }
		public IBuffer PublicExponent { get; private set; }
		public PublicKey(IBuffer key)
		{
			ParseBuffer(key);
		}

		private void ParseBuffer(IBuffer key)
		{
			var dataReader = DataReader.FromBuffer(key);
			byte startByte = dataReader.ReadByte();
			if (startByte != 0x30)
				throw new FormatException("Indicating Quceqence Failed");

			uint length = GetLength(dataReader);

			ParseModulus(dataReader);

			ParsePublicExponent(dataReader);
		}

		private void ParseModulus(DataReader reader)
		{
			byte tag = reader.ReadByte();
			if (tag == 0x02)
				Modulus = ReadInteger(reader);
			else
				throw new Exception("Invalid Tag In Modulus");

		}

		private void ParsePublicExponent(DataReader reader)
		{
			byte tag = reader.ReadByte();
			if (tag == 0x02)
				PublicExponent = ReadInteger(reader);
			else
				throw new Exception("Invalid Tag In PublicExponent");

		}
		private IBuffer ReadInteger(DataReader reader)
		{
			uint length = GetLength(reader);
			var temp = reader.ReadBuffer(length);
			var bytes = new byte[temp.Length];
			using (var dataReader = DataReader.FromBuffer(temp))
			{
				dataReader.ReadBytes(bytes);
			}
			if ( bytes[0] == 0x00 )
			{
				var newBytes = new byte[bytes.Length - 1];
				System.Buffer.BlockCopy(bytes, 1, newBytes, 0, newBytes.Length);
				return newBytes.AsBuffer();
			}
			else
			{
				return bytes.AsBuffer();
			}
		}
		private uint GetLength(DataReader reader)
		{
			byte dataByte = reader.ReadByte();
			if ( (dataByte & 0x80) != 0x80 )
			{
				return dataByte;
			}
			else
			{
				uint count = (uint)(dataByte & 0x7F);
				var lengthBytes = reader.ReadBuffer(count);

				var bytes = new byte[lengthBytes.Length];
				using (var dataReader = DataReader.FromBuffer(lengthBytes))
				{
					dataReader.ReadBytes(bytes);
				}
				return ByteArrayToInt(bytes);
			}
		}

		private uint ByteArrayToInt(byte[] array)
		{
			int count = array.Length;
			uint length = 0;
			for (int i = 0; i < count; ++i)
			{
				length += (uint)array[count - (i + 1)] << (8 * i);
			}
			return length;
		}
	}
}
