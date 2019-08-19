namespace Neosmartpen.Net.Usb.Events
{
    /// <summary>
    /// Event arguments that contain simple results for the request
    /// </summary>
    public class ResultReceivedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Result type
        /// </summary>
        public enum ResultType { Success, Failed }

        /// <summary>
        /// Result
        /// </summary>
        public ResultType Result { get; private set; }

        internal ResultReceivedEventArgs(ResultType result)
        {
            Result = result;
        }
    }
}
