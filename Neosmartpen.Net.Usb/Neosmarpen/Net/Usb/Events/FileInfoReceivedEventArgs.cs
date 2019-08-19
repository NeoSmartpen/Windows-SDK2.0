namespace Neosmartpen.Net.Usb.Events
{
    /// <summary>
    /// Event arguments containing file information
    /// </summary>
    public class FileInfoReceivedEventArgs : System.EventArgs
    {
        /// <summary>
        /// File information result type
        /// </summary>
        public enum ResultType { Success = 0x00, FileNotExists = 0x01 }

        /// <summary>
        /// File information result
        /// </summary>
        public ResultType Result { get; private set; }

        /// <summary>
        /// File name
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Size of file (bytes)
        /// </summary>
        public int FileSize { get; private set; }

        internal FileInfoReceivedEventArgs(ResultType result, string fileName, int fileSize)
        {
            Result = result;
            FileName = fileName;
            FileSize = fileSize;
        }
    }
}
