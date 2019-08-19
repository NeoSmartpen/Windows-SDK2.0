using System;

namespace Neosmartpen.Net.Usb
{
    public class Const
    {
        public const char CMD_PRIFIX_1 = 'A';
        public const char CMD_PRIFIX_2 = 'T';
        public const char CMD_PRIFIX_3 = '+';

        public const char CMD_DELIMITER_REQUEST = '=';
        public const char CMD_DELIMITER_RESPONSE = '?';
        public const char CMD_DELIMITER_EVENT = '$';

        public const char CMD_DELIMITER_DATA = ',';
    }

    [Flags]
    public enum Cmd
    {
        START,
        GETDEVINFO,
        GETCONFIG,
        SETCONFIG,
        FORMAT,
        GETOFFLINEDATALIST,
        GETLOGFILELIST,
        GETFILE_H,
        GETFILE_D,
        DELETEFILE,
        POWEROFF,
        UPDATE_START,
        UPDATE_DO
    };

    [Flags]
    public enum PacketType
    {
        Request,
        Response,
        Event
    };

    /// <summary>
    /// An enum indicating the setting type of the pen
    /// </summary>
    [Flags]
    public enum ConfigType
    {
        DateTime = 0x01,
        AutoPowerOffTime = 0x02,
        AutoPowerOn = 0x03,
        PenCapOff = 0x04,
        Beep = 0x05,
        SaveOfflineData = 0x07,
        DownSampling = 0x08,
        Battery = 0x09,
        Storage = 0x10
    };

    /// <summary>
    /// Enum indicating the file type of the pen
    /// </summary>
    [Flags]
    public enum FileType
    {
        Data,
        Log
    };
}
