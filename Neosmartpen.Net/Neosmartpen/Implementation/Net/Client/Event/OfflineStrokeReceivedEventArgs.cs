namespace Neosmartpen.Net
{
    /// <summary>
    /// Contains properties that offline data and status of transmission for the OfflineStrokeReceived event
    /// </summary>
    public sealed class OfflineStrokeReceivedEventArgs
	{
		internal OfflineStrokeReceivedEventArgs() { }
		//mTotalOfflineStroke, mReceivedOfflineStroke, result.ToArray());
		internal OfflineStrokeReceivedEventArgs(int total, int amountDone, Stroke[] strokes)
		{
			Total = total;
			AmountDone = amountDone;
			Strokes = strokes;
		}
        /// <summary>
        /// Gets total of offline data transmission
        /// </summary>
        public int Total { get; internal set; }
        /// <summary>
        /// Gets amount of transmission done
        /// </summary>
        public int AmountDone { get; internal set; }
        /// <summary>
        /// Gets array of offline data's stroke
        /// </summary>
        public Stroke[] Strokes { get; internal set; }
	}
}
