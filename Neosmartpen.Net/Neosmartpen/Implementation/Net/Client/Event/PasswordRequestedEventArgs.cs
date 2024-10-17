namespace Neosmartpen.Net
{
    /// <summary>
    /// Contains properties that password count for the PasswordRequested
    /// When you received this signal, you have to enter password by PenController.InputPassword()
    /// </summary>
    public sealed class PasswordRequestedEventArgs
	{
		internal PasswordRequestedEventArgs() { }
		internal PasswordRequestedEventArgs(int retryCount, int resetCount)
		{
			RetryCount = retryCount;
			ResetCount = resetCount;
		}
        /// <summary>
        /// count of enter password
        /// </summary>
        public int RetryCount { get; internal set; }
        /// <summary>
        /// if retry count reached reset count, delete all data in pen
        /// </summary>
        public int ResetCount { get; internal set; }
	}
}
