namespace Neosmartpen.Net
{
    /// <summary>
    /// Contains properties that status and setting of pen for the PenStatusReceived
    /// </summary>
    public sealed class PenStatusReceivedEventArgs
	{
        internal PenStatusReceivedEventArgs() { }
		// TODO Check
		// for v1
		internal PenStatusReceivedEventArgs(int timeoffset, long timetick, int maxForce, int battery, int usedMem, int penColor, bool autoPowerMode, bool accelerationMode, bool hoverMode, bool beep
			, short autoShutdownTime, short penSensitivity, string modelName)
		{
			TimeOffset = timeoffset;
			Timestamp = timetick;
			MaxForce = maxForce;
			Battery = battery;
			UsedMem = usedMem;
			PenColor = penColor;
			AutoPowerOn = autoPowerMode;
			AccelerationMode = accelerationMode;
			HoverMode = hoverMode;
			Beep = beep;
			AutoShutdownTime = autoShutdownTime;
			PenSensitivity = penSensitivity;
			ModelName = modelName;
		}
		// for v2
		internal PenStatusReceivedEventArgs(bool locked, int passwordMaxRetryCount, int passwordRetryCount, long timestamp, short autoShutdownTime
			, int maxForce, int battery, int usedMem, bool useOfflineData, bool autoPowerOn, bool penCapPower, bool hover, bool beep, short penSensitivity)
		{
			Locked = locked;
			PasswordMaxRetryCount = passwordMaxRetryCount;
			PasswordRetryCount = passwordRetryCount;
			Timestamp = timestamp;
			AutoShutdownTime = autoShutdownTime;
			MaxForce = maxForce;
			Battery = battery;
			UsedMem = usedMem;
			UseOfflineData = useOfflineData;
			AutoPowerOn = autoPowerOn;
			PenCapPower = penCapPower;
			HoverMode = hover;
			Beep = beep;
			PenSensitivity = penSensitivity;
		}

        // common
        /// <summary>
        /// Gets RTC millisecond timestamp of pen (from 1970-01-01)
        /// </summary>
        public long Timestamp { get; internal set; }

        /// <summary>
        /// Gets the power-off setting when there is no input for a certain period of time
        /// </summary>
        public short AutoShutdownTime { get; internal set; }

        /// <summary>
        /// Gets maximum level of force sensor
        /// </summary>
        public int MaxForce { get; internal set; }

        /// <summary>
        /// Gets battery status of pen
        /// </summary>
        public int Battery { get; internal set; }

        /// <summary>
        /// Gets status of pen's storage
        /// </summary>
        public int UsedMem { get; internal set; }

        /// <summary>
        /// Gets the power-on setting when the pen tip is pressed
        /// </summary>
        public bool AutoPowerOn { get; internal set; }
		public bool HoverMode { get; internal set; }

        /// <summary>
        /// Gets the status of the beep sound property
        /// </summary>
        public bool Beep { get; internal set; }

        /// <summary>
        /// Gets the value of the pen's sensitivity property that controls the force sensor of pen
        /// </summary>
        public short PenSensitivity { get; internal set; }


        // V1
        /// <summary>
        /// timestamp offset
        /// In most cases, you can ignore it
        /// </summary>
        public int TimeOffset { get; internal set; }

        /// <summary>
        /// Gets color of pen's led
        /// </summary>
        public int PenColor { get; internal set; }

        /// <summary>
        /// Gets the status of the acceleration sensor property
        /// </summary>
        public bool AccelerationMode { get; internal set; }

        // V2
        /// <summary>
        /// Gets the status of pen's authorization
        /// true if pen is locked, otherwise false
        /// </summary>
        public bool Locked { get; internal set; }

        /// <summary>
        /// Gets the maximum password input count
        /// </summary>
        public int PasswordMaxRetryCount { get; internal set; }

        /// <summary>
        /// Gets the current password input count
        /// </summary>
        public int PasswordRetryCount { get; internal set; }

        /// <summary>
        /// Gets the usage of offline data
        /// true if offline data available, otherwise false
        /// </summary>
        public bool UseOfflineData { get; internal set; }

        /// <summary>
        /// Gets the property that can be control by cap of pen
        /// </summary>
        public bool PenCapPower { get; internal set; }
		/// <summary>
		/// Model Name 
		/// This property use in protocol ver 1
		/// </summary>
		public string ModelName { get; internal set; }
	}
}
