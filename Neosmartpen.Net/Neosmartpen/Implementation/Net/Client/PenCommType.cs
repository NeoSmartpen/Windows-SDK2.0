namespace Neosmartpen.Net.Client
{
    public enum PenCommType
	{
		PenCommV1,
		PenCommV2
	}

	//public class PenCommTypeConverter
	//{
	//	public static uint ParseClassOfDevice(PenCommType penCommType)
	//	{
	//		switch(penCommType)
	//		{
	//			case PenCommType.PenCommV1:
	//				return 0x0500;
	//			case PenCommType.PenCommV2:
	//				return 0x2510;
	//			default:
	//				return 0x00;
	//		}
	//	}
	//	public static uint ParseServiceUUID(PenCommType penCommType)
	//	{
	//		switch(penCommType)
	//		{
	//			case PenCommType.PenCommV1:
	//				return 0x18F1;
	//			case PenCommType.PenCommV2:
	//				return 0x19F1;
	//			default:
	//				return 0x0000;
	//		}
	//	}
	//}
}