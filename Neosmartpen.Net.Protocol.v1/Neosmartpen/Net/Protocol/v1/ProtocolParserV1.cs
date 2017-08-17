using Neosmartpen.Net.Support;
using System;

namespace Neosmartpen.Net.Protocol.v1
{
    public enum Cmd : byte
    {
        A_PenOnState = 0x01,
        P_PenOnResponse = 0x02,

        P_RTCset = 0x03,
        A_RTCsetResponse = 0x04,

        P_HoverOnOff = 0x05,
        A_HoverOnOffResponse = 0x06,

        P_ForceCalibrate = 0x07,
        A_ForceCalibrateResponse = 0x08,

        P_AutoShutdownTime = 0x09,
        A_AutoShutdownTimeResponse = 0x0A,
        P_PenSensitivity = 0x2C,
        A_PenSensitivityResponse = 0x2D,
        P_PenColorSet = 0x28,
        A_PenColorSetResponse = 0x29,
        P_AutoPowerOnSet = 0x2A,
        A_AutoPowerOnResponse = 0x2B,
        P_BeepSet = 0x2E,
        A_BeepSetResponse = 0x2F,

        P_UsingNoteNotify = 0x0B,
        A_UsingNoteNotifyResponse = 0x0C,

        A_PasswordRequest = 0x0D,
        P_PasswordResponse = 0x0E,
        P_PasswordSet = 0x0F,
        A_PasswordSetResponse = 0x10,

        A_DotData = 0x11,
        A_DotUpDownData = 0x13,
        P_DotUpDownResponse = 0x14,
        A_DotIDChange = 0x15,
        A_DotUpDownDataNew = 0x16,

        P_PenStatusRequest = 0x21,
        A_PenStatusOldResponse = 0x22,
        A_PenStatusResponse = 0x25,

        P_OfflineDataRequest = 0x47,
        A_OfflineDataInfo = 0x49,
        A_OfflineFileInfo = 0x41,
        P_OfflineFileInfoResponse = 0x42,
        A_OfflineChunk = 0x43,
        P_OfflineChunkResponse = 0x44,
        A_OfflineResultResponse = 0x48,
        P_OfflineNoteList = 0x45,
        A_OfflineNoteListResponse = 0x46,
        P_OfflineDataRemove = 0x4A,
        A_OfflineDataRemoveResponse = 0x4B,

        P_PenSWUpgradeCommand = 0x51,
        A_PenSWUpgradeRequest = 0x52,
        P_PenSWUpgradeResponse = 0x53,
        A_PenSWUpgradeStatus = 0x54
    }

    /// <summary>
    /// 읽어드린 바이트에 대한 파서
    /// </summary>
    public class ProtocolParserV1 : IProtocolParser
    {
        public event EventHandler<PacketEventArgs> PacketCreated;

        private const int PKT_START          = 0xC0;
        private const int PKT_END            = 0xC1;
        private const int PKT_EMPTY          = 0x00;
        private const int PKT_HEADER_LEN     = 3;
        private const int PKT_LENGTH_POS1    = 1;
        private const int PKT_LENGTH_POS2    = 2;
        private const int PKT_MAX_LEN        = 8200;

        private int counter = 0;
        private int dataLength = 0;

        // length
        private byte[] lbuffer = new byte[2];

        private static int buffer_size = PKT_MAX_LEN + 1;

        private ByteUtil nbuffer = new ByteUtil(buffer_size);

        private bool isStart = true;

        public void Put(byte[] buff, int size)
        {
            for (int i = 0; i < size; i++)
            {
                ParseOneByte(buff[i]);
            }
        }

        private void ParseOneByte(byte data)
        {
            int int_data = (int)(data & 0xFF);
            //int int_data = data;

            if (int_data == PKT_START && isStart)
            {
                //System.Console.WriteLine( "ProtocolParser : PKT_START" );
                counter = 0;
                isStart = false;
            }
            else if (int_data == PKT_END && counter == dataLength + PKT_HEADER_LEN)
            {
                Packet.Builder builder = new Packet.Builder();

                // 커맨드를 뽑는다.
                int cmd = nbuffer.GetByteToInt();

                // 길이를 뽑는다.
                int length = nbuffer.GetShort();

                // 커맨드, 길이를 제외한 나머지 바이트를 컨텐트로 지정
                byte[] content = nbuffer.GetBytes();

                PacketCreated(this, new PacketEventArgs(builder.cmd(cmd).data(content).Build()));

                dataLength = 0;
                counter = 10;
                nbuffer.Clear();
                isStart = true;
            }
            else if (counter > PKT_MAX_LEN)
            {
                //System.Console.WriteLine( "ProtocolParser : PKT_MAX_LEN" );
                counter = 10;
                dataLength = 0;
                isStart = true;
            }
            else
            {
                if (counter == PKT_LENGTH_POS1)
                {
                    lbuffer[0] = data;
                }
                else if (counter == PKT_LENGTH_POS2)
                {
                    //System.Console.WriteLine( "ProtocolParser : PKT_LENGTH_POS2" );
                    lbuffer[1] = data;
                    dataLength = ByteConverter.ByteToShort(lbuffer);
                }

                if (!isStart)
                {
                    nbuffer.Put(data);
                    counter++;
                }
            }
        }
    }
}
