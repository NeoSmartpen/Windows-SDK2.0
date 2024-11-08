namespace Neosmartpen.Net
{
    /// <exclude />
    public sealed class PdsReceivedEventArgs
	{
		internal PdsReceivedEventArgs() { }
		internal PdsReceivedEventArgs(Pds pds)
		{
			PDS = pds;
		}

		public Pds PDS { get; internal set; }
	}
}
