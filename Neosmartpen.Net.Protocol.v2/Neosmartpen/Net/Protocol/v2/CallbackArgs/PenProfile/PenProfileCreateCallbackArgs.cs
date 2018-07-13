namespace Neosmartpen.Net.Protocol.v2
{
	public sealed class PenProfileCreateCallbackArgs : PenProfileReceivedCallbackArgs
	{
		internal PenProfileCreateCallbackArgs()
		{
			Type = PenProfileType.Create;
			Result = ResultType.Success;
		}
		internal PenProfileCreateCallbackArgs(string profileName, int status) : this()
		{
			ProfileName = profileName;
			Status = status;
		}
	}
}
