namespace Neosmartpen.Net
{
    /// <summary>
    /// Provides battery data for BatteryAlarmReceived event
    /// </summary>
    public sealed class BatteryAlarmReceivedEventArgs
	{
		internal BatteryAlarmReceivedEventArgs() { }
		internal BatteryAlarmReceivedEventArgs(int battery) { Battery = battery; }

        /// <summary>
        /// Gets battery level of the pen
        /// </summary>
        public int Battery { get; internal set; }
	}
}
