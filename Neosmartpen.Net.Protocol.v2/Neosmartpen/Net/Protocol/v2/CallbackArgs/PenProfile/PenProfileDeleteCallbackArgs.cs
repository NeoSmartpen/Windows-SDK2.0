namespace Neosmartpen.Net.Protocol.v2
{
	public sealed class PenProfileDeleteCallbackArgs : PenProfileReceivedCallbackArgs
	{
		internal PenProfileDeleteCallbackArgs()
		{
			Result = ResultType.Success;
			Type = PenProfileType.Delete;
		}
		internal PenProfileDeleteCallbackArgs(string profileName, int status) : this()
		{
			ProfileName = profileName;
			Status = status;
		}
	}
}
