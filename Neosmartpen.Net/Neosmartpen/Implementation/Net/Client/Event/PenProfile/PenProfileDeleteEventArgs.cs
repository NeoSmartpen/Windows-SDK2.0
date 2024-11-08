namespace Neosmartpen.Net
{
    /// <exclude />
    public sealed class PenProfileDeleteEventArgs : PenProfileReceivedEventArgs
	{
		internal PenProfileDeleteEventArgs()
		{
			Result = ResultType.Success;
			Type = PenProfileType.Delete;
		}
		internal PenProfileDeleteEventArgs(string profileName, int status) : this()
		{
			ProfileName = profileName;
			Status = status;
		}
	}
}
