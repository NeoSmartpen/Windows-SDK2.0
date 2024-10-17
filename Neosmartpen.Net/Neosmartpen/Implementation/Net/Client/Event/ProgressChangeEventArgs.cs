namespace Neosmartpen.Net
{
    /// <summary>
    /// Defines a provider for progress updates
    /// </summary>
    public sealed class ProgressChangeEventArgs
	{
		internal ProgressChangeEventArgs() { }
		internal ProgressChangeEventArgs(int total, int amountDone)
		{
			Total = total;
			AmountDone = amountDone;
		}

        /// <summary>
        /// Gets the amount of work total
        /// </summary>
		public int Total { get; internal set; }

        /// <summary>
        /// Gets the amount of work done
        /// </summary>
		public int AmountDone { get; internal set; }
	}
}
