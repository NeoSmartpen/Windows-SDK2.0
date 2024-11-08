namespace Neosmartpen.Net
{
    /// <exclude />
    public interface IPacket
    {
        int Cmd { get; }

        byte GetByte();

        byte[] GetBytes();

        byte[] GetBytes( int size );

        int GetByteToInt();

        byte GetChecksum();

        byte GetChecksum( int length );
        
        int GetInt();
        
        long GetLong();
        
        short GetShort();
        
        string GetString( int length );

        IPacket Move( int length );

        IPacket Reset();

        int Result { get; }
        
        string ToString();
    }
}
