using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Neosmartpen.Net.Support;
using Windows.Storage;
using Windows.Storage.Streams;
using Neosmartpen.Net.Filter;
using System.Threading;
using Windows.Foundation;

namespace Neosmartpen.Net
{
    public class PenClientParserV1 : IPenClientParser, OfflineWorkResponseHandler
    {
        public enum Cmd : byte
        {
            A_PenOnState = 0x01,
            P_PenOnResponse = 0x02,

            P_RTCset = 0x03,
            A_RTCsetResponse = 0x04,

            P_HoverOnOff = 0x05,
            A_HoverOnOffResponse = 0x06,

            P_ForceCalibrate = 0x07,
            A_ForceCalibrateResponse = 0x08,

            P_AutoShutdownTime = 0x09,
            A_AutoShutdownTimeResponse = 0x0A,
            P_PenSensitivity = 0x2C,
            A_PenSensitivityResponse = 0x2D,
            P_PenColorSet = 0x28,
            A_PenColorSetResponse = 0x29,
            P_AutoPowerOnSet = 0x2A,
            A_AutoPowerOnResponse = 0x2B,
            P_BeepSet = 0x2E,
            A_BeepSetResponse = 0x2F,

            P_UsingNoteNotify = 0x0B,
            A_UsingNoteNotifyResponse = 0x0C,

            A_PasswordRequest = 0x0D,
            P_PasswordResponse = 0x0E,
            P_PasswordSet = 0x0F,
            A_PasswordSetResponse = 0x10,

            A_DotData = 0x11,
            A_DotUpDownData = 0x13,
            P_DotUpDownResponse = 0x14,
            A_DotIDChange = 0x15,
            A_DotUpDownDataNew = 0x16,

            P_PenStatusRequest = 0x21,
            A_PenStatusOldResponse = 0x22,
            A_PenStatusResponse = 0x25,

            P_OfflineDataRequest = 0x47,
            A_OfflineDataInfo = 0x49,
            A_OfflineFileInfo = 0x41,
            P_OfflineFileInfoResponse = 0x42,
            A_OfflineChunk = 0x43,
            P_OfflineChunkResponse = 0x44,
            A_OfflineResultResponse = 0x48,
            P_OfflineNoteList = 0x45,
            A_OfflineNoteListResponse = 0x46,
            P_OfflineDataRemove = 0x4A,
            A_OfflineDataRemoveResponse = 0x4B,

            P_PenSWUpgradeCommand = 0x51,
            A_PenSWUpgradeRequest = 0x52,
            P_PenSWUpgradeResponse = 0x53,
            A_PenSWUpgradeStatus = 0x54,

            P_ProfileRequest = 0x61,
            A_ProfileResponse = 0x62
        }

        private readonly int PKT_START = 0xC0;
        private readonly int PKT_END = 0xC1;
        private readonly int PKT_EMPTY = 0x00;
        private readonly int PKT_HEADER_LEN = 3;
        private readonly int PKT_LENGTH_POS1 = 1;
        private readonly int PKT_LENGTH_POS2 = 2;
        private readonly int PKT_MAX_LEN = 8200;

        private readonly string DEFAULT_PASSWORD = "0000";

        //private IPacket mPrevPacket;
        private int mOwnerId = 0, mSectionId = 0, mNoteId = 0, mPageId = 0;
        private long mPrevDotTime = 0;
        //private bool IsPrevDotDown = false;
        private bool IsStartWithDown = false;
        private int mCurrentColor = 0x000000;
        private String mOfflineFileName;
        private long mOfflineFileSize;
        private short mOfflinePacketCount, mOfflinePacketSize;
        private OfflineDataSerializer mOfflineDataBuilder;
        private bool isAuthenticated = false;
        private int mOfflineTotalDataSize = 0, mOfflineTotalFileCount = 0, mOfflineRcvDataSize = 0;
        private List<OfflineDataInfo> mOfflineNotes = new List<OfflineDataInfo>();
        private bool IsStartOfflineTask = false;
        private Chunk mFwChunk;
        private bool IsUploading = false;
        private OfflineWorker mOfflineworker = null;
        private int PenMaxForce = 0;        // 상수로 박아도 될것같지만 혹시 모르니 connection시 받는다.
        private bool needToInputDefaultPassword;
        private FilterForPaper dotFilterForPaper = null;

        private string connectedDeviceName = string.Empty;
        public string protocolVersion;

        public static readonly float PEN_PROFILE_SUPPORT_VERSION_F110 = 1.06f;
        public static readonly float PEN_PROFILE_SUPPORT_VERSION_F110C = 1.06f;
        public static readonly string PEN_MODEL_NAME_F110 = "NWP-F110";
        public static readonly string PEN_MODEL_NAME_F110C = "NWP-F110C";

        private bool isIgnorePenStatus = false;

        public PenClientParserV1()
        {
            //this.PenController = penClient;

            dotFilterForPaper = new FilterForPaper(SendDotReceiveEvent);

            if (mOfflineworker == null)
            {
                mOfflineworker = new OfflineWorker(this);
                mOfflineworker.Startup();
            }
        }

        public void OnDisconnected()
        {
            if (IsStartWithDown && IsBeforeMiddle && mPrevDot != null)
            {
                MakeUpDot();
            }

            mOfflineworker.Reset();

            onDisconnected();
        }

        //public PenController PenController { get; private set; }
        public IPenClient PenClient { get; set; }

        public void onReceiveOfflineStrokes(int totalDataSize, int receiveDataSize, Stroke[] strokes)
        {
            onReceiveOfflineStrokes(new OfflineStrokeReceivedEventArgs(mOfflineTotalDataSize, mOfflineRcvDataSize, strokes));
        }

        public void onRequestDownloadOfflineData(int sectionId, int ownerId, int noteId)
        {
            SendReqOfflineData(sectionId, ownerId, noteId);
        }

        public void onRequestRemoveOfflineData(int sectionId, int ownerId)
        {
            ReqRemoveOfflineData(sectionId, ownerId);
        }
        private void Reset()
        {
            Debug.WriteLine("[PenCommCore] Reset");

            IsStartWithDown = false;
            IsBeforeMiddle = false;
            IsStartWithPaperInfo = false;
            IsBeforePaperInfo = false;

            isAuthenticated = false;

            IsUploading = false;

            IsStartOfflineTask = false;
        }

        private Dot mPrevDot;

        private bool IsBeforeMiddle = false;

        private bool IsStartWithPaperInfo = false;
        private bool IsBeforePaperInfo = false;

        private long PenDownTime = -1;

        public void ParsePacket(Packet packet)
        {
            //Debug.WriteLine("[PenCommCore] ParsePacket : " + packet.Cmd);

            switch ((Cmd)packet.Cmd)
            {
                case Cmd.A_DotData:
                    {
                        long time = packet.GetByteToInt();
                        int x = packet.GetShort();
                        int y = packet.GetShort();
                        int fx = packet.GetByteToInt();
                        int fy = packet.GetByteToInt();
                        int force = packet.GetByteToInt();

                        long timeLong = mPrevDotTime + time;

                        Dot.Builder builder = null;
                        if (PenMaxForce == 0)
                            builder = new Dot.Builder();
                        else builder = new Dot.Builder(PenMaxForce);

                        builder.owner(mOwnerId)
                            .section(mSectionId)
                            .note(mNoteId)
                            .page(mPageId)
                            .timestamp(timeLong)
                            .coord(x + fx * 0.01f, y + fy * 0.01f)
                            .force(force)
                            .color(mCurrentColor);

                        if (!IsStartWithDown)
                        {
                            if (!IsStartWithPaperInfo)
                            {
                                //펜 다운 없이 페이퍼 정보 없고 무브가 오는 현상(다운 - 무브 - 업 - 다운X - 무브)
                                onErrorDetected(new ErrorDetectedEventArgs(ErrorType.MissingPenDown, -1));
                            }
                            else
                            {
                                timeLong = Time.GetUtcTimeStamp();
                                PenDownTime = timeLong;
                                //펜 다운 없이 페이퍼 정보 있고 무브가 오는 현상(다운 - 무브 - 업 - 다운X - 무브)
                                builder.dotType(DotTypes.PEN_ERROR);
                                var errorDot = builder.Build();
                                onErrorDetected(new ErrorDetectedEventArgs(ErrorType.MissingPenDown, errorDot, PenDownTime));
                                IsStartWithDown = true;
                                builder.timestamp(timeLong);
                            }
                        }
                        else if (timeLong < 10000)
                        {
                            // 타임스템프가 10000보다 작을 경우 도트 필터링
                            builder.dotType(DotTypes.PEN_ERROR);
                            var errorDot = builder.Build();
                            onErrorDetected(new ErrorDetectedEventArgs(ErrorType.InvalidTime, errorDot, PenDownTime));
                        }

                        Dot dot = null;

                        if (IsStartWithDown && IsStartWithPaperInfo && IsBeforePaperInfo)
                        {
                            // 펜다운의 경우 시작 도트로 저장
                            // 펜다운 도트는 펜다운 시간으로 한다.
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
                            onErrorDetected(new ErrorDetectedEventArgs(ErrorType.MissingPageChange, PenDownTime));
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
                        // TODO Check
                        long updownTime = packet.GetLong();

                        int updown = packet.GetByteToInt();

                        byte[] cbyte = packet.GetBytes(3);

                        mCurrentColor = ByteConverter.ByteToInt(new byte[] { cbyte[2], cbyte[1], cbyte[0], (byte)0xFF });

                        if (updown == 0x00)
                        {
                            // 펜 다운 일 경우 Start Dot의 timestamp 설정
                            mPrevDotTime = updownTime;

                            if (IsBeforeMiddle && mPrevDot != null)
                            {
                                MakeUpDot();
                            }

                            IsStartWithDown = true;

                            PenDownTime = updownTime;
                        }
                        else if (updown == 0x01)
                        {
                            mPrevDotTime = -1;

                            if (IsStartWithDown && IsBeforeMiddle && mPrevDot != null)
                            {
                                MakeUpDot(false);
                            }
                            else if (!IsStartWithDown && !IsBeforeMiddle)
                            {
                                // 다운 무브없이 업만 들어올 경우 UP dot을 보내지 않음
                                onErrorDetected(new ErrorDetectedEventArgs(ErrorType.MissingPenDownPenMove, PenDownTime));
                            }
                            else if (!IsBeforeMiddle)
                            {
                                // 무브없이 다운-업만 들어올 경우 UP dot을 보내지 않음
                                onErrorDetected(new ErrorDetectedEventArgs(ErrorType.MissingPenMove, PenDownTime));
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

                    // 미들도트 중에 페이지가 바뀐다면 강제로 펜업을 만들어 준다.
                    if (IsStartWithDown && IsBeforeMiddle && mPrevDot != null)
                    {
                        MakeUpDot(false);
                    }

                    byte[] rb = packet.GetBytes(4);

                    mSectionId = (int)(rb[3] & 0xFF);
                    mOwnerId = ByteConverter.ByteToInt(new byte[] { rb[0], rb[1], rb[2], (byte)0x00 });
                    mNoteId = packet.GetInt();
                    mPageId = packet.GetInt();

                    //IsPrevDotDown = true;

                    IsBeforePaperInfo = true;
                    IsStartWithPaperInfo = true;

                    break;

                case Cmd.A_PenOnState:
                    packet.Move(8);

                    int STATUS = packet.GetByteToInt();

                    int FORCE_MAX = packet.GetByteToInt();

                    string SW_VER = packet.GetString(5);
                    protocolVersion = SW_VER;

                    if (STATUS == 0x00)
                    {
                        SendPenOnOffData();
                    }
                    else if (STATUS == 0x01)
                    {
                        Reset();

                        SendPenOnOffData();
                        SendRTCData();

                        isIgnorePenStatus = true;
                        needToInputDefaultPassword = true;
                        onConnected(new ConnectedEventArgs(SW_VER, FORCE_MAX));
                        PenMaxForce = FORCE_MAX;
                        mOfflineworker.PenMaxForce = FORCE_MAX;
                    }

                    break;

                case Cmd.A_RTCsetResponse:
                    break;

                case Cmd.A_PenStatusResponse:
                    if (needToInputDefaultPassword)
                        break;

                    if (!isAuthenticated)
                    {
                        isAuthenticated = true;
                        onPenAuthenticated();
                    }

                    packet.Move(2);

                    int stat_timezone = packet.GetInt();
                    long stat_timetick = packet.GetLong();
                    int stat_forcemax = packet.GetByteToInt();
                    int stat_battery = packet.GetByteToInt();
                    int stat_usedmem = packet.GetByteToInt();
                    int stat_pencolor = packet.GetInt();

                    bool stat_autopower = packet.GetByteToInt() == 2 ? false : true;
                    bool stat_accel = packet.GetByteToInt() == 2 ? false : true;
                    bool stat_hovermode = packet.GetByteToInt() == 2 ? false : true;
                    bool stat_beep = packet.GetByteToInt() == 2 ? false : true;

                    short stat_autoshutdowntime = packet.GetShort();
                    short stat_pensensitivity = packet.GetShort();

                    connectedDeviceName = string.Empty;
                    if (packet.CheckMoreData())
                    {
                        int model_name_length = packet.GetByte();
                        connectedDeviceName = packet.GetString(model_name_length);
                    }

                    onReceivePenStatus(new PenStatusReceivedEventArgs(stat_timezone, stat_timetick, stat_forcemax, stat_battery, stat_usedmem, stat_pencolor, stat_autopower, stat_accel, stat_hovermode, stat_beep, stat_autoshutdowntime, stat_pensensitivity, connectedDeviceName));
                    break;

                // 오프라인 데이터 크기,갯수 전송
                case Cmd.A_OfflineDataInfo:

                    mOfflineTotalFileCount = packet.GetInt();
                    mOfflineTotalDataSize = packet.GetInt();

                    Debug.WriteLine("[PenCommCore] A_OfflineDataInfo : {0}, {1}", mOfflineTotalFileCount, mOfflineTotalDataSize);

                    onStartOfflineDownload();

                    IsStartOfflineTask = true;

                    break;

                // 오프라인 전송 최종 결과 응답
                case Cmd.A_OfflineResultResponse:

                    int result = packet.GetByteToInt();

                    Debug.WriteLine("[PenCommCore] A_OfflineDataResponse : {0}", result);

                    IsStartOfflineTask = false;

                    onFinishedOfflineDownload(new SimpleResultEventArgs(result == 0x01));

                    mOfflineworker.onFinishDownload();

                    mOfflineRcvDataSize = 0;

                    break;

                // 오프라인 파일 정보
                case Cmd.A_OfflineFileInfo:

                    mOfflineFileName = packet.GetString(128);
                    mOfflineFileSize = packet.GetInt();
                    mOfflinePacketCount = packet.GetShort();
                    mOfflinePacketSize = packet.GetShort();

                    Debug.WriteLine("[PenCommCore] offline file transfer is started ( name : " + mOfflineFileName + ", size : " + mOfflineFileSize + ", packet_qty : " + mOfflinePacketCount + ", packet_size : " + mOfflinePacketSize + " )");

                    mOfflineDataBuilder = null;
                    mOfflineDataBuilder = new OfflineDataSerializer(mOfflineFileName, mOfflinePacketCount, mOfflineFileName.Contains(".zip") ? true : false);

                    SendOfflineInfoResponse();

                    break;

                // 오프라인 파일 조각 전송
                case Cmd.A_OfflineChunk:

                    int index = packet.GetShort();

                    // 체크섬 필드
                    byte cs = packet.GetByte();

                    // 체크섬 계산
                    byte calcChecksum = packet.GetChecksum();

                    // 오프라인 데이터
                    byte[] data = packet.GetBytes();

                    // 체크섬이 틀리거나, 카운트, 사이즈 정보가 맞지 않으면 버린다.
                    if (cs == calcChecksum && mOfflinePacketCount > index && mOfflinePacketSize >= data.Length)
                    {
                        mOfflineDataBuilder.Put(data, index);

                        // 만약 Chunk를 다 받았다면 offline data를 처리한다.
                        if (mOfflinePacketCount == mOfflineDataBuilder.chunks.Count)
                        {
                            string output = mOfflineDataBuilder.MakeFile();

                            if (output != null)
                            {
                                SendOfflineChunkResponse((short)index);
                                mOfflineworker.onCreateFile(mOfflineDataBuilder.sectionId, mOfflineDataBuilder.ownerId, mOfflineDataBuilder.noteId, output, mOfflineTotalDataSize, mOfflineRcvDataSize);
                            }

                            mOfflineDataBuilder = null;
                        }
                        else
                        {
                            SendOfflineChunkResponse((short)index);
                        }

                        mOfflineRcvDataSize += data.Length;

                        // TODO Check
                        //if (mOfflineTotalDataSize > 0)
                        //{
                        //	Debug.WriteLine("[PenCommCore] mOfflineRcvDataSize : " + mOfflineRcvDataSize);

                        //	//Callback.onUpdateOfflineDownload(this, mOfflineTotalDataSize, mOfflineRcvDataSize);
                        //}
                    }
                    else
                    {
                        Debug.WriteLine("[PenCommCore] offline data file verification failed ( index : " + index + " )");
                    }

                    break;

                case Cmd.A_UsingNoteNotifyResponse:
                    onAvailableNoteAdded();
                    break;

                case Cmd.A_OfflineNoteListResponse:
                    {
                        int status = packet.GetByteToInt();

                        byte[] rxb = packet.GetBytes(4);

                        int section = (int)(rxb[3] & 0xFF);

                        int owner = ByteConverter.ByteToInt(new byte[] { rxb[0], rxb[1], rxb[2], (byte)0x00 });

                        int noteCnt = packet.GetByteToInt();

                        for (int i = 0; i < noteCnt; i++)
                        {
                            int note = packet.GetInt();
                            mOfflineNotes.Add(new OfflineDataInfo(section, owner, note));
                        }

                        if (status == 0x01)
                        {
                            OfflineDataInfo[] array = mOfflineNotes.ToArray();

                            onReceiveOfflineDataList(new OfflineDataListReceivedEventArgs(array));
                            mOfflineNotes.Clear();
                        }
                        else
                        {
                            onReceiveOfflineDataList(new OfflineDataListReceivedEventArgs(new OfflineDataInfo[0]));
                        }
                    }
                    break;

                case Cmd.A_OfflineDataRemoveResponse:
                    //System.Console.WriteLine( "[PenCommCore] CMD.A_OfflineDataRemoveResponse" );
                    break;

                case Cmd.A_PasswordRequest:
                    {
                        int countRetry = packet.GetByteToInt();
                        int countReset = packet.GetByteToInt();

                        Debug.WriteLine("[PenCommCore] A_PasswordRequest ( " + countRetry + " / " + countReset + " )");

                        if (needToInputDefaultPassword)
                        {
                            _ReqInputPassword(DEFAULT_PASSWORD);
                            needToInputDefaultPassword = false;
                        }
                        else
                            onPenPasswordRequest(new PasswordRequestedEventArgs(countRetry, countReset));
                    }
                    break;


                case Cmd.A_PasswordSetResponse:
                    {
                        int setResult = packet.GetByteToInt();

                        //System.Console.WriteLine( "[PenCommCore] A_PasswordSetResponse => " + setResult );
                        if (setResult == 0x00)
                            needToInputDefaultPassword = true;

                        onPenPasswordSetupResponse(new SimpleResultEventArgs(setResult == 0x00));
                    }
                    break;

                case Cmd.A_PenSensitivityResponse:
                case Cmd.A_AutoShutdownTimeResponse:
                case Cmd.A_AutoPowerOnResponse:
                case Cmd.A_BeepSetResponse:
                case Cmd.A_PenColorSetResponse:
                    ResPenSetup((Cmd)packet.Cmd, packet.GetByteToInt() == 0x01);
                    break;

                case Cmd.A_PenSWUpgradeRequest:

                    short idx = packet.GetShort();

                    Debug.WriteLine("[PenCommCore] A_PenSWUpgradeRequest => " + idx);

                    ResponseChunkRequest(idx);

                    break;

                case Cmd.A_PenSWUpgradeStatus:
                    {
                        int upgStatus = packet.GetByteToInt();

                        if (upgStatus == 0x02)
                        {
                            return;
                        }

                        onReceiveFirmwareUpdateResult(new SimpleResultEventArgs(upgStatus == 0x01));
                        mFwChunk = null;
                    }
                    break;

                #region Pen Profile	
                case Cmd.A_ProfileResponse:
                    {
                        if (packet.Result == 0x00)
                        {
                            string profileName = packet.GetString(8);
                            byte type = packet.GetByte();
                            PenProfileReceivedEventArgs eventArgs = null;
                            if (type == PenProfile.PROFILE_CREATE)
                            {
                                eventArgs = PenProfileCreate(profileName, packet);
                            }
                            else if (type == PenProfile.PROFILE_DELETE)
                            {
                                eventArgs = PenProfileDelete(profileName, packet);
                            }
                            else if (type == PenProfile.PROFILE_INFO)
                            {
                                eventArgs = PenProfileInfo(profileName, packet);
                            }
                            else if (type == PenProfile.PROFILE_READ_VALUE)
                            {
                                eventArgs = PenProfileReadValue(profileName, packet);
                            }
                            else if (type == PenProfile.PROFILE_WRITE_VALUE)
                            {
                                eventArgs = PenProfileWriteValue(profileName, packet);
                            }
                            else if (type == PenProfile.PROFILE_DELETE_VALUE)
                            {
                                eventArgs = PenProfileDeleteValue(profileName, packet);
                            }

                            if (eventArgs != null)
                                onPenProfileReceived(eventArgs);
                            else
                                onPenProfileReceived(new PenProfileReceivedEventArgs(PenProfileReceivedEventArgs.ResultType.Failed));
                        }
                        else
                            onPenProfileReceived(new PenProfileReceivedEventArgs(PenProfileReceivedEventArgs.ResultType.Failed));
                    }
                    break;
                    #endregion
            }
        }

        #region Pen Profile Response
        private PenProfileReceivedEventArgs PenProfileCreate(string profileName, Packet packet)
        {
            byte status = packet.GetByte();
            return new PenProfileCreateEventArgs(profileName, status);
        }

        private PenProfileReceivedEventArgs PenProfileDelete(string profileName, Packet packet)
        {
            byte status = packet.GetByte();
            return new PenProfileDeleteEventArgs(profileName, status);
        }

        private PenProfileReceivedEventArgs PenProfileInfo(string profileName, Packet packet)
        {
            byte status = packet.GetByte();
            var args = new PenProfileInfoEventArgs(profileName, status);
            if (status == 0x00)
            {
                args.TotalSectionCount = packet.GetShort();
                args.SectionSize = packet.GetShort();
                args.UseSectionCount = packet.GetShort();
                args.UseKeyCount = packet.GetShort();
            }
            return args;
        }

        private PenProfileReceivedEventArgs PenProfileReadValue(string profileName, Packet packet)
        {
            int count = packet.GetByte();
            var args = new PenProfileReadValueEventArgs(profileName);
            try
            {
                for (int i = 0; i < count; ++i)
                {
                    var result = new PenProfileReadValueEventArgs.ReadValueResult();
                    result.Key = packet.GetString(16);
                    result.Status = packet.GetByte();
                    int dataSize = packet.GetShort();
                    result.Data = packet.GetBytes(dataSize);
                    args.Data.Add(result);
                }
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.StackTrace);
            }
            return args;
        }
        private PenProfileReceivedEventArgs PenProfileWriteValue(string profileName, Packet packet)
        {
            int count = packet.GetByte();
            var args = new PenProfileWriteValueEventArgs(profileName);
            try
            {
                for (int i = 0; i < count; ++i)
                {
                    var result = new PenProfileWriteValueEventArgs.WriteValueResult();
                    result.Key = packet.GetString(16);
                    result.Status = packet.GetByte();
                    args.Data.Add(result);
                }
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.StackTrace);
            }

            return args;
        }

        private PenProfileReceivedEventArgs PenProfileDeleteValue(string profileName, Packet packet)
        {
            int count = packet.GetByte();
            var args = new PenProfileDeleteValueEventArgs(profileName);

            try
            {
                for (int i = 0; i < count; ++i)
                {
                    var result = new PenProfileDeleteValueEventArgs.DeleteValueResult();
                    result.Key = packet.GetString(16);
                    result.Status = packet.GetByte();
                    args.Data.Add(result);
                }
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.StackTrace);
            }

            return args;
        }
        #endregion

        private void ResPenSetup(Cmd cmd, bool result)
        {
            switch (cmd)
            {
                case Cmd.A_PenSensitivityResponse:
                    onPenSensitivitySetupResponse(new SimpleResultEventArgs(result));
                    break;

                case Cmd.A_AutoShutdownTimeResponse:
                    onPenAutoShutdownTimeSetupResponse(new SimpleResultEventArgs(result));
                    break;

                case Cmd.A_AutoPowerOnResponse:
                    onPenAutoPowerOnSetupResponse(new SimpleResultEventArgs(result));
                    break;

                case Cmd.A_BeepSetResponse:
                    onPenBeepSetupResponse(new SimpleResultEventArgs(result));
                    break;

                case Cmd.A_PenColorSetResponse:
                    onPenColorSetupResponse(new SimpleResultEventArgs(result));
                    break;
            }
        }

        private void MakeUpDot(bool isError = true)
        {
            if (isError)
            {
                var errorDot = mPrevDot.Clone();
                errorDot.DotType = DotTypes.PEN_ERROR;
                onErrorDetected(new ErrorDetectedEventArgs(ErrorType.MissingPenUp, errorDot, PenDownTime));
            }

            var udot = mPrevDot.Clone();
            udot.DotType = DotTypes.PEN_UP;
            ProcessDot(udot);
        }


        private void ProcessDot(Dot dot)
        {
            SendDotReceiveEvent(dot, null);
            //dotFilterForPaper.Put(dot);
        }

        private void SendDotReceiveEvent(Dot dot, object obj)
        {
            onReceiveDot(new DotReceivedEventArgs(dot));
        }

        private void SendPenOnOffData()
        {
            ByteUtil bf = new ByteUtil();

            bf.Put((byte)0xC0)
              .Put((byte)Cmd.P_PenOnResponse)
              .PutShort(9)
              .PutLong(Time.GetUtcTimeStamp())
              .Put((byte)0x00)
              .Put((byte)0xC1);

            PenClient.Write(bf.ToArray());

            bf = null;
        }

        private void SendRTCData()
        {
            ByteUtil bf = new ByteUtil();

            bf.Put((byte)0xC0)
              .Put((byte)Cmd.P_RTCset)
              .PutShort(12)
              .PutLong(Time.GetUtcTimeStamp())
              .PutInt(Time.GetLocalTimeOffset())
              .Put((byte)0xC1);

            PenClient.Write(bf.ToArray());

            bf = null;
        }

        private void SendOfflineInfoResponse()
        {
            ByteUtil bf = new ByteUtil();

            bf.Put((byte)0xC0)
              .Put((byte)Cmd.P_OfflineFileInfoResponse)
              .PutShort(2)
              .PutShort(1)
              .Put((byte)0xC1);

            PenClient.Write(bf.ToArray());

            bf = null;
        }

        private void SendOfflineChunkResponse(short index)
        {
            ByteUtil bf = new ByteUtil();

            bf.Put((byte)0xC0)
              .Put((byte)Cmd.P_OfflineChunkResponse)
              .PutShort(2)
              .PutShort(index)
              .Put((byte)0xC1);

            PenClient.Write(bf.ToArray());

            bf = null;
        }

        /// <summary>
        /// Sets the available paper type
        /// </summary>
        /// <param name="section">The Section Id of the paper</param>
        /// <param name="owner">The Owner Id of the paper</param>
        /// <param name="note">The Note Id of the paper</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqAddUsingNote(int section, int owner, int note)
        {
            List<int> alnoteIds = new List<int>();
            alnoteIds.Add(note);

            return SendAddUsingNote(section, owner, alnoteIds);
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
        public bool ReqAddUsingNote(int section, int owner)
        {
            byte[] ownerByte = ByteConverter.IntToByte(owner);

            ByteUtil bf = new ByteUtil();

            bf.Put((byte)0xC0)
              .Put((byte)Cmd.P_UsingNoteNotify)
              .PutShort(6)
              .Put((byte)2)
              .Put((byte)1)
              .Put(ownerByte[0])
              .Put(ownerByte[1])
              .Put(ownerByte[2])
              .Put((byte)section)
              .Put((byte)0xC1);

            PenClient.Write(bf.ToArray());

            bf.Clear();
            bf = null;

            return true;
        }

        /// <summary>
        /// Sets the available notebook type
        /// </summary>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqAddUsingNote()
        {
            ByteUtil bf = new ByteUtil();

            bf.Put((byte)0xC0)
              .Put((byte)Cmd.P_UsingNoteNotify)
              .PutShort(2)
              .Put((byte)3)
              .Put((byte)0)
              .Put((byte)0xC1);

            PenClient.Write(bf.ToArray());

            bf.Clear();
            bf = null;

            return true;
        }

        /// <summary>
        /// Sets the available notebook type
        /// </summary>
        /// <param name="section">The Section Id of the paper</param>
        /// <param name="owner">The Owner Id of the paper</param>
        /// <param name="notes">The array of Note Id list</param>
        public void ReqAddUsingNote(int section, int owner, int[] notes)
        {
            if (notes == null || notes.Length == 0)
                SendAddUsingNote(section, owner);
            else
            {
                List<int> alnoteIds = new List<int>();

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

        private bool SendAddUsingNote(int sectionId, int ownerId, List<int> noteIds)
        {
            byte[] ownerByte = ByteConverter.IntToByte(ownerId);

            short length = (short)(6 + (noteIds.Count * 4));

            ByteUtil bf = new ByteUtil();

            bf.Put((byte)0xC0)
              .Put((byte)Cmd.P_UsingNoteNotify)
              .PutShort(length)
              .Put((byte)1)
              .Put((byte)noteIds.Count)
              .Put(ownerByte[0])
              .Put(ownerByte[1])
              .Put(ownerByte[2])
              .Put((byte)sectionId);

            foreach (int item in noteIds)
            {
                bf.PutInt(item);
            }

            bf.Put((byte)0xC1);

            PenClient.Write(bf.ToArray());

            bf.Clear();
            bf = null;

            return true;
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

            PenClient.Write(bf.ToArray());

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

            bf.Put((byte)0xC0)
              .Put((byte)Cmd.P_OfflineNoteList)
              .PutShort(1)
              .Put((byte)0x00)
              .Put((byte)0xC1);

            PenClient.Write(bf.ToArray());

            bf.Clear();
            bf = null;

            return true;
        }

        /// <summary>
        /// Requests the transmission of data
        /// </summary>
        /// <param name="note">A OfflineDataInfo that specifies the information for the offline data.</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqOfflineData(OfflineDataInfo note)
        {
            mOfflineworker.Put(note);

            return true;
        }

        /// <summary>
        /// Requests the transmission of data
        /// </summary>
        /// <param name="notes">A OfflineDataInfo that specifies the information for the offline data.</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqOfflineData(OfflineDataInfo[] notes)
        {
            mOfflineworker.Put(notes);

            return true;
        }

        private bool SendReqOfflineData(int sectionId, int ownerId, int noteId)
        {
            byte[] ownerByte = ByteConverter.IntToByte(ownerId);

            short length = (short)(5 + 40);

            ByteUtil bf = new ByteUtil();

            bf.Put((byte)0xC0)
              .Put((byte)Cmd.P_OfflineDataRequest)
              .PutShort(length)
              .Put(ownerByte[0])
              .Put(ownerByte[1])
              .Put(ownerByte[2])
              .Put((byte)sectionId)
              .Put((byte)1)
              .PutInt(noteId)
              .PutNull(36)
              .Put((byte)0xC1);

            PenClient.Write(bf.ToArray());

            bf.Clear();
            bf = null;

            return true;
        }

        /// <summary>
        /// Request to remove offline data in device.
        /// </summary>
        /// <param name="section">The Section Id of the paper</param>
        /// <param name="owner">The Owner Id of the paper</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqRemoveOfflineData(int section, int owner)
        {
            byte[] ownerByte = ByteConverter.IntToByte(owner);

            ByteUtil bf = new ByteUtil();

            bf.Put((byte)0xC0)
              .Put((byte)Cmd.P_OfflineDataRemove)
              .PutShort(12)
              .Put(ownerByte[0])
              .Put(ownerByte[1])
              .Put(ownerByte[2])
              .Put((byte)section)
              .Put((byte)0x00)
              .Put((byte)0x00)
              .Put((byte)0x00)
              .Put((byte)0x00)
              .Put((byte)0x00)
              .Put((byte)0x00)
              .Put((byte)0x00)
              .Put((byte)0x00)
              .Put((byte)0xC1);

            PenClient.Write(bf.ToArray());

            bf.Clear();
            bf = null;

            return true;
        }

        /// <summary>
        /// Request the status of pen.
        /// If you requested, you can receive result by PenCommV1Callbacks.onReceivedPenStatus method.
        /// </summary>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqPenStatus()
        {
            ByteUtil bf = new ByteUtil();
            bf.Put((byte)0xC0)
              .Put((byte)Cmd.P_PenStatusRequest)
              .PutShort(0)
              .Put((byte)0xC1);

            PenClient.Write(bf.ToArray());

            bf.Clear();
            bf = null;

            return true;
        }

        /// <summary>
        /// Input password if device is locked.
        /// </summary>
        /// <param name="password">Specifies the password for authentication. Password is a string</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqInputPassword(string password)
        {
            if (password == null)
                return false;

            if (password.Equals(DEFAULT_PASSWORD))
                return false;

            byte[] bStrByte = Encoding.UTF8.GetBytes(password);

            ByteUtil bf = new ByteUtil();
            bf.Put((byte)0xC0)
              .Put((byte)Cmd.P_PasswordResponse)
              .PutShort(16)
              .Put(bStrByte, 16)
              .Put((byte)0xC1);

            PenClient.Write(bf.ToArray());

            bf.Clear();
            bf = null;

            return true;
        }

        public bool _ReqInputPassword(string password)
        {
            if (password == null)
                return false;

            byte[] bStrByte = Encoding.UTF8.GetBytes(password);

            ByteUtil bf = new ByteUtil();
            bf.Put((byte)0xC0)
              .Put((byte)Cmd.P_PasswordResponse)
              .PutShort(16)
              .Put(bStrByte, 16)
              .Put((byte)0xC1);

            PenClient.Write(bf.ToArray());

            bf.Clear();
            bf = null;

            return true;
        }

        /// <summary>
        /// Change the password of device.
        /// </summary>
        /// <param name="oldPassword">Current password</param>
        /// <param name="newPassword">New password</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqSetUpPassword(string oldPassword, string newPassword)
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


            byte[] oPassByte = Encoding.UTF8.GetBytes(oldPassword);
            byte[] nPassByte = Encoding.UTF8.GetBytes(newPassword);

            ByteUtil bf = new ByteUtil();
            bf.Put((byte)0xC0)
              .Put((byte)Cmd.P_PasswordSet)
              .PutShort(32)
              .Put(oPassByte, 16)
              .Put(nPassByte, 16)
              .Put((byte)0xC1);

            PenClient.Write(bf.ToArray());

            bf.Clear();
            bf = null;

            return true;
        }

        /// <summary>
        /// Sets the value of the pen's sensitivity property that controls the force sensor of pen.
        /// </summary>
        /// <param name="level">the value of sensitivity. (0~4, 0 means maximum sensitivity)</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqSetupPenSensitivity(short level)
        {
            ByteUtil bf = new ByteUtil();

            bf.Put((byte)0xC0)
              .Put((byte)Cmd.P_PenSensitivity)
              .PutShort(2)
              .PutShort(level)
              .Put((byte)0xC1);

            PenClient.Write(bf.ToArray());

            bf.Clear();
            bf = null;

            return true;
        }

        /// <summary>
        /// Sets the value of the auto shutdown time property that if pen stay idle, shut off the pen.
        /// </summary>
        /// <param name="minute">minute of maximum idle time, staying power on (0~)</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqSetupPenAutoShutdownTime(short minute)
        {
            ByteUtil bf = new ByteUtil();

            bf.Put((byte)0xC0)
              .Put((byte)Cmd.P_AutoShutdownTime)
              .PutShort(2)
              .PutShort(minute)
              .Put((byte)0xC1);

            PenClient.Write(bf.ToArray());

            bf.Clear();
            bf = null;

            return true;
        }

        /// <summary>
        /// Sets the status of the beep property.
        /// </summary>
        /// <param name="enable">true if you want to listen sound of pen, otherwise false.</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqSetupPenBeep(bool enable)
        {
            ByteUtil bf = new ByteUtil();

            bf.Put((byte)0xC0)
              .Put((byte)Cmd.P_BeepSet)
              .PutShort(1)
              .Put((byte)(enable ? 1 : 0))
              .Put((byte)0xC1);

            PenClient.Write(bf.ToArray());

            bf.Clear();
            bf = null;

            return true;
        }

        /// <summary>
        /// Sets the status of the auto power on property that if write the pen, turn on when pen is down.
        /// </summary>
        /// <param name="seton">true if you want to use, otherwise false.</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqSetupPenAutoPowerOn(bool enable)
        {
            ByteUtil bf = new ByteUtil();

            bf.Put((byte)0xC0)
              .Put((byte)Cmd.P_AutoPowerOnSet)
              .PutShort(1)
              .Put((byte)(enable ? 1 : 0))
              .Put((byte)0xC1);

            PenClient.Write(bf.ToArray());

            bf.Clear();
            bf = null;

            return true;
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
        public bool ReqSetupPenColor(int rgbcolor)
        {
            byte[] cbyte = ByteConverter.IntToByte(rgbcolor);

            byte[] nbyte = new byte[] { cbyte[0], cbyte[1], cbyte[2], (byte)0x01 };

            ByteUtil bf = new ByteUtil();

            bf.Put((byte)0xC0)
              .Put((byte)Cmd.P_PenColorSet)
              .PutShort(4)
              .Put(nbyte, 4)
              .Put((byte)0xC1);

            PenClient.Write(bf.ToArray());

            bf.Clear();
            bf = null;

            return true;
        }

        /// <summary>
        /// Sets the hover mode.
        /// </summary>
        /// <param name="enable">true if you want to enable hover mode, otherwise false.</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqSetupHoverMode(bool enable)
        {
            ByteUtil bf = new ByteUtil();

            bf.Put((byte)0xC0)
              .Put((byte)Cmd.P_HoverOnOff)
              .PutShort(1)
              .Put((byte)(enable ? 1 : 0))
              .Put((byte)0xC1);

            PenClient.Write(bf.ToArray());

            bf.Clear();
            bf = null;

            return true;
        }

        public async Task<byte[]> ReadAll(StorageFile file)
        {
            IBuffer buffer = await FileIO.ReadBufferAsync(file);
            byte[] bytes = System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeBufferExtensions.ToArray(buffer);
            return bytes;
        }

        /// <summary>
        /// Requests the firmware installation
        /// </summary>
        /// <param name="filepath">absolute path of firmware file</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public async void ReqPenSwUpgrade(StorageFile filepath)
        {
            if (IsUploading)
            {
                Debug.WriteLine("[FileUploadWorker] Upgrade task is still excuting.");
                return;
            }

            IsUploading = true;

            mFwChunk = new Chunk(1024);

            //byte[] bytes = await ReadAll(filepath);

            bool loaded = await mFwChunk.Load(filepath);

            if (!loaded)
            {
                return;
            }

            int file_size = mFwChunk.GetFileSize();
            short chunk_count = (short)mFwChunk.GetChunkLength();
            short chunk_size = (short)mFwChunk.GetChunksize();

            byte[] StrByte = Encoding.UTF8.GetBytes("\\N2._v_");

            Debug.WriteLine("[FileUploadWorker] file upload => filesize : {0}, packet count : {1}, packet size {2}", file_size, chunk_count, chunk_size);

            ByteUtil bf = new ByteUtil();

            bf.Put((byte)0xC0)
                .Put((byte)Cmd.P_PenSWUpgradeCommand)
                .PutShort(136)
                .Put(StrByte, 128)
                .PutInt(file_size)
                .PutShort(chunk_count)
                .PutShort(chunk_size)
                .Put((byte)0xC1);

            PenClient.Write(bf.ToArray());

            bf = null;

            onStartFirmwareInstallation();

        }

        private void ResponseChunkRequest(short index)
        {
            byte[] data = null;
            if (mFwChunk == null || (data = mFwChunk.Get(index)) == null)
            {
                IsUploading = false;
                return;
            }

            byte checksum = mFwChunk.GetChecksum(index);

            short dataLength = (short)(data.Length + 3);

            ByteUtil bf = new ByteUtil();
            bf.Put((byte)0xC0)
              .Put((byte)Cmd.P_PenSWUpgradeResponse)
              .PutShort(dataLength)
              .PutShort(index)
              .Put(checksum)
              .Put(data)
              .Put((byte)0xC1);
            PenClient.Write(bf.ToArray());

            onReceiveFirmwareUpdateStatus(new ProgressChangeEventArgs(mFwChunk.GetChunkLength(), (int)index));
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

        public bool IsSupportPenProfile()
        {
            //if (connectedDeviceName != null)
            //{
            //	float ver = 0f;
            //	string[] temp = protocolVersion.Split('.');
            //	try
            //	{
            //		ver = FloatConverter.ToSingle(temp[0] + "." + temp[1]);
            //	}
            //	catch (Exception e)
            //	{
            //		Debug.WriteLine(e.StackTrace);
            //	}

            //	if (connectedDeviceName.Equals(PEN_MODEL_NAME_F110) && ver >= PEN_PROFILE_SUPPORT_VERSION_F110)
            //		return true;
            //	else if (connectedDeviceName.Equals(PEN_MODEL_NAME_F110C) && ver >= PEN_PROFILE_SUPPORT_VERSION_F110C)
            //		return true;
            //	else
            //		return false;
            //}

            return false;
        }

        #region Pen Profile
        public bool ReqCreateProfile(byte[] profileName, byte[] password)
        {
            ByteUtil bf = new ByteUtil();
            bf.Put(0xC0)
                .Put((byte)Cmd.P_ProfileRequest)    // command
                .PutShort((short)(PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME + 1 + PenProfile.LIMIT_BYTE_LENGTH_PASSWORD + 2 + 2))        // length
                .Put(profileName, PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME)                // profile file name
                .Put(PenProfile.PROFILE_CREATE)     // type
                .Put(password, PenProfile.LIMIT_BYTE_LENGTH_PASSWORD)                   // password
                .PutShort(32)                       // section 크기 -> 32인 이유? 우선 android따라감. 확인필요
                .PutShort(32)                        // sector 개수(2^N 현재는 고정 2^8)
                .Put(0xC1);

            return Send(bf);
        }

        public bool ReqDeleteProfile(byte[] profileName, byte[] password)
        {
            ByteUtil bf = new ByteUtil();
            bf.Put(0xC0)
                .Put((byte)Cmd.P_ProfileRequest)    // command
                .PutShort((short)(PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME + 1 + PenProfile.LIMIT_BYTE_LENGTH_PASSWORD))                // length
                .Put(profileName, PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME)                // profile file name
                .Put(PenProfile.PROFILE_DELETE)     // type
                .Put(password, PenProfile.LIMIT_BYTE_LENGTH_PASSWORD)                   // password
                .Put(0xC1);

            return Send(bf);
        }

        public bool ReqProfileInfo(byte[] profileName)
        {
            ByteUtil bf = new ByteUtil();
            bf.Put(0xC0)
                .Put((byte)Cmd.P_ProfileRequest) // command
                .PutShort((short)(PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME + 1))                    // length
                .Put(profileName, PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME)           // profile file name
                .Put(PenProfile.PROFILE_INFO)       // type
                .Put(0xC1);

            return Send(bf);
        }

        public bool ReqWriteProfileValue(byte[] profileName, byte[] password, byte[][] keys, byte[][] data)
        {
            int dataLength = 0;
            int dataCount = data.Length;
            for (int i = 0; i < dataCount; ++i)
            {
                dataLength += PenProfile.LIMIT_BYTE_LENGTH_KEY;               // key
                dataLength += 2;                // data length
                dataLength += data[i].Length;   // data 
            }

            ByteUtil bf = new ByteUtil();
            bf.Put(0xC0)
                .Put((byte)Cmd.P_ProfileRequest)             // command
                .PutShort((short)(PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME + 1 + PenProfile.LIMIT_BYTE_LENGTH_PASSWORD + 1 + dataLength))  // length
                .Put(profileName, PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME)                       // profile file name
                .Put(PenProfile.PROFILE_WRITE_VALUE)            // type
                .Put(password, PenProfile.LIMIT_BYTE_LENGTH_PASSWORD)                          // password
                .Put((byte)dataCount);                          // count

            for (int i = 0; i < dataCount; ++i)
            {
                bf.Put(keys[i], PenProfile.LIMIT_BYTE_LENGTH_KEY)
                    .PutShort((short)data[i].Length)
                    .Put(data[i]);
            }

            bf.Put(0xC1);

            return Send(bf);
        }

        public bool ReqReadProfileValue(byte[] profileName, byte[][] keys)
        {
            ByteUtil bf = new ByteUtil();
            bf.Put(0xC0)
                .Put((byte)Cmd.P_ProfileRequest)                 // command
                .PutShort((short)(PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME + 1 + 1 + PenProfile.LIMIT_BYTE_LENGTH_KEY * keys.Length))    // Length
                .Put(profileName, PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME)                           // profile file name
                .Put(PenProfile.PROFILE_READ_VALUE)                 // Type
                .Put((byte)keys.Length);                            // Key Count

            for (int i = 0; i < keys.Length; ++i)
            {
                bf.Put(keys[i], PenProfile.LIMIT_BYTE_LENGTH_KEY);
            }

            bf.Put(0xC1);

            return Send(bf);
        }

        public bool ReqDeleteProfileValue(byte[] profileName, byte[] password, byte[][] keys)
        {
            ByteUtil bf = new ByteUtil();
            bf.Put(0xC0)
                .Put((byte)Cmd.P_ProfileRequest)                     // command
                .PutShort((short)(PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME + 1 + PenProfile.LIMIT_BYTE_LENGTH_PASSWORD + 1 + PenProfile.LIMIT_BYTE_LENGTH_KEY * keys.Length))    // Length
                .Put(profileName, PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME)                               // profile file name
                .Put(PenProfile.PROFILE_DELETE_VALUE)                   // Type
                .Put(password, PenProfile.LIMIT_BYTE_LENGTH_PASSWORD)                                  // password
                .Put((byte)keys.Length);                                // key count

            for (int i = 0; i < keys.Length; ++i)
            {
                bf.Put(keys[i], PenProfile.LIMIT_BYTE_LENGTH_KEY);
            }

            bf.Put(0xC1);

            return Send(bf);
        }
        #endregion


        #region Event Methods
        /// <summary>
        /// Occurs when a connection is made
        /// </summary>
        public event TypedEventHandler<IPenClient, ConnectedEventArgs> Connected;
        internal void onConnected(ConnectedEventArgs args)
        {
            Support.PressureCalibration.Instance.Clear();
            Connected?.Invoke(PenClient, args);
        }

        /// <summary>
        /// Occurs when a connection is closed
        /// </summary>
		public event TypedEventHandler<IPenClient, object> Disconnected;
        internal void onDisconnected()
        {
            Support.PressureCalibration.Instance.Clear();
            Disconnected?.Invoke(PenClient, new object());
        }

        /// <summary>
        /// Occurs when finished offline data downloading
        /// </summary>
		public event TypedEventHandler<IPenClient, SimpleResultEventArgs> OfflineDownloadFinished;
        internal void onFinishedOfflineDownload(SimpleResultEventArgs args)
        {
            OfflineDownloadFinished?.Invoke(PenClient, args);
        }

        /// <summary>
        /// Occurs when authentication is complete, the password entered has been verified.
        /// </summary>
		public event TypedEventHandler<IPenClient, object> Authenticated;
        internal void onPenAuthenticated()
        {
            Authenticated?.Invoke(PenClient, new object());
        }

        /// <summary>
        /// Occurs when the note information to be used is added
        /// </summary>
        public event TypedEventHandler<IPenClient, object> AvailableNoteAdded;
        internal void onAvailableNoteAdded()
        {
            AvailableNoteAdded?.Invoke(PenClient, new object());
        }

        /// <summary>
        /// Occurs when the power-on setting is applied when the pen tip is pressed
        /// </summary>
        public event TypedEventHandler<IPenClient, SimpleResultEventArgs> AutoPowerOnChanged;
        internal void onPenAutoPowerOnSetupResponse(SimpleResultEventArgs args)
        {
            AutoPowerOnChanged?.Invoke(PenClient, args);
        }

        /// <summary>
        /// Occurs when the power-off setting is applied when there is no input for a certain period of time
        /// </summary>
		public event TypedEventHandler<IPenClient, SimpleResultEventArgs> AutoPowerOffTimeChanged;
        internal void onPenAutoShutdownTimeSetupResponse(SimpleResultEventArgs args)
        {
            AutoPowerOffTimeChanged?.Invoke(PenClient, args);
        }

        /// <summary>
        /// Occurs when the beep setting is applied
        /// </summary>
		public event TypedEventHandler<IPenClient, SimpleResultEventArgs> BeepSoundChanged;
        internal void onPenBeepSetupResponse(SimpleResultEventArgs args)
        {
            BeepSoundChanged?.Invoke(PenClient, args);
        }

        /// <summary>
        /// Occurs when the cap is closed and the power-on and power-off setting is applied
        /// </summary>
		public event TypedEventHandler<IPenClient, SimpleResultEventArgs> PenCapPowerOnOffChanged;
        internal void onPenCapPowerOnOffSetupResponse(SimpleResultEventArgs args)
        {
            PenCapPowerOnOffChanged?.Invoke(PenClient, args);
        }

        /// <summary>
        /// Occurs when the pen's new LED color value is applied
        /// </summary>
		public event TypedEventHandler<IPenClient, SimpleResultEventArgs> PenColorChanged;
        internal void onPenColorSetupResponse(SimpleResultEventArgs args)
        {
            PenColorChanged?.Invoke(PenClient, args);
        }

        public event TypedEventHandler<IPenClient, SimpleResultEventArgs> HoverChanged;
        internal void onPenHoverSetupResponse(SimpleResultEventArgs args)
        {
            HoverChanged?.Invoke(PenClient, args);
        }

        /// <summary>
        /// Occurs when settings to store offline data are applied
        /// </summary>
		public event TypedEventHandler<IPenClient, SimpleResultEventArgs> OfflineDataChanged;
        internal void onPenOfflineDataSetupResponse(SimpleResultEventArgs args)
        {
            OfflineDataChanged?.Invoke(PenClient, args);
        }

        /// <summary>
        /// Occurs when requesting a password when the pen is locked with a password
        /// </summary>
		public event TypedEventHandler<IPenClient, PasswordRequestedEventArgs> PasswordRequested;
        internal void onPenPasswordRequest(PasswordRequestedEventArgs args)
        {
            PasswordRequested?.Invoke(PenClient, args);
        }

        /// <summary>
        /// Occurs when the pen's new password is applied
        /// </summary>
		public event TypedEventHandler<IPenClient, SimpleResultEventArgs> PasswordChanged;
        internal void onPenPasswordSetupResponse(SimpleResultEventArgs args)
        {
            PasswordChanged?.Invoke(PenClient, args);
        }

        /// <summary>
        /// Occurs when the pen's new fsr sensitivity setting is applied
        /// </summary>
		public event TypedEventHandler<IPenClient, SimpleResultEventArgs> SensitivityChanged;
        internal void onPenSensitivitySetupResponse(SimpleResultEventArgs args)
        {
            SensitivityChanged?.Invoke(PenClient, args);
        }

        /// <summary>
        /// Occurs when the pen's new fsc sensitivity setting is applied
        /// </summary>
        public event TypedEventHandler<IPenClient, SimpleResultEventArgs> FscSensitivityChanged;
        internal void onPenFscSensitivitySetupResponse(SimpleResultEventArgs args)
        {
            FscSensitivityChanged?.Invoke(PenClient, args);
        }

        /// <summary>
        /// Occurs when pen's RTC time is applied
        /// </summary>
		public event TypedEventHandler<IPenClient, SimpleResultEventArgs> RtcTimeChanged;
        internal void onPenTimestampSetupResponse(SimpleResultEventArgs args)
        {
            RtcTimeChanged?.Invoke(PenClient, args);
        }

        /// <summary>
        /// Occurs when pen's beep and light is applied
        /// </summary>
        public event TypedEventHandler<IPenClient, SimpleResultEventArgs> BeepAndLightChanged;
        internal void onPenBeepAndLightSetupResponse(SimpleResultEventArgs args)
        {
            BeepAndLightChanged?.Invoke(PenClient, args);
        }

        /// <summary>
        /// Occurs when the pen's new bt local name setting is applied
        /// </summary>
        public event TypedEventHandler<IPenClient, SimpleResultEventArgs> BtLocalNameChanged;
        internal void onPenBtLocalNameSetupResponse(SimpleResultEventArgs args)
        {
            BtLocalNameChanged?.Invoke(PenClient, args);
        }

        /// <summary>
        /// Occurs when the pen's new data transmission type setting is applied
        /// </summary>
        public event TypedEventHandler<IPenClient, SimpleResultEventArgs> DataTransmissionTypeChanged;
        internal void onPenDataTransmissionTypeSetupResponse(SimpleResultEventArgs args)
        {
            DataTransmissionTypeChanged?.Invoke(PenClient, args);
        }


        /// <summary>
        /// Occurs when the pen's new down sampling setting is applied
        /// </summary>
        public event TypedEventHandler<IPenClient, SimpleResultEventArgs> DownSamplingChanged;
        internal void onPenDownSamplingSetupResponse(SimpleResultEventArgs args)
        {
            DownSamplingChanged?.Invoke(PenClient, args);
        }

        /// <summary>
        /// Occurs when the pen's new usb mode setting is applied
        /// </summary>
        public event TypedEventHandler<IPenClient, SimpleResultEventArgs> UsbModeChanged;
        internal void onUsbModeSetupResponse(SimpleResultEventArgs args)
        {
            UsbModeChanged?.Invoke(PenClient, args);
        }

        /// <summary>
        /// Occurs when the pen's battery status changes
        /// </summary>
		public event TypedEventHandler<IPenClient, BatteryAlarmReceivedEventArgs> BatteryAlarmReceived;
        internal void onReceiveBatteryAlarm(BatteryAlarmReceivedEventArgs args)
        {
            BatteryAlarmReceived?.Invoke(PenClient, args);
        }

        /// <summary>
        /// Occurs when new coordinate data is received
        /// </summary>
		public event TypedEventHandler<IPenClient, DotReceivedEventArgs> DotReceived;
        internal void onReceiveDot(DotReceivedEventArgs args)
        {
            DotReceived?.Invoke(PenClient, args);
        }

        /// <summary>
        /// Occurs when firmware installation is complete
        /// </summary>
		public event TypedEventHandler<IPenClient, SimpleResultEventArgs> FirmwareInstallationFinished;
        internal void onReceiveFirmwareUpdateResult(SimpleResultEventArgs args)
        {
            FirmwareInstallationFinished?.Invoke(PenClient, args);
        }

        /// <summary>
        /// Occurs when firmware installation is started
        /// </summary>
        public event TypedEventHandler<IPenClient, object> FirmwareInstallationStarted;
        internal void onStartFirmwareInstallation()
        {
            FirmwareInstallationStarted?.Invoke(PenClient, new object());
        }

        /// <summary>
        /// Notice the progress while the firmware installation is in progress
        /// </summary>
        public event TypedEventHandler<IPenClient, ProgressChangeEventArgs> FirmwareInstallationStatusUpdated;
        internal void onReceiveFirmwareUpdateStatus(ProgressChangeEventArgs args)
        {
            FirmwareInstallationStatusUpdated?.Invoke(PenClient, args);
        }

        /// <summary>
        /// Occurs when a list of offline data is received
        /// </summary>
		public event TypedEventHandler<IPenClient, OfflineDataListReceivedEventArgs> OfflineDataListReceived;
        internal void onReceiveOfflineDataList(OfflineDataListReceivedEventArgs args)
        {
            OfflineDataListReceived?.Invoke(PenClient, args);
        }

        /// <summary>
        /// Occurs when an offline stroke is received
        /// </summary>
		public event TypedEventHandler<IPenClient, OfflineStrokeReceivedEventArgs> OfflineStrokeReceived;
        internal void onReceiveOfflineStrokes(OfflineStrokeReceivedEventArgs args)
        {
            OfflineStrokeReceived?.Invoke(PenClient, args);
        }

        /// <summary>
        /// Occurs when a status of pen is received
        /// </summary>
		public event TypedEventHandler<IPenClient, PenStatusReceivedEventArgs> PenStatusReceived;
        internal void onReceivePenStatus(PenStatusReceivedEventArgs args)
        {
            PenStatusReceived?.Invoke(PenClient, args);
        }

        /// <summary>
        /// Occurs when an offline data is removed
        /// </summary>
		public event TypedEventHandler<IPenClient, SimpleResultEventArgs> OfflineDataRemoved;
        internal void onRemovedOfflineData(SimpleResultEventArgs args)
        {
            OfflineDataRemoved?.Invoke(PenClient, args);
        }

        /// <summary>
        /// Occurs when offline downloading starts
        /// </summary>
		public event TypedEventHandler<IPenClient, object> OfflineDataDownloadStarted;

        /// <summary>
        /// Occurs when a response to an operation request for a pen profile is received
        /// </summary>
        public event TypedEventHandler<IPenClient, PenProfileReceivedEventArgs> PenProfileReceived;
        internal void onPenProfileReceived(PenProfileReceivedEventArgs args)
        {
            PenProfileReceived?.Invoke(PenClient, args);
        }

        internal void onStartOfflineDownload()
        {
            OfflineDataDownloadStarted?.Invoke(PenClient, new object());
        }

        /// <summary>
        /// Occurs when error received
        /// </summary>
        public event TypedEventHandler<IPenClient, ErrorDetectedEventArgs> ErrorDetected;
        internal void onErrorDetected(ErrorDetectedEventArgs args)
        {
            ErrorDetected?.Invoke(PenClient, args);
        }
        #endregion

        #region Protocol Parse

        public void ProtocolParse(byte[] buff, int size)
        {
            for (int i = 0; i < size; i++)
            {
                ParseOneByte(buff[i]);
            }
        }

        public void ProtocolParse1(byte[] buff, int size)
        {
            for (int i = 0; i < size; i++)
            {
                if (buff[i] != PKT_START)
                {
                    continue;
                }

                Packet.Builder builder = new Packet.Builder();

                int cmd = buff[i + 1];

                int length = ByteConverter.ByteToShort(new byte[] { buff[i + PKT_LENGTH_POS1], buff[i + PKT_LENGTH_POS2] });

                byte[] rs = new byte[length];

                Array.Copy(buff, i + 1 + PKT_HEADER_LEN, rs, 0, length);

                ParsePacket(builder.cmd(cmd).data(rs).Build());

                i += PKT_HEADER_LEN + length;
            }
        }

        private bool isStart = true;

        private int counter = 0;

        private ByteUtil mBuffer = null;

        private int dataLength = 0;

        // length
        private byte[] lbuffer = new byte[2];

        private void ParseOneByte(byte data)
        {
            int int_data = (int)(data & 0xFF);

            if (int_data == PKT_START && isStart)
            {
                mBuffer = new ByteUtil();

                counter = 0;
                isStart = false;
            }
            else if (int_data == PKT_END && counter == dataLength + PKT_HEADER_LEN)
            {
                Packet.Builder builder = new Packet.Builder();

                // 커맨드를 뽑는다.
                int cmd = mBuffer.GetByteToInt();

                // 길이를 뽑는다.
                int length = mBuffer.GetShort();

                // 커맨드, 길이를 제외한 나머지 바이트를 컨텐트로 지정
                byte[] content = mBuffer.GetBytes();

                Packet packet = builder.cmd(cmd).data(content).Build();
                //if ((Cmd)packet.Cmd == Cmd.A_DotUpDownDataNew && content[8] == 0x00)
                //{ }else
                ParsePacket(packet);

                dataLength = 0;
                counter = 10;
                mBuffer.Clear();
                isStart = true;
            }
            else if (counter > PKT_MAX_LEN)
            {
                counter = 10;
                dataLength = 0;
                isStart = true;
            }
            else
            {
                if (counter == PKT_LENGTH_POS1)
                {
                    lbuffer[0] = data;
                }
                else if (counter == PKT_LENGTH_POS2)
                {
                    lbuffer[1] = data;
                    dataLength = ByteConverter.ByteToShort(lbuffer);
                }

                if (!isStart)
                {
                    mBuffer.Put(data);
                    counter++;
                }
            }
        }

        #endregion

        private bool Send(ByteUtil bf)
        {
            PenClient.Write(bf.ToArray());

            bf.Clear();
            bf = null;

            return true;
        }
    }
}
