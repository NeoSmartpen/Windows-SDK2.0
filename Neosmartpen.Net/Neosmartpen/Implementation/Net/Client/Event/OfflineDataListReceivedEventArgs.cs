namespace Neosmartpen.Net
{
    /// <summary>
    /// Contains properties that list of offline data for the OfflineDataListReceived event
    /// </summary>
    public sealed class OfflineDataListReceivedEventArgs
	{
		internal OfflineDataListReceivedEventArgs() { }
		internal OfflineDataListReceivedEventArgs(params OfflineDataInfo[] offlineNotes)
		{
			OfflineNotes = offlineNotes;
		}

        /// <summary>
        /// Gets array of Offline data information in storage of pen
        /// </summary>
        public OfflineDataInfo[] OfflineNotes { get; internal set; }
	}
}
