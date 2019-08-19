namespace Neosmartpen.Net.Usb.Events
{
    /// <summary>
    /// Event arguments containing a list of files
    /// </summary>
    public class FileListReceivedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Result type
        /// </summary>
        public enum ResultType { Success, Failed, TooManyFileExists, UnknownError }

        /// <summary>
        /// Result
        /// </summary>
        public ResultType Result { get; private set; }

        /// <summary>
        /// List of files
        /// </summary>
        public string[] Files { get; private set; }

        internal FileListReceivedEventArgs(ResultType result, string[] files)
        {
            Result = result;
            Files = files;
        }
    }
}
