using Windows.Networking.Sockets;

namespace Neosmartpen.Net
{
    /// <exclude />
    /// <summary>
    /// IPenClient class provides fuctions that can handle pen.
    /// </summary>
    public interface IPenClient
    {
        IPenController PenController
        {
            get;
        }

        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Get the connection status of StreamSocket, ie, whether there is an active connection with remote device.  
        /// </summary>
        bool Alive
        {
            get;
        }

        /// <summary>
        /// unbind a socket instance
        /// </summary>
		System.Threading.Tasks.Task Unbind();


        /// <summary>
        /// To write data to stream
        /// </summary>
        /// <param name="data"></param>
        void Write(byte[] data);

        /// <summary>
        /// To read data when device write something
        /// </summary>
        void Read();
    }
}
