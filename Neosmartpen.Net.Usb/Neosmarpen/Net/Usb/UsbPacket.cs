using Neosmartpen.Net.Support;
using System;
using System.Text;

namespace Neosmartpen.Net.Usb
{
    public class UsbPacket
    {
        public Cmd Cmd { protected set; get; }

        public PacketType Type{ protected set; get; }

        public byte PacketNumber { protected set; get; }

        public byte ErrorNumber { protected set; get; }

        public byte[] Data { protected set; get; }

        private int mIndex = 0;

        public byte GetChecksum(int length)
        {
            byte[] bytes = new byte[length];

            Array.Copy(Data, mIndex, bytes, 0, length);

            int CheckSum = 0;

            for (int i = 0; i < bytes.Length; i++)
            {
                CheckSum += (int)(bytes[i] & 0xFF);
            }

            return (byte)CheckSum;
        }

        public byte GetChecksum()
        {
            return GetChecksum(Data.Length - mIndex);
        }

        public bool CheckMoreData()
        {
            return Data.Length > mIndex;
        }

        public int GetInt()
        {
            return BitConverter.ToInt32(GetBytes(4), 0);
        }

        public short GetShort()
        {
            return BitConverter.ToInt16(GetBytes(2), 0);
        }

        public ushort GetUShort()
        {
            return BitConverter.ToUInt16(GetBytes(2), 0);
        }

        public long GetLong()
        {
            return BitConverter.ToInt64(GetBytes(8), 0);
        }

        public int GetByteToInt()
        {
            return (int)(GetByte() & 0xFF);
        }

        public byte[] GetBytes()
        {
            return GetBytes(Data.Length - mIndex);
        }

        public byte[] GetBytes(int size)
        {
            byte[] result = new byte[size];

            Array.Copy(Data, mIndex, result, 0, size);

            Move(size);

            return result;
        }

        public UsbPacket Move(int size)
        {
            mIndex += size;
            return this;
        }

        public UsbPacket Reset()
        {
            mIndex = 0;
            return this;
        }

        public byte GetByte()
        {
            return GetBytes(1)[0];
        }

        public string GetString(int length)
        {
            return System.Text.Encoding.Default.GetString(GetBytes(length)).Trim('\0');
        }

        public byte[] ToArray()
        {
            ByteUtil byteUtil = new ByteUtil();
            byteUtil.Put((byte)Const.CMD_PRIFIX_1);
            byteUtil.Put((byte)Const.CMD_PRIFIX_2);
            byteUtil.Put((byte)Const.CMD_PRIFIX_3);
            byteUtil.Put(Encoding.UTF8.GetBytes(Cmd.ToString()));

            if (Type == PacketType.Event)
            {
                //24
                byteUtil.Put((byte)Const.CMD_DELIMITER_EVENT);
                byteUtil.Put(PacketNumber);
                byteUtil.PutShort((short)Data.Length);
            }
            else if (Type == PacketType.Response)
            {
                //3f
                byteUtil.Put((byte)Const.CMD_DELIMITER_RESPONSE);
                byteUtil.Put(PacketNumber);
                byteUtil.Put(ErrorNumber);
                byteUtil.PutShort((short)Data.Length);
            }
            else
            {
                //3d
                byteUtil.Put((byte)Const.CMD_DELIMITER_REQUEST);
                byteUtil.Put(PacketNumber);
                byteUtil.PutShort((short)Data.Length);
            }

            //2c
            byteUtil.Put((byte)Const.CMD_DELIMITER_DATA);
            if (Data != null && Data.Length > 0)
                byteUtil.Put(Data);
            byte[] bytes = byteUtil.ToArray();
            return bytes;
        }

        public override string ToString()
        {
            return BitConverter.ToString( Data, 0 );
        }

        public class Builder
        {
            private UsbPacket packet;
            private ByteUtil byteUtil;

            public Builder()
            {
                packet = new UsbPacket();
                byteUtil = new ByteUtil();
            }

            public Builder Cmd( Cmd cmd )
            {
                packet.Cmd = cmd;
                return this;
            }

            public Builder Type(PacketType type)
            {
                packet.Type = type;
                return this;
            }

            public Builder PacketNumber( byte number )
            {
                packet.PacketNumber = number;
                return this;
            }

            public Builder ErrorNumber(byte code )
            {
                packet.ErrorNumber = code;
                return this;
            }

            public Builder Data( byte[] data )
            {
                packet.Data = data;
                return this;
            }

            public Builder Put(byte input)
            {
                byteUtil.Put(input);
                return this;
            }

            public Builder Put(byte[] inputs)
            {
                byteUtil.Put(inputs);
                return this;
            }

            public Builder Put(byte[] inputs, int length)
            {
                byteUtil.Put(inputs, length);
                return this;
            }

            public Builder PutShort(short input)
            {
                byteUtil.PutShort(input);
                return this;
            }

            public Builder PutInt(int input)
            {
                byteUtil.PutInt(input);
                return this;
            }

            public Builder PutLong(long input)
            {
                byteUtil.PutLong(input);
                return this;
            }

            public Builder PutNull(int length)
            {
                byteUtil.PutNull(length);
                return this;
            }

            public Builder PutString(string text)
            {
                byteUtil.Put(Encoding.UTF8.GetBytes(text));
                return this;
            }

            public UsbPacket Build()
            {
                packet.Data = byteUtil.ToArray();
                return packet;
            }
        }
    }
}
