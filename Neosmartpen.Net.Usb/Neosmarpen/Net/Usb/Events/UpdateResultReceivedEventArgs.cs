namespace Neosmartpen.Net.Usb.Events
{
    /// <summary>
    /// Event argument containing the result of the firmware update request
    /// </summary>
    public class UpdateResultReceivedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Result type
        /// </summary>
        public enum ResultType { Complete, DeviceIsNotCorrect, FirmwareVersionIsNotCorrect, UnknownError }

        /// <summary>
        /// Result
        /// </summary>
        public ResultType Result { get; private set; }

        internal UpdateResultReceivedEventArgs(ResultType result)
        {
            Result = result;
        }
    }
}
