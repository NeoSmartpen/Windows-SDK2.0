namespace Neosmartpen.Net.Usb.Events
{
    /// <summary>
    /// Event argument containing the result of the file download
    /// </summary>
    public class FileDownloadResultReceivedEventArgs : System.EventArgs
    {
        /// <summary>
        /// File download result type
        /// </summary>
        public enum ResultType { Success, FileNotExists, Failed, OffsetInvalid, CannotOpenFile, UnknownError }

        /// <summary>
        /// File download result
        /// </summary>
        public ResultType Result { get; private set; }

        internal FileDownloadResultReceivedEventArgs(ResultType result)
        {
            Result = result;
        }
    }
}
