namespace Neosmartpen.Net
{
    internal interface IPenClientParser
	{
		void ProtocolParse(byte[] buff, int size);

		void ParsePacket(Packet packet);

        IPenClient PenClient { get; }
    }
}
