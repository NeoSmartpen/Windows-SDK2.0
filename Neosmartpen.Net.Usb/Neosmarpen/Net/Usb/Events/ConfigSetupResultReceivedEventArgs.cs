namespace Neosmartpen.Net.Usb.Events
{
    /// <summary>
    /// Event argument indicating the result of the pen's configuration change.
    /// </summary>
    public class ConfigSetupResultReceivedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Setting type of the pen
        /// </summary>
        public ConfigType Type { get; private set; }
        /// <summary>
        /// Change
        /// </summary>
        public bool IsChanged { get; private set; }

        internal ConfigSetupResultReceivedEventArgs(ConfigType type, bool isChanged)
        {
            Type = type;
            IsChanged = isChanged;
        }
    }
}
