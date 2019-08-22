namespace Neosmartpen.Net.Usb.Events
{
    /// <summary>
    /// Event argument containing the storage status of the connected pen
    /// </summary>
    public class StorageStatusReceivedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Total Storage Size (KB)
        /// </summary>
        public int TotalSize { get; private set; }

        /// <summary>
        /// Free Storage Size (KB)
        /// </summary>
        public int FreeSize { get; private set; }

        internal StorageStatusReceivedEventArgs(int total, int free)
        {
            TotalSize = total;
            FreeSize = free;
        }
    }
}
