namespace Neosmartpen.Net
{
    /// <exclude />
    public sealed class PenProfileCreateEventArgs : PenProfileReceivedEventArgs
	{
		internal PenProfileCreateEventArgs()
		{
			Type = PenProfileType.Create;
			Result = ResultType.Success;
		}
		internal PenProfileCreateEventArgs(string profileName, int status) : this()
		{
			ProfileName = profileName;
			Status = status;
		}
	}
}
