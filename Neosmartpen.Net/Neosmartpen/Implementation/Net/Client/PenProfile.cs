namespace Neosmartpen.Net
{
    /// <exclude />
    public class PenProfile
	{
		public static readonly int LIMIT_BYTE_LENGTH_PROFILE_NAME = 8;
		public static readonly int LIMIT_BYTE_LENGTH_PASSWORD = 8;
		public static readonly int LIMIT_BYTE_LENGTH_KEY = 16;
		/***
		 * request type
		 */
		internal static readonly byte PROFILE_CREATE = 0x01;
		internal static readonly byte PROFILE_DELETE = 0x02;
		internal static readonly byte PROFILE_INFO = 0x03;
		internal static readonly byte PROFILE_READ_VALUE = 0x12;
		internal static readonly byte PROFILE_WRITE_VALUE = 0x11;
		internal static readonly byte PROFILE_DELETE_VALUE = 0x13;
		/***
		 * status
		 */
		public const byte PROFILE_STATUS_SUCCESS = 0x00;
		public const byte PROFILE_STATUS_FAILURE = 0x01;
		public const byte PROFILE_STATUS_EXIST_PROFILE_ALREADY = 0x10;
		public const byte PROFILE_STATUS_NO_EXIST_PROFILE = 0x11;
		//    public static readonly byte PROFILE_STATUS_EXIST_KEY_ALREADY = 0x20;
		public const byte PROFILE_STATUS_NO_EXIST_KEY = 0x21;
		public const byte PROFILE_STATUS_NO_PERMISSION = 0x30;
		public const byte PROFILE_STATUS_BUFFER_SIZE_ERR = 0x40;
	}
}