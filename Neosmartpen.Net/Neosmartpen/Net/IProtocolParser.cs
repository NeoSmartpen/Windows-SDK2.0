using System;

namespace Neosmartpen.Net
{
    public interface IProtocolParser
    {
        event EventHandler<PacketEventArgs> PacketCreated;

        void Put( byte[] buff, int size );
    }

    public class PacketEventArgs : EventArgs
    {
        public PacketEventArgs( IPacket _packet )
        {
            Packet = _packet;
        }

        public IPacket Packet { get; private set; }
    }
}
