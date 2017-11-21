using System;

namespace Neosmartpen.Net.Protocol.v2
{
    public class Const
    {
        public const byte PK_STX        = 0xC0;
        public const byte PK_ETX        = 0xC1;
        public const byte PK_DLE        = 0x7D;

        public const int PK_POS_CMD     = 1;
        public const int PK_POS_RESULT  = 2;
        public const int PK_POS_LENG1   = 2;
        public const int PK_POS_LENG2   = 3;

        public const int PK_HEADER_SIZE = 3;
    }

    [Flags]
    public enum Cmd
    {
        VERSION_REQUEST = 0x01,
        VERSION_RESPONSE = 0x81,

        PASSWORD_REQUEST = 0x02,
        PASSWORD_RESPONSE = 0X82,

        PASSWORD_CHANGE_REQUEST = 0X03,
        PASSWORD_CHANGE_RESPONSE = 0X83,

        SETTING_INFO_REQUEST = 0X04,
        SETTING_INFO_RESPONSE = 0X84,

        LOW_BATTERY_EVENT = 0X61,
        SHUTDOWN_EVENT = 0X62,

        SETTING_CHANGE_REQUEST = 0X05,
        SETTING_CHANGE_RESPONSE = 0X85,

        ONLINE_DATA_REQUEST = 0X11,
        ONLINE_DATA_RESPONSE = 0X91,

        ONLINE_PEN_DATA_REQUEST = 0X12,
        ONLINE_PEN_DATA_RESPONSE = 0X92,

        ONLINE_PEN_UPDOWN_EVENT = 0X63,
        ONLINE_PAPER_INFO_EVENT = 0X64,
        ONLINE_PEN_DOT_EVENT = 0X65,

        OFFLINE_NOTE_LIST_REQUEST = 0X21,
        OFFLINE_NOTE_LIST_RESPONSE = 0XA1,

        OFFLINE_PAGE_LIST_REQUEST = 0X22,
        OFFLINE_PAGE_LIST_RESPONSE = 0XA2,

        OFFLINE_DATA_REQUEST = 0X23,
        OFFLINE_DATA_RESPONSE = 0XA3,
        OFFLINE_PACKET_REQUEST = 0X24,
        OFFLINE_PACKET_RESPONSE = 0XA4,

        OFFLINE_DATA_DELETE_REQUEST = 0X25,
        OFFLINE_DATA_DELETE_RESPONSE = 0XA5,

        FIRMWARE_UPLOAD_REQUEST = 0X31,
        FIRMWARE_UPLOAD_RESPONSE = 0XB1,
        FIRMWARE_PACKET_REQUEST = 0X32,
        FIRMWARE_PACKET_RESPONSE = 0XB2
    };
}
