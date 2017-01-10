using System.Net.Sockets;

namespace Neosmartpen.Net
{
    /// <summary>
    /// Interface of PenCommXXX object
    /// </summary>
    public interface IPenComm
    {
        IProtocolParser Parser
        {
            get;
        }

        /// <summary>
        /// Returns the Class of Device of the remote device.
        /// </summary>
        uint DeviceClass
        {
            get;
        }

        /// <summary>
        /// Gets or sets a name of PenComm object
        /// </summary>
        string Name
        {
            get;
            set;
        }


        /// <summary>
        /// Gets a version of PenComm object
        /// </summary>
        string Version
        {
            get;
        }

        /// <summary>
        /// Bind the bluetooth socket with IPenComm class.
        /// </summary>
        /// <param name="socket">General socket object that implemented the Berkeley sockets interface.</param>
        /// <param name="name">Name of PenComm object</param>
        void Bind( Socket socket, string name = null );

        /// <summary>
        /// 
        /// </summary>
        void Clean();
    }
}
