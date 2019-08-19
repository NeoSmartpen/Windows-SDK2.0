namespace Neosmartpen.Net.Usb.Events
{
    /// <summary>
    /// Event argument that returns a UsbPenComm object that communicates with the pen when there is a change in connectivity with the pen.
    /// </summary>
    public class ConnectionStatusChangedEventArgs : System.EventArgs
    {
        /// <summary>
        /// UsbPenComm object for communicating with the pen
        /// </summary>
        public UsbPenComm UsbPenComm { get; private set; }

        internal ConnectionStatusChangedEventArgs(UsbPenComm usbPenComm) : base()
        {
            UsbPenComm = usbPenComm;
        }
    }
}
