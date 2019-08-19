using Neosmartpen.Net.Support;
using System;

namespace Neosmartpen.Net.Usb.Events
{
    /// <summary>
    /// Event arguments that contain date and time settings for the pen
    /// </summary>
    public class DateTimeReceivedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Current date and time settings for the pen
        /// </summary>
        public System.DateTime DateTime { get; private set; }

        internal DateTimeReceivedEventArgs(long timestamp)
        {
            DateTime = Time.GetLocalDateTimeFromUtcTimestamp(timestamp);
        }
    }
}
