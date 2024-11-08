namespace Neosmartpen.Net
{
    /// <summary>
    /// Contains propertie that a single boolean type result for common purpose
    /// </summary>
    public sealed class SimpleResultEventArgs
	{
		internal SimpleResultEventArgs() { }
		internal SimpleResultEventArgs(bool result)
		{
			Result = result;
		}
        /// <summary>
        /// Gets boolean type result
        /// </summary>
        public bool Result { get; internal set; }
	}
}
