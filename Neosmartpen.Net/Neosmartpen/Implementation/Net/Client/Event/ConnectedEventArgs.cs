namespace Neosmartpen.Net
{
    /// <summary>
    /// Contains properties that information of device with the ConnectedEvent event
    /// </summary>
    public sealed class ConnectedEventArgs
	{
		internal ConnectedEventArgs() { }
		internal ConnectedEventArgs(string firmwareVersion, int maxForce)
		{
			FirmwareVersion = firmwareVersion;
			MaxForce = maxForce;
		}
		internal ConnectedEventArgs(string macAddress, string deviceName, string firmwareVersion, string protocolVersion, string subName, int maxForce)
			: this(firmwareVersion, maxForce)
		{
			MacAddress = macAddress;
			DeviceName = deviceName;
			ProtocolVersion = protocolVersion;
			SubName = subName;
		}

        /// <summary>
        /// Gets the maximum level of force sensor
        /// </summary>
        public int MaxForce { get; internal set; }

        /// <summary>
        /// Gets current version of pen's firmware
        /// </summary>
        public string FirmwareVersion { get; internal set; }

        /// <summary>
        /// Gets the device identifier
        /// </summary>
        public string MacAddress { get; internal set; }

        /// <summary>
        /// Gets a common name of a device
        /// </summary>
        public string DeviceName { get; internal set; }

        /// <summary>
        /// Gets a version of a protocol
        /// </summary>
        public string ProtocolVersion { get; internal set; }

        /// <summary>
        /// Gets a subname of a device
        /// </summary>
        public string SubName { get; internal set; }
	}
}