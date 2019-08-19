namespace Neosmartpen.Net.Usb.Events
{
    /// <summary>
    /// Event argument containing the storage status of the connected pen
    /// </summary>
    public class StorageStatusReceivedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Storage current status (0~100%)
        /// </summary>
        public int Storage { get; private set; }

        internal StorageStatusReceivedEventArgs(int storage)
        {
            Storage = storage;
        }
    }
}
