using System;

namespace Neosmartpen.Net.Usb
{
    public class UsbPacketEventArgs : System.EventArgs
    {
        public UsbPacket Packet { get; private set; }
        internal UsbPacketEventArgs(UsbPacket packet)
        {
            Packet = packet;
        }
    }

    /// <summary>
    /// 읽어드린 바이트에 대한 파서
    /// </summary>
    public class UsbProtocolParser
    {
        public event EventHandler<UsbPacketEventArgs> PacketCreated;

        private byte step = 0;

        private byte[] command = new byte[128];
        private byte commandIdx = 0;
        private byte[] length = new byte[2];
        private ushort reqDataLen = 0;
        private int m_rawDataCnt;

        private UsbPacket.Builder builder;

        public void Put(byte[] buff, int size)
        {
            //byte[] test = new byte[size];
            //Array.Copy( buff, 0, test, 0, size );
            //System.Console.WriteLine( "Read Buffer : {0}", BitConverter.ToString( test ) );
            //System.Console.WriteLine();

            for (int dataIdx = 0; dataIdx < buff.Length; dataIdx++)
            {
                byte recvByte = buff[dataIdx];

                switch (step)
                {
                    case 0:
                        if (recvByte == Const.CMD_PRIFIX_1)
                        {
                            step = 1;
                            //command[commandIdx++] = recvByte;
                        }
                        else
                        {
                            step = 0;
                        }
                        break;
                    case 1:
                        if (recvByte == Const.CMD_PRIFIX_2)
                        {
                            step = 2;
                            //command[commandIdx++] = recvByte;
                        }
                        else
                        {
                            step = 0;
                        }
                        break;
                    case 2:
                        if (recvByte == Const.CMD_PRIFIX_3)
                        {
                            step = 3;
                            commandIdx = 0;
                            //command[commandIdx++] = recvByte;

                            reqDataLen = 0;
                            m_rawDataCnt = 0;

                            builder = new UsbPacket.Builder();
                        }
                        else
                        {
                            step = 0;
                        }
                        break;
                    case 3:
                        if (recvByte != Const.CMD_DELIMITER_RESPONSE && recvByte != Const.CMD_DELIMITER_EVENT)
                        {
                            command[commandIdx++] = recvByte;
                        }
                        else
                        {
                            string cmdstring = System.Text.Encoding.Default.GetString(command, 0, commandIdx).Trim('\0').Trim();
                            Cmd cmd = (Cmd)Enum.Parse(typeof(Cmd), cmdstring);
                            builder.Cmd(cmd);
                            if (recvByte == Const.CMD_DELIMITER_RESPONSE)
                            {
                                //Req
                                builder.Type(PacketType.Response);
                                step = 4;
                            }
                            else if (recvByte == Const.CMD_DELIMITER_EVENT)
                            {
                                //Event
                                builder.Type(PacketType.Event);
                                step = 5;
                            }
                        }
                        break;
                    case 4:
                        builder.PacketNumber(recvByte);
                        step = 6;
                        break;
                    case 5:
                        builder.PacketNumber(recvByte);
                        step = 7;
                        break;
                    case 6:
                        builder.ErrorNumber(recvByte);
                        step = 7;
                        break;
                    case 7:
                        length[0] = recvByte;
                        step = 8;
                        break;
                    case 8:
                        length[1] = recvByte;
                        reqDataLen = BitConverter.ToUInt16(length, 0);
                        step = 9;
                        break;
                    case 9:
                        if (recvByte == Const.CMD_DELIMITER_DATA)
                        {
                            step = 10;
                        }
                        else
                        {
                            step = 0;
                        }
                        break;
                    case 10:
                        builder.Put(recvByte);
                        if ((m_rawDataCnt++ == reqDataLen - 1) || (reqDataLen == 0))
                        {
                            step = 0; //초기화
                            PacketCreated(this, new UsbPacketEventArgs(builder.Build()));
                        }
                        break;
                    default:
                        break;
                }
            }

        }
    }
}
