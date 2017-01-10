using Neosmartpen.Net.Support;
using System;

namespace Neosmartpen.Net.Protocol.v2
{
    /// <summary>
    /// 읽어드린 바이트에 대한 파서
    /// </summary>
    public class ProtocolParserV2 : IProtocolParser
    {
        public event EventHandler<PacketEventArgs> PacketCreated;

        private ByteUtil mBuffer = null;

        private bool IsEscape = false;

        public void Put( byte[] buff, int size )
        {
            byte[] test = new byte[size];

            //Array.Copy( buff, 0, test, 0, size );
            //System.Console.WriteLine( "Read Buffer : {0}", BitConverter.ToString( test ) );
            //System.Console.WriteLine();

            for ( int i = 0; i < size; i++ )
            {
                if ( buff[i] == Const.PK_STX )
                {
                    // 패킷 시작
                    mBuffer = new ByteUtil();

                    IsEscape = false;
                }
                else if ( buff[i] == Const.PK_ETX )
                {
                    // 패킷 끝
                    Packet.Builder builder = new Packet.Builder();

                    int cmd = mBuffer.GetByteToInt();

                    string cmdstr = Enum.GetName( typeof( Cmd ), cmd );

                    int result_size = cmdstr != null && cmdstr.EndsWith( "RESPONSE" ) ? 1 : 0;

                    int result = result_size > 0 ? mBuffer.GetByteToInt() : -1;

                    int length = mBuffer.GetShort();

                    byte[] data = mBuffer.GetBytes();

                    //System.Console.WriteLine( "length : {0}, data : {1}", length, data.Length );

                    builder.cmd( cmd )
                        .result( result )
                        .data( data );

                    //System.Console.WriteLine( "Read Packet : {0}", BitConverter.ToString( data ) );
                    //System.Console.WriteLine();

                    mBuffer.Clear();
                    mBuffer = null;

                    PacketCreated( this, new PacketEventArgs( builder.Build() ) );

                    IsEscape = false;
                }
                else if ( buff[i] == Const.PK_DLE )
                {
                    if ( i < size - 1 )
                    {
                        mBuffer.Put( (byte)( buff[++i] ^ 0x20 ) );
                    }
                    else
                    {
                        IsEscape = true;
                    }
                }
                else if ( IsEscape )
                {
                    mBuffer.Put( (byte)( buff[i] ^ 0x20 ) );

                    IsEscape = false;
                }
                else
                {
                    mBuffer.Put( buff[i] );
                }
            }
        }
    }
}
