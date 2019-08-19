namespace Neosmartpen.Net.Usb.Events
{
    /// <summary>
    /// Event arguments that contain progress information
    /// </summary>
    public class ProgressChangedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Current progress (0~100%)
        /// </summary>
        public int Progress { get; private set; }

        internal ProgressChangedEventArgs(int progress)
        {
            Progress = progress;
        }
    }
}
