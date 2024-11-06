using Neosmartpen.Net.Encryption;
using Neosmartpen.Net.Filter;
using Neosmartpen.Net.Support;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Windows.Foundation;
using Windows.Storage;


namespace Neosmartpen.Net
{
    public class PenClientParserV2 : IPenClientParser
	{
		public class Const
		{
			public const byte PK_STX = 0xC0;
			public const byte PK_ETX = 0xC1;
			public const byte PK_DLE = 0x7D;

			public const int PK_POS_CMD = 1;
			public const int PK_POS_RESULT = 2;
			public const int PK_POS_LENG1 = 2;
			public const int PK_POS_LENG2 = 3;

			public const int PK_HEADER_SIZE = 3;
		}

		[Flags]
		public enum Cmd
		{
			VERSION_REQUEST = 0x01,
			VERSION_RESPONSE = 0x81,

			PASSWORD_REQUEST = 0x02,
			PASSWORD_RESPONSE = 0X82,

			PASSWORD_CHANGE_REQUEST = 0X03,
			PASSWORD_CHANGE_RESPONSE = 0X83,

			SETTING_INFO_REQUEST = 0X04,
			SETTING_INFO_RESPONSE = 0X84,

			LOW_BATTERY_EVENT = 0X61,
			SHUTDOWN_EVENT = 0X62,

			SETTING_CHANGE_REQUEST = 0X05,
			SETTING_CHANGE_RESPONSE = 0X85,

			ONLINE_DATA_REQUEST = 0X11,
			ONLINE_DATA_RESPONSE = 0X91,

			ONLINE_PEN_UPDOWN_EVENT = 0X63,
			ONLINE_PAPER_INFO_EVENT = 0X64,
			ONLINE_PEN_DOT_EVENT = 0X65,
            ONLINE_PEN_ERROR_EVENT = 0X68,

            ONLINE_NEW_PEN_DOWN_EVENT = 0X69,
            ONLINE_NEW_PEN_UP_EVENT = 0X6A,
            ONLINE_NEW_PAPER_INFO_EVENT = 0X6B,
            ONLINE_NEW_PEN_DOT_EVENT = 0X6C,
            ONLINE_NEW_PEN_ERROR_EVENT = 0X6D,

            ONLINE_ENCRYPTION_PAPER_INFO_EVENT = 0X6E,
            ONLINE_ENCRYPTION_PEN_DOT_EVENT = 0X6F,

            OFFLINE_NOTE_LIST_REQUEST = 0X21,
			OFFLINE_NOTE_LIST_RESPONSE = 0XA1,

			OFFLINE_PAGE_LIST_REQUEST = 0X22,
			OFFLINE_PAGE_LIST_RESPONSE = 0XA2,

			OFFLINE_DATA_REQUEST = 0X23,
			OFFLINE_DATA_RESPONSE = 0XA3,
			OFFLINE_PACKET_REQUEST = 0X24,
			OFFLINE_PACKET_RESPONSE = 0XA4,

			OFFLINE_DATA_DELETE_REQUEST = 0X25,
			OFFLINE_DATA_DELETE_RESPONSE = 0XA5,

			FIRMWARE_UPLOAD_REQUEST = 0X31,
			FIRMWARE_UPLOAD_RESPONSE = 0XB1,
			FIRMWARE_PACKET_REQUEST = 0X32,
			FIRMWARE_PACKET_RESPONSE = 0XB2,

			PEN_PROFILE_REQUEST =0x41,
			PEN_PROFILE_RESPONSE = 0xC1,

            AES_KEY_REQUEST = 0X76,
            AES_KEY_RESPONSE = 0XF6,

            PDS_COMMAND_EVENT = 0x73
        };

		public static readonly float PEN_PROFILE_SUPPORT_PROTOCOL_VERSION = 2.10f;
        public static readonly float COMPRESSED_UPLOAD_INFO_SUPPORT_PROTOCOL_VERSION = 2.22f;

		public static readonly string[] COMPRESSED_UPLOAD_DISABLED_DEVICES = new string[] {
			"NWP-F151",
			"NWP-F45",
			"NWP-F63",
			"NWP-F53MG",
			"NEP-E100",
			"NEP-E101",
			"NSP-D100",
			"NSP-D101",
			"NSP-C200",
			"NPP-P201"
		};
		
        private readonly string DEFAULT_PASSWORD = "0000";
		private readonly string F121 = "NWP-F121";
		private readonly string F121MG = "NWP-F121MG";

        public static readonly float FW_UPDATE_CANCEL_BUG_FIX_FIRMWARE_VERSION = 1.04f;
        public static readonly string FW_UPDATE_CANCEL_BUG_DEVICE_NAME  = "NWP-F51";

        public PenClientParserV2()
		{
            dotFilterForPaper = new FilterForPaper(SendDotReceiveEvent);
			offlineFilterForPaper = new FilterForPaper(AddOfflineFilteredDot);
		}

        public IPenClient PenClient { get; set; }

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

		public ushort DeviceType { get; private set; }

		public int PressureSensorType { get; private set; }

        public byte[] DeviceColorTypeId { get; private set; }

		public bool CompressedFileUploadEnabled { get; private set; } = true;

        /// <summary>
        /// Gets the maximum level of force sensor.
        /// </summary>
        public short MaxForce { get; private set; }

        public const string SupportedProtocolVersion = "2.12";

        private long mTime = -1L;

		private PenTipType mPenTipType = PenTipType.Normal;

		private int mPenTipColor = -1;

		public enum PenTipType { Normal = 0, Eraser = 1 };

		private bool IsStartWithDown = false;

		private int mDotCount = -1;

		private int mCurSection = -1, mCurOwner = -1, mCurNote = -1, mCurPage = -1;

		private int mTotalOfflineStroke = -1, mReceivedOfflineStroke = 0, mTotalOfflineDataSize = -1;
		private int PenMaxForce = 0;

		private bool reCheckPassword = false;
		private string newPassword;

		private FilterForPaper dotFilterForPaper = null;
		private FilterForPaper offlineFilterForPaper = null;

        private bool isConnectWrite = false;

		private AES256Chiper aesChiper;

		public bool HoverMode
		{
			get;
			private set;
		}

        int offlineDataPacketRetryCount = 0;
		
		public void ParsePacket(Packet packet)
		{
			Cmd cmd = (Cmd)packet.Cmd;

			//Debug.WriteLine("Cmd : {0}", cmd.ToString());

			isConnectWrite = true;
			switch (cmd)
			{
				case Cmd.VERSION_RESPONSE:
					{
						DeviceName = packet.GetString(16);
						FirmwareVersion = packet.GetString(16);
						ProtocolVersion = packet.GetString(8);
						SubName = packet.GetString(16);
						DeviceType = packet.GetUShort();
						MaxForce = -1;
						MacAddress = BitConverter.ToString(packet.GetBytes(6)).Replace("-", "");
						PressureSensorType = packet.CheckMoreData() ? packet.GetByte() : 0;

                        if (packet.CheckMoreData())
                        {
                            DeviceColorTypeId = packet.GetBytes(4);
                        }

                        try
						{
                            if (packet.CheckMoreData() && float.Parse(ProtocolVersion) >= COMPRESSED_UPLOAD_INFO_SUPPORT_PROTOCOL_VERSION)
                            {
                                CompressedFileUploadEnabled = packet.GetByte() != 0x00;
                            }
							else if (COMPRESSED_UPLOAD_DISABLED_DEVICES.Contains(DeviceName))
							{
								CompressedFileUploadEnabled = false;
                            }
						}
						catch
						{
                            CompressedFileUploadEnabled = false;
                        }

                        bool isMG = isF121MG(MacAddress);
						if (isMG && DeviceName.Equals(F121) && SubName.Equals("Mbest_smartpenS"))
						{
							DeviceName = F121MG;
						}

						IsUploading = false;

                        EventCount = 0;

						aesChiper = null;
						rsaChiper = null;

                        ReqPenStatus();
					}
					break;

				#region event
				case Cmd.SHUTDOWN_EVENT:
					{
						byte reason = packet.GetByte();
						Debug.Write(" => SHUTDOWN_EVENT : {0}", reason.ToString());

                        if (reason == 2)
                        {
                            // 업데이트를 위해 파워가 꺼지면 업데이트 완료 콜백
                            onReceiveFirmwareUpdateResult(new SimpleResultEventArgs(true));
                        }

                        OnDisconnected();
                    }
					break;

				case Cmd.LOW_BATTERY_EVENT:
					{
						int battery = (int)(packet.GetByte() & 0xff);

                        onReceiveBatteryAlarm(new BatteryAlarmReceivedEventArgs(battery));
                    }
					break;

				case Cmd.ONLINE_PEN_UPDOWN_EVENT:
				case Cmd.ONLINE_PEN_DOT_EVENT:
				case Cmd.ONLINE_PAPER_INFO_EVENT:
                case Cmd.ONLINE_PEN_ERROR_EVENT:
                case Cmd.ONLINE_NEW_PEN_DOWN_EVENT:
                case Cmd.ONLINE_NEW_PEN_UP_EVENT:
                case Cmd.ONLINE_NEW_PEN_DOT_EVENT:
                case Cmd.ONLINE_NEW_PAPER_INFO_EVENT:
                case Cmd.ONLINE_NEW_PEN_ERROR_EVENT:
                case Cmd.PDS_COMMAND_EVENT:
                    {
						ParseDotPacket(cmd, packet);
					}
					break;
				case Cmd.ONLINE_ENCRYPTION_PAPER_INFO_EVENT:
				case Cmd.ONLINE_ENCRYPTION_PEN_DOT_EVENT:
					{
						ParseEncryptionDotPacket(cmd, packet);
					}
					break;
				#endregion

				#region setting response
				case Cmd.SETTING_INFO_RESPONSE:
					{
						// 비밀번호 사용 여부
						bool lockyn = packet.GetByteToInt() == 1;

						// 비밀번호 입력 최대 시도 횟수
						int pwdMaxRetryCount = packet.GetByteToInt();

						// 비밀번호 입력 시도 횟수
						int pwdRetryCount = packet.GetByteToInt();

						// 1970년 1월 1일부터 millisecond tick
						long time = packet.GetLong();

						// 사용하지 않을때 자동으로 전원이 종료되는 시간 (단위:분)
						short autoPowerOffTime = packet.GetShort();

						// 최대 필압
						short maxForce = packet.GetShort();

						// 현재 메모리 사용량
						int usedStorage = packet.GetByteToInt();

						// 펜의 뚜껑을 닫아서 펜의 전원을 차단하는 기능 사용 여부
						bool penCapOff = packet.GetByteToInt() == 1;

						// 전원이 꺼진 펜에 필기를 시작하면 자동으로 펜의 켜지는 옵션 사용 여부
						bool autoPowerON = packet.GetByteToInt() == 1;

						// 사운드 사용여부
						bool beep = packet.GetByteToInt() == 1;

						// 호버기능 사용여부
						bool hover = packet.GetByteToInt() == 1;
                        HoverMode = hover;

                        // 남은 배터리 수치
                        int batteryLeft = packet.GetByteToInt();

						// 오프라인 데이터 저장 기능 사용 여부
						bool useOffline = packet.GetByteToInt() == 1;

						// 필압 단계 설정 (0~4) 0이 가장 민감
						short fsrStep = (short)packet.GetByteToInt();

						// 최초 연결시
						if (MaxForce == -1)
						{
							MaxForce = maxForce;

							var connectedEventArgs = new ConnectedEventArgs();

                            onConnected(new ConnectedEventArgs(MacAddress, DeviceName, FirmwareVersion, ProtocolVersion, SubName, MaxForce));
                            PenMaxForce = MaxForce;

							if (lockyn)
							{
                                onPenPasswordRequest(new PasswordRequestedEventArgs(pwdRetryCount, pwdMaxRetryCount));
                            }
							else
							{
								ReqSetupTime(Time.GetUtcTimeStamp());
								onPenAuthenticated();
								//AesKeyRequest();
							}
						}
						else
						{
                            onReceivePenStatus(new PenStatusReceivedEventArgs(lockyn, pwdMaxRetryCount, pwdRetryCount, time, autoPowerOffTime, MaxForce, batteryLeft, usedStorage, useOffline, autoPowerON, penCapOff, hover, beep, fsrStep));
                        }
					}
					break;

				case Cmd.SETTING_CHANGE_RESPONSE:
					{
						int inttype = packet.GetByteToInt();

						SettingType stype = (SettingType)inttype;

						bool result = packet.Result == 0x00;

						switch (stype)
						{
                            case SettingType.Timestamp:
                                onPenTimestampSetupResponse(new SimpleResultEventArgs(result));
                                break;
                                
                            case SettingType.AutoPowerOffTime:
                                onPenAutoShutdownTimeSetupResponse(new SimpleResultEventArgs(result));
                                break;

                            case SettingType.PenCapOff:
                                onPenCapPowerOnOffSetupResponse(new SimpleResultEventArgs(result));
                                break;

                            case SettingType.AutoPowerOn:
                                onPenAutoPowerOnSetupResponse(new SimpleResultEventArgs(result));
                                break;

							case SettingType.Beep:
                                onPenBeepSetupResponse(new SimpleResultEventArgs(result));
                                break;

							case SettingType.Hover:
                                onPenHoverSetupResponse(new SimpleResultEventArgs(result));
                                break;

                            case SettingType.OfflineData:
                                onPenOfflineDataSetupResponse(new SimpleResultEventArgs(result));
                                break;

                            case SettingType.LedColor:
                                onPenColorSetupResponse(new SimpleResultEventArgs(result));
                                break;

							case SettingType.Sensitivity:
                                onPenSensitivitySetupResponse(new SimpleResultEventArgs(result));
                                break;

                            case SettingType.UsbMode:
                                onUsbModeSetupResponse(new SimpleResultEventArgs(result));
                                break;

                            case SettingType.DownSampling:
                                onPenDownSamplingSetupResponse(new SimpleResultEventArgs(result));
                                break;

                            case SettingType.BtLocalName:
                                onPenBtLocalNameSetupResponse(new SimpleResultEventArgs(result));
                                break;

                            case SettingType.FscSensitivity:
                                onPenFscSensitivitySetupResponse(new SimpleResultEventArgs(result));
                                break;

                            case SettingType.DataTransmissionType:
                                onPenDataTransmissionTypeSetupResponse(new SimpleResultEventArgs(result));
                                break;

                            case SettingType.BeepAndLight:
                                onPenBeepAndLightSetupResponse(new SimpleResultEventArgs(result));
                                break;
                        }
					}
					break;
				#endregion

				#region password response
				case Cmd.PASSWORD_RESPONSE:
					{
						int status = packet.GetByteToInt();
						int cntRetry = packet.GetByteToInt();
						int cntMax = packet.GetByteToInt();

						if (status == 1)
						{
							if (reCheckPassword)
							{
                                onPenPasswordSetupResponse(new SimpleResultEventArgs(true));
                                reCheckPassword = false;
								break;
							}

							ReqSetupTime(Time.GetUtcTimeStamp());
                            onPenAuthenticated();
                        }
						else
						{
							if (reCheckPassword)
							{
								reCheckPassword = false;
                                onPenPasswordSetupResponse(new SimpleResultEventArgs(false));
                            }
							else
							{
                                onPenPasswordRequest(new PasswordRequestedEventArgs(cntRetry, cntMax));
                            }
						}
					}
					break;

				case Cmd.PASSWORD_CHANGE_RESPONSE:
					{
						int cntRetry = packet.GetByteToInt();
						int cntMax = packet.GetByteToInt();

						if (packet.Result == 0x00)
						{
							reCheckPassword = true;
							ReqInputPassword(newPassword);
						}
						else
						{
							newPassword = string.Empty;
                            onPenPasswordSetupResponse(new SimpleResultEventArgs(false));
                        }
					}
					break;
				#endregion

				#region offline response
				case Cmd.OFFLINE_NOTE_LIST_RESPONSE:
					{
						short length = packet.GetShort();

						List<OfflineDataInfo> result = new List<OfflineDataInfo>();

						for (int i = 0; i < length; i++)
						{
							byte[] rb = packet.GetBytes(4);

							int section = (int)(rb[3] & 0xFF);
							int owner = ByteConverter.ByteToInt(new byte[] { rb[0], rb[1], rb[2], (byte)0x00 });
							int note = packet.GetInt();

							result.Add(new OfflineDataInfo(section, owner, note));
						}

                        onReceiveOfflineDataList(new OfflineDataListReceivedEventArgs(result.ToArray()));
                    }
					break;

				case Cmd.OFFLINE_PAGE_LIST_RESPONSE:
					{
						byte[] rb = packet.GetBytes(4);

						int section = (int)(rb[3] & 0xFF);
						int owner = ByteConverter.ByteToInt(new byte[] { rb[0], rb[1], rb[2], (byte)0x00 });
						int note = packet.GetInt();

						short length = packet.GetShort();

						int[] pages = new int[length];

						for (int i = 0; i < length; i++)
						{
							pages[i] = packet.GetInt();
						}

						OfflineDataInfo info = new OfflineDataInfo(section, owner, note, pages);

                        onReceiveOfflineDataList(new OfflineDataListReceivedEventArgs(info));
                    }
					break;

				case Cmd.OFFLINE_DATA_RESPONSE:
					{
						mTotalOfflineStroke = packet.GetInt();
						mReceivedOfflineStroke = 0;
						mTotalOfflineDataSize = packet.GetInt();

						bool isCompressed = packet.GetByte() == 1;

                        if (mTotalOfflineStroke == 0)
                        {
                            onFinishedOfflineDownload(new SimpleResultEventArgs(false));
                        }
                        else
                        {
                            onStartOfflineDownload();
                        }

                    }
					break;

				case Cmd.OFFLINE_PACKET_REQUEST:
					{
						#region offline data parsing

						List<Stroke> result = new List<Stroke>();

						short packetId = packet.GetShort();

						bool isCompressed = packet.GetByte() == 1;

						short sizeBefore = packet.GetShort();
						short sizeAfter = packet.GetShort();

						short location = (short)(packet.GetByte() & 0xFF);

						byte[] rb = packet.GetBytes(4);
						int section = (int)(rb[3] & 0xFF);
						int owner = ByteConverter.ByteToInt(new byte[] { rb[0], rb[1], rb[2], (byte)0x00 });
						int note = packet.GetInt();

						short strCount = packet.GetShort();

						mReceivedOfflineStroke += strCount;

						Debug.WriteLine($"packetId : {packetId}, isCompressed : {isCompressed}, sizeBefore : {sizeBefore}, sizeAfter : {sizeAfter}, size : {packet.Data.Length - 18}");

						if (sizeAfter != (packet.Data.Length - 18))
						{
							if ( offlineDataPacketRetryCount < 3)
							{
								SendOfflinePacketResponse(packetId, false);
								++offlineDataPacketRetryCount;
							}
							else
							{
								offlineDataPacketRetryCount = 0;
                                onFinishedOfflineDownload(new SimpleResultEventArgs(false));
                            }
							return;
						}

						byte[] compressedData = packet.GetBytes(sizeAfter);

						byte[] decompressedData = Compression.Decompress(compressedData);

						if (decompressedData.Length != sizeBefore)
						{
							if ( offlineDataPacketRetryCount < 3)
							{
								SendOfflinePacketResponse(packetId, false);
								++offlineDataPacketRetryCount;
							}
							else
							{
								offlineDataPacketRetryCount = 0;
                                onFinishedOfflineDownload(new SimpleResultEventArgs(false));
                            }
							return;
						}

						ByteUtil butil = new ByteUtil(decompressedData);

                        int checksumErrorCount = 0;

						for (int i = 0; i < strCount; i++)
						{
							int pageId = butil.GetInt();

							long timeStart = butil.GetLong();
							long timeEnd = butil.GetLong();

							int penTipType = (int)(butil.GetByte() & 0xFF);

							int color = butil.GetInt();

							short dotCount = butil.GetShort();

							long time = timeStart;

							//System.Console.WriteLine( "pageId : {0}, timeStart : {1}, timeEnd : {2}, penTipType : {3}, color : {4}, dotCount : {5}, time : {6},", pageId, timeStart, timeEnd, penTipType, color, dotCount, time );

							offlineStroke = new Stroke(section, owner, note, pageId);

							for (int j = 0; j < dotCount; j++)
							{
								byte dotChecksum = butil.GetChecksum(15);

								int timeadd = butil.GetByte();

								time += timeadd;

								int force = butil.GetShort();

								int x = butil.GetUShort();
								int y = butil.GetUShort();

								int fx = butil.GetByte();
								int fy = butil.GetByte();

								int tx = butil.GetByte();
								int ty = butil.GetByte();

								int twist = butil.GetShort();

								short reserved = butil.GetShort();

								byte checksum = butil.GetByte();

								//System.Console.WriteLine( "x : {0}, y : {1}, force : {2}, checksum : {3}, dotChecksum : {4}", tx, ty, twist, checksum, dotChecksum );

								if (dotChecksum != checksum)
								{
                                    // 체크섬 에러 3번 이상이면 에러로 전송 종료
                                    if (checksumErrorCount++ > 1)
                                    {
                                        result.Clear();
                                        onFinishedOfflineDownload(new SimpleResultEventArgs(false));
                                        return;
                                    }

									continue;
								}

								DotTypes dotType;

								if (j == 0)
								{
									dotType = DotTypes.PEN_DOWN;
								}
								else if (j == dotCount - 1)
								{

									dotType = DotTypes.PEN_UP;
								}
								else
								{
									dotType = DotTypes.PEN_MOVE;
								}

								offlineFilterForPaper.Put(MakeDot(PenMaxForce, owner, section, note, pageId, time, x, y, fx, fy, force, dotType, color), null);
								//stroke.Add(MakeDot(PenMaxForce, owner, section, note, pageId, time, x, y, fx, fy, force, dotType, color));
							}

							result.Add(offlineStroke);
						}

						SendOfflinePacketResponse(packetId);

						offlineDataPacketRetryCount = 0;

						onReceiveOfflineStrokes(new OfflineStrokeReceivedEventArgs(mTotalOfflineStroke, mReceivedOfflineStroke, result.ToArray()));

						if (location == 2)
						{
							onFinishedOfflineDownload(new SimpleResultEventArgs(true));
						}

						#endregion
					}
					break;

				case Cmd.OFFLINE_DATA_DELETE_RESPONSE:
					{
						onRemovedOfflineData(new SimpleResultEventArgs(packet.Result == 0x00));
					}
					break;
				#endregion

				#region firmware response
				case Cmd.FIRMWARE_UPLOAD_RESPONSE:
					{
						if (packet.Result != 0 || packet.GetByteToInt() != 0)
						{
							IsUploading = false;
							onReceiveFirmwareUpdateResult(new SimpleResultEventArgs(false));
						}
					}
					break;

				case Cmd.FIRMWARE_PACKET_REQUEST:
					{
						int status = packet.GetByteToInt();
						int offset = packet.GetInt();
                        ResponseChunkRequest(offset, status != 3);
					}
					break;
				#endregion

				#region Pen Profile
				case Cmd.PEN_PROFILE_RESPONSE:
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
							else if ( type == PenProfile.PROFILE_READ_VALUE )
							{
								eventArgs = PenProfileReadValue(profileName, packet);
							}
							else if ( type == PenProfile.PROFILE_WRITE_VALUE )
							{
								eventArgs = PenProfileWriteValue(profileName, packet);
							}
							else if ( type == PenProfile.PROFILE_DELETE_VALUE)
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

				#region Encryption
				case Cmd.AES_KEY_RESPONSE:
					byte[] keyBytes = packet.GetBytes(256);

					if (rsaChiper != null)
					{
						//Debug.WriteLine("Receive data Enc : " + BitConverter.ToString(keyBytes));
						var aesKey = rsaChiper.Decrypt(keyBytes);

						//Debug.WriteLine("Receive data Enc : " + BitConverter.ToString(aesKey));
						//Debug.WriteLine("Receive data Enc : " + Encoding.UTF8.GetString(aesKey));
						aesChiper = new AES256Chiper(aesKey);
						rsaChiper = null;
					}
					break;
				#endregion

				case Cmd.ONLINE_DATA_RESPONSE:
                    onAvailableNoteAdded();
                    break;

				default:
					break;
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
			catch(Exception exp)
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
				for(int i =0; i < count; ++i)
				{
					var result = new PenProfileWriteValueEventArgs.WriteValueResult();
					result.Key = packet.GetString(16);
					result.Status = packet.GetByte();
					args.Data.Add(result);
				}
			}
			catch(Exception exp)
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
				for(int i = 0; i < count; ++i)
				{
					var result = new PenProfileDeleteValueEventArgs.DeleteValueResult();
					result.Key = packet.GetString(16);
					result.Status = packet.GetByte();
					args.Data.Add(result);
				}
			}
			catch(Exception exp)
			{
				Debug.WriteLine(exp.StackTrace);
			}

			return args;
		}
		#endregion

        private Dot mPrevDot = null;

        private bool IsBeforeMiddle = false;

        private bool IsStartWithPaperInfo = false;

        private bool IsDownCreated = false;

        private long SessionTs = -1;

        private int EventCount = -1;

        private void CheckEventCount(int ecount)
        {
            //Debug.WriteLine("COUNT : " + ecount + ", " + EventCount);

            if (ecount - EventCount != 1 && (ecount != 0 || EventCount != 255))
            {
                // 이벤트 카운트 오류
                Dot errorDot = null;

                if (mPrevDot != null)
                {
                    errorDot = mPrevDot.Clone();
                    errorDot.DotType = DotTypes.PEN_ERROR;
                }

                if (ecount - EventCount > 1)
                {
                    string extraData = string.Format("missed event count {0}-{1}", EventCount + 1, ecount - 1);
                    onErrorDetected(new ErrorDetectedEventArgs(ErrorType.InvalidEventCount, errorDot, SessionTs, extraData));
                }
                else if (ecount < EventCount)
                {
                    string extraData = string.Format("invalid event count {0},{1}", EventCount, ecount);
                    onErrorDetected(new ErrorDetectedEventArgs(ErrorType.InvalidEventCount, errorDot, SessionTs, extraData));
                }
            }

            EventCount = ecount;
        }

        private void ParseDotPacket(Cmd cmd, Packet pk)
		{
            //Debug.Write("CMD : " + cmd.ToString() + ", ");

            switch (cmd)
			{
                case Cmd.ONLINE_NEW_PEN_DOWN_EVENT:
                    {
                        if (IsStartWithDown && IsBeforeMiddle && mPrevDot != null)
                        {
							MakeUpDot();
						}

						int ecount = pk.GetByteToInt();

                        CheckEventCount(ecount);

                        IsStartWithDown = true;

                        mTime = pk.GetLong();

                        SessionTs = mTime;

                        IsBeforeMiddle = false;
                        IsStartWithPaperInfo = false;

                        IsDownCreated = false;

                        mDotCount = 0;

                        mPenTipType = pk.GetByte() == 0x00 ? PenTipType.Normal : PenTipType.Eraser;
                        mPenTipColor = pk.GetInt();

                        mPrevDot = null;
                    }
                    break;

                case Cmd.ONLINE_NEW_PEN_UP_EVENT:
                    {
                        int ecount = pk.GetByteToInt();

                        CheckEventCount(ecount);

                        long timestamp = pk.GetLong();

                        int dotCount = pk.GetShort();
                        int totalImageCount = pk.GetShort();
                        int procImageCount = pk.GetShort();
                        int succImageCount = pk.GetShort();
                        int sendImageCount = pk.GetShort();

                        if (IsStartWithDown && IsBeforeMiddle && mPrevDot != null)
                        {
                            var udot = mPrevDot.Clone();
                            udot.DotType = DotTypes.PEN_UP;

                            ImageProcessingInfo imageInfo = null;

                            if (!IsDownCreated)
                            {
                                imageInfo = new ImageProcessingInfo
                                {
                                    DotCount = dotCount,
                                    Total = totalImageCount,
                                    Processed = procImageCount,
                                    Success = succImageCount,
                                    Transferred = sendImageCount
                                };
                            }

                            ProcessDot(udot, imageInfo);
                        }
                        else if (!IsStartWithDown && !IsBeforeMiddle)
                        {
                            // 즉 다운업(무브없이) 혹은 업만 들어올 경우 UP dot을 보내지 않음
                            onErrorDetected(new ErrorDetectedEventArgs(ErrorType.MissingPenDownPenMove, -1));
                        }
                        else if (!IsBeforeMiddle)
                        {
                            // 무브없이 다운-업만 들어올 경우 UP dot을 보내지 않음
                            onErrorDetected(new ErrorDetectedEventArgs(ErrorType.MissingPenMove, SessionTs));
                        }

                        mTime = -1;
                        SessionTs = -1;

                        IsStartWithDown = false;
                        IsBeforeMiddle = false;
                        IsStartWithPaperInfo = false;
                        IsDownCreated = false;

                        mDotCount = 0;

                        mPrevDot = null;
                    }
                    break;

                case Cmd.ONLINE_PEN_UPDOWN_EVENT:
                    {
                        bool IsDown = pk.GetByte() == 0x00;

                        if (IsDown)
                        {
                            if (IsStartWithDown && IsBeforeMiddle && mPrevDot != null)
                            {
                                // 펜업이 넘어오지 않음
								MakeUpDot();
                            }

                            IsStartWithDown = true;

                            mTime = pk.GetLong();

                            SessionTs = mTime;
                        }
                        else
                        {
							if (IsStartWithDown && IsBeforeMiddle && mPrevDot != null)
                            {
								MakeUpDot(false);
                            }
                            else if (!IsStartWithDown && !IsBeforeMiddle)
                            {
                                // 즉 다운업(무브없이) 혹은 업만 들어올 경우 UP dot을 보내지 않음
                                onErrorDetected(new ErrorDetectedEventArgs(ErrorType.MissingPenDownPenMove, -1));
                            }
                            else if (!IsBeforeMiddle)
                            {
                                // 무브없이 다운-업만 들어올 경우 UP dot을 보내지 않음
                                onErrorDetected(new ErrorDetectedEventArgs(ErrorType.MissingPenMove, SessionTs));
                            }

                            IsStartWithDown = false;

                            mTime = -1;

                            SessionTs = -1;
                        }

                        IsBeforeMiddle = false;
                        IsStartWithPaperInfo = false;
                        IsDownCreated = false;

                        mDotCount = 0;

                        mPenTipType = pk.GetByte() == 0x00 ? PenTipType.Normal : PenTipType.Eraser;
                        mPenTipColor = pk.GetInt();

                        mPrevDot = null;
                    }
                    break;

				case Cmd.ONLINE_PEN_DOT_EVENT:
                case Cmd.ONLINE_NEW_PEN_DOT_EVENT:
                    {
						PenDotEvent(cmd, pk);
                    }
					break;

				case Cmd.ONLINE_PAPER_INFO_EVENT:
                case Cmd.ONLINE_NEW_PAPER_INFO_EVENT:
                    {
						PaperInfoEvent(cmd, pk);
                    }
                    break;
                    
                case Cmd.ONLINE_PEN_ERROR_EVENT:
                case Cmd.ONLINE_NEW_PEN_ERROR_EVENT:
                    {
                        if (cmd == Cmd.ONLINE_NEW_PEN_ERROR_EVENT)
                        {
                            int ecount = pk.GetByteToInt();

                            CheckEventCount(ecount);
                        }

                        int timeadd = pk.GetByteToInt();
                        mTime += timeadd;

                        int force = pk.GetShort();
                        int brightness = pk.GetByteToInt();
                        int exposureTime = pk.GetByteToInt();
                        int ndacProcessTime = pk.GetByteToInt();
                        int labelCount = pk.GetShort();
                        int ndacErrorCode = pk.GetByteToInt();
                        int classType = pk.GetByteToInt();
                        int errorCount = pk.GetByteToInt();

                        ImageProcessErrorInfo newInfo = new ImageProcessErrorInfo {
                            Timestamp = mTime,
                            Force = force,
                            Brightness = brightness,
                            ExposureTime = exposureTime,
                            ProcessTime = ndacProcessTime,
                            LabelCount = labelCount,
                            ErrorCode = ndacErrorCode,
                            ClassType = classType,
                            ErrorCount = errorCount
                        };

                        Dot errorDot = null;

                        if (mPrevDot != null)
                        {
                            errorDot = mPrevDot.Clone();
                            errorDot.DotType = DotTypes.PEN_UP;
                        }

                        onErrorDetected(new ErrorDetectedEventArgs(ErrorType.ImageProcessingError, errorDot, SessionTs, newInfo));
                    }
                    break;
                case Cmd.PDS_COMMAND_EVENT:
                    int owner = pk.GetInt();
                    int section = pk.GetInt();
                    int contents = pk.GetInt();
                    int page = pk.GetInt();
                    int x = pk.GetInt();
                    int y = pk.GetInt();
                    short fx = pk.GetShort();
                    short fy = pk.GetShort();

                    onPdsReceived(new PdsReceivedEventArgs(new Pds(section, owner, contents, page, x + (fx * 0.01f), y + (fy * 0.01f))));
                    break;
            }
		}

        private void ParseEncryptionDotPacket(Cmd cmd, Packet pk)
		{
			if (aesChiper == null)	// Do not ready to decoding
				throw new NullReferenceException("Do not ready to aes encryption");

			var decrypt = aesChiper.Decrypt(pk.Data);
			if ( decrypt == null )
			{
				// TODO Decoding failed
				return;
			}
			var newPacket = new Packet.Builder();
			newPacket.cmd(pk.Cmd);
			newPacket.result(pk.Result);
			newPacket.data(decrypt);
			switch(cmd)
			{
				case Cmd.ONLINE_ENCRYPTION_PAPER_INFO_EVENT:
					PaperInfoEvent(cmd, newPacket.Build());
					break;
				case Cmd.ONLINE_ENCRYPTION_PEN_DOT_EVENT:
					PenDotEvent(cmd, newPacket.Build());
					break;
			}
		}

		private void PaperInfoEvent(Cmd cmd, Packet pk)
		{
			if (cmd == Cmd.ONLINE_NEW_PAPER_INFO_EVENT || cmd == Cmd.ONLINE_ENCRYPTION_PAPER_INFO_EVENT)
			{
				int ecount = pk.GetByteToInt();

				CheckEventCount(ecount);
			}

			// 미들도트 중에 페이지가 바뀐다면 강제로 펜업을 만들어 준다.
			if (IsStartWithDown && IsBeforeMiddle && mPrevDot != null)
			{
				MakeUpDot(false);
			}

			byte[] rb = pk.GetBytes(4);

			mCurSection = (int)(rb[3] & 0xFF);
			mCurOwner = ByteConverter.ByteToInt(new byte[] { rb[0], rb[1], rb[2], (byte)0x00 });
			mCurNote = pk.GetInt();
			mCurPage = pk.GetInt();

			mDotCount = 0;

			IsStartWithPaperInfo = true;
		}

		private void PenDotEvent(Cmd cmd, Packet pk)
		{
			if (cmd == Cmd.ONLINE_NEW_PEN_DOT_EVENT || cmd == Cmd.ONLINE_ENCRYPTION_PEN_DOT_EVENT)
			{
				int ecount = pk.GetByteToInt();

				CheckEventCount(ecount);
			}

			int timeadd = pk.GetByte();

			mTime += timeadd;

			int force = pk.GetShort();

			int x = pk.GetUShort();
			int y = pk.GetUShort();

			int fx = pk.GetByte();
			int fy = pk.GetByte();

			if (!HoverMode && !IsStartWithDown)
			{
				if (!IsStartWithPaperInfo)
				{
					//펜 다운 없이 페이퍼 정보 없고 무브가 오는 현상(다운 - 무브 - 업 - 다운X - 무브)
					onErrorDetected(new ErrorDetectedEventArgs(ErrorType.MissingPenDown, -1));
				}
				else
				{
					mTime = Time.GetUtcTimeStamp();

					SessionTs = mTime;

					var errorDot = MakeDot(PenMaxForce, mCurOwner, mCurSection, mCurNote, mCurPage, mTime, x, y, fx, fy, force, DotTypes.PEN_ERROR, mPenTipColor);

					//펜 다운 없이 페이퍼 정보 있고 무브가 오는 현상(다운 - 무브 - 업 - 다운X - 무브)
					onErrorDetected(new ErrorDetectedEventArgs(ErrorType.MissingPenDown, errorDot, SessionTs));

					IsStartWithDown = true;
					IsDownCreated = true;

				}
			}

            Dot dot = null;

            if (HoverMode && !IsStartWithDown && mCurOwner > -1 && mCurSection > -1 && mCurNote > -1 && mCurPage > -1)
			{
				dot = MakeDot(PenMaxForce, mCurOwner, mCurSection, mCurNote, mCurPage, mTime, x, y, fx, fy, force, DotTypes.PEN_HOVER, mPenTipColor);
			}
			else if (IsStartWithDown)
			{
				if (IsStartWithPaperInfo)
				{
					dot = MakeDot(PenMaxForce, mCurOwner, mCurSection, mCurNote, mCurPage, mTime, x, y, fx, fy, force, mDotCount == 0 ? DotTypes.PEN_DOWN : DotTypes.PEN_MOVE, mPenTipColor);
				}
				else
				{
					//펜 다운 이후 페이지 체인지 없이 도트가 들어왔을 경우
					onErrorDetected(new ErrorDetectedEventArgs(ErrorType.MissingPageChange, SessionTs));
				}
			}

			if (dot != null)
			{
				ProcessDot(dot, null);
			}

			IsBeforeMiddle = true;
			mPrevDot = dot;
			mDotCount++;
		}

        public void OnDisconnected()
        {
            if (IsStartWithDown && IsBeforeMiddle && mPrevDot != null)
            {
                MakeUpDot();

                mTime = -1;
                SessionTs = -1;

                IsStartWithDown = false;
                IsBeforeMiddle = false;
                IsStartWithPaperInfo = false;

                mDotCount = 0;

                mPrevDot = null;
            }

            onDisconnected();
        }

        private void ProcessDot(Dot dot, object obj)
		{
            //dotFilterForPaper.Put(dot, obj);
            SendDotReceiveEvent(dot, obj);
        }

		private void SendDotReceiveEvent(Dot dot, object obj)
		{
			onReceiveDot(new DotReceivedEventArgs(dot, obj == null? null : (ImageProcessingInfo)obj));
		}

		private Stroke offlineStroke;
		private void AddOfflineFilteredDot(Dot dot, object obj)
		{
			offlineStroke.Add(dot);
		}

		private void ParseDot(Packet mPack, DotTypes type)
		{
			int timeadd = mPack.GetByte();

			mTime += timeadd;

			int force = mPack.GetShort();

			int x = mPack.GetUShort();
			int y = mPack.GetUShort();

			int fx = mPack.GetByte();
			int fy = mPack.GetByte();

			int tx = mPack.GetByte();
			int ty = mPack.GetByte();

			int twist = mPack.GetShort();

			ProcessDot(MakeDot(PenMaxForce, mCurOwner, mCurSection, mCurNote, mCurPage, mTime, x, y, fx, fy, force, type, mPenTipColor), null);
		}

		private void MakeUpDot(bool isError = true)
		{
            if (isError)
            {
                var errorDot = mPrevDot.Clone();
                errorDot.DotType = DotTypes.PEN_ERROR;
                onErrorDetected(new ErrorDetectedEventArgs(ErrorType.MissingPenUp, errorDot, SessionTs));
            }

            var audot = mPrevDot.Clone();
			audot.DotType = DotTypes.PEN_UP;
			ProcessDot(audot, null);
        }

		private byte[] Escape(byte input)
		{
			if (input == Const.PK_STX || input == Const.PK_ETX || input == Const.PK_DLE)
			{
				return new byte[] { Const.PK_DLE, (byte)(input ^ 0x20) };
			}
			else
			{
				return new byte[] { input };
			}
		}

		private bool Send(ByteUtil bf)
		{
			PenClient?.Write(bf.ToArray());

			bf.Clear();
			bf = null;

			return true;
		}

		public void ReqVersion()
		{
			ByteUtil bf = new ByteUtil(Escape);

			// TODO 정상적으로 넘어오는지 확인이 필요하다.
			Assembly assemObj = this.GetType().GetTypeInfo().Assembly;
			Version v = assemObj.GetName().Version; // 현재 실행되는 어셈블리..dll의 버전 가져오기

			byte[] StrAppVersion = Encoding.UTF8.GetBytes(String.Format("{0}.{1}.{2}.{3}", v.Major, v.Minor, v.Build, v.Revision));
            byte[] StrProtocolVersion = Encoding.UTF8.GetBytes(SupportedProtocolVersion);

            bf.Put(Const.PK_STX, false)
			  .Put((byte)Cmd.VERSION_REQUEST)
			  .PutShort(42)
			  .PutNull(16)
			  .Put(0x12)
			  .Put(0x01)
			  .Put(StrAppVersion, 16)
              .Put(StrProtocolVersion, 8)
              .Put(Const.PK_ETX, false);

			Send(bf);
		}

        private static bool doTryReqVersion = false;
        public async void ReqVersionTask()
        {
            isConnectWrite = false;
            if (doTryReqVersion)
                return;
            await System.Threading.Tasks.Task.Factory.StartNew(async () =>
            {
                doTryReqVersion = true;
                for (int i = 0; i < 3; ++i)
                {
                    Debug.WriteLine($"Connection Task Try {i + 1}");
                    if (isConnectWrite == false)
                    {
                        ReqVersion();

                        await System.Threading.Tasks.Task.Delay(2000);
                    }
                    else
                        break;
                }
                Debug.WriteLine($"Connection Finish");
                if (isConnectWrite == false)
                {
                    OnDisconnected();
                    //PenClient?.Unbind();
                }

                doTryReqVersion = false;
            });
        }

        #region password

        /// <summary>
        /// Change the password of device.
        /// </summary>
        /// <param name="oldPassword">Current password</param>
        /// <param name="newPassword">New password</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqSetUpPassword(string oldPassword, string newPassword = "")
		{
			if (oldPassword == null || newPassword == null)
				return false;

			if (oldPassword.Equals(DEFAULT_PASSWORD))
				return false;
			if (newPassword.Equals(DEFAULT_PASSWORD))
				return false;

			this.newPassword = newPassword;

			byte[] oPassByte = Encoding.UTF8.GetBytes(oldPassword);
			byte[] nPassByte = Encoding.UTF8.GetBytes(newPassword);

			ByteUtil bf = new ByteUtil(Escape);
			bf.Put(Const.PK_STX, false)
			  .Put((byte)Cmd.PASSWORD_CHANGE_REQUEST)
			  .PutShort(33)
			  .Put((byte)(newPassword == "" ? 0 : 1))
			  .Put(oPassByte, 16)
			  .Put(nPassByte, 16)
			  .Put(Const.PK_ETX, false);

			return Send(bf);
		}

        /// <summary>
        /// Remove the password of device.
        /// </summary>
        /// <param name="oldPassword">Current password</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqRemovePassword(string oldPassword)
        {
            return ReqSetUpPassword(oldPassword);
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

			ByteUtil bf = new ByteUtil(Escape);
			bf.Put(Const.PK_STX, false)
			  .Put((byte)Cmd.PASSWORD_REQUEST)
			  .PutShort(16)
			  .Put(bStrByte, 16)
			  .Put(Const.PK_ETX, false);

			return Send(bf);
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
			ByteUtil bf = new ByteUtil(Escape);

			bf.Put(Const.PK_STX, false)
				.Put((byte)Cmd.SETTING_INFO_REQUEST)
				.PutShort(0)
				.Put(Const.PK_ETX, false);

			return Send(bf);
		}

		public enum SettingType : byte { Timestamp = 1, AutoPowerOffTime = 2, PenCapOff = 3, AutoPowerOn = 4, Beep = 5, Hover = 6, OfflineData = 7, LedColor = 8, Sensitivity = 9, UsbMode = 10, DownSampling = 11, BtLocalName = 12, FscSensitivity = 13, DataTransmissionType = 14, BeepAndLight = 16 };

		private bool RequestChangeSetting(SettingType stype, object value)
		{
			ByteUtil bf = new ByteUtil(Escape);

			bf.Put(Const.PK_STX, false).Put((byte)Cmd.SETTING_CHANGE_REQUEST);

			switch (stype)
			{
				case SettingType.Timestamp:
					bf.PutShort(9).Put((byte)stype).PutLong((long)value);
					break;

				case SettingType.AutoPowerOffTime:
					bf.PutShort(3).Put((byte)stype).PutShort((short)value);
					break;

				case SettingType.LedColor:
					byte[] b = BitConverter.GetBytes((int)value);
					byte[] nBytes = new byte[] { b[3], b[2], b[1], b[0] };
					bf.PutShort(5).Put((byte)stype).Put(nBytes, 4);

					//bf.PutShort(5).Put((byte)stype).PutInt((int)value);
					break;

				case SettingType.PenCapOff:
				case SettingType.AutoPowerOn:
				case SettingType.Beep:
				case SettingType.Hover:
				case SettingType.OfflineData:
                case SettingType.DownSampling:
                    bf.PutShort(2).Put((byte)stype).Put((byte)((bool)value ? 1 : 0));
					break;
				case SettingType.Sensitivity:
					bf.PutShort(2).Put((byte)stype).Put((byte)((short)value));
					break;
                case SettingType.UsbMode:
                    bf.PutShort(2).Put((byte)stype).Put((byte)value);
                    break;
                case SettingType.BtLocalName:
                    byte[] StrByte = Encoding.UTF8.GetBytes((string)value);
                    bf.PutShort(18).Put((byte)stype).Put(16).Put(StrByte, 16);
                    break;
                case SettingType.FscSensitivity:
                    bf.PutShort(2).Put((byte)stype).Put((byte)((short)value));
                    break;
                case SettingType.DataTransmissionType:
                    bf.PutShort(2).Put((byte)stype).Put((byte)value);
                    break;
                case SettingType.BeepAndLight:
                    bf.PutShort(2).Put((byte)stype).Put((byte)0);
                    break;
            }

			bf.Put(Const.PK_ETX, false);

			return Send(bf);
		}

		/// <summary>
		/// Sets the RTC timestamp.
		/// </summary>
		/// <param name="timetick">milisecond timestamp tick (from 1970-01-01)</param>
		/// <returns>true if the request is accepted; otherwise, false.</returns>
		public bool ReqSetupTime(long timetick)
		{
			return RequestChangeSetting(SettingType.Timestamp, timetick);
		}

		/// <summary>
		/// Sets the value of the auto shutdown time property that if pen stay idle, shut off the pen.
		/// </summary>
		/// <param name="minute">minute of maximum idle time, staying power on (0~)</param>
		/// <returns>true if the request is accepted; otherwise, false.</returns>
		public bool ReqSetupPenAutoShutdownTime(short minute)
		{
			return RequestChangeSetting(SettingType.AutoPowerOffTime, minute);
		}

		/// <summary>
		/// Sets the status of the power control by cap on property.
		/// </summary>
		/// <param name="seton">true if you want to use, otherwise false.</param>
		/// <returns>true if the request is accepted; otherwise, false.</returns>
		public bool ReqSetupPenCapPower(bool enable)
		{
			return RequestChangeSetting(SettingType.PenCapOff, enable);
		}

		/// <summary>
		/// Sets the status of the auto power on property that if write the pen, turn on when pen is down.
		/// </summary>
		/// <param name="seton">true if you want to use, otherwise false.</param>
		/// <returns>true if the request is accepted; otherwise, false.</returns>
		public bool ReqSetupPenAutoPowerOn(bool enable)
		{
			return RequestChangeSetting(SettingType.AutoPowerOn, enable);
		}

		/// <summary>
		/// Sets the status of the beep property.
		/// </summary>
		/// <param name="enable">true if you want to listen sound of pen, otherwise false.</param>
		/// <returns>true if the request is accepted; otherwise, false.</returns>
		public bool ReqSetupPenBeep(bool enable)
		{
			return RequestChangeSetting(SettingType.Beep, enable);
		}

		/// <summary>
		/// Sets the hover mode.
		/// </summary>
		/// <param name="enable">true if you want to enable hover mode, otherwise false.</param>
		/// <returns>true if the request is accepted; otherwise, false.</returns>
		public bool ReqSetupHoverMode(bool enable)
		{
			return RequestChangeSetting(SettingType.Hover, enable);
		}

		/// <summary>
		/// Sets the offline data option whether save offline data or not.
		/// </summary>
		/// <param name="enable">true if you want to enable offline mode, otherwise false.</param>
		/// <returns>true if the request is accepted; otherwise, false.</returns>
		public bool ReqSetupOfflineData(bool enable)
		{
			return RequestChangeSetting(SettingType.OfflineData, enable);
		}

		/// <summary>
		/// Sets the color of LED.
		/// </summary>
		/// <param name="rgbcolor">integer type color formatted 0xAARRGGBB</param>
		/// <returns>true if the request is accepted; otherwise, false.</returns>
		public bool ReqSetupPenColor(int color)
		{
			return RequestChangeSetting(SettingType.LedColor, color);
		}

		/// <summary>
		/// Sets the value of the pen's sensitivity property that controls the force sensor of pen.
		/// </summary>
		/// <param name="level">the value of sensitivity. (0~4, 0 means maximum sensitivity)</param>
		/// <returns>true if the request is accepted; otherwise, false.</returns>
		public bool ReqSetupPenSensitivity(short step)
		{
			return RequestChangeSetting(SettingType.Sensitivity, step);
		}

        /// <summary>
        /// Sets the status of usb mode property that determine if usb mode is disk or bulk.
        /// You can choose between Disk mode, which is used as a removable disk, and Bulk mode, which is capable of high volume data communication, when connected with usb
        /// </summary>
        /// <param name="mode">enum of UsbMode</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqSetupUsbMode(UsbMode mode)
        {
            return RequestChangeSetting(SettingType.UsbMode, mode);
        }

        /// <summary>
        /// Sets the status of the down sampling property.
        /// Downsampling is a function of avoiding unnecessary data communication by omitting coordinates at the same position.
        /// </summary>
        /// <param name="enable">true if you want to enable down sampling, otherwise false.</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqSetupDownSampling(bool enable)
        {
            return RequestChangeSetting(SettingType.DownSampling, enable);
        }

        /// <summary>
        /// Sets the local name of the bluetooth device property.
        /// </summary>
        /// <param name="btLocalName">Bluetooth local name to set</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqSetupBtLocalName(string btLocalName)
        {
            return RequestChangeSetting(SettingType.BtLocalName, btLocalName);
        }

        /// <summary>
        /// Sets the value of the pen's sensitivity property that controls the force sensor(c-type) of pen.
        /// </summary>
        /// <param name="level">the value of sensitivity. (0~4, 0 means maximum sensitivity)</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqSetupPenFscSensitivity(short step)
        {
            return RequestChangeSetting(SettingType.FscSensitivity, step);
        }

        /// <summary>
        /// Sets the status of data transmission type property that determine if data transmission type is event or request-response.
        /// </summary>
        /// <param name="type">enum of DataTransmissionType</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqSetupDataTransmissionType(DataTransmissionType type)
        {
            return RequestChangeSetting(SettingType.DataTransmissionType, type);
        }

        /// <summary>
        /// Request Beeps and light on.
        /// </summary>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqBeepAndLight()
        {
            return RequestChangeSetting(SettingType.BeepAndLight, null);
        }

        public bool IsSupportPenProfile()
		{
			string[] temp = ProtocolVersion.Split('.');
			float ver = 0f;
			try
			{
				ver = FloatConverter.ToSingle(temp[0] + "." + temp[1]);
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.StackTrace);
			}
			if (ver >= PEN_PROFILE_SUPPORT_PROTOCOL_VERSION)
				return true;
			else
				return false;
		}

		#endregion

		#region using note

		private bool SendAddUsingNote(int sectionId = -1, int ownerId = -1, int[] noteIds = null)
		{
			ByteUtil bf = new ByteUtil(Escape);

			bf.Put(Const.PK_STX, false)
			  .Put((byte)Cmd.ONLINE_DATA_REQUEST);

			if (sectionId >= 0 && ownerId > 0 && noteIds == null)
			{
				bf.PutShort(2 + 8)
				  .PutShort(1)
				  .Put(GetSectionOwnerByte(sectionId, ownerId))
				  .Put(0xFF).Put(0xFF).Put(0xFF).Put(0xFF);
			}
			else if (sectionId >= 0 && ownerId > 0 && noteIds != null)
			{
				short length = (short)(2 + (noteIds.Length * 8));

				bf.PutShort(length)
				  .PutShort((short)noteIds.Length);

				foreach (int item in noteIds)
				{
					bf.Put(GetSectionOwnerByte(sectionId, ownerId))
					.PutInt(item);
				}
			}
			else
			{
				bf.PutShort(2)
				  .Put(0xFF)
				  .Put(0xFF);
			}

			bf.Put(Const.PK_ETX, false);

			return Send(bf);
		}
		private bool SendAddUsingNote(int[] sectionId, int[] ownerId)
		{
			ByteUtil bf = new ByteUtil(Escape);

			bf.Put(Const.PK_STX, false)
			  .Put((byte)Cmd.ONLINE_DATA_REQUEST);

			bf.PutShort((short)(2 + sectionId.Length * 8))
				.PutShort((short)sectionId.Length);
			for(int i = 0; i < sectionId.Length; ++i)
			{
				bf.Put(GetSectionOwnerByte(sectionId[i], ownerId[i]))
				  .Put(0xFF).Put(0xFF).Put(0xFF).Put(0xFF);
			}

			bf.Put(Const.PK_ETX, false);

			return Send(bf);
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
		public bool ReqAddUsingNote(int section, int owner, int[] notes = null)
		{
			return SendAddUsingNote(section, owner, notes);
		}

		/// <summary>
		/// Set the available notebook type lits
		/// </summary>
		/// <param name="section">The array of section Id of the paper list</param>
		/// <param name="owner">The array of owner Id of the paper list</param>
		/// <returns></returns>
		public bool ReqAddUsingNote(int[] section, int[] owner)
		{
			return SendAddUsingNote(section, owner);
		}

		#endregion

		#region offline

		/// <summary>
		/// Requests the list of Offline data.
		/// </summary>
		/// <param name="section">The Section Id of the paper</param>
		/// <param name="owner">The Owner Id of the paper</param>
		/// <returns>true if the request is accepted; otherwise, false.</returns>
		public bool ReqOfflineDataList(int section = -1, int owner = -1)
		{
			ByteUtil bf = new ByteUtil(Escape);

			byte[] pInfo = section > 0 && owner > 0 ? GetSectionOwnerByte(section, owner) : new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };

			bf.Put(Const.PK_STX, false)
			  .Put((byte)Cmd.OFFLINE_NOTE_LIST_REQUEST)
			  .PutShort(4)
			  .Put(pInfo)
			  .Put(Const.PK_ETX, false);

			return Send(bf);
		}

		/// <summary>
		/// Requests the list of Offline data.
		/// </summary>
		/// <param name="section">The Section Id of the paper</param>
		/// <param name="owner">The Owner Id of the paper</param>
		/// <param name="note">The Note Id of the paper</param>
		/// <returns>true if the request is accepted; otherwise, false.</returns>
		public bool ReqOfflineDataList(int section, int owner, int note)
		{
			ByteUtil bf = new ByteUtil(Escape);

			bf.Put(Const.PK_STX, false)
			  .Put((byte)Cmd.OFFLINE_PAGE_LIST_REQUEST)
			  .PutShort(8)
			  .Put(GetSectionOwnerByte(section, owner))
			  .PutInt(note)
			  .Put(Const.PK_ETX, false);

			return Send(bf);
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
		public bool ReqOfflineData(int section, int owner, int note, bool deleteOnFinished = true, int[] pages = null)
		{
			byte[] ownerByte = ByteConverter.IntToByte(owner);

			short length = 14;

			length += (short)(pages == null ? 0 : pages.Length * 4);

			ByteUtil bf = new ByteUtil(Escape);

			bf.Put(Const.PK_STX, false)
			  .Put((byte)Cmd.OFFLINE_DATA_REQUEST)
			  .PutShort(length)
			  .Put((byte)(deleteOnFinished ? 1 : 2))
			  .Put((byte)1)
			  .Put(GetSectionOwnerByte(section, owner))
			  .PutInt(note)
			  .PutInt(pages == null ? 0 : pages.Length);

			if (pages != null)
			{
				foreach (int page in pages)
				{
					bf.PutInt(page);
				}
			}

			bf.Put(Const.PK_ETX, false);

			return Send(bf);
		}

		private void SendOfflinePacketResponse(short index, bool isSuccess = true)
		{
			ByteUtil bf = new ByteUtil(Escape);

			bf.Put(Const.PK_STX, false)
			  .Put((byte)Cmd.OFFLINE_PACKET_RESPONSE)
			  .Put((byte)(isSuccess ? 0 : 1))
			  .PutShort(3)
			  .PutShort(index)
			  .Put(1)
			  .Put(Const.PK_ETX, false);

			Send(bf);
		}

        /// <summary>
        /// Request to remove offline data in device.
        /// </summary>
        /// <param name="section">The Section Id of the paper</param>
        /// <param name="owner">The Owner Id of the paper</param>
        /// <param name="notes">The Note Id's array of the paper</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public bool ReqRemoveOfflineData(int section, int owner, int[] notes)
		{
			ByteUtil bf = new ByteUtil(Escape);

			bf.Put(Const.PK_STX, false)
			  .Put((byte)Cmd.OFFLINE_DATA_DELETE_REQUEST);

			short length = (short)(5 + (notes.Length * 4));

			bf.PutShort(length)
			  .Put(GetSectionOwnerByte(section, owner))
			  .Put((byte)notes.Length);

			foreach (int noteId in notes)
			{
				bf.PutInt(noteId);
			}

			bf.Put(Const.PK_ETX, false);

			return Send(bf);
		}

        #endregion

        #region firmware

        public bool HasBugInFirmwareUpdate()
        {
            string[] temp = FirmwareVersion.Split('.');
            float ver = 0f;
            try
            {
                ver = FloatConverter.ToSingle(temp[0] + "." + temp[1]);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }
			if (FW_UPDATE_CANCEL_BUG_DEVICE_NAME == DeviceName && ver < FW_UPDATE_CANCEL_BUG_FIX_FIRMWARE_VERSION)
			{
                Debug.WriteLine($"Device {DeviceName}'s firmare {FirmwareVersion} has bug");
                return true;
			}
			else
			{
				return false;
			}
        }

        private ChunkEx mFwChunk;

		private bool IsUploading = false;

		private bool UploadingWithCompression;

        /// <summary>
        /// Requests the firmware installation
        /// </summary>
        /// <param name="filepath">absolute path of firmware file</param>
        /// <param name="version">version of firmware, this value is string</param>
		/// <param name="forceWithCompression">force upload compressed file</param>
        /// <returns>true if the request is accepted; otherwise, false.</returns>
        public async void ReqPenSwUpgrade(string filepath, string version, Compressible? forceCompression = null)
		{
			if (IsUploading)
			{
				return;
			}

			if (forceCompression == null)
			{
				UploadingWithCompression = CompressedFileUploadEnabled;
            }
			else 
			{
				UploadingWithCompression = forceCompression == Compressible.Enabled;

            }
            SwUpgradeFailCallbacked = false;
            IsUploading = true;

			mFwChunk = new ChunkEx(1024);

			bool loaded = await mFwChunk.Load(filepath);

			if (!loaded)
			{
				return;
			}

			int fileSize = mFwChunk.GetFileSize();

            byte[] versionStrBytes = Encoding.UTF8.GetBytes(version);

			string deviceName = DeviceName;
			if (deviceName.Equals(F121MG))
				deviceName = F121;

			byte[] deviceStrBytes = Encoding.UTF8.GetBytes(deviceName);

			Debug.WriteLine($"ReqPenSwUpgrade - file size : {fileSize}, chunk size {mFwChunk.GetChunksize()}, compressed : {UploadingWithCompression}");

			ByteUtil bf = new ByteUtil(Escape);

			bf.Put(Const.PK_STX, false)
			  .Put((byte)Cmd.FIRMWARE_UPLOAD_REQUEST)
			  .PutShort(42)
			  .Put(deviceStrBytes, 16)
			  .Put(versionStrBytes, 16)
			  .PutInt(fileSize)
			  .PutInt(mFwChunk.GetChunksize())
			  .Put((byte)(UploadingWithCompression ? 1 : 0))
			  .Put(mFwChunk.GetTotalChecksum())
			  .Put(Const.PK_ETX, false);

			Send(bf);

			onStartFirmwareInstallation();
		}

        private bool SwUpgradeFailCallbacked = false;

		private void ResponseChunkRequest(int offset, bool status = true)
		{
            Debug.WriteLine($"ResponseChunkRequest - offset : {offset} / {mFwChunk.GetFileSize()}, status : {status}");
            ByteUtil bf = new ByteUtil(Escape);

			if (!status || mFwChunk == null || !IsUploading)
			{
                bf.Put(Const.PK_STX, false)
                  .Put((byte)Cmd.FIRMWARE_PACKET_RESPONSE)
                  .Put(0)
                  .PutShort(14)
                  .Put(1)
                  .PutInt(offset)
                  .Put(0)
                  .PutNull(4)
                  .PutNull(4)
                  .Put(Const.PK_ETX, false);

                IsUploading = false;

				if (!HasBugInFirmwareUpdate())
				{
                    Send(bf);
				}

                if (!SwUpgradeFailCallbacked)
                {
                    Debug.WriteLine("ResponseChunkRequest - fail callbacked");
                    onReceiveFirmwareUpdateResult(new SimpleResultEventArgs(false));
                    SwUpgradeFailCallbacked = true;
                }
            }
			else
			{
                byte[] data = mFwChunk.Get(offset);
				byte[] dataToUpload;

                if (UploadingWithCompression)
				{
                    dataToUpload = Compression.Compress(data);
                    Debug.WriteLine($"ResponseChunkRequest size - original : {data.Length} / uploadable : {dataToUpload.Length}");
                }
				else
				{
                    dataToUpload = data;
                }

				byte checksum = ChunkEx.CalcChecksum(data);
				short dataLength = (short)(dataToUpload.Length + 14);

				bf.Put(Const.PK_STX, false)
				  .Put((byte)Cmd.FIRMWARE_PACKET_RESPONSE)
				  .Put(0)
				  .PutShort(dataLength)
				  .Put(0)
				  .PutInt(offset)
				  .Put(checksum)
				  .PutInt(data.Length)
				  .PutInt(UploadingWithCompression ? dataToUpload.Length : 0)
				  .Put(dataToUpload)
				  .Put(Const.PK_ETX, false);
                Send(bf);

                onReceiveFirmwareUpdateStatus(new ProgressChangeEventArgs(mFwChunk.GetChunkLength(), (int)(offset / mFwChunk.GetChunksize()) + 1));
            }
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

		#region Pen Profile
		public bool ReqCreateProfile(byte[] profileName, byte[] password)
		{
			ByteUtil bf = new ByteUtil(Escape);
			bf.Put(Const.PK_STX, false)
				.Put((byte)Cmd.PEN_PROFILE_REQUEST) // command
				.PutShort((short)(PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME + 1 + PenProfile.LIMIT_BYTE_LENGTH_PASSWORD + 2 + 2))        // length
				.Put(profileName, PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME)				// profile file name
				.Put(PenProfile.PROFILE_CREATE)     // type
				.Put(password, PenProfile.LIMIT_BYTE_LENGTH_PASSWORD)					// password
				.PutShort(32)                       // section 크기 -> 32인 이유? 우선 android따라감. 확인필요
				.PutShort(32)                        // sector 개수(2^N 현재는 고정 2^8)
				.Put(Const.PK_ETX, false);

			return Send(bf);
		}

		public bool ReqDeleteProfile(byte[] profileName, byte[] password)
		{
			ByteUtil bf = new ByteUtil(Escape);
			bf.Put(Const.PK_STX, false)
				.Put((byte)Cmd.PEN_PROFILE_REQUEST) // command
				.PutShort((short)(PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME + 1 + PenProfile.LIMIT_BYTE_LENGTH_PASSWORD))                // length
				.Put(profileName, PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME)				// profile file name
				.Put(PenProfile.PROFILE_DELETE)     // type
				.Put(password, PenProfile.LIMIT_BYTE_LENGTH_PASSWORD)					// password
				.Put(Const.PK_ETX, false);

			return Send(bf);
		}

		public bool ReqProfileInfo(byte[] profileName)
		{
			ByteUtil bf = new ByteUtil(Escape);
			bf.Put(Const.PK_STX, false)
				.Put((byte)Cmd.PEN_PROFILE_REQUEST) // command
				.PutShort((short)(PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME + 1))                    // length
				.Put(profileName, PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME)           // profile file name
				.Put(PenProfile.PROFILE_INFO)       // type
				.Put(Const.PK_ETX, false);

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

			ByteUtil bf = new ByteUtil(Escape);
			bf.Put(Const.PK_STX, false)
				.Put((byte)Cmd.PEN_PROFILE_REQUEST)             // command
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

			bf.Put(Const.PK_ETX, false);

			return Send(bf);
		}

		public bool ReqReadProfileValue(byte[] profileName, byte[][] keys)
		{
			ByteUtil bf = new ByteUtil(Escape);
			bf.Put(Const.PK_STX, false)
				.Put((byte)Cmd.PEN_PROFILE_REQUEST)                 // command
				.PutShort((short)(PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME + 1 + 1 + PenProfile.LIMIT_BYTE_LENGTH_KEY * keys.Length))    // Length
				.Put(profileName, PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME)                           // profile file name
				.Put(PenProfile.PROFILE_READ_VALUE)                 // Type
				.Put((byte)keys.Length);                            // Key Count

			for (int i = 0; i < keys.Length; ++i)
			{
				bf.Put(keys[i], PenProfile.LIMIT_BYTE_LENGTH_KEY);
			}

			bf.Put(Const.PK_ETX, false);

			return Send(bf);
		}

		public bool ReqDeleteProfileValue(byte[] profileName, byte[] password, byte[][] keys)
		{
			ByteUtil bf = new ByteUtil(Escape);
			bf.Put(Const.PK_STX, false)
				.Put((byte)Cmd.PEN_PROFILE_REQUEST)                     // command
				.PutShort((short)(PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME + 1 + PenProfile.LIMIT_BYTE_LENGTH_PASSWORD + 1 + PenProfile.LIMIT_BYTE_LENGTH_KEY * keys.Length))    // Length
				.Put(profileName, PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME)                               // profile file name
				.Put(PenProfile.PROFILE_DELETE_VALUE)                   // Type
				.Put(password, PenProfile.LIMIT_BYTE_LENGTH_PASSWORD)                                  // password
				.Put((byte)keys.Length);                                // key count

			for (int i = 0; i < keys.Length; ++i)
			{
				bf.Put(keys[i], PenProfile.LIMIT_BYTE_LENGTH_KEY);
			}

			bf.Put(Const.PK_ETX, false);

			return Send(bf);
		}
		#endregion

		#region Encryption
		RSAChiper rsaChiper = null;
        public bool AesKeyRequest()
        {
            if (rsaChiper == null)
            {
                rsaChiper = new RSAChiper();
                rsaChiper.CreateKey();
                // test
                //var ret = rsaChiper.Encrypt("NEOLAP_123456789ABCDEFGHIJKLMNOP");
                //Debug.WriteLine("ret : " + BitConverter.ToString(ret));
                //var ret2 = rsaChiper.Decrypt(ret);
                //return Encoding.UTF8.GetString(result.ToByteArray());
                //Debug.WriteLine("ret2 : " + BitConverter.ToString(ret2));
                //Debug.WriteLine("ret2 str : " + Encoding.UTF8.GetString(ret2));
            }

            ByteUtil bf = new ByteUtil(Escape);
            //bf.Put(Const.PK_STX, false)
            //	.Put((byte)Cmd.AES_KEY_REQUEST) // command
            //	.PutShort(256 + 3)
            //	.Put(rsaChiper.GetPublicKeyModulus())
            //	.Put(rsaChiper.GetPublicExponent())
            //	.Put(Const.PK_ETX, false);
            bf.Put(Const.PK_STX, false)
                .Put((byte)Cmd.AES_KEY_REQUEST) // command
                .PutShort(8)
                .Put(Const.PK_ETX, false);

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
            PressureCalibration.Instance.Clear();
            Connected?.Invoke(PenClient, args);
        }

        /// <summary>
        /// Occurs when a connection is closed
        /// </summary>
		public event TypedEventHandler<IPenClient, object> Disconnected;
        internal void onDisconnected()
        {
            PressureCalibration.Instance.Clear();
            PenClient?.Unbind();
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

        public event TypedEventHandler<IPenClient, PdsReceivedEventArgs> PdsReceived;
        internal void onPdsReceived(PdsReceivedEventArgs args)
        {
            PdsReceived?.Invoke(PenClient, args);
        }

        #endregion

        #region util

        private static byte[] GetSectionOwnerByte(int section, int owner)
		{
			byte[] ownerByte = ByteConverter.IntToByte(owner);
			ownerByte[3] = (byte)section;

			return ownerByte;
		}

		//public Dot( 
		private Dot MakeDot(int penMaxForce, int owner, int section, int note, int page, long timestamp, int x, int y, int fx, int fy, int force, DotTypes type, int color)
		{
			Dot.Builder builder = null;
			if (penMaxForce == 0) builder = new Dot.Builder();
			else builder = new Dot.Builder(penMaxForce);

			builder.owner(owner)
				.section(section)
				.note(note)
				.page(page)
				.timestamp(timestamp)
				.coord(x + fx * 0.01f, y + fy * 0.01f)
				.force(force)
				.dotType(type)
				.color(color);
			return builder.Build();
		}

		private bool isF121MG(string macAddress)
		{
			const string MG_F121_MAC_START = "9C:7B:D2:22:00:00";
			const string MG_F121_MAC_END = "9C:7B:D2:22:18:06";
			ulong address = Convert.ToUInt64(macAddress.Replace(":", ""), 16);
			ulong mgStart = Convert.ToUInt64(MG_F121_MAC_START.Replace(":", ""), 16);
			ulong mgEnd = Convert.ToUInt64(MG_F121_MAC_END.Replace(":", ""), 16);

			if (address >= mgStart && address <= mgEnd)
				return true;
			else
				return false;
		}

		#endregion

		#region Protocol Parse
		private ByteUtil mBuffer = null;
		private bool IsEscape = false;
        private object protocolParseLock = new object();
		public void ProtocolParse(byte[] buff, int size)
		{
            lock (protocolParseLock)
            {
                byte[] test = new byte[size];

                //Array.Copy(buff, 0, test, 0, size);
                //Debug.WriteLine("Read Buffer : {0}", BitConverter.ToString(test));
                //Debug.WriteLine("");

                for (int i = 0; i < size; i++)
                {
                    if (buff[i] == Const.PK_STX)
                    {
                        // 패킷 시작
                        mBuffer = new ByteUtil();

                        IsEscape = false;
                    }
                    else if (buff[i] == Const.PK_ETX)
                    {
                        // 패킷 끝
                        Packet.Builder builder = new Packet.Builder();

                        int cmd = mBuffer.GetByteToInt();

                        // event command is 0x6X
                        string cmdstr = Enum.GetName(typeof(Cmd), cmd);

                        int result_size = ((cmd >> 4) != 0x6 && cmd != (int)Cmd.PDS_COMMAND_EVENT) && cmdstr != null && cmdstr.EndsWith("RESPONSE") ? 1 : 0;

                        int result = result_size > 0 ? mBuffer.GetByteToInt() : -1;

                        int length = mBuffer.GetUShort();

                        byte[] data = mBuffer.GetBytes();

                        //System.Console.WriteLine( "length : {0}, data : {1}", length, data.Length );

                        builder.cmd(cmd)
                            .result(result)
                            .data(data);

                        //System.Console.WriteLine( "Read Packet : {0}", BitConverter.ToString( data ) );
                        //System.Console.WriteLine();

                        mBuffer.Clear();
                        mBuffer = null;
                        //if ((Cmd)cmd == Cmd.ONLINE_NEW_PEN_UP_EVENT)
                        //{ }
                        //else
                        try
                        {
                            ParsePacket(builder.Build());
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine($"{e.Message}\n{e.StackTrace}");
                        }

                        IsEscape = false;
                    }
                    else if (buff[i] == Const.PK_DLE)
                    {
                        if (i < size - 1)
                        {
                            mBuffer.Put((byte)(buff[++i] ^ 0x20));
                        }
                        else
                        {
                            IsEscape = true;
                        }
                    }
                    else if (IsEscape)
                    {
                        mBuffer.Put((byte)(buff[i] ^ 0x20));

                        IsEscape = false;
                    }
                    else
                    {
                        mBuffer.Put(buff[i]);
                    }
                }
            }
		}
		#endregion
	}
}
