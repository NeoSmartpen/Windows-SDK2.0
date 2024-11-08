namespace Neosmartpen.Net
{
    /// <exclude />
    public enum ErrorType
    {
        MissingPenUp = 1,
        MissingPenDown = 2,
        InvalidTime = 3,
        MissingPenDownPenMove = 4,
        ImageProcessingError = 5,
        InvalidEventCount = 6,
        MissingPageChange = 7,
        MissingPenMove = 8
    }

    /// <exclude />
    public sealed class ErrorDetectedEventArgs
    {
        internal ErrorDetectedEventArgs(ErrorType errorType, long ts)
        {
            ErrorType = errorType;
            Timestamp = ts;
        }

        internal ErrorDetectedEventArgs(ErrorType errorType, Dot dot, long ts)
        {
            ErrorType = errorType;
            Dot = dot;
            Timestamp = ts;
        }

        internal ErrorDetectedEventArgs(ErrorType errorType, Dot dot, long ts, string extraData)
        {
            ErrorType = errorType;
            Dot = dot;
            Timestamp = ts;
            ExtraData = extraData;
        }

        internal ErrorDetectedEventArgs(ErrorType errorType, Dot dot, long ts, ImageProcessErrorInfo imageProcessErrorInfo)
        {
            ErrorType = errorType;
            Dot = dot;
            Timestamp = ts;
            ImageProcessErrorInfo = imageProcessErrorInfo;
        }

        public ErrorType ErrorType { get; internal set; }

        public Dot Dot { get; internal set; }

        public long Timestamp { get; internal set; }

        public string ExtraData { get; internal set; }

        public ImageProcessErrorInfo ImageProcessErrorInfo { get; internal set; }
    } 
}
