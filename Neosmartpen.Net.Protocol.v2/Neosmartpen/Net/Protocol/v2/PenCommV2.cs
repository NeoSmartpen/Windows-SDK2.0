using Neosmartpen.Net.Support;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Neosmartpen.Net.Protocol.v2
{
    /// <summary>
    /// Provides fuctions that can handle F50, F120 Smartpen
    /// </summary>
    public class PenCommV2 : PenComm
    {
        private PenCommV2Callbacks Callback;

        /// <summary>
        /// Gets a name of a device.
        /// </summary>
        public string DeviceName { get; private set; }

        /// <summary>
        /// Gets a version of a firmware.
        /// </summary>
        public string FirmwareVersion { get; private set; }

        /// <summary>
        /// Gets a version of a protocol.
        /// </summary>
        public string ProtocolVersion { get; private set; }

        public string SubName { get; private set; }

        /// <summary>
        /// Gets the device identifier.
        /// </summary>
        public string MacAddress { get; private set; }

        public short DeviceType { get; private set; }
        
        /// <summary>
        /// Gets the maximum level of force sensor.
        /// </summary>
        public short MaxForce { get; private set; }

        private long mTime = -1L;

        private PenTipType mPenTipType = PenTipType.Normal;

        private int mPenTipColor = -1;

        public enum PenTipType { Normal = 0, Eraser = 1 };

        private bool IsStartWithDown = false;

        private int mDotCount = -1;

        private int mCurSection = -1, mCurOwner = -1, mCurNote = -1, mCurPage = -1;

        private Packet mPrevPacket = null;

        private int mTotalOfflineStroke = -1, mReceivedOfflineStroke = 0, mTotalOfflineDataSize = -1;

        public bool HoverMode
        {
            get;
            private set;
        }

        /// <inheritdoc/>
        public override string Version
        {
            get { return "2.00"; }
        }

        /// <inheritdoc/>
        public override uint DeviceClass
        {
            get { return 0x2510; }
        }

        /// <inheritdoc/>
        public override string Name
        {
            get;
            set;
        }

        public PenCommV2( PenCommV2Callbacks handler, IProtocolParser parser = null  ) : base( parser == null ? new ProtocolParserV2() : parser )
        {
            Callback = handler;
            Parser.PacketCreated += mParser_PacketCreated;
        }

        private void mParser_PacketCreated( object sender, PacketEventArgs e )
        {
            ParsePacket( e.Packet as Packet );
        }

        protected override void OnConnected()
        {
            Thread.Sleep( 500 );
            ReqVersion();
        }

        protected override void OnDisconnected()
        {
            Callback.onDisconnected( this );
        }

        private void ParsePacket( Packet pk )
        {
            Cmd cmd = (Cmd)pk.Cmd;

            System.Console.Write( "Cmd : {0}", cmd.ToString() );

            switch ( cmd )
            {
                case Cmd.VERSION_RESPONSE:
                    {
                        DeviceName = pk.GetString( 16 );
                        FirmwareVersion = pk.GetString( 16 );
                        ProtocolVersion = pk.GetString( 8 );
                        SubName = pk.GetString( 16 );
                        DeviceType = pk.GetShort();
                        MaxForce = -1;
                        MacAddress = BitConverter.ToString( pk.GetBytes( 6 ) ).Replace( "-", "" );

                        ReqPenStatus();
                    }
                    break;

                #region event
                case Cmd.SHUTDOWN_EVENT:
                    {
                        byte reason = pk.GetByte();
                        System.Console.Write( " => SHUTDOWN_EVENT : {0}", reason );
                    }
                    break;

                case Cmd.LOW_BATTERY_EVENT:
                    {
                        int battery = (int)( pk.GetByte() & 0xff );
                        Callback.onReceiveBatteryAlarm( this, battery );
                    }
                    break;

                case Cmd.ONLINE_PEN_UPDOWN_EVENT:
                case Cmd.ONLINE_PEN_DOT_EVENT:
                case Cmd.ONLINE_PAPER_INFO_EVENT:
                    {
                        ParseDotPacket( cmd, pk );
                    }
                    break;
                #endregion

                #region setting response
                case Cmd.SETTING_INFO_RESPONSE:
                {
                    // 비밀번호 사용 여부
                    bool lockyn = pk.GetByteToInt() == 1;

                    // 비밀번호 입력 최대 시도 횟수
                    int pwdMaxRetryCount = pk.GetByteToInt();
                    
                    // 비밀번호 입력 시도 횟수
                    int pwdRetryCount = pk.GetByteToInt();

                    // 1970년 1월 1일부터 millisecond tick
                    long time = pk.GetLong();
                    
                    // 사용하지 않을때 자동으로 전원이 종료되는 시간 (단위:분)
                    short autoPowerOffTime = pk.GetShort();
                    
                    // 최대 필압
                    short maxForce = pk.GetShort();
                    
                    // 현재 메모리 사용량
                    int usedStorage = pk.GetByteToInt();
                    
                    // 펜의 뚜껑을 닫아서 펜의 전원을 차단하는 기능 사용 여부
                    bool penCapOff = pk.GetByteToInt() == 1;
                    
                    // 전원이 꺼진 펜에 필기를 시작하면 자동으로 펜의 켜지는 옵션 사용 여부
                    bool autoPowerON = pk.GetByteToInt() == 1;
                    
                    // 사운드 사용여부
                    bool beep = pk.GetByteToInt() == 1;
                    
                    // 호버기능 사용여부
                    bool hover = pk.GetByteToInt() == 1;
                    
                    // 남은 배터리 수치
                    int batteryLeft = pk.GetByteToInt();
                    
                    // 오프라인 데이터 저장 기능 사용 여부
                    bool useOffline = pk.GetByteToInt() == 1;
                    
                    // 필압 단계 설정 (0~4) 0이 가장 민감
                    short fsrStep = (short)pk.GetByteToInt();

                    // 최초 연결시
                    if ( MaxForce == -1 )
                    {
                        MaxForce = maxForce;

                        Callback.onConnected( this, MacAddress, DeviceName, FirmwareVersion, ProtocolVersion, SubName, MaxForce );

                        if ( lockyn )
                        {
                            Callback.onPenPasswordRequest( this, pwdRetryCount, pwdMaxRetryCount );
                        }
                        else
                        {
                            Callback.onPenAuthenticated( this );
                        }
                    }
                    else
                    {
                        Callback.onReceivePenStatus( this, lockyn, pwdMaxRetryCount, pwdRetryCount, time, autoPowerOffTime, MaxForce, batteryLeft, usedStorage, useOffline, autoPowerON, penCapOff, hover, beep, fsrStep );
                    }
                }
                    break;

                case Cmd.SETTING_CHANGE_RESPONSE:
                    {
                        int inttype = pk.GetByteToInt();

                        SettingType stype = (SettingType)inttype;

                        bool result = pk.Result == 0x00;

                        switch ( stype )
                        {
                            case SettingType.AutoPowerOffTime:
                                Callback.onPenAutoShutdownTimeSetUpResponse( this, result );
                                break;

                            case SettingType.AutoPowerOn:
                                Callback.onPenAutoPowerOnSetUpResponse( this, result );
                                break;

                            case SettingType.Beep:
                                Callback.onPenBeepSetUpResponse( this, result );
                                break;

                            case SettingType.Hover:
                                Callback.onPenHoverSetUpResponse( this, result );
                                break;

                            case SettingType.LedColor:
                                Callback.onPenColorSetUpResponse( this, result );
                                break;

                            case SettingType.OfflineData:
                                Callback.onPenOfflineDataSetUpResponse( this, result );
                                break;

                            case SettingType.PenCapOff:
                                Callback.onPenCapPowerOnOffSetupResponse( this, result );
                                break;

                            case SettingType.Sensitivity:
                                Callback.onPenSensitivitySetUpResponse( this, result );
                                break;

                            case SettingType.Timestamp:
                                Callback.onPenTimestampSetUpResponse( this, result );
                                break;
                        }
                    }
                    break;
                #endregion

                #region password response
                case Cmd.PASSWORD_RESPONSE:
                    {
                        int status = pk.GetByteToInt();
                        int cntRetry = pk.GetByteToInt();
                        int cntMax = pk.GetByteToInt();

                        if ( status == 0 )
                        {
                            Callback.onPenPasswordRequest( this, cntRetry, cntMax );
                        }
                        else
                        {
                            Callback.onPenAuthenticated( this );
                        }
                    }
                    break;

                case Cmd.PASSWORD_CHANGE_RESPONSE:
                    {
                        int cntRetry = pk.GetByteToInt();
                        int cntMax = pk.GetByteToInt();

                        Callback.onPenPasswordSetUpResponse( this, pk.Result == 0x00 );
                    }
                    break;
                #endregion

                #region offline response
                case Cmd.OFFLINE_NOTE_LIST_RESPONSE:
                    {
                        short length = pk.GetShort();

                        List<OfflineDataInfo> result = new List<OfflineDataInfo>();

                        for ( int i = 0; i < length; i++ )
                        {
                            byte[] rb = pk.GetBytes( 4 );

                            int section = (int)( rb[3] & 0xFF );
                            int owner = ByteConverter.ByteToInt( new byte[] { rb[0], rb[1], rb[2], (byte)0x00 } );
                            int note = pk.GetInt();

                            result.Add( new OfflineDataInfo( section, owner, note ) );
                        }

                        Callback.onReceiveOfflineDataList( this, result.ToArray() );
                    }
                    break;

                case Cmd.OFFLINE_PAGE_LIST_RESPONSE:
                    {
                        byte[] rb = pk.GetBytes( 4 );

                        int section = (int)( rb[3] & 0xFF );
                        int owner = ByteConverter.ByteToInt( new byte[] { rb[0], rb[1], rb[2], (byte)0x00 } );
                        int note = pk.GetInt();

                        short length = pk.GetShort();

                        int[] pages = new int[length];

                        for ( int i = 0; i < length; i++ )
                        {
                            pages[i] = pk.GetInt();
                        }

                        OfflineDataInfo info = new OfflineDataInfo( section, owner, note );

                        Callback.onReceiveOfflineDataList( this, info );
                    }
                    break;

                case Cmd.OFFLINE_DATA_RESPONSE:
                    {
                        mTotalOfflineStroke = pk.GetInt();
                        mReceivedOfflineStroke = 0;
                        mTotalOfflineDataSize = pk.GetInt();

                        bool isCompressed = pk.GetByte() == 1;

                        Callback.onStartOfflineDownload( this );
                    }
                    break;

                case Cmd.OFFLINE_PACKET_REQUEST:
                    {
                        #region offline data parsing

                        List<Stroke> result = new List<Stroke>();

                        short packetId = pk.GetShort();
                        
                        bool isCompressed = pk.GetByte() == 1;

                        short sizeBefore = pk.GetShort();
                        
                        short sizeAfter = pk.GetShort();

                        short location = (short)( pk.GetByte() & 0xFF );

                        byte[] rb = pk.GetBytes( 4 );

                        int section = (int)( rb[3] & 0xFF );
                        
                        int owner = ByteConverter.ByteToInt( new byte[] { rb[0], rb[1], rb[2], (byte)0x00 } );
                        
                        int note = pk.GetInt();

                        short strCount = pk.GetShort();

                        mReceivedOfflineStroke += strCount;

                        System.Console.WriteLine( " packetId : {0}, isCompressed : {1}, sizeBefore : {2}, sizeAfter : {3}, size : {4}", packetId, isCompressed, sizeBefore, sizeAfter, pk.Data.Length - 18 );

                        if ( sizeAfter != (pk.Data.Length - 18) )
                        {
                            SendOfflinePacketResponse( packetId, false );
                            return;
                        }

                        byte[] oData = pk.GetBytes( sizeAfter );

                        byte[] strData = Ionic.Zlib.ZlibStream.UncompressBuffer( oData );

                        if ( strData.Length != sizeBefore )
                        {
                            SendOfflinePacketResponse( packetId, false );
                            return;
                        }

                        ByteUtil butil = new ByteUtil( strData ); 

                        for ( int i = 0; i < strCount; i++ )
                        {
                            int pageId = butil.GetInt();

                            long timeStart = butil.GetLong();

                            long timeEnd = butil.GetLong();

                            int penTipType = (int)( butil.GetByte() & 0xFF );

                            int color = butil.GetInt();

                            short dotCount = butil.GetShort();

                            long time = timeStart;

                            //System.Console.WriteLine( "pageId : {0}, timeStart : {1}, timeEnd : {2}, penTipType : {3}, color : {4}, dotCount : {5}, time : {6},", pageId, timeStart, timeEnd, penTipType, color, dotCount, time );

                            Stroke stroke = new Stroke( section, owner, note, pageId );

                            for ( int j = 0; j < dotCount; j++ )
                            {
                                byte dotChecksum = butil.GetChecksum( 15 );

                                int timeadd = butil.GetByte();

                                time += timeadd;

                                int force = butil.GetShort();

                                int x = butil.GetShort();
                                int y = butil.GetShort();

                                int fx = butil.GetByte();
                                int fy = butil.GetByte();

                                int tx = butil.GetByte();
                                int ty = butil.GetByte();

                                int twist = butil.GetShort();

                                short reserved = butil.GetShort();

                                byte checksum = butil.GetByte();

                                //System.Console.WriteLine( "x : {0}, y : {1}, force : {2}, checksum : {3}, dotChecksum : {4}", tx, ty, twist, checksum, dotChecksum );

                                if ( dotChecksum != checksum )
                                {
                                    SendOfflinePacketResponse( packetId, false );
                                    result.Clear();
                                    return;
                                }

                                DotTypes dotType;

                                if ( j == 0 )
                                {
                                    dotType = DotTypes.PEN_DOWN;
                                }
                                else if ( j ==  dotCount-1 )
                                {
                                    dotType = DotTypes.PEN_UP;
                                }
                                else
                                {
                                    dotType = DotTypes.PEN_MOVE;
                                }

                                stroke.Add( new Dot( owner, section, note, pageId, time, x, y, fx, fy, force, dotType, color ) );
                            }

                            result.Add( stroke );
                        }

                        SendOfflinePacketResponse( packetId );

                        Callback.onReceiveOfflineStrokes( this, mTotalOfflineStroke, mReceivedOfflineStroke, result.ToArray() );

                        if ( location == 2 )
                        {
                            Callback.onFinishedOfflineDownload( this, true );
                        }

                        #endregion
                    }
                    break;

                case Cmd.OFFLINE_DATA_DELETE_RESPONSE:
                    {
                        Callback.onRemovedOfflineData( this, pk.Result == 0x00 );
                    }
                    break;
                #endregion

                #region firmware response
                case Cmd.FIRMWARE_UPLOAD_RESPONSE:
                    {
                        if ( pk.Result != 0 || pk.GetByteToInt() != 0 )
                        {
                            IsUploading = false;
                            Callback.onReceiveFirmwareUpdateResult( this, false );
                        }
                    }
                    break;

                case Cmd.FIRMWARE_PACKET_REQUEST:
                    {
                        int status = pk.GetByteToInt();
                        int offset = pk.GetInt();

                        System.Console.WriteLine();  

                        ResponseChunkRequest( offset, status != 3 );
                    }
                    break;
                #endregion

                case Cmd.ONLINE_DATA_RESPONSE:
                    break;

                default:
                    break;
            }

            System.Console.WriteLine();    
        }

        private void ParseDotPacket( Cmd cmd, Packet pk )
        {
            switch( cmd )
            {
                case Cmd.ONLINE_PEN_UPDOWN_EVENT:

                IsStartWithDown = pk.GetByte() == 0x00;

                mDotCount = 0;

                mTime = pk.GetLong();

                mPenTipType = pk.GetByte() == 0x00 ? PenTipType.Normal : PenTipType.Eraser;

                mPenTipColor = pk.GetInt();

                if ( mPrevPacket != null && !IsStartWithDown )
                {
                    mPrevPacket.Reset();
                    ParseDot( mPrevPacket, DotTypes.PEN_UP );
                }

                break;

            case Cmd.ONLINE_PEN_DOT_EVENT:

                if ( HoverMode && !IsStartWithDown )
                {
                    // 호버모드 처리
                    ParseDot( pk, DotTypes.PEN_HOVER );
                }
                else if ( IsStartWithDown )
                {
                    ParseDot( pk, mDotCount == 0 ? DotTypes.PEN_DOWN : DotTypes.PEN_MOVE );
                }
                else
                {
                    //오류
                }

                mPrevPacket = pk;
                mDotCount++;

                break;

            case Cmd.ONLINE_PAPER_INFO_EVENT:

                byte[] rb = pk.GetBytes( 4 );

                mCurSection = (int)( rb[3] & 0xFF );
                mCurOwner = ByteConverter.ByteToInt( new byte[] { rb[0], rb[1], rb[2], (byte)0x00 } );
                mCurNote = pk.GetInt();
                mCurPage = pk.GetInt();

                break;
            }
        }

        private void ParseDot( Packet mPack, DotTypes type )
        {
            int timeadd = mPack.GetByte();

            mTime += timeadd;

            int force = mPack.GetShort();

            int x = mPack.GetShort();
            int y = mPack.GetShort();

            int fx = mPack.GetByte();
            int fy = mPack.GetByte();

            int tx = mPack.GetByte();
            int ty = mPack.GetByte();

            int twist = mPack.GetShort();

            Callback.onReceiveDot( this, new Dot( mCurOwner, mCurSection, mCurNote, mCurPage, mTime, x, y, fx, fy, force, type, mPenTipColor ) );
        }

        private byte[] Escape( byte input )
        {
            if ( input == Const.PK_STX || input == Const.PK_ETX || input == Const.PK_DLE  )
            {
                return new byte[] { Const.PK_DLE, (byte)(input ^ 0x20)  };
            }
            else
            {
                return new byte[] { input };
            }
        }

        private bool Send( ByteUtil bf )
        {
            bool result = Write( bf.ToArray() );

            bf.Clear();
            bf = null;

            return result;
        }

        private void ReqVersion()
        {
            ByteUtil bf = new ByteUtil( Escape );

            Assembly assemObj = Assembly.GetExecutingAssembly();
            Version v = assemObj.GetName().Version; // 현재 실행되는 어셈블리..dll의 버전 가져오기

            byte[] StrByte = Encoding.UTF8.GetBytes( String.Format( "{0}.{1}.{2}.{3}", v.Major, v.Minor, v.Build, v.Revision ) ); 

            bf.Put( Const.PK_STX, false )
              .Put( (byte)Cmd.VERSION_REQUEST )
              .PutShort( 34 )
              .PutNull( 16 )
              .Put( 0x12 )
              .Put( 0x01 )
              .Put( StrByte, 16 )
              .Put( Const.PK_ETX, false );

            Send( bf );
        }

        #region password

        /// <summary>
        /// Change the password of device.
        /// </summary>
        /// <param name="oldPassword">Current password</param>
        /// <param name="newPassword">New password</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqSetUpPassword( string oldPassword, string newPassword = "" )
        {
            byte[] oPassByte = Encoding.UTF8.GetBytes( oldPassword );
            byte[] nPassByte = Encoding.UTF8.GetBytes( newPassword );

            ByteUtil bf = new ByteUtil( Escape );
            bf.Put( Const.PK_STX, false )
              .Put( (byte)Cmd.PASSWORD_CHANGE_REQUEST )
              .PutShort( 33 )
              .Put( (byte)( newPassword == "" ? 0 : 1 ) )
              .Put( oPassByte, 16 )
              .Put( nPassByte, 16 )
              .Put( Const.PK_ETX, false );

            return Send( bf );
        }

        /// <summary>
        /// Input password if device is locked.
        /// </summary>
        /// <param name="password">Specifies the password for authentication. Password is a string</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqInputPassword( string password )
        {
            byte[] bStrByte = Encoding.UTF8.GetBytes( password );

            ByteUtil bf = new ByteUtil( Escape );
            bf.Put( Const.PK_STX, false )
              .Put( (byte)Cmd.PASSWORD_REQUEST )
              .PutShort( 16 )
              .Put( bStrByte, 16 )
              .Put( Const.PK_ETX, false );

            return Send( bf );
        }

        #endregion

        #region pen setup

        /// <summary>
        /// Request the status of pen.
        /// If you requested, you can receive result by PenCommV2Callbacks.onReceivedPenStatus method.
        /// </summary>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqPenStatus()
        {
            ByteUtil bf = new ByteUtil();

            bf.Put( Const.PK_STX )
                .Put( (byte)Cmd.SETTING_INFO_REQUEST )
                .PutShort( 0 )
                .Put( Const.PK_ETX );

            return Send( bf );
        }

        public enum SettingType : byte { Timestamp = 1, AutoPowerOffTime = 2, PenCapOff = 3, AutoPowerOn = 4, Beep = 5, Hover = 6, OfflineData = 7, LedColor = 8, Sensitivity = 9 };

        private bool RequestChangeSetting( SettingType stype, object value )
        {
            ByteUtil bf = new ByteUtil();

            bf.Put( Const.PK_STX ).Put( (byte)Cmd.SETTING_CHANGE_REQUEST );

            switch ( stype )
            {
                case SettingType.Timestamp:
                    bf.PutShort( 9 ).Put( (byte)stype ).PutLong( (long)value );
                    break;

                case SettingType.AutoPowerOffTime:
                    bf.PutShort( 3 ).Put( (byte)stype ).PutShort( (short)value );
                    break;

                case SettingType.LedColor:
                    bf.PutShort( 5 ).Put( (byte)stype ).PutInt( (int)value );
                    break;

                case SettingType.PenCapOff:
                case SettingType.AutoPowerOn:
                case SettingType.Beep:
                case SettingType.Hover:
                case SettingType.OfflineData:
                    bf.PutShort( 2 ).Put( (byte)stype ).Put( (byte)( (bool)value ? 1 : 0 ) );
                    break;
                case SettingType.Sensitivity:
                    bf.PutShort( 2 ).Put( (byte)stype ).Put( (byte)( (short)value ) );
                    break;
            }

            bf.Put( Const.PK_ETX );

            return Send( bf );
        }

        /// <summary>
        /// Sets the RTC timestamp.
        /// </summary>
        /// <param name="timetick">milisecond timestamp tick (from 1970-01-01)</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqSetupTime( long timetick )
        {
            return RequestChangeSetting( SettingType.Timestamp, timetick );
        }

        /// <summary>
        /// Sets the value of the auto shutdown time property that if pen stay idle, shut off the pen.
        /// </summary>
        /// <param name="minute">minute of maximum idle time, staying power on (0~)</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqSetupPenAutoShutdownTime( short minute )
        {
            return RequestChangeSetting( SettingType.AutoPowerOffTime, minute );
        }

        /// <summary>
        /// Sets the status of the power control by cap on property.
        /// </summary>
        /// <param name="seton">true if you want to use, otherwise false.</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqSetupPenCapPower( bool enable )
        {
            return RequestChangeSetting( SettingType.PenCapOff, enable );
        }

        /// <summary>
        /// Sets the status of the auto power on property that if write the pen, turn on when pen is down.
        /// </summary>
        /// <param name="seton">true if you want to use, otherwise false.</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqSetupPenAutoPowerOn( bool enable )
        {
            return RequestChangeSetting( SettingType.AutoPowerOn, enable );
        }

        /// <summary>
        /// Sets the status of the beep property.
        /// </summary>
        /// <param name="enable">true if you want to listen sound of pen, otherwise false.</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqSetupPenBeep( bool enable )
        {
            return RequestChangeSetting( SettingType.Beep, enable );
        }

        /// <summary>
        /// Sets the hover mode.
        /// </summary>
        /// <param name="enable">true if you want to enable hover mode, otherwise false.</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqSetupHoverMode( bool enable )
        {
            return RequestChangeSetting( SettingType.Hover, enable );
        }

        /// <summary>
        /// Sets the offline data option whether save offline data or not.
        /// </summary>
        /// <param name="enable">true if you want to enable offline mode, otherwise false.</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqSetupOfflineData( bool enable )
        {
            return RequestChangeSetting( SettingType.OfflineData, enable );
        }

        /// <summary>
        /// Sets the color of LED.
        /// </summary>
        /// <param name="rgbcolor">integer type color formatted 0xAARRGGBB</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqSetupPenColor( int color )
        {
            return RequestChangeSetting( SettingType.LedColor, color );
        }

        /// <summary>
        /// Sets the value of the pen's sensitivity property that controls the force sensor of pen.
        /// </summary>
        /// <param name="level">the value of sensitivity. (0~4, 0 means maximum sensitivity)</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqSetupPenSensitivity( short step )
        {
            return RequestChangeSetting( SettingType.Sensitivity, step );
        }

        #endregion

        #region using note

        private bool SendAddUsingNote( int sectionId = -1, int ownerId = -1, int[] noteIds = null )
        {
            ByteUtil bf = new ByteUtil();

            bf.Put( Const.PK_STX )
              .Put( (byte)Cmd.ONLINE_DATA_REQUEST );

            if ( sectionId > 0 && ownerId > 0 && noteIds == null )
            {
                bf.PutShort( 2 + 8 )
                  .PutShort( 1 )
                  .Put( GetSectionOwnerByte( sectionId, ownerId ) )
                  .Put( 0xFF ).Put( 0xFF ).Put( 0xFF ).Put( 0xFF );
            }
            else if ( sectionId > 0 && ownerId > 0 && noteIds != null )
            {
                short length = (short)( 2 + ( noteIds.Length * 8 ) );

                bf.PutShort( length )
                  .PutShort( (short)noteIds.Length );

                foreach ( int item in noteIds )
                {
                    bf.Put( GetSectionOwnerByte( sectionId, ownerId ) )
                    .PutInt( item );
                }
            }
            else
            {
                bf.PutShort( 2 )
                  .Put( 0xFF )
                  .Put( 0xFF );
            }

            bf.Put( Const.PK_ETX );

            return Send( bf );
        }

        /// <summary>
        /// Sets the available notebook type
        /// </summary>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqAddUsingNote()
        {
            return SendAddUsingNote();
        }

        /// <summary>
        /// Sets the available notebook type
        /// </summary>
        /// <param name="section">The Section Id of the paper</param>
        /// <param name="owner">The Owner Id of the paper</param>
        /// <param name="notes">The array of Note Id list</param>
        public bool ReqAddUsingNote( int section, int owner, int[] notes = null )
        {
            return SendAddUsingNote( section, owner, notes );
        }

        #endregion

        #region offline

        /// <summary>
        /// Requests the list of Offline data.
        /// </summary>
        /// <param name="section">The Section Id of the paper</param>
        /// <param name="owner">The Owner Id of the paper</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqOfflineDataList( int section = -1, int owner = -1 )
        {
            ByteUtil bf = new ByteUtil( Escape );

            byte[] pInfo = section > 0 && owner > 0 ? GetSectionOwnerByte( section, owner ) : new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };

            bf.Put( Const.PK_STX, false )
              .Put( (byte)Cmd.OFFLINE_NOTE_LIST_REQUEST )
              .PutShort( 4 )
              .Put( pInfo )
              .Put( Const.PK_ETX, false );

            return Send( bf );
        }

        /// <summary>
        /// Requests the list of Offline data.
        /// </summary>
        /// <param name="section">The Section Id of the paper</param>
        /// <param name="owner">The Owner Id of the paper</param>
        /// <param name="note">The Note Id of the paper</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqOfflineDataList( int section, int owner, int note )
        {
            ByteUtil bf = new ByteUtil( Escape );

            bf.Put( Const.PK_STX, false )
              .Put( (byte)Cmd.OFFLINE_PAGE_LIST_REQUEST )
              .PutShort( 8 )
              .Put( GetSectionOwnerByte( section, owner ) )
              .PutInt( note )
              .Put( Const.PK_ETX, false );

            return Send( bf );
        }

        /// <summary>
        /// Requests the transmission of data
        /// </summary>
        /// <param name="section">The Section Id of the paper</param>
        /// <param name="owner">The Owner Id of the paper</param>
        /// <param name="note">The Note Id of the paper</param>
        /// <param name="deleteOnFinished">delete offline data when transmission is finished,</param>
        /// <param name="pages">The number of page</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqOfflineData( int section, int owner, int note, bool deleteOnFinished = true, int[] pages = null )
        {
            byte[] ownerByte = ByteConverter.IntToByte( owner );

            short length = 14;

            length += (short)( pages == null ? 0 : pages.Length * 4 );

            ByteUtil bf = new ByteUtil( Escape );

            bf.Put( Const.PK_STX, false )
              .Put( (byte)Cmd.OFFLINE_DATA_REQUEST )
              .PutShort( length )
              .Put( (byte)( deleteOnFinished ? 1 : 2 ) )
              .Put( (byte)1 )
              .Put( GetSectionOwnerByte( section, owner ) )
              .PutInt( note )
              .PutInt( pages == null ? 0 : pages.Length );

            if ( pages != null )
            {
                foreach ( int page in pages )
                {
                    bf.PutInt( page );
                }
            }

            bf.Put( Const.PK_ETX, false );

            return Send( bf );
        }

        private void SendOfflinePacketResponse( short index, bool isSuccess = true )
        {
            ByteUtil bf = new ByteUtil( Escape );

            bf.Put( Const.PK_STX, false )
              .Put( (byte)Cmd.OFFLINE_PACKET_RESPONSE )
              .Put( (byte)( isSuccess ? 0 : 1 ) )
              .PutShort( 3 )
              .PutShort( index )
              .Put( 1 )
              .Put( Const.PK_ETX, false );

            Send( bf );
        }

        /// <summary>
        /// Request to remove offline data in device.
        /// </summary>
        /// <param name="section">The Section Id of the paper</param>
        /// <param name="owner">The Owner Id of the paper</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqRemoveOfflineData( int section, int owner, int[] notes )
        {
            ByteUtil bf = new ByteUtil( Escape );

            bf.Put( Const.PK_STX, false )
              .Put( (byte)Cmd.OFFLINE_DATA_DELETE_REQUEST );

            short length = (short)( 5 + ( notes.Length * 4 ) );

            bf.PutShort( length )
              .Put( GetSectionOwnerByte( section, owner ) )
              .Put( (byte)notes.Length );

            foreach ( int noteId in notes )
            {
                bf.PutInt( noteId );
            }

            bf.Put( Const.PK_ETX, false );

            return Send( bf );
        }

        #endregion

        #region firmware

        private Chunk mFwChunk;

        private bool IsUploading = false;

        /// <summary>
        /// Requests the firmware installation
        /// </summary>
        /// <param name="filepath">absolute path of firmware file</param>
        /// <param name="version">version of firmware, this value is string</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqPenSwUpgrade( string filepath, string version )
        {
            if ( IsUploading )
            {
                return false;
            }

            IsUploading = true;

            mFwChunk = new Chunk(1024);

            bool loaded = mFwChunk.Load( filepath );

            if ( !loaded )
            {
                return false;
            }

            int file_size = mFwChunk.GetFileSize();

            short chunk_count = (short)mFwChunk.GetChunkLength();
            short chunk_size = (short)mFwChunk.GetChunksize();

            byte[] StrVersionByte = Encoding.UTF8.GetBytes( version );

            byte[] StrDeviceByte = Encoding.UTF8.GetBytes( DeviceName );

            System.Console.WriteLine( "[FileUploadWorker] file upload => filesize : {0}, packet count : {1}, packet size {2}", file_size, chunk_count, chunk_size );

            ByteUtil bf = new ByteUtil( Escape );

            bf.Put( Const.PK_STX, false )
              .Put( (byte)Cmd.FIRMWARE_UPLOAD_REQUEST )
              .PutShort( 42 )
              .Put( StrDeviceByte, 16 )
              .Put( StrVersionByte, 16 )
              .PutInt( file_size )
              .PutInt( chunk_size )
              .Put( 1 )
              .Put( mFwChunk.GetTotalChecksum() )
              .Put( Const.PK_ETX, false );

            return Send( bf );
        }

        private void ResponseChunkRequest( int offset, bool status = true )
        {
            byte[] data = null;

            int index = (int)( offset / mFwChunk.GetChunksize() );

            System.Console.WriteLine( "[FileUploadWorker] ResponseChunkRequest upload => index : {0}", index );

            ByteUtil bf = new ByteUtil( Escape );

            if ( !status || mFwChunk == null || !IsUploading || (data = mFwChunk.Get( index )) == null )
            {
                bf.Put( Const.PK_STX, false )
                  .Put( (byte)Cmd.FIRMWARE_PACKET_RESPONSE )
                  .Put( 1 )
                  .PutShort( 0 )
                  .Put( Const.PK_ETX, false );

                IsUploading = false;
            }
            else
            {
                byte[] cdata = Ionic.Zlib.ZlibStream.CompressBuffer( data );

                byte checksum = mFwChunk.GetChecksum( index );

                short dataLength = (short)( cdata.Length + 14 );

                bf.Put( Const.PK_STX, false )
                  .Put( (byte)Cmd.FIRMWARE_PACKET_RESPONSE )
                  .Put( 0 )
                  .PutShort( dataLength )
                  .Put( 0 )
                  .PutInt( offset )
                  .Put( checksum )
                  .PutInt( data.Length )
                  .PutInt( cdata.Length )
                  .Put( cdata )
                  .Put( Const.PK_ETX, false );
            }

            Send( bf );

            Callback.onReceiveFirmwareUpdateStatus( this, mFwChunk.GetChunkLength(), (int)index );
        }

        /// <summary>
        /// To suspend firmware installation.
        /// </summary>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool SuspendSwUpgrade()
        {
            mFwChunk = null;
            return true;
        }

        #endregion

        #region util
        
        private static byte[] GetSectionOwnerByte( int section, int owner )
        {
            byte[] ownerByte = ByteConverter.IntToByte( owner );
            ownerByte[3] = (byte)section;

            return ownerByte;
        }

        #endregion
    }
}
