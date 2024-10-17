namespace Neosmartpen.Net
{
    /// <summary>
    /// Contains properties that data of coordinate with the DotReceived event
    /// </summary>
    public sealed class DotReceivedEventArgs
	{
		internal DotReceivedEventArgs() { }
        internal DotReceivedEventArgs(Dot dot)
		{
			Dot = dot;
		}
        internal DotReceivedEventArgs(Dot dot, ImageProcessingInfo imageProcessingInfo)
        {
            Dot = dot;
            ImageProcessingInfo = imageProcessingInfo;
        }

        /// <summary>
        /// Gets coordinate data of pen
        /// </summary>
        public Dot Dot { get; internal set; }

        public ImageProcessingInfo ImageProcessingInfo { get; internal set; }
    }
}
