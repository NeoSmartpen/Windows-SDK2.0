using Neosmartpen.Net.Support;
using Neosmartpen.Net.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Neosmartpen.Net.Metadata;
using Neosmartpen.Net.Metadata.Model;
using System.Linq;

namespace Neosmartpen.Net.Protocol.v1
{
    /// <summary>
    /// Provides fuctions that can handle N2 Smartpen.
    /// </summary>
    public class PenCommV1 : PenComm, OfflineWorkResponseHandler
    {
		[System.Runtime.InteropServices.DllImport("Kernel32.dll")]
		private static extern bool GetDiskFreeSpace(string lpRootPathName, ref ulong lpSectorsPerCluster, ref ulong lpBytesPerSector, ref ulong lpNumberOfFreeClusters, ref ulong lpTotalNumberOfClusters);

		private PenCommV1Callbacks Callback;

        //private IPacket mPrevPacket;

        /// <summary>
        /// Gets the maximum level of force sensor.
        /// </summary>
        public short MaxForce { get; private set; }

        private int mOwnerId = 0, mSectionId = 0, mNoteId = 0, mPageId = 0;

        private long mPrevDotTime = 0;

        //private bool IsPrevDotDown = false;

        private bool IsStartWithDown = false;

        private int mCurrentColor = 0x000000;

        private String mOfflineFileName;

        private long mOfflineFileSize;

        private short mOfflinePacketCount, mOfflinePacketSize;

        private OfflineDataSerializer mOfflineDataBuilder;

        private bool Authenticated = false;

        private int mOfflineTotalDataSize = 0, mOfflineTotalFileCount = 0, mOfflineRcvDataSize = 0;

        private List<OfflineDataInfo> mOfflineNotes = new List<OfflineDataInfo>();

        private bool IsStartOfflineTask = false;

        private Chunk mFwChunk;

        private bool IsUploading = false;

        private OfflineWorker mOfflineworker;

		private readonly string DEFAULT_PASSWORD = "0000";
		private bool needToInputDefaultPassword;
		private FilterForPaper dotFilterForPage = null;

		private void Reset()
        {
            System.Console.WriteLine( "[PenCommCore] Reset" );

            //IsPrevDotDown = false;
            IsStartWithDown = false;
			IsBeforeMiddle = false;
			IsStartWithPaperInfo = false;
			IsBeforePaperInfo = false;

            Authenticated = false;

            IsUploading = false;

            IsStartOfflineTask = false;
        }

		/// <inheritdoc/>
		public override string Version
        {
            get { return "1.03" ; }
        }

        /// <inheritdoc/>
        public override uint DeviceClass
        {
            get { return 0x0500; }
        }

        /// <inheritdoc/>
        public override string Name
        {
            get;
            set;
        }

        public PenCommV1( PenCommV1Callbacks callback, IProtocolParser parser = null, IMetadataManager metadataManager = null ) : base( parser == null ? parser = new ProtocolParserV1() : parser, metadataManager )
        {
            Callback = callback;
            Parser.PacketCreated += Parser_PacketCreated;

			dotFilterForPage = new FilterForPaper(SendDotReceiveEvent);

            // 오프라인 데이터 처리
            if ( mOfflineworker == null )
            {
                mOfflineworker = new OfflineWorker(this);
                mOfflineworker.Startup( null );
            }
        }

        void OfflineWorkResponseHandler.onReceiveOfflineStrokes( Stroke[] strokes )
        {
            var resultSymbol = new List<Symbol>();

            if (MetadataManager != null)
            {
                foreach (var stroke in strokes)
                {
                    var symbols = MetadataManager.FindApplicableSymbols(stroke);

                    if (symbols != null && symbols.Count > 0)
                    {
                        foreach (var symbol in symbols)
                        {
                            if (resultSymbol.Where(s => s.Id == symbol.Id).Count() <= 0)
                            {
                                resultSymbol.Add(symbol);
                            }
                        }
                    }
                }
            }

            Callback.onReceiveOfflineStrokes( this, strokes, resultSymbol.ToArray());
        }

        void OfflineWorkResponseHandler.onRequestDownloadOfflineData( int sectionId, int ownerId, int noteId )
        {
            SendReqOfflineData( sectionId, ownerId, noteId );
        }

        void OfflineWorkResponseHandler.onRequestRemoveOfflineData( int sectionId, int ownerId )
        {
            ReqRemoveOfflineData( sectionId, ownerId );
        }

        private void Parser_PacketCreated( object sender, PacketEventArgs e )
        {
            ParsePacket( e.Packet );
        }

        protected override void OnConnected()
        {
        }

        protected override void OnDisconnected()
        {
			if (IsStartWithDown && IsBeforeMiddle && mPrevDot != null)
			{
				MakeUpDot();
			}

			mOfflineworker.Reset();
            Callback.onDisconnected( this );
        }

        new public void Clean()
        {
            base.Clean();
        }

		Dot mPrevDot;
		bool IsBeforeMiddle = false;

		private bool IsStartWithPaperInfo = false;
		private bool IsBeforePaperInfo = false;

		private long PenDownTime = -1;

		private void ParsePacket( IPacket pk )
        {
            switch ( (Cmd)pk.Cmd )
            {
                case Cmd.A_DotData:
                    {
                        long time = pk.GetByteToInt();
                        int x = pk.GetShort();
                        int y = pk.GetShort();
                        int fx = pk.GetByteToInt();
                        int fy = pk.GetByteToInt();
                        int force = pk.GetByteToInt();

                        long timeLong = mPrevDotTime + time;

						Dot.Builder builder = new Dot.Builder(MaxForce);

						builder.owner(mOwnerId)
							.section(mSectionId)
							.note(mNoteId)
							.page(mPageId)
							.timestamp(timeLong)
							.coord(x, fx, y, fy)
							.force(force)
							.color(mCurrentColor);


						if (!IsStartWithDown)
						{
							if (!IsStartWithPaperInfo)
							{
								//펜 다운 없이 페이퍼 정보 없고 무브가 오는 현상(다운 - 무브 - 업 - 다운X - 무브)
								Callback.onErrorDetected(this, ErrorType.MissingPenDown, -1, null, null);
							}
							else
							{
								//펜 다운 없이 페이퍼 정보 있고 무브가 오는 현상(다운 - 무브 - 업 - 다운X - 무브)
								builder.dotType(DotTypes.PEN_ERROR);
								var errorDot = builder.Build();
								Callback.onErrorDetected(this, ErrorType.MissingPenDown, -1, errorDot, null);
								IsStartWithDown = true;
								builder.timestamp(Time.GetUtcTimeStamp());
							}
						}

						if (!IsStartWithDown)
						{
							if (!IsStartWithPaperInfo)
							{
								//펜 다운 없이 페이퍼 정보 없고 무브가 오는 현상(다운 - 무브 - 업 - 다운X - 무브)
								Callback.onErrorDetected(this, ErrorType.MissingPenDown, -1, null, null);
							}
							else
							{
								timeLong = Time.GetUtcTimeStamp();
								PenDownTime = timeLong;
								//펜 다운 없이 페이퍼 정보 있고 무브가 오는 현상(다운 - 무브 - 업 - 다운X - 무브)
								builder.dotType(DotTypes.PEN_ERROR);
								var errorDot = builder.Build();
								Callback.onErrorDetected(this, ErrorType.MissingPenDown, PenDownTime, errorDot, null);
								IsStartWithDown = true;
								builder.timestamp(timeLong);
							}
						}

						if (timeLong < 10000)
						{
							// 타임스템프가 10000보다 작을 경우 도트 필터링
							builder.dotType(DotTypes.PEN_ERROR);
							var errorDot = builder.Build();
							Callback.onErrorDetected(this, ErrorType.InvalidTime, PenDownTime, errorDot, null);
						}

						Dot dot = null;

						if (IsStartWithDown && IsStartWithPaperInfo && IsBeforePaperInfo)
						{
							// 펜다운의 경우 시작 도트로 저장
							dot = builder.timestamp(PenDownTime).dotType(DotTypes.PEN_DOWN).Build();
						}
						else if (IsStartWithDown && IsStartWithPaperInfo && !IsBeforePaperInfo && IsBeforeMiddle)
						{
							// 펜다운이 아닌 경우 미들 도트로 저장
							dot = builder.dotType(DotTypes.PEN_MOVE).Build();
						}
						else if (IsStartWithDown && !IsStartWithPaperInfo)
						{
							//펜 다운 이후 페이지 체인지 없이 도트가 들어왔을 경우
							Callback.onErrorDetected(this, ErrorType.MissingPageChange, PenDownTime, null, null);
						}

						if (dot != null)
						{
							ProcessDot(dot);
						}

						mPrevDot = dot;
						mPrevDotTime = timeLong;
						IsBeforeMiddle = true;
						IsBeforePaperInfo = false;
					}
                    break;

                case Cmd.A_DotUpDownDataNew:
                case Cmd.A_DotUpDownData:
                    {
                        long updownTime = pk.GetLong();

                        int updown = pk.GetByteToInt();

                        byte[] cbyte = pk.GetBytes( 3 );

                        mCurrentColor = ByteConverter.ByteToInt( new byte[] { cbyte[2], cbyte[1], cbyte[0], (byte)0xFF } );

                        if ( updown == 0x00 )
                        {
                            // 펜 다운 일 경우 Start Dot의 timestamp 설정
                            mPrevDotTime = updownTime;
                            //IsPrevDotDown = true;

							if (IsBeforeMiddle && mPrevDot != null)
							{
								// 펜업이 넘어오지 않는 경우
								//var errorDot = mPrevDot.Clone();
								//errorDot.DotType = DotTypes.PEN_ERROR;
								//Callback.onErrorDetected(this, ErrorType.MissingPenUp, PenDownTime, errorDot, null);

								MakeUpDot();
							}

                            IsStartWithDown = true;

							PenDownTime = updownTime;
							//Callback.onUpDown( this, false );
						}
                        else if ( updown == 0x01 )
                        {
							mPrevDotTime = -1;

							if (IsStartWithDown && IsBeforeMiddle && mPrevDot != null)
							{
								MakeUpDot(false);
							}
							else if(!IsStartWithDown && !IsBeforeMiddle)
							{
								Callback.onErrorDetected(this, ErrorType.MissingPenDownPenMove, PenDownTime, null, null);
							}
							else if (!IsBeforeMiddle)
							{
								// 무브없이 다운-업만 들어올 경우 UP dot을 보내지 않음
								Callback.onErrorDetected(this, ErrorType.MissingPenMove, PenDownTime, null, null);
							}

							IsStartWithDown = false;

							PenDownTime = -1;
                        }

						IsBeforeMiddle = false;
						IsStartWithPaperInfo = false;

						mPrevDot = null;
                    }
                    break;

                case Cmd.A_DotIDChange:

					if (IsStartWithDown && IsBeforeMiddle && mPrevDot != null)
					{
						MakeUpDot(false);
					}

					byte[] rb = pk.GetBytes(4);

                    mSectionId = (int)( rb[3] & 0xFF );
                    mOwnerId = ByteConverter.ByteToInt( new byte[] { rb[0], rb[1], rb[2], (byte)0x00 } );
                    mNoteId = pk.GetInt();
                    mPageId = pk.GetInt();

					//IsPrevDotDown = true;

					IsBeforePaperInfo = true;
					IsStartWithPaperInfo = true;

                    break;

                case Cmd.A_PenOnState:

                    pk.Move(8);

                    int STATUS = pk.GetByteToInt();

                    int FORCE_MAX = pk.GetByteToInt();

                    MaxForce = (short)FORCE_MAX;

                    string SW_VER = pk.GetString( 5 );

                    if ( STATUS == 0x00 )
                    {
                        SendPenOnOffData();
                        Clean();
                    }
                    else if ( STATUS == 0x01 )
                    {
                        Reset();

                        SendPenOnOffData();
                        SendRTCData();
						needToInputDefaultPassword = true;
                        mOfflineworker.PenMaxForce = FORCE_MAX;
                        Callback.onConnected( this, FORCE_MAX, SW_VER );
                    }

                    break;

                case Cmd.A_RTCsetResponse:
                    break;

                case Cmd.A_PenStatusResponse:

                    if ( !Authenticated )
                    {
                        Authenticated = true;
						needToInputDefaultPassword = false;
                        Callback.onPenAuthenticated( this );
                    }

                    pk.Move( 2 );

                    int stat_timezone = pk.GetInt();
                    long stat_timetick = pk.GetLong();
                    int stat_forcemax = pk.GetByteToInt();
                    int stat_battery = pk.GetByteToInt();
                    int stat_usedmem = pk.GetByteToInt();
                    int stat_pencolor = pk.GetInt();

                    bool stat_autopower = pk.GetByteToInt() == 2 ? false : true;
                    bool stat_accel = pk.GetByteToInt() == 2 ? false : true;
                    bool stat_hovermode = pk.GetByteToInt() == 2 ? false : true;
                    bool stat_beep = pk.GetByteToInt() == 2 ? false : true;

                    short stat_autoshutdowntime = pk.GetShort();
                    short stat_pensensitivity = pk.GetShort();

					string model_name = string.Empty;
					if (pk.CheckMoreData())
					{
						int model_name_length = pk.GetByte();
						model_name = pk.GetString(model_name_length);
					}

                    Callback.onReceivedPenStatus( this, stat_timezone, stat_timetick, stat_forcemax, stat_battery, stat_usedmem, stat_pencolor, stat_autopower, stat_accel, stat_hovermode, stat_beep, stat_autoshutdowntime, stat_pensensitivity, model_name );

                    break;

                // 오프라인 데이터 크기,갯수 전송
                case Cmd.A_OfflineDataInfo:

                    mOfflineTotalFileCount = pk.GetInt();
                    mOfflineTotalDataSize = pk.GetInt();

                    System.Console.WriteLine( "[PenCommCore] A_OfflineDataInfo : {0}, {1}", mOfflineTotalFileCount, mOfflineTotalDataSize );

                    Callback.onStartOfflineDownload( this );

                    IsStartOfflineTask = true;

                    break;

                // 오프라인 전송 최종 결과 응답
                case Cmd.A_OfflineResultResponse:

                    int result = pk.GetByteToInt();

                    //System.Console.WriteLine( "[PenCommCore] A_OfflineDataResponse : {0}", result );

                    IsStartOfflineTask = false;

                    Callback.onFinishedOfflineDownload( this, result == 0x00 ? false : true );

                    mOfflineworker.onFinishDownload();

					mOfflineRcvDataSize = 0;

                    break;

                // 오프라인 파일 정보
                case Cmd.A_OfflineFileInfo:

                    mOfflineFileName = pk.GetString(128);
                    mOfflineFileSize = pk.GetInt();
                    mOfflinePacketCount = pk.GetShort();
                    mOfflinePacketSize = pk.GetShort();

                    System.Console.WriteLine( "[PenCommCore] offline file transfer is started ( name : " + mOfflineFileName + ", size : " + mOfflineFileSize + ", packet_qty : " + mOfflinePacketCount + ", packet_size : " + mOfflinePacketSize + " )" );

                    mOfflineDataBuilder = null;
                    mOfflineDataBuilder = new OfflineDataSerializer( mOfflineFileName, mOfflinePacketCount, mOfflineFileName.Contains( ".zip" ) ? true : false );

                    SendOfflineInfoResponse();

                    break;

                // 오프라인 파일 조각 전송
                case Cmd.A_OfflineChunk:

                    int index = pk.GetShort();
                    
                    // 체크섬 필드
                    byte cs = pk.GetByte();

                    // 체크섬 계산
                    byte calcChecksum = pk.GetChecksum();

                    // 오프라인 데이터
                    byte[] data = pk.GetBytes();

                    // 체크섬이 틀리거나, 카운트, 사이즈 정보가 맞지 않으면 버린다.
                    if ( cs == calcChecksum && mOfflinePacketCount > index && mOfflinePacketSize >= data.Length )
                    {
                        mOfflineDataBuilder.Put( data, index );

                        // 만약 Chunk를 다 받았다면 offline data를 처리한다.
                        if ( mOfflinePacketCount == mOfflineDataBuilder.chunks.Count )
                        {
                            string output = mOfflineDataBuilder.MakeFile();

                            if ( output != null )
                            {
                                SendOfflineChunkResponse( (short)index );
                                mOfflineworker.onCreateFile( mOfflineDataBuilder.sectionId, mOfflineDataBuilder.ownerId, mOfflineDataBuilder.noteId, output );
                            }

                            mOfflineDataBuilder = null;
                        }
                        else
                        {
                            SendOfflineChunkResponse( (short)index );
                        }

                        mOfflineRcvDataSize += data.Length;

                        if ( mOfflineTotalDataSize > 0 )
                        {
                            System.Console.WriteLine( "[PenCommCore] mOfflineRcvDataSize : " + mOfflineRcvDataSize);

                            Callback.onUpdateOfflineDownload( this, mOfflineTotalDataSize, mOfflineRcvDataSize );
                        }
                    }
                    else
                    {
                        System.Console.WriteLine( "[PenCommCore] offline data file verification failed ( index : " + index + " )" );
                    }

                    break;

                case Cmd.A_UsingNoteNotifyResponse:
                    {
                        bool accepted = pk.GetByteToInt() == 0x01;

                        Callback.onAvailableNoteAccepted(this, accepted);
                    }
                    break;

                case Cmd.A_OfflineNoteListResponse:
                    {
                        int status = pk.GetByteToInt();

                        byte[] rxb = pk.GetBytes( 4 );

                        int section = (int)( rxb[3] & 0xFF );

                        int owner = ByteConverter.ByteToInt( new byte[] { rxb[0], rxb[1], rxb[2], (byte)0x00 } );

                        int noteCnt = pk.GetByteToInt();

                        for ( int i = 0; i < noteCnt; i++ )
                        {
                            int note = pk.GetInt();
                            mOfflineNotes.Add( new OfflineDataInfo( section, owner, note ) );
                        }

                        if ( status == 0x01 )
                        {
                            OfflineDataInfo[] array = mOfflineNotes.ToArray();

                            Callback.onOfflineDataList( this, array );
                            mOfflineNotes.Clear();
                        }
                        else
                        {
                            Callback.onOfflineDataList( this, new OfflineDataInfo[0] );
                        }
                    }
                    break;

                case Cmd.A_OfflineDataRemoveResponse:
                    //System.Console.WriteLine( "[PenCommCore] CMD.A_OfflineDataRemoveResponse" );
                    break;

                case Cmd.A_PasswordRequest:
                    {
                        int countRetry = pk.GetByteToInt();
                        int countReset = pk.GetByteToInt();

                        System.Console.WriteLine( "[PenCommCore] A_PasswordRequest ( " + countRetry + " / " + countReset + " )" );

						if (needToInputDefaultPassword)
						{
							_ReqInputPassword(DEFAULT_PASSWORD);
							needToInputDefaultPassword = false;
						}
						else 
							Callback.onPenPasswordRequest(this, countRetry, countReset);
					}
					break;


                case Cmd.A_PasswordSetResponse:
                    {
                        int setResult = pk.GetByteToInt();

						//System.Console.WriteLine( "[PenCommCore] A_PasswordSetResponse => " + setResult );
						if (setResult == 0x00)
							needToInputDefaultPassword = true;

                        Callback.onPenPasswordSetUpResponse( this, setResult == 0x00 ? true : false );
                    }
                    break;

                case Cmd.A_PenSensitivityResponse:
                case Cmd.A_AutoShutdownTimeResponse:
                case Cmd.A_AutoPowerOnResponse:
                case Cmd.A_BeepSetResponse:
                case Cmd.A_PenColorSetResponse:
                case Cmd.A_HoverOnOffResponse:
                    ResPenSetup( (Cmd)pk.Cmd, pk.GetByteToInt() == 0x01 );
                    break;

                case Cmd.A_PenSWUpgradeRequest:

                    short idx = pk.GetShort();

                    ResponseChunkRequest( idx );

                    break;

                case Cmd.A_PenSWUpgradeStatus:
                    {
                        int upgStatus = pk.GetByteToInt();

                        if ( upgStatus == 0x02 )
                        {
                            return;
                        }

                        Callback.onReceivedFirmwareUpdateResult( this, upgStatus == 0x01 );
                        mFwChunk = null;
                    }
                    break;
            }
        }

        private void ResPenSetup( Cmd cmd, bool result )
        {
            switch( cmd )
            {
                case Cmd.A_PenSensitivityResponse:
                    Callback.onPenSensitivitySetUpResponse( this, result );
                    break;

                case Cmd.A_AutoShutdownTimeResponse:
                    Callback.onPenAutoShutdownTimeSetUpResponse( this, result );
                    break;

                case Cmd.A_AutoPowerOnResponse:
                    Callback.onPenAutoPowerOnSetUpResponse( this, result );
                    break;

                case Cmd.A_BeepSetResponse:
                    Callback.onPenBeepSetUpResponse( this, result );
                    break;

                case Cmd.A_PenColorSetResponse:
                    Callback.onPenColorSetUpResponse( this, result );
                    break;

                case Cmd.A_HoverOnOffResponse:
                    Callback.onPenHoverSetUpResponse(this, result);
                    break;
            }
        }

		private void MakeUpDot(bool isError = true)
		{
			if (isError)
			{
				var errorDot = mPrevDot.Clone();
				errorDot.DotType = DotTypes.PEN_ERROR;
				Callback.onErrorDetected(this, ErrorType.MissingPenUp, PenDownTime, errorDot, null);
			}

			var udot = mPrevDot.Clone();
			udot.DotType = DotTypes.PEN_UP;
			ProcessDot(udot);
		}

		private void ProcessDot(Dot dot, object obj = null)
        {
			dotFilterForPage.Put(dot, obj);
        }

        private Stroke curStroke;

        private void SendDotReceiveEvent(Dot dot, object obj)
		{
            if (curStroke == null || dot.DotType == DotTypes.PEN_DOWN)
            {
                curStroke = new Stroke(dot.Section, dot.Owner, dot.Note, dot.Page);
            }

            curStroke.Add(dot);

            Callback.onReceiveDot(this, dot);

            if (dot.DotType == DotTypes.PEN_UP && MetadataManager != null)
            {
                var symbols = MetadataManager.FindApplicableSymbols(curStroke);

                if (symbols != null && symbols.Count > 0)
                {
                    Callback.onSymbolDetected(this, symbols);
                }
            }
        }

		private void SendPenOnOffData()
        {
            ByteUtil bf = new ByteUtil();

            bf.Put( (byte)0xC0 )
              .Put( (byte)Cmd.P_PenOnResponse )
              .PutShort( 9 )
              .PutLong( Time.GetUtcTimeStamp() )
              .Put( (byte)0x00 )
              .Put( (byte)0xC1 );

            Write( bf.ToArray() );

            bf = null;
        }

        private void SendRTCData()
        {
            ByteUtil bf = new ByteUtil();

            bf.Put( (byte)0xC0 )
              .Put( (byte)Cmd.P_RTCset )
              .PutShort( 12 )
              .PutLong( Time.GetUtcTimeStamp() )
              .PutInt( Time.GetLocalTimeOffset() )
              .Put( (byte)0xC1 );

            Write( bf.ToArray() );

            bf = null;
        }

        private void SendOfflineInfoResponse()
        {
            ByteUtil bf = new ByteUtil();

            bf.Put( (byte)0xC0 )
              .Put( (byte)Cmd.P_OfflineFileInfoResponse )
              .PutShort( 2 )
              .PutShort( 1 )
              .Put( (byte)0xC1 );

            Write( bf.ToArray() );

            bf = null;
        }

        private void SendOfflineChunkResponse( short index )
        {
            ByteUtil bf = new ByteUtil();

            bf.Put( (byte)0xC0 )
              .Put( (byte)Cmd.P_OfflineChunkResponse )
              .PutShort( 2 )
              .PutShort( index )
              .Put( (byte)0xC1 );

            Write( bf.ToArray() );

            bf = null;
        }

        /// <summary>
        /// Sets the available paper type
        /// </summary>
        /// <param name="section">The Section Id of the paper</param>
        /// <param name="owner">The Owner Id of the paper</param>
        /// <param name="note">The Note Id of the paper</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqAddUsingNote( int section, int owner, int note )
        {
            ArrayList alnoteIds = new ArrayList();
            alnoteIds.Add( note );

            return SendAddUsingNote( section, owner, alnoteIds );
        }

		public bool ReqAddUsingNote(int[] section, int[] owner)
		{
			for (int i = 0; i < section.Length; ++i)
			{
				ReqAddUsingNote(section[i], owner[i]);
			}
			return true;
		}

		/// <summary>
		/// Sets the available notebook type
		/// </summary>
		/// <param name="section">The Section Id of the paper</param>
		/// <param name="owner">The Owner Id of the paper</param>
		/// <returns>true if the request is accepted; otherwise, false.</returns>
		public bool ReqAddUsingNote( int section, int owner )
        {
            byte[] ownerByte = ByteConverter.IntToByte( owner );

            ByteUtil bf = new ByteUtil();

            bf.Put( (byte)0xC0 )
              .Put( (byte)Cmd.P_UsingNoteNotify )
              .PutShort( 6 )
              .Put( (byte)2 )
              .Put( (byte)1 )
              .Put( ownerByte[0] )
              .Put( ownerByte[1] )
              .Put( ownerByte[2] )
              .Put( (byte)section )
              .Put( (byte)0xC1 );

            bool result = Write( bf.ToArray() );

            return result;
        }

        /// <summary>
        /// Sets the available notebook type
        /// </summary>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqAddUsingNote()
        {
            ByteUtil bf = new ByteUtil();

            bf.Put( (byte)0xC0 )
              .Put( (byte)Cmd.P_UsingNoteNotify )
              .PutShort( 2 )
              .Put( (byte)3 )
              .Put( (byte)0 )
              .Put( (byte)0xC1 );

            bool result = Write( bf.ToArray() );

            bf.Clear();
            bf = null;

            return result;
        }

        /// <summary>
        /// Sets the available notebook type
        /// </summary>
        /// <param name="section">The Section Id of the paper</param>
        /// <param name="owner">The Owner Id of the paper</param>
        /// <param name="notes">The array of Note Id list</param>
        public void ReqAddUsingNote( int section, int owner, int[] notes )
        {
			if (notes == null || notes.Length == 0)
				SendAddUsingNote(section, owner);
			else
			{
				ArrayList alnoteIds = new ArrayList();

				for (int i = 0; i < notes.Length; i++)
				{
					alnoteIds.Add(notes[i]);

					if (i > 0 && i % 8 == 0)
					{
						SendAddUsingNote(section, owner, alnoteIds);
						alnoteIds.Clear();
					}
				}

				if (alnoteIds.Count > 0)
				{
					SendAddUsingNote(section, owner, alnoteIds);
					alnoteIds.Clear();
				}
			}
		}

		private bool SendAddUsingNote( int sectionId, int ownerId, ArrayList noteIds )
        {
            byte[] ownerByte = ByteConverter.IntToByte( ownerId );

            short length = (short)( 6 + ( noteIds.Count * 4 ) );

            ByteUtil bf = new ByteUtil();

            bf.Put( (byte)0xC0 )
              .Put( (byte)Cmd.P_UsingNoteNotify )
              .PutShort( length )
              .Put( (byte)1 )
              .Put( (byte)noteIds.Count )
              .Put( ownerByte[0] )
              .Put( ownerByte[1] )
              .Put( ownerByte[2] )
              .Put( (byte)sectionId );

            foreach ( int item in noteIds )
            {
                bf.PutInt( item );
            }

            bf.Put( (byte)0xC1 );

            bool result = Write( bf.ToArray() );

            bf.Clear();
            bf = null;

            return result;
        }

		private bool SendAddUsingNote(int sectionId, int ownerId)
		{
			byte[] ownerByte = ByteConverter.IntToByte(ownerId);

			short length = 42;

			ByteUtil bf = new ByteUtil();

			bf.Put((byte)0xC0)
			  .Put((byte)Cmd.P_UsingNoteNotify)
			  .PutShort(length)
			  .Put((byte)2)
			  .Put((byte)1)
			  .Put(ownerByte[0])
			  .Put(ownerByte[1])
			  .Put(ownerByte[2])
			  .Put((byte)sectionId)
			  .PutNull(36)
			  .Put((byte)0xC1);

            bool result = Write( bf.ToArray() );

			bf.Clear();
			bf = null;

			return true;
		}


		/// <summary>
		/// Requests the list of Offline data.
		/// </summary>
		/// <returns>true if the request is accepted; otherwise, false.</returns>
		public bool ReqOfflineDataList()
        {
            ByteUtil bf = new ByteUtil();

            bf.Put( (byte)0xC0 )
              .Put( (byte)Cmd.P_OfflineNoteList )
              .PutShort( 1 )
              .Put( (byte)0x00 )
              .Put( (byte)0xC1 );

            bool result = Write( bf.ToArray() );

            bf.Clear();
            bf = null;

            return result;
        }

        /// <summary>
        /// Requests the transmission of data
        /// </summary>
        /// <param name="note">A OfflineDataInfo that specifies the information for the offline data.</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqOfflineData( OfflineDataInfo note )
        {
			ulong freeCapacity = 0;
			if (GetAvailableCapacity(ref freeCapacity))
			{
				if (freeCapacity < 50) // 50MB
					return false;
			}

			mOfflineworker.Put( note );

            return true;
        }

        /// <summary>
        /// Requests the transmission of data
        /// </summary>
        /// <param name="notes">A OfflineDataInfo that specifies the information for the offline data.</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqOfflineData( OfflineDataInfo[] notes )
        {
			ulong freeCapacity = 0;
			if (GetAvailableCapacity(ref freeCapacity))
			{
				if (freeCapacity < 50) // 50MB
					return false;
			}

			mOfflineworker.Put( notes );

            return true;
        }

        private bool SendReqOfflineData( int sectionId, int ownerId, int noteId )
        {
            byte[] ownerByte = ByteConverter.IntToByte( ownerId );

            short length = (short)( 5 + 40 );

            ByteUtil bf = new ByteUtil();

            bf.Put( (byte)0xC0 )
              .Put( (byte)Cmd.P_OfflineDataRequest )
              .PutShort( length )
              .Put( ownerByte[0] )
              .Put( ownerByte[1] )
              .Put( ownerByte[2] )
              .Put( (byte)sectionId )
              .Put( (byte)1 )
              .PutInt( noteId )
			  .PutNull(36)
              .Put( (byte)0xC1 );

            bool result = Write( bf.ToArray() );

            bf.Clear();
            bf = null;

            return result;
        }
        
        /// <summary>
        /// Request to remove offline data in device.
        /// </summary>
        /// <param name="section">The Section Id of the paper</param>
        /// <param name="owner">The Owner Id of the paper</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqRemoveOfflineData( int section, int owner )
        {
            byte[] ownerByte = ByteConverter.IntToByte( owner );

            ByteUtil bf = new ByteUtil();

            bf.Put( (byte)0xC0 )
              .Put( (byte)Cmd.P_OfflineDataRemove )
              .PutShort( 12 )
              .Put( ownerByte[0] )
              .Put( ownerByte[1] )
              .Put( ownerByte[2] )
              .Put( (byte)section )
              .Put( (byte)0x00 )
              .Put( (byte)0x00 )
              .Put( (byte)0x00 )
              .Put( (byte)0x00 )
              .Put( (byte)0x00 )
              .Put( (byte)0x00 )
              .Put( (byte)0x00 )
              .Put( (byte)0x00 )
              .Put( (byte)0xC1 );

            bool result = Write( bf.ToArray() );

            bf.Clear();
            bf = null;

            return result;
        }

        /// <summary>
        /// Request the status of pen.
        /// If you requested, you can receive result by PenCommV1Callbacks.onReceivedPenStatus method.
        /// </summary>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqPenStatus()
        {
            ByteUtil bf = new ByteUtil();
            bf.Put( (byte)0xC0 )
              .Put( (byte)Cmd.P_PenStatusRequest )
              .PutShort( 0 )
              .Put( (byte)0xC1 );

            bool result = Write( bf.ToArray() );

            bf.Clear();
            bf = null;

            return result;
        }

        /// <summary>
        /// Input password if device is locked.
        /// </summary>
        /// <param name="password">Specifies the password for authentication. Password is a string</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqInputPassword( string password )
        {
			if (password == null)
				return false;

			if (password.Equals(DEFAULT_PASSWORD))
				return false;

            byte[] bStrByte = Encoding.UTF8.GetBytes( password );

            ByteUtil bf = new ByteUtil();
            bf.Put( (byte)0xC0 )
              .Put( (byte)Cmd.P_PasswordResponse )
              .PutShort( 16 )
              .Put( bStrByte, 16 )
              .Put( (byte)0xC1 );

            bool result = Write( bf.ToArray() );

            bf.Clear();
            bf = null;

            return result;
        }
        private bool _ReqInputPassword( string password )
        {
			if (password == null)
				return false;

            byte[] bStrByte = Encoding.UTF8.GetBytes( password );

            ByteUtil bf = new ByteUtil();
            bf.Put( (byte)0xC0 )
              .Put( (byte)Cmd.P_PasswordResponse )
              .PutShort( 16 )
              .Put( bStrByte, 16 )
              .Put( (byte)0xC1 );

            bool result = Write( bf.ToArray() );

            bf.Clear();
            bf = null;

            return result;
        }

        /// <summary>
        /// Change the password of device.
        /// </summary>
        /// <param name="oldPassword">Current password</param>
        /// <param name="newPassword">New password</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqSetUpPassword( string oldPassword, string newPassword )
        {
			if (oldPassword == null || newPassword == null)
				return false;

			if (oldPassword.Equals(DEFAULT_PASSWORD))
				return false;
			if (newPassword.Equals(DEFAULT_PASSWORD))
				return false;

			if (oldPassword.Equals(string.Empty))
				oldPassword = DEFAULT_PASSWORD;
			if (newPassword.Equals(string.Empty))
				newPassword = DEFAULT_PASSWORD;

            byte[] oPassByte = Encoding.UTF8.GetBytes( oldPassword );
            byte[] nPassByte = Encoding.UTF8.GetBytes( newPassword );

            ByteUtil bf = new ByteUtil();
            bf.Put( (byte)0xC0 )
              .Put( (byte)Cmd.P_PasswordSet )
              .PutShort( 32 )
              .Put( oPassByte, 16 )
              .Put( nPassByte, 16 )
              .Put( (byte)0xC1 );

            bool result = Write( bf.ToArray() );

            bf.Clear();
            bf = null;

            return result;
        }

        /// <summary>
        /// Sets the value of the pen's sensitivity property that controls the force sensor of pen.
        /// </summary>
        /// <param name="level">the value of sensitivity. (0~4, 0 means maximum sensitivity)</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqSetupPenSensitivity( short level )
        {
            ByteUtil bf = new ByteUtil();

            bf.Put( (byte)0xC0 )
              .Put( (byte)Cmd.P_PenSensitivity )
              .PutShort( 2 )
              .PutShort( level )
              .Put( (byte)0xC1 );

            bool result = Write( bf.ToArray() );

            bf.Clear();
            bf = null;

            return result;
        }

        /// <summary>
        /// Sets the value of the auto shutdown time property that if pen stay idle, shut off the pen.
        /// </summary>
        /// <param name="minute">minute of maximum idle time, staying power on (0~)</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqSetupPenAutoShutdownTime( short minute )
        {
            ByteUtil bf = new ByteUtil();

            bf.Put( (byte)0xC0 )
              .Put( (byte)Cmd.P_AutoShutdownTime )
              .PutShort( 2 )
              .PutShort( minute )
              .Put( (byte)0xC1 );

            bool result = Write( bf.ToArray() );

            bf.Clear();
            bf = null;

            return result;
        }

        /// <summary>
        /// Sets the status of the beep property.
        /// </summary>
        /// <param name="enable">true if you want to listen sound of pen, otherwise false.</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqSetupPenBeep( bool enable )
        {
            ByteUtil bf = new ByteUtil();

            bf.Put( (byte)0xC0 )
              .Put( (byte)Cmd.P_BeepSet )
              .PutShort( 1 )
              .Put( (byte)( enable ? 1 : 0 ) )
              .Put( (byte)0xC1 );

            bool result = Write( bf.ToArray() );

            bf.Clear();
            bf = null;

            return result;
        }

        /// <summary>
        /// Sets the status of the auto power on property that if write the pen, turn on when pen is down.
        /// </summary>
        /// <param name="seton">true if you want to use, otherwise false.</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqSetupPenAutoPowerOn( bool enable )
        {
            ByteUtil bf = new ByteUtil();

            bf.Put( (byte)0xC0 )
              .Put( (byte)Cmd.P_AutoPowerOnSet )
              .PutShort( 1 )
              .Put( (byte)( enable ? 1 : 0 ) )
              .Put( (byte)0xC1 );

            bool result = Write( bf.ToArray() );

            bf.Clear();
            bf = null;

            return result;
        }

        /// <summary>
        /// Sets the color of pen ink.
        /// If you want to change led color of pen, you should choose one among next preset values.
        /// 
        /// violet = 0x9C3FCD
        /// blue = 0x3c6bf0
        /// gray = 0xbdbdbd
        /// yellow = 0xfbcb26
        /// pink = 0xff2084
        /// mint = 0x27e0c8
        /// red = 0xf93610
        /// black = 0x000000
        /// </summary>
        /// <param name="rgbcolor">integer type color formatted 0xRRGGBB</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqSetupPenColor( int rgbcolor )
        {
            byte[] cbyte = ByteConverter.IntToByte( rgbcolor );

            byte[] nbyte = new byte[] { cbyte[0], cbyte[1], cbyte[2], (byte)0x01 };
            
            ByteUtil bf = new ByteUtil();

            bf.Put( (byte)0xC0 )
              .Put( (byte)Cmd.P_PenColorSet )
              .PutShort( 4 )
              .Put( nbyte, 4 )
              .Put( (byte)0xC1 );

            bool result = Write( bf.ToArray() );

            bf.Clear();
            bf = null;

            return result;
        }

        /// <summary>
        /// Sets the hover mode.
        /// </summary>
        /// <param name="enable">true if you want to enable hover mode, otherwise false.</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        private bool ReqSetupHoverMode( bool enable )
        {
            ByteUtil bf = new ByteUtil();

            bf.Put( (byte)0xC0 )
              .Put( (byte)Cmd.P_HoverOnOff )
              .PutShort( 1 )
              .Put( (byte)( enable ? 1 : 0 ) )
              .Put( (byte)0xC1 );

            bool result = Write( bf.ToArray() );

            bf.Clear();
            bf = null;

            return result;
        }

        /// <summary>
        /// Requests the firmware installation
        /// </summary>
        /// <param name="filepath">absolute path of firmware file</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqPenSwUpgrade( string filepath )
        {
            if ( IsUploading )
            {
                System.Console.WriteLine( "[FileUploadWorker] Upgrade task is still excuting." );
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

            byte[] StrByte = Encoding.UTF8.GetBytes( "\\N2._v_" );

            System.Console.WriteLine( "[FileUploadWorker] file upload => filesize : {0}, packet count : {1}, packet size {2}", file_size, chunk_count, chunk_size );

            ByteUtil bf = new ByteUtil();

            bf.Put( (byte)0xC0 )
              .Put( (byte)Cmd.P_PenSWUpgradeCommand )
              .PutShort( 136 )
              .Put( StrByte, 128 )
              .PutInt( file_size )
              .PutShort( chunk_count )
              .PutShort( chunk_size )
              .Put( (byte)0xC1 );

            Write( bf.ToArray() );

            bf = null;

            return true;
        }

        private void ResponseChunkRequest( short index )
        {
			byte[] data = null;

			if (mFwChunk == null || (data = mFwChunk.Get(index)) == null)
            {
				IsUploading = false;
                return;
            }

            byte checksum = mFwChunk.GetChecksum( index );

            short dataLength = (short)( data.Length + 3 );

            ByteUtil bf = new ByteUtil();
            bf.Put( (byte)0xC0 )
              .Put( (byte)Cmd.P_PenSWUpgradeResponse )
              .PutShort( dataLength )
              .PutShort( index )
              .Put( checksum )
              .Put( data )
              .Put( (byte)0xC1 );
            Write( bf.ToArray() );

            Callback.onReceivedFirmwareUpdateStatus( this, mFwChunk.GetChunkLength(), (int)index + 1 );
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

        private bool GetAvailableCapacity(ref ulong capacity)
		{
			ulong sectionPerCluster = 0, bytesPerSection = 0, freeClusters = 0, totalClusters = 0;
			var ret = GetDiskFreeSpace(System.IO.Directory.GetCurrentDirectory(), ref sectionPerCluster, ref bytesPerSection, ref freeClusters, ref totalClusters);
			capacity = ((sectionPerCluster * bytesPerSection * freeClusters) / 1024) / 1024;
			return ret;
		}
    }
}
