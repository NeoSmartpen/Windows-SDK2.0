namespace Neosmartpen.Net.Usb.Events
{
    /// <summary>
    /// Event argument containing the battery status of the connected pen
    /// </summary>
    public class BatteryStatusReceivedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Battery current status (0~100%)
        /// </summary>
        public int Battery { get; private set; }

        internal BatteryStatusReceivedEventArgs()
        {
        }

        internal BatteryStatusReceivedEventArgs(int battery) : base()
        {
            Battery = battery;
        }
    }
}
