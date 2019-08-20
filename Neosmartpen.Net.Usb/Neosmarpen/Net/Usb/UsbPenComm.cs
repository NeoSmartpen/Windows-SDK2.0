using Neosmartpen.Net.Support;
using Neosmartpen.Net.Usb.Events;
using Neosmartpen.Net.Usb.Exceptions;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace Neosmartpen.Net.Usb
{
    /// <summary>
    /// A class that implements functions related to communication and control with a connected pen
    /// </summary>
    public class UsbPenComm : IDisposable
    {
        private const string DataFilePath = "0:/data/";
        private const string LogFilePath = "0:/log/";

        /// <summary>
        /// Name of the currently connected COM port
        /// </summary>
        public string PortName { get; private set; }

        /// <summary>
        /// Value indicating open or closed state of the serial port
        /// </summary>
        public bool IsOpen { get { return _serialPort.IsOpen; } }

        /// <summary>
        /// A value indicating whether the connection is in a possible state
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Device Name of the currently connected pen
        /// </summary>
        public string DeviceName { get; private set; }

        /// <summary>
        /// Firmware version of the currently connected pen
        /// </summary>
        public string FirmwareVersion { get; private set; }

        /// <summary>
        /// Bluetooth protocol version of the currently connected pen
        /// </summary>
        public string BluetoothProtocolVersion { get; private set; }

        /// <summary>
        /// Sub name of the currently connected pen
        /// </summary>
        public string SubName { get; private set; }

        /// <summary>
        /// mac address of the currently connected pen
        /// </summary>
        public string MacAddress { get; private set; }

        /// <summary>
        /// bluetooth model name of the currently connected pen
        /// </summary>
        public string BluetoothModelName { get; private set; }

        /// <summary>
        /// Auto Power Off time of the currently connected pen
        /// </summary>
        public TimeSpan AutoPowerOffTime { get; private set; }

        /// <summary>
        /// Auto Power On setting of the currently connected pen
        /// </summary>
        public bool IsAutoPowerOnEnabled { get; private set; }

        /// <summary>
        /// Pen Cap Off setting of the currently connected pen (power off when the pen's cap is closed)
        /// </summary>
        public bool IsPenCapOffEnabled { get; private set; }

        /// <summary>
        /// Setting beep output of currently connected pen
        /// </summary>
        public bool IsBeepEnabled { get; private set; }

        /// <summary>
        /// Setting whether to save offline data of the currently connected pen
        /// </summary>
        public bool CanSaveOfflineData { get; private set; }

        /// <summary>
        /// Setting value of using downsampling function of currently connected pen
        /// </summary>
        public bool IsDownsamplingEnabled { get; private set; }

        private SerialPort _serialPort;

        private UsbProtocolParser _parser;

        private byte packetNumber = 0;

        /// <summary>
        /// Create an instance of UsbPenComm associated with the COM port.
        /// </summary>
        /// <param name="portName">COM port name to connect to</param>
        public UsbPenComm(string portName)
        {
            PortName = portName;
            _serialPort = new SerialPort {
                PortName = portName,
                DataBits = 8,
                Parity = System.IO.Ports.Parity.None,
                Encoding = System.Text.Encoding.Default,
            };

            _parser = new UsbProtocolParser();
            _parser.PacketCreated += _parser_PacketCreated;

            _serialPort.DataReceived += new SerialDataReceivedEventHandler(_serialPort_DataReceived);
            _serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(_serialPort_ErrorReceived);
        }

        /// <summary>
        /// Open a new serial port.
        /// </summary>
        public void Open()
        {
            _serialPort.Open();
        }

        /// <summary>
        /// Start communication with the pen.
        /// </summary>
        public void Connect()
        {
            StartRequest();
        }

        private void _parser_PacketCreated(object sender, UsbPacketEventArgs e)
        {
            ParsePacket(e.Packet);
        }

        private bool isFirstStatus = true;

        private void ParsePacket(UsbPacket pk)
        {
            Cmd cmd = (Cmd)pk.Cmd;
            //System.Diagnostics.Debug.WriteLine( "Cmd : {0}", cmd.ToString() );

            switch (cmd)
            {
                case Cmd.START:
                    {
                        isFirstStatus = true;
                        int protocolVersion = pk.GetInt();
                        bool available = pk.GetByte() == 0x01;
                        if (available)
                            GetDeviceInfoRequest();
                        else
                            ConnectionRefused?.Invoke(this, null);
                    }
                    break;

                case Cmd.GETDEVINFO:
                    {
                        DeviceName = pk.GetString(16);
                        FirmwareVersion = pk.GetString(16);
                        BluetoothProtocolVersion = pk.GetString(8);
                        SubName = pk.GetString(16);
                        MacAddress = BitConverter.ToString(pk.GetBytes(6)).Replace("-", ":");
                        BluetoothModelName = pk.GetString(32);

                        GetConfigRequest(ConfigType.AutoPowerOffTime, ConfigType.AutoPowerOn, ConfigType.Beep, ConfigType.DownSampling, ConfigType.PenCapOff, ConfigType.SaveOfflineData);
                    }
                    break;

                case Cmd.GETCONFIG:
                    {
                        int typeCount = pk.GetByteToInt();

                        for(int i=0; i<typeCount; i++)
                        {
                            int type = pk.GetByteToInt();

                            switch((ConfigType)type)
                            {
                                case ConfigType.DateTime:
                                    long timestamp = pk.GetLong();
                                    DateTimeReceived?.Invoke(this, new DateTimeReceivedEventArgs(timestamp));
                                    break;
                                case ConfigType.AutoPowerOffTime:
                                    AutoPowerOffTime = new TimeSpan( 0, pk.GetShort(), 0);
                                    if (!isFirstStatus)
                                        ConfigSetupResultReceived?.Invoke(this, new ConfigSetupResultReceivedEventArgs(ConfigType.AutoPowerOffTime, true));
                                    break;
                                case ConfigType.AutoPowerOn:
                                    IsAutoPowerOnEnabled = pk.GetByteToInt() == 0x01;
                                    if (!isFirstStatus)
                                        ConfigSetupResultReceived?.Invoke(this, new ConfigSetupResultReceivedEventArgs(ConfigType.AutoPowerOn, true));
                                    break;
                                case ConfigType.PenCapOff:
                                    IsPenCapOffEnabled = pk.GetByteToInt() == 0x01;
                                    if (!isFirstStatus)
                                        ConfigSetupResultReceived?.Invoke(this, new ConfigSetupResultReceivedEventArgs(ConfigType.PenCapOff, true));
                                    break;
                                case ConfigType.Beep:
                                    IsBeepEnabled = pk.GetByteToInt() == 0x01;
                                    if (!isFirstStatus)
                                        ConfigSetupResultReceived?.Invoke(this, new ConfigSetupResultReceivedEventArgs(ConfigType.Beep, true));
                                    break;
                                case ConfigType.SaveOfflineData:
                                    CanSaveOfflineData = pk.GetByteToInt() == 0x01;
                                    if (!isFirstStatus)
                                        ConfigSetupResultReceived?.Invoke(this, new ConfigSetupResultReceivedEventArgs(ConfigType.SaveOfflineData, true));
                                    break;
                                case ConfigType.DownSampling:
                                    IsDownsamplingEnabled = pk.GetByteToInt() == 0x01;
                                    if (!isFirstStatus)
                                        ConfigSetupResultReceived?.Invoke(this, new ConfigSetupResultReceivedEventArgs(ConfigType.DownSampling, true));
                                    break;
                                case ConfigType.Battery:
                                    var battery = pk.GetByteToInt();
                                    BatteryStatusReceived?.Invoke(this, new BatteryStatusReceivedEventArgs(battery));
                                    break;
                                case ConfigType.Storage:
                                    var storage = pk.GetByteToInt();
                                    StorageStatusReceived?.Invoke(this, new StorageStatusReceivedEventArgs(storage));
                                    break;
                            }
                        }

                        if (isFirstStatus)
                        {
                            IsActive = true;
                            Authenticated?.Invoke(this, null);
                            isFirstStatus = false;
                        }
                    }
                    break;

                case Cmd.SETCONFIG:
                    {
                        int typeCount = pk.GetByteToInt();
                        for (int i = 0; i < typeCount; i++)
                        {
                            int type = pk.GetByteToInt();
                            bool result = pk.GetByteToInt() == 0x00;

                            if (result)
                            {
                                if ((ConfigType)type == ConfigType.DateTime)
                                {
                                    ConfigSetupResultReceived?.Invoke(this, new ConfigSetupResultReceivedEventArgs(ConfigType.DateTime, true));
                                }
                                else
                                {
                                    GetConfigRequest((ConfigType)type);
                                }
                            }
                            else
                            {
                                ConfigSetupResultReceived?.Invoke(this, new ConfigSetupResultReceivedEventArgs((ConfigType)type, false));
                            }
                        }
                    }
                    break;

                case Cmd.FORMAT:
                    {
                        bool result = pk.GetByteToInt() == 0x00;
                        FormatResultReceived?.Invoke(this, new ResultReceivedEventArgs(result ? ResultReceivedEventArgs.ResultType.Success : ResultReceivedEventArgs.ResultType.Failed));
                    }
                    break;

                case Cmd.GETOFFLINEDATALIST:
                case Cmd.GETLOGFILELIST:
                    {
                        int result = pk.GetByteToInt();
                        int fileCount = pk.GetInt();
                        var resultList = new List<string>();
                        for (int i = 0; i < fileCount; i++)
                        {
                            string fileName = pk.GetString(16);
                            resultList.Add(fileName);
                        }
                        FileListReceivedEventArgs.ResultType resultType;
                        if (result == 0)
                            resultType = FileListReceivedEventArgs.ResultType.Success;
                        else if (result == 1)
                            resultType = FileListReceivedEventArgs.ResultType.Failed;
                        else if (result == 2)
                            resultType = FileListReceivedEventArgs.ResultType.TooManyFileExists;
                        else
                            resultType = FileListReceivedEventArgs.ResultType.UnknownError;

                        if (pk.Cmd == Cmd.GETOFFLINEDATALIST)
                            OfflineFileListReceived?.Invoke(this, new FileListReceivedEventArgs(resultType, resultList.ToArray()));
                        else
                            LogFileListReceived?.Invoke(this, new FileListReceivedEventArgs(resultType, resultList.ToArray()));
                    }
                    break;

                case Cmd.GETFILE_H:
                    {
                        bool result = pk.GetByteToInt() == 0x00;
                        int fileSize = pk.GetInt();
                        int packetSize = pk.GetInt() - 8;

                        if (fileRequestInfo)
                        {
                            FileInfoReceived?.Invoke(this, new FileInfoReceivedEventArgs(result ? FileInfoReceivedEventArgs.ResultType.Success : FileInfoReceivedEventArgs.ResultType.FileNotExists, requestFileName, fileSize));
                        }
                        else
                        {
                            if (!result)
                            {
                                FileDownloadResultReceived?.Invoke(this, new FileDownloadResultReceivedEventArgs(FileDownloadResultReceivedEventArgs.ResultType.FileNotExists));
                            }
                            else
                            {
                                fileSerializer = new FileBuilder(targetFilePath, fileSize, packetSize);
                                downloadRetryCount = 0;
                                FileDownloadProgressChanged?.Invoke(this, new ProgressChangedEventArgs(0));
                                GetFilePacketRequest(0);
                            }
                        }
                    }
                    break;

                case Cmd.GETFILE_D:
                    {
                        int result = pk.GetByteToInt();
                        int offset = pk.GetInt();
                        int dataLength = pk.GetInt();
                        byte[] data = pk.GetBytes(dataLength);

                        if (result == 0x00)
                        {
                            fileSerializer.Put(data, offset);

                            int progress = (int)((double)offset / (double)(fileSerializer.FileSize - 1));
                            if (fileSerializer.GetNextOffset() < fileSerializer.FileSize)
                            {
                                FileDownloadProgressChanged?.Invoke(this, new ProgressChangedEventArgs(progress));
                                GetFilePacketRequest(fileSerializer.GetNextOffset());
                            }
                            else
                            {
                                bool fileCreated = fileSerializer.MakeFile();
                                if (fileCreated)
                                    FileDownloadProgressChanged?.Invoke(this, new ProgressChangedEventArgs(100));
                                FileDownloadResultReceived?.Invoke(this, new FileDownloadResultReceivedEventArgs(fileCreated ? FileDownloadResultReceivedEventArgs.ResultType.Success : FileDownloadResultReceivedEventArgs.ResultType.Failed));
                            }
                        }
                        else
                        {
                            if (downloadRetryCount++ > 3)
                            {
                                FileDownloadResultReceivedEventArgs.ResultType resultType;
                                if (result == 0x1)
                                    resultType = FileDownloadResultReceivedEventArgs.ResultType.Failed;
                                else if (result == 0x10)
                                    resultType = FileDownloadResultReceivedEventArgs.ResultType.OffsetInvalid;
                                else if (result == 0x11)
                                    resultType = FileDownloadResultReceivedEventArgs.ResultType.CannotOpenFile;
                                else
                                    resultType = FileDownloadResultReceivedEventArgs.ResultType.UnknownError;

                                FileDownloadResultReceived?.Invoke(this, new FileDownloadResultReceivedEventArgs(resultType));
                            }
                            else
                            {
                                GetFilePacketRequest(offset);
                            }
                        }
                    }
                    break;

                case Cmd.DELETEFILE:
                    {
                        bool result = pk.GetByteToInt() == 0x00;
                        DeleteFileResultReceived?.Invoke(this, new ResultReceivedEventArgs(result ? ResultReceivedEventArgs.ResultType.Success : ResultReceivedEventArgs.ResultType.Failed));
                    }
                    break;

                case Cmd.POWEROFF:
                    {
                        bool result = pk.GetByteToInt() == 0x00;
                        PowerOffResultReceived?.Invoke(this, new ResultReceivedEventArgs(result ? ResultReceivedEventArgs.ResultType.Success : ResultReceivedEventArgs.ResultType.Failed));
                    }
                    break;

                case Cmd.UPDATE_START:
                    {
                        int result = pk.GetByteToInt();
                        int packetSize = pk.GetInt() - 8;
                        if (result == 0x00 && fileSplitter != null)
                        {
                            updateRetryCount = 0;

                            if (fileSplitter.Split(packetSize) && fileSplitter.PacketCount > 0)
                            {
                                UpdateProgressChanged?.Invoke(this, new ProgressChangedEventArgs(0));
                                UpdatePacketUploadRequest(0, fileSplitter.GetBytes(0));
                            }
                            else
                            {
                                isUpdating = false;
                                UpdateResultReceived?.Invoke(this, new UpdateResultReceivedEventArgs(UpdateResultReceivedEventArgs.ResultType.DeviceIsNotCorrect));
                            }
                        }
                        else
                        {
                            isUpdating = false;
                            UpdateResultReceived?.Invoke(this, new UpdateResultReceivedEventArgs(result == 1 ? UpdateResultReceivedEventArgs.ResultType.DeviceIsNotCorrect : UpdateResultReceivedEventArgs.ResultType.FirmwareVersionIsNotCorrect));
                        }
                    }
                    break;

                case Cmd.UPDATE_DO:
                    {
                        if (!isUpdating)
                            return;

                        int result = pk.GetByteToInt();
                        int offset = pk.GetInt();
                        
                        if (result == 0x00)
                        {
                            updateRetryCount = 0;

                            if (offset + fileSplitter.PacketSize >= fileSplitter.FileSize - 1)
                            {
                                UpdateProgressChanged?.Invoke(this, new ProgressChangedEventArgs(100));
                                UpdateResultReceived?.Invoke(this, new UpdateResultReceivedEventArgs(UpdateResultReceivedEventArgs.ResultType.Complete));
                            }
                            else
                            {
                                int progress = (int)((double)offset / (double)(fileSplitter.FileSize - 1) * 100);
                                UpdateProgressChanged?.Invoke(this, new ProgressChangedEventArgs(progress));
                                UpdatePacketUploadRequest(offset + fileSplitter.PacketSize, fileSplitter.GetBytes(offset + fileSplitter.PacketSize));
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("UPDATE_DO : {0} {1}", offset, result);

                            if (updateRetryCount++ > 3)
                            {
                                UpdateResultReceived?.Invoke(this, new UpdateResultReceivedEventArgs(UpdateResultReceivedEventArgs.ResultType.UnknownError));
                            }
                            else
                            {
                                UpdatePacketUploadRequest(offset, fileSplitter.GetBytes(offset));
                            }
                        }
                    }
                    break;
            }
        }

        private int downloadRetryCount = 0;
        private int updateRetryCount = 0;

        /// <summary>
        /// Terminates communication with the pen and returns the allocated resource.
        /// </summary>
        public void Dispose()
        {
            try
            {
                IsActive = false;

                _serialPort.DataReceived -= new SerialDataReceivedEventHandler(_serialPort_DataReceived);
                _serialPort.ErrorReceived -= new SerialErrorReceivedEventHandler(_serialPort_ErrorReceived);
                _serialPort.Dispose();
                _serialPort = null;

                Disconnected?.Invoke(this, null);
            }
            catch
            {
            }
        }

        private void _serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int size = _serialPort.BytesToRead;
            byte[] buffer = new byte[size];
            _serialPort.Read(buffer, 0, size);
            _parser.Put(buffer, buffer.Length);
        }

        private void StartRequest()
        {
            var builder = new UsbPacket.Builder();
            builder.Cmd(Cmd.START)
                .PacketNumber(packetNumber)
                .Type(PacketType.Request)
                .PutInt(2)
                .Put((byte)0x00);

            byte[] result = builder.Build().ToArray();

            _serialPort?.Write(result, 0, result.Length);
        }

        private void GetDeviceInfoRequest()
        {
            var builder = new UsbPacket.Builder();
            builder.Cmd(Cmd.GETDEVINFO)
                .PacketNumber(++packetNumber)
                .Type(PacketType.Request);

            byte[] result = builder.Build().ToArray();

            _serialPort?.Write(result, 0, result.Length);
        }

        private void GetConfigRequest(params ConfigType[] types)
        {
            var builder = new UsbPacket.Builder();
            builder.Cmd(Cmd.GETCONFIG)
                .PacketNumber(++packetNumber)
                .Type(PacketType.Request)
                .Put((byte)types.Length);

            foreach(var type in types)
            {
                builder.Put((byte)type);
            }

            byte[] result = builder.Build().ToArray();

            _serialPort?.Write(result, 0, result.Length);
        }

        /// <summary>
        /// Request the time setting of the current pen.
        /// </summary>
        /// <exception cref="IsNotActiveException">Occurs when communication with the pen is not established</exception>
        public void GetDateTimeRequest()
        {
            if (!IsActive)
                throw new IsNotActiveException();

            GetConfigRequest(ConfigType.DateTime);
        }

        /// <summary>
        /// Request the current battery status of the pen.
        /// </summary>
        /// <exception cref="IsNotActiveException">Occurs when communication with the pen is not established</exception>
        public void GetBatteryStatusRequest()
        {
            if (!IsActive)
                throw new IsNotActiveException();

            GetConfigRequest(ConfigType.Battery);
        }

        /// <summary>
        /// Request the storage state of the current pen.
        /// (Firmware not implemented)
        /// </summary>
        /// <exception cref="IsNotActiveException">Occurs when communication with the pen is not established</exception>
        public void GetStorageStatusRequest()
        {
            if (!IsActive)
                throw new IsNotActiveException();

            GetConfigRequest(ConfigType.Storage);
        }

        /// <summary>
        /// Set the date / time value of the current pen.
        /// </summary>
        /// <param name="localDateTime">Local date time value to set</param>
        /// <exception cref="IsNotActiveException">Occurs when communication with the pen is not established</exception>
        public void SetDateTimeRequest(System.DateTime localDateTime)
        {
            if (!IsActive)
                throw new IsNotActiveException();

            SetConfigRequest(ConfigType.DateTime, Time.GetUtcTimeStamp(localDateTime));
        }

        /// <summary>
        /// Requests to set the current pen's Auto Power Off Time value.
        /// </summary>
        /// <param name="timeSpan">A TimeSpan object representing the time to set</param>
        /// <exception cref="IsNotActiveException">Occurs when communication with the pen is not established</exception>
        public void SetAutoPowerOffTimeRequest(TimeSpan timeSpan)
        {
            if (!IsActive)
                throw new IsNotActiveException();

            SetConfigRequest(ConfigType.AutoPowerOffTime, (short)timeSpan.Minutes);
        }

        /// <summary>
        /// Sets whether to use the current pen's Auto Power On function.
        /// </summary>
        /// <param name="enable">True if used false if not</param>
        /// <exception cref="IsNotActiveException">Occurs when communication with the pen is not established</exception>
        public void SetIsAutoPowerOnEnabledRequest(bool enable)
        {
            if (!IsActive)
                throw new IsNotActiveException();

            SetConfigRequest(ConfigType.AutoPowerOn, enable);
        }

        /// <summary>
        /// Sets whether to use the current pen's PenCap Off function.
        /// </summary>
        /// <param name="enable">True if used false if not</param>
        /// <exception cref="IsNotActiveException">Occurs when communication with the pen is not established</exception>
        public void SetIsPenCapOffEnabledRequest(bool enable)
        {
            if (!IsActive)
                throw new IsNotActiveException();

            SetConfigRequest(ConfigType.PenCapOff, enable);
        }

        /// <summary>
        /// Sets whether the current pen beeps.
        /// </summary>
        /// <param name="enable">True if used false if not</param>
        /// <exception cref="IsNotActiveException">Occurs when communication with the pen is not established</exception>
        public void SetIsBeepEnabledRequest(bool enable)
        {
            if (!IsActive)
                throw new IsNotActiveException();

            SetConfigRequest(ConfigType.Beep, enable);
        }

        /// <summary>
        /// Sets whether to save offline data of current pen.
        /// </summary>
        /// <param name="enable">True if used false if not</param>
        /// <exception cref="IsNotActiveException">Occurs when communication with the pen is not established</exception>
        public void SetCanSaveOfflineDataRequest(bool enable)
        {
            if (!IsActive)
                throw new IsNotActiveException();

            SetConfigRequest(ConfigType.SaveOfflineData, enable);
        }

        /// <summary>
        /// Enables or disables downsampling of the current pen.
        /// </summary>
        /// <param name="enable">True if used false if not</param>
        /// <exception cref="IsNotActiveException">Occurs when communication with the pen is not established</exception>
        public void SetIsDownsamplingEnabledRequest(bool enable)
        {
            if (!IsActive)
                throw new IsNotActiveException();

            SetConfigRequest(ConfigType.DownSampling, enable);
        }

        private void SetConfigRequest(ConfigType type, object value)
        {
            var builder = new UsbPacket.Builder();
            builder.Cmd(Cmd.SETCONFIG)
                .PacketNumber(++packetNumber)
                .Type(PacketType.Request)
                .Put((byte)0x01)
                .Put((byte)type);

            switch (type)
            {
                case ConfigType.DateTime:
                    builder.PutLong((long)value);
                    break;
                case ConfigType.AutoPowerOffTime:
                    builder.PutShort((short)value);
                    break;
                case ConfigType.AutoPowerOn:
                case ConfigType.PenCapOff:
                case ConfigType.Beep:
                case ConfigType.SaveOfflineData:
                case ConfigType.DownSampling:
                    builder.Put((byte)((bool)value ? 0x01 : 0x00));
                    break;
            }

            byte[] result = builder.Build().ToArray();

            _serialPort?.Write(result, 0, result.Length);
        }

        /// <summary>
        /// Request to initialize the data of the current pen.
        /// </summary>
        /// <exception cref="IsNotActiveException">Occurs when communication with the pen is not established</exception>
        public void FormatRequest()
        {
            if (!IsActive)
                throw new IsNotActiveException();

            var builder = new UsbPacket.Builder();
            builder.Cmd(Cmd.FORMAT)
                .PacketNumber(++packetNumber)
                .Type(PacketType.Request)
                .Put((byte)0);

            byte[] result = builder.Build().ToArray();

            _serialPort?.Write(result, 0, result.Length);
        }

        /// <summary>
        /// Request a list of offline data files currently stored in the pen.
        /// </summary>
        /// <exception cref="IsNotActiveException">Occurs when communication with the pen is not established</exception>
        public void GetOfflineFileListRequest()
        {
            if (!IsActive)
                throw new IsNotActiveException();

            var builder = new UsbPacket.Builder();
            builder.Cmd(Cmd.GETOFFLINEDATALIST)
                .PacketNumber(++packetNumber)
                .Type(PacketType.Request);

            byte[] result = builder.Build().ToArray();

            _serialPort?.Write(result, 0, result.Length);
        }

        /// <summary>
        /// Request a list of log data files currently stored in the pen.
        /// </summary>
        /// <exception cref="IsNotActiveException">Occurs when communication with the pen is not established</exception>
        public void GetLogFileListRequest()
        {
            if (!IsActive)
                throw new IsNotActiveException();

            var builder = new UsbPacket.Builder();
            builder.Cmd(Cmd.GETLOGFILELIST)
                .PacketNumber(++packetNumber)
                .Type(PacketType.Request);

            byte[] result = builder.Build().ToArray();

            _serialPort?.Write(result, 0, result.Length);
        }

        private void FileRequest(FileType fileType, string fileName)
        {
            var filePathBytes = Encoding.UTF8.GetBytes(fileType == FileType.Data ? DataFilePath + fileName : LogFilePath + fileName);

            if (filePathBytes.Length > 64)
                throw new FileNameIsTooLongException();

            var builder = new UsbPacket.Builder();
            builder.Cmd(Cmd.GETFILE_H)
                .PacketNumber(++packetNumber)
                .Type(PacketType.Request)
                .Put(filePathBytes, 64);

            byte[] result = builder.Build().ToArray();

            _serialPort?.Write(result, 0, result.Length);
        }

        private bool fileRequestInfo = false;
        private string targetFilePath;
        private string requestFileName;
        private FileBuilder fileSerializer;

        /// <summary>
        /// Request the file details.
        /// </summary>
        /// <param name="fileType">FileType enum value to distinguish whether the file is log or offline data</param>
        /// <param name="fileName">Filename (56 byte length limit)</param>
        /// <exception cref="FileNameIsTooLongException">Occurs when file name is longer than 56 bytes</exception>
        /// <exception cref="IsNotActiveException">Occurs when communication with the pen is not established</exception>
        public void GetFileInfoRequest(FileType fileType, string fileName)
        {
            if (!IsActive)
                throw new IsNotActiveException();
            requestFileName = fileName;
            fileRequestInfo = true;
            FileRequest(fileType, fileName);
        }

        /// <summary>
        /// Request the file data.
        /// </summary>
        /// <param name="fileType">FileType enum value to distinguish whether the file is log or offline data</param>
        /// <param name="fileName">Filename (56 byte length limit)</param>
        /// <param name="targetPath">Where to save the file</param>
        /// <exception cref="FileNameIsTooLongException">Occurs when file name is longer than 56 bytes</exception>
        /// <exception cref="IsNotActiveException">Occurs when communication with the pen is not established</exception>
        public void GetFileDataRequest(FileType fileType, string fileName, string targetPath)
        {
            if (!IsActive)
                throw new IsNotActiveException();

            fileRequestInfo = false;
            targetFilePath = targetPath + fileName;
            FileRequest(fileType, fileName);
        }

        private void GetFilePacketRequest(int index)
        {
            var builder = new UsbPacket.Builder();
            builder.Cmd(Cmd.GETFILE_D)
                .PacketNumber(++packetNumber)
                .Type(PacketType.Request)
                .PutInt(index)
                .PutInt(fileSerializer.GetPacketSize(index));

            byte[] result = builder.Build().ToArray();

            _serialPort?.Write(result, 0, result.Length);
        }

        /// <summary>
        /// Request to delete the file.
        /// </summary>
        /// <param name="fileType">FileType enum value to distinguish whether the file is log or offline data</param>
        /// <param name="fileName">Filename (56 byte length limit)</param>
        /// <exception cref="FileNameIsTooLongException">Occurs when file name is longer than 56 bytes</exception>
        /// <exception cref="IsNotActiveException">Occurs when communication with the pen is not established</exception>
        public void DeleteFileRequest(FileType fileType, string fileName)
        {
            if (!IsActive)
                throw new IsNotActiveException();

            var filePathBytes = Encoding.UTF8.GetBytes(fileType == FileType.Data ? DataFilePath + fileName : LogFilePath + fileName);

            if (filePathBytes.Length > 64)
                throw new FileNameIsTooLongException();

            var builder = new UsbPacket.Builder();
            builder.Cmd(Cmd.DELETEFILE)
                .PacketNumber(++packetNumber)
                .Type(PacketType.Request)
                .Put(filePathBytes, 64);

            byte[] result = builder.Build().ToArray();

            _serialPort?.Write(result, 0, result.Length);
        }

        /// <summary>
        /// Shut down the current pen.
        /// </summary>
        /// <exception cref="IsNotActiveException">Occurs when communication with the pen is not established</exception>
        public void PowerOffRequest()
        {
            if (!IsActive)
                throw new IsNotActiveException();

            var builder = new UsbPacket.Builder();
            builder.Cmd(Cmd.POWEROFF)
                .PacketNumber(++packetNumber)
                .Type(PacketType.Request)
                .Put((byte)0);

            byte[] result = builder.Build().ToArray();

            _serialPort?.Write(result, 0, result.Length);
        }

        private FileSplitter fileSplitter;
        private bool isUpdating = false;

        /// <summary>
        /// Request to update the pen's firmware.
        /// </summary>
        /// <param name="firmwareVersion">Version of new firmware (24 byte length limit)</param>
        /// <param name="filePath">File path of new firmware</param>
        /// <exception cref="FirmwareVersionIsTooLongException">Occurs when firmware version is longer than 24 bytes</exception>
        /// <exception cref="FileCannotLoadException">Occurs when the firmware file at the specified location cannot be opened</exception>
        /// <exception cref="IsNotActiveException">Occurs when communication with the pen is not established</exception>
        public void UpdateRequest(string firmwareVersion, string filePath)
        {
            if (!IsActive)
                throw new IsNotActiveException();

            if (isUpdating)
                return;

            var deviceBytes = Encoding.UTF8.GetBytes(DeviceName);

            var firmwareVersionBytes = Encoding.UTF8.GetBytes(firmwareVersion);
            if (firmwareVersionBytes.Length > 24)
                throw new FirmwareVersionIsTooLongException();

            fileSplitter = new FileSplitter();
            if (!fileSplitter.Load(filePath))
            {
                throw new FileCannotLoadException();
            }

            isUpdating = true;

            var builder = new UsbPacket.Builder();
            builder.Cmd(Cmd.UPDATE_START)
                .PacketNumber(++packetNumber)
                .Type(PacketType.Request)
                .Put(deviceBytes, 24)
                .Put(firmwareVersionBytes, 24)
                .PutInt(fileSplitter.FileSize);

            byte[] result = builder.Build().ToArray();

            _serialPort?.Write(result, 0, result.Length);
        }

        private void UpdatePacketUploadRequest(int offset, byte[] data)
        {
            var builder = new UsbPacket.Builder();
            builder.Cmd(Cmd.UPDATE_DO)
                .PacketNumber(++packetNumber)
                .Type(PacketType.Request)
                .PutInt(offset)
                .PutInt(data.Length)
                .Put(data);

            byte[] result = builder.Build().ToArray();

            _serialPort?.Write(result, 0, result.Length);
        }

        #region Event
        /// <summary>
        /// Event occured when communication with the pen is established
        /// </summary>
        public event EventHandler Authenticated;
        /// <summary>
        /// Occurs when a request is made to the pen but the pen rejects it.
        /// </summary>
        public event EventHandler ConnectionRefused;
        /// <summary>
        /// Occurs when the connection to the pen is terminated.
        /// </summary>
        public event EventHandler Disconnected;
        /// <summary>
        /// Event that returns status value when battery status is requested
        /// </summary>
        public event EventHandler<BatteryStatusReceivedEventArgs> BatteryStatusReceived;
        /// <summary>
        /// Event that returns status value when storage status is requested
        /// </summary>
        public event EventHandler<StorageStatusReceivedEventArgs> StorageStatusReceived;
        /// <summary>
        /// Event that returns status when requesting current date / time setting of pen
        /// </summary>
        public event EventHandler<DateTimeReceivedEventArgs> DateTimeReceived;
        /// <summary>
        /// Event that returns whether or not the pen's settings changed.
        /// </summary>
        public event EventHandler<ConfigSetupResultReceivedEventArgs> ConfigSetupResultReceived;
        /// <summary>
        /// Event that returns result when requesting data initialization of pen
        /// </summary>
        public event EventHandler<ResultReceivedEventArgs> FormatResultReceived;
        /// <summary>
        /// Event that returns a value when requesting a list of offline data files on the pen
        /// </summary>
        public event EventHandler<FileListReceivedEventArgs> OfflineFileListReceived;
        /// <summary>
        /// Event that returns a value when requesting a list of log data files on the pen
        /// </summary>
        public event EventHandler<FileListReceivedEventArgs> LogFileListReceived;
        /// <summary>
        /// Event that returns the result when requesting the file details of the pen
        /// </summary>
        public event EventHandler<FileInfoReceivedEventArgs> FileInfoReceived;
        /// <summary>
        /// Event indicating the current transfer status when requesting file data transfer from the pen
        /// </summary>
        public event EventHandler<ProgressChangedEventArgs> FileDownloadProgressChanged;
        /// <summary>
        /// Event indicating the transfer result when requesting file data transfer from the pen
        /// </summary>
        public event EventHandler<FileDownloadResultReceivedEventArgs> FileDownloadResultReceived;
        /// <summary>
        /// Event that informs the result when the pen file is deleted
        /// </summary>
        public event EventHandler<ResultReceivedEventArgs> DeleteFileResultReceived;
        /// <summary>
        /// Event that informs result when requesting power off of pen
        /// </summary>
        public event EventHandler<ResultReceivedEventArgs> PowerOffResultReceived;
        /// <summary>
        /// Event that informs progress when the pen's firmware is updated
        /// </summary>
        public event EventHandler<ProgressChangedEventArgs> UpdateProgressChanged;
        /// <summary>
        /// Event that informs result you when the pen's firmware is updated
        /// </summary>
        public event EventHandler<UpdateResultReceivedEventArgs> UpdateResultReceived;
        #endregion
    }
}
