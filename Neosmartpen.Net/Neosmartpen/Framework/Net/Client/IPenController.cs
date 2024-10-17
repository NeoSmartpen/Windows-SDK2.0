namespace Neosmartpen.Net
{
    /// <summary>
    /// Represents a controller of pen
    /// </summary>
    public interface IPenController
    {
        /// <summary>
        /// Fired when read data, override to parse data in your implementation
        /// </summary>
        /// <param name="buff">byte array of new data</param>
        void OnDataReceived( byte[] buff );

        /// <summary>
        /// Gets binded PenClient
        /// </summary>
        IPenClient PenClient { get; set; }

		/// <summary>
		/// Get Protocol version
		/// </summary>
		int Protocol { get; set; }
        /// <summary>
        /// Fired when a connection is made, override to handle in your own code. 
        /// </summary>
        void OnConnected();

        /// <summary>
        /// Fired when a connection is destroyed, override to handle in your own code. 
        /// </summary>
        void OnDisconnected();
    }
}
