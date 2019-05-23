using Neosmartpen.Net;
using Neosmartpen.Net.Bluetooth;
using Neosmartpen.Net.Metadata;
using Neosmartpen.Net.Metadata.Exceptions;
using Neosmartpen.Net.Metadata.Model;
using Neosmartpen.Net.Protocol.v1;
using Neosmartpen.Net.Protocol.v2;
using Neosmartpen.Net.Support;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace PenDemo
{
    public partial class MainForm : Form, PenCommV1Callbacks, PenCommV2Callbacks
    {
        public PressureFilter mFilter;

        public static int[] UsingNote = new int[] { 301, 302, 303 };

        public static string DefaultPassword = "0000";

        private Bitmap mBitmap;

        private Stroke mStroke;

        private int mWidth, mHeight;

        private object mDrawLock = new object();

        private PasswordInputForm mPwdForm;

        private ProgressForm mPrgForm;

        // Adapter for Bluetooth communication control
        private BluetoothAdapter mBtAdt;

        // Communication with F110
        private PenCommV1 mPenCommV1;

        // Communication with devices other than F110
        private PenCommV2 mPenCommV2;

        private IMetadataManager mMetadataManager;

        public delegate void RequestDele();

        public MainForm()
        {
            InitializeComponent();

            mBtAdt = new BluetoothAdapter();

            // Create MetadataManager
            mMetadataManager = new GenericMetadataManager(new NProjParser());

            mPenCommV1 = new PenCommV1( this );
            mPenCommV2 = new PenCommV2( this );

            // Bind MetadataManager to PenComm.
            mPenCommV1.MetadataManager = mMetadataManager;
            mPenCommV2.MetadataManager = mMetadataManager;

            mWidth = pictureBox1.Width;
            mHeight = pictureBox1.Height;

            mBitmap = new Bitmap( pictureBox1.Width, pictureBox1.Height );

            mPwdForm = new PasswordInputForm( OnInputPassword );

            LoadMetadata();
        }

        private void LoadMetadata()
        {
            try
            {
                // Load the Metadata file.
                mMetadataManager.Load(Application.StartupPath + "\\Resources\\note_603.nproj");
            }
            catch (ParseException)
            {
                // todo : Handling of parsing errors
            }
            catch (FileNotFoundException)
            {
            }
            catch
            {
            }
        }

        private void OnInputPassword( string password )
        {
            Request( 
                delegate { 
                    mPenCommV1.ReqInputPassword( password ); 
                }, delegate {
                    mPenCommV2.ReqInputPassword( password );
                } );
        }

        private void InitImage()
        {
            Graphics g = Graphics.FromImage( mBitmap );
            g.Clear( Color.Transparent );
            g.Dispose();
            pictureBox1.Invalidate();
        }

        private void btnSearch_Click( object sender, EventArgs e )
        {
            btnSearch.Enabled = false;

            Thread thread = new Thread( unused =>
            {
                PenDevice[] devices = mBtAdt.FindAllDevices();

                this.BeginInvoke( new MethodInvoker( delegate()
                {
                    lbDevices.Items.Clear();

                    if ( devices == null || devices.Length <= 0 )
                    {
                        MessageBox.Show( "device is not exists" );
                    }
                    else
                    {
                        lbDevices.Items.AddRange( devices );
                    }

                    btnSearch.Enabled = true;
                } ) );
            } );

            thread.IsBackground = true;
            thread.Start();
        }

        private void Request( RequestDele onRequestToV1, RequestDele onRequestToV2, RequestDele onFailure = null, RequestDele onAfter = null )
        {
            if ( !mBtAdt.Connected )
            {
                MessageBox.Show( "Device is not connected." );

                if ( onFailure != null )
                {
                    onFailure();
                }
            }
            else if ( mBtAdt.DeviceClass == mPenCommV1.DeviceClass )
            {
                onRequestToV1();

                if ( onAfter != null )
                {
                    onAfter();
                }
            }
            else if ( mBtAdt.DeviceClass == mPenCommV2.DeviceClass )
            {
                onRequestToV2();

                if ( onAfter != null )
                {
                    onAfter();
                }
            }
        }

        private void btnConnect_Click( object sender, EventArgs e )
        {
            if ( txtMacAddress.Text == "" )
            {
                MessageBox.Show( "Select Mac Address of Pen!!" );
                return;
            }

            btnConnect.Enabled = false;

            Thread thread = new Thread( unused =>
            {
                // Binds a socket created by connecting with pen through Bluetooth interface according to Device Class
                bool result = mBtAdt.Connect( txtMacAddress.Text, delegate( uint deviceClass )
                {
                    // Binding when Device Class is F110
                    if ( deviceClass == mPenCommV1.DeviceClass )
                    {
                        mBtAdt.Bind( mPenCommV1 );
                    }
                    // Binding if Device Class is not F110
                    else if ( deviceClass == mPenCommV2.DeviceClass )
                    {
                        mBtAdt.Bind( mPenCommV2 );
                    }
                } );

                if ( !result )
                {
                    MessageBox.Show( "Fail to connect" );

                    this.BeginInvoke( new MethodInvoker( delegate()
                    {
                        btnConnect.Enabled = true;
                    } ) );
                }
            } );

            thread.IsBackground = true;
            thread.Start();

            lbHistory.Items.Add( txtMacAddress.Text );
        }

        private void btnDisconnect_Click( object sender, EventArgs e )
        {
            btnDisconnect.Enabled = false;

            if ( !mBtAdt.Disconnect() )
            {
                btnDisconnect.Enabled = true;
            }
        }

        private void ProcessDot( Dot dot )
        {
            // Filtered pressure
            dot.Force = mFilter.Filter( dot.Force );

            // TODO: Drawing sample code
            if ( dot.DotType == DotTypes.PEN_DOWN )
            {
                mStroke = new Stroke( dot.Section, dot.Owner, dot.Note, dot.Page );
                mStroke.Add( dot );
            }
            else if ( dot.DotType == DotTypes.PEN_MOVE )
            {
                mStroke.Add( dot );
            }
            else if ( dot.DotType == DotTypes.PEN_UP )
            {
                mStroke.Add( dot );

                DrawStroke( mStroke );

                mFilter.Reset();
            }
        }

        private void DrawStroke( Stroke stroke )
        {
            this.Invoke( new MethodInvoker( delegate()
            {
                lock ( mDrawLock )
                {
                    //603 Ring Note Height  5.52  5.41 	63.46 	88.88 
                    int dx = (int)( ( 5.52 * mWidth ) / 63.46 );
                    int dy = (int)( ( 5.41 * mHeight ) / 88.88 );

                    Renderer.draw( mBitmap, stroke, (float)( mWidth / 63.46f ), (float)( mHeight / 88.88f ), -dx, -dy, 1, Color.FromArgb( 200, Color.Blue ) );

                    pictureBox1.Image = mBitmap;
                }
            } ) );
        }

        private void lbDevices_SelectedIndexChanged( object sender, EventArgs e )
        {
            ListBox lbEvent = sender as ListBox;
            PenDevice dev = lbEvent.SelectedItem as PenDevice;
            if( dev != null )
                txtMacAddress.Text = dev.Address;
        }

        private void lbHistory_SelectedIndexChanged( object sender, EventArgs e )
        {
            ListBox lbEvent = sender as ListBox;
            string dev = lbEvent.SelectedItem as string;
            txtMacAddress.Text = dev;
        }

        private void button2_Click( object sender, EventArgs e )
        {
            this.BeginInvoke( new MethodInvoker( delegate()
            {
                InitImage();
            }) );
        }

        private void Form1_FormClosed( object sender, FormClosedEventArgs e )
        {
            if ( mBtAdt != null )
            {
                if ( mBtAdt.Connected )
                {
                    mBtAdt.Disconnect();
                }
            }
        }

        #region PenCommV1Callbacks

        void PenCommV1Callbacks.onConnected( IPenComm sender, int maxForce, string swVersion )
        {
            mFilter = new PressureFilter( maxForce );

            this.BeginInvoke( new MethodInvoker( delegate()
            {
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;
                tbPenInfo.Text = String.Format( "Firmware Version : {0}", swVersion );

                ToggleOption( true );
            } ) );
        }

        void PenCommV1Callbacks.onDisconnected( IPenComm sender )
        {
            this.BeginInvoke( new MethodInvoker( delegate()
            {
                lbOfflineData.Items.Clear();

                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;
                tbPenInfo.Text = "";

                ToggleOption( false );
            } ) );

            CloseProgress();
        }

        void PenCommV1Callbacks.onPenPasswordRequest( IPenComm sender, int retryCount, int resetCount )
        {
            this.BeginInvoke( new MethodInvoker( delegate()
            {
                mPwdForm.ShowDialog();
            } ) );
        }

        void PenCommV1Callbacks.onPenAuthenticated( IPenComm sender )
        {
            mPenCommV1.ReqAddUsingNote();
            mPenCommV1.ReqOfflineDataList();
            mPenCommV1.ReqPenStatus();
        }

        void PenCommV1Callbacks.onAvailableNoteAccepted(IPenComm sender, bool result)
        {
        }

        void PenCommV1Callbacks.onReceiveDot( IPenComm sender, Dot dot )
        {
            ProcessDot( dot );
        }

        void PenCommV1Callbacks.onUpDown( IPenComm sender, bool isUp )
        {
        }

        void PenCommV1Callbacks.onReceivedPenStatus( IPenComm sender, int timeoffset, long timetick, int forcemax, int battery, int usedmem, int pencolor, bool autopower, bool accel, bool hovermode, bool beep, short autoshutdownTime, short penSensitivity, string modelName )
        {
            this.BeginInvoke( new MethodInvoker( delegate()
            {
                nmPowerOffTime.Value = autoshutdownTime;
                tbFsrStep.Value = penSensitivity;
                cbBeep.Checked = beep;

                cbOfflineData.Checked = false;
                cbOfflineData.Enabled = false;
                cbPenCapPower.Checked = false;
                cbPenCapPower.Enabled = false;

                cbPenTipPowerOn.Checked = autopower;

                prgPower.Maximum = 100;
                prgPower.Value = battery;

                prgStorage.Maximum = 100;
                prgStorage.Value = usedmem;
            } ) );
        }

        void PenCommV1Callbacks.onPenPasswordSetUpResponse( IPenComm sender, bool result )
        {
            if ( !result )
            {
                MessageBox.Show( "Can not change password." );
            }
            else
            {
                tbOldPassword.Text = "";
                tbNewPassword.Text = "";
            }
        }

        void PenCommV1Callbacks.onPenSensitivitySetUpResponse( IPenComm sender, bool isSuccess )
        {
            mPenCommV1.ReqPenStatus();
        }

        void PenCommV1Callbacks.onPenAutoShutdownTimeSetUpResponse( IPenComm sender, bool isSuccess )
        {
            mPenCommV1.ReqPenStatus();
        }

        void PenCommV1Callbacks.onPenBeepSetUpResponse( IPenComm sender, bool isSuccess )
        {
            mPenCommV1.ReqPenStatus();
        }

        void PenCommV1Callbacks.onPenAutoPowerOnSetUpResponse( IPenComm sender, bool isSuccess )
        {
            mPenCommV1.ReqPenStatus();
        }

        void PenCommV1Callbacks.onPenColorSetUpResponse( IPenComm sender, bool isSuccess )
        {
            mPenCommV1.ReqPenStatus();
        }

        void PenCommV1Callbacks.onPenHoverSetUpResponse( IPenComm sender, bool result )
        {
            mPenCommV1.ReqPenStatus();
        }

        void PenCommV1Callbacks.onReceivedFirmwareUpdateResult( IPenComm sender, bool isSuccess )
        {
            CloseProgress();
        }
        
        private object mProgressLock = new object();

        private void DisplayProgress( string title, int total, int amountDone )
        {
            this.BeginInvoke( new MethodInvoker( delegate()
            {
                lock ( mProgressLock )
                {
                    if ( mPrgForm == null )
                    {
                        mPrgForm = new ProgressForm();
                    }

                    if ( mPrgForm != null )
                    {
                        mPrgForm.SetStatus( title, total, amountDone );
                    }

                    if ( !mPrgForm.Visible )
                    {
                        mPrgForm.ShowDialog();
                    }
                }
            } ) );
        }

        private void CloseProgress()
        {
            this.BeginInvoke( new MethodInvoker( delegate()
            {
                lock ( mProgressLock )
                {
                    if ( mPrgForm != null && mPrgForm.Visible )
                    {
                        mPrgForm.Close();
                        mPrgForm.Dispose();
                        mPrgForm = null;
                    }
                }
            } ) );
        }
        
        public const string ProgressTitleOffline = "Download Offline Data";

        public const string ProgressTitleFirmware = "Firmware Update";

        void PenCommV1Callbacks.onReceivedFirmwareUpdateStatus( IPenComm sender, int total, int progress )
        {
            DisplayProgress( ProgressTitleFirmware, total, progress );
        }

        void PenCommV1Callbacks.onOfflineDataList( IPenComm sender, OfflineDataInfo[] notes )
        {
            this.BeginInvoke( new MethodInvoker( delegate()
            {
                lbOfflineData.Items.Clear();

                foreach ( OfflineDataInfo item in notes )
                {
                    lbOfflineData.Items.Add( item );
                }
            } ) );
        }

        void PenCommV1Callbacks.onStartOfflineDownload( IPenComm sender )
        {
            DisplayProgress( ProgressTitleOffline, 100, 0 );
        }

        void PenCommV1Callbacks.onUpdateOfflineDownload( IPenComm sender, int total, int progress )
        {
            DisplayProgress( ProgressTitleOffline, total, progress );
        }

        void PenCommV1Callbacks.onFinishedOfflineDownload( IPenComm sender, bool status )
        {
            CloseProgress();
            mPenCommV1.ReqOfflineDataList();
        }

        void PenCommV1Callbacks.onReceiveOfflineStrokes( IPenComm sender, Stroke[] strokes )
        {
            foreach ( Stroke stroke in strokes )
            {
                DrawStroke( stroke );
            }
        }
		void PenCommV1Callbacks.onErrorDetected(IPenComm sender, ErrorType errorType, long timestamp,  Dot dot, string extraData)
		{
		}

        void PenCommV1Callbacks.onSymbolDetected(IPenComm sender, List<Symbol> symbols)
        {
            // TODO : Processing for detected symbols
            // Implement functions corresponding to predefined Actions.
            // For example, if Symbol's Action is email, it sends an email.
        }

        #endregion

        #region PenCommV2Callbacks

        void PenCommV2Callbacks.onConnected( IPenComm sender, string macAddress, string deviceName, string fwVersion, string protocolVersion, string subName, int maxForce )
        {
            mFilter = new PressureFilter( maxForce );

            this.BeginInvoke( new MethodInvoker( delegate()
            {
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;

                tbPenInfo.Text = String.Format( "Mac : {0}\r\n\r\nName : {1}\r\n\r\nSubName : {2}\r\n\r\nFirmware Version : {3}\r\n\r\nProtocol Version : {4}", macAddress, deviceName, subName, fwVersion, protocolVersion );

                ToggleOption( true );
            } ) );
        }

        void PenCommV2Callbacks.onPenAuthenticated( IPenComm sender )
        {
            mPenCommV2.ReqAddUsingNote();
            mPenCommV2.ReqOfflineDataList();
            mPenCommV2.ReqPenStatus();
        }

        void PenCommV2Callbacks.onDisconnected( IPenComm sender )
        {
            this.BeginInvoke( new MethodInvoker( delegate()
            {
                lbOfflineData.Items.Clear();

                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;
                tbPenInfo.Text = "";

                ToggleOption( false );
            } ) );

            CloseProgress();
        }

        void PenCommV2Callbacks.onAvailableNoteAccepted(IPenComm sender, bool result)
        {
        }

        void PenCommV2Callbacks.onReceiveDot(IPenComm sender, Dot dot, ImageProcessingInfo info)
        {
            ProcessDot(dot);
        }

        void PenCommV2Callbacks.onReceiveOfflineDataList( IPenComm sender, params OfflineDataInfo[] offlineNotes )
        {
            this.BeginInvoke( new MethodInvoker( delegate()
            {
                lbOfflineData.Items.Clear();

                foreach ( OfflineDataInfo item in offlineNotes )
                {
                    lbOfflineData.Items.Add( item );
                }
            } ) );
        }

        void PenCommV2Callbacks.onStartOfflineDownload( IPenComm sender )
        {
            DisplayProgress( ProgressTitleOffline, 100, 0 );
        }

        void PenCommV2Callbacks.onReceiveOfflineStrokes( IPenComm sender, int total, int progress, Stroke[] strokes )
        {
            foreach ( Stroke stroke in strokes )
            {
                DrawStroke( stroke );
            }

            DisplayProgress( ProgressTitleOffline, total, progress );

            Array.Clear( strokes, 0, strokes.Length );
        }

        void PenCommV2Callbacks.onFinishedOfflineDownload( IPenComm sender, bool isSuccess )
        {
            CloseProgress();
            mPenCommV2.ReqOfflineDataList();
        }

        void PenCommV2Callbacks.onRemovedOfflineData( IPenComm sender, bool result )
        {
            mPenCommV2.ReqOfflineDataList();
        }

        void PenCommV2Callbacks.onReceivePenStatus( IPenComm sender, bool locked, int passwdMaxReTryCount, int passwdRetryCount, long timestamp, short autoShutdownTime, int maxForce, int battery, int usedmem, bool useOfflineData, bool autoPowerOn, bool penCapPower, bool hoverMode, bool beep, short penSensitivity, PenCommV2.UsbMode usbmode, bool downsampling, string btLocalName, PenCommV2.DataTransmissionType dataTransmissionType )
        {
            this.BeginInvoke( new MethodInvoker( delegate()
            {
                nmPowerOffTime.Value = autoShutdownTime;
                tbFsrStep.Value = penSensitivity;
                cbBeep.Checked = beep;
                cbOfflineData.Enabled = true;
                cbOfflineData.Checked = useOfflineData;
                cbPenCapPower.Enabled = true;
                cbPenCapPower.Checked = penCapPower;
                cbPenTipPowerOn.Checked = autoPowerOn;
                
                prgPower.Maximum = 100;
                prgPower.Value = battery > 100 ? 100 : battery;

                prgStorage.Maximum = 100;
                prgStorage.Value = usedmem;
            } ) );
        }

        void PenCommV2Callbacks.onPenPasswordRequest( IPenComm sender, int retryCount, int resetCount )
        {
            this.BeginInvoke( new MethodInvoker( delegate() 
            {
                mPwdForm.ShowDialog();
            } ) );
        }

        void PenCommV2Callbacks.onPenPasswordSetUpResponse( IPenComm sender, bool result )
        {
            if ( !result )
            {
                MessageBox.Show( "Can not change password." );
            }
            else
            {
                this.BeginInvoke( new MethodInvoker( delegate()
                {
                    tbOldPassword.Text = "";
                    tbNewPassword.Text = "";
                } ) );
            }
        }

        void PenCommV2Callbacks.onPenOfflineDataSetUpResponse( IPenComm sender, bool result )
        {
            mPenCommV2.ReqPenStatus();
        }

        void PenCommV2Callbacks.onPenTimestampSetUpResponse( IPenComm sender, bool result )
        {
            mPenCommV2.ReqPenStatus();
        }

        void PenCommV2Callbacks.onPenSensitivitySetUpResponse( IPenComm sender, bool result )
        {
            mPenCommV2.ReqPenStatus();
        }

        void PenCommV2Callbacks.onPenAutoShutdownTimeSetUpResponse( IPenComm sender, bool result )
        {
            mPenCommV2.ReqPenStatus();
        }

        void PenCommV2Callbacks.onPenAutoPowerOnSetUpResponse( IPenComm sender, bool result )
        {
            mPenCommV2.ReqPenStatus();
        }

        void PenCommV2Callbacks.onPenCapPowerOnOffSetupResponse( IPenComm sender, bool result )
        {
            mPenCommV2.ReqPenStatus();
        }

        void PenCommV2Callbacks.onPenBeepSetUpResponse( IPenComm sender, bool result )
        {
            mPenCommV2.ReqPenStatus();
        }

        void PenCommV2Callbacks.onPenHoverSetUpResponse( IPenComm sender, bool result )
        {
            mPenCommV2.ReqPenStatus();
        }

        void PenCommV2Callbacks.onPenColorSetUpResponse( IPenComm sender, bool result )
        {
            mPenCommV2.ReqPenStatus();
        }

        void PenCommV2Callbacks.onReceiveFirmwareUpdateStatus( IPenComm sender, int total, int progress )
        {
            DisplayProgress( ProgressTitleFirmware, total, progress );
        }

        void PenCommV2Callbacks.onReceiveFirmwareUpdateResult( IPenComm sender, bool result )
        {
            CloseProgress();
        }

        void PenCommV2Callbacks.onReceiveBatteryAlarm( IPenComm sender, int battery )
        {
            this.BeginInvoke( new MethodInvoker( delegate()
            {
                prgPower.Maximum = 100;
                prgPower.Value = battery;
            } ) );
        }

        void PenCommV2Callbacks.onPenUsbModeSetUpResponse(IPenComm sender, bool result)
        {
            mPenCommV2.ReqPenStatus();
        }

        void PenCommV2Callbacks.onPenDownSamplingSetUpResponse(IPenComm sender, bool result)
        {
            mPenCommV2.ReqPenStatus();
        }

        void PenCommV2Callbacks.onPenBtLocalNameSetUpResponse(IPenComm sender, bool result)
        {
            mPenCommV2.ReqPenStatus();
        }

        void PenCommV2Callbacks.onPenFscSensitivitySetUpResponse(IPenComm sender, bool result)
        {
            mPenCommV2.ReqPenStatus();
        }

        void PenCommV2Callbacks.onPenDataTransmissionTypeSetUpResponse(IPenComm sender, bool result)
        {
            mPenCommV2.ReqPenStatus();
        }

        void PenCommV2Callbacks.onErrorDetected(IPenComm sender, ErrorType errorType, long timestamp, Dot dot, string extraData, ImageProcessErrorInfo imageProcessErrorInfo)
        {
        }

        void PenCommV2Callbacks.onPenBeepAndLightResponse(IPenComm sender, bool result)
        {
        }

        void PenCommV2Callbacks.onPenProfileReceived(IPenComm sender, PenProfileReceivedCallbackArgs args)
        {
        }

        void PenCommV2Callbacks.onSymbolDetected(IPenComm sender, List<Symbol> symbols)
        {
            // TODO : Processing for detected symbols
            // Implement functions corresponding to predefined Actions.
            // For example, if Symbol's Action is email, it sends an email.
        }
        #endregion

        #region pencontrol

        private void button3_Click( object sender, EventArgs e )
        {
            if ( lbOfflineData.SelectedItem == null )
            {
                return;
            }

            OfflineDataInfo data = lbOfflineData.SelectedItem as OfflineDataInfo;

            Request(
                delegate { mPenCommV1.ReqOfflineData( data );  }, 
                delegate { mPenCommV2.ReqOfflineData( data.Section, data.Owner, data.Note, false, data.Pages );
            } );
        }

        private void button1_Click( object sender, EventArgs e )
        {
            if ( lbOfflineData.SelectedItem == null )
            {
                return;
            }

            OfflineDataInfo data = lbOfflineData.SelectedItem as OfflineDataInfo;
            
            Request( 
                delegate { mPenCommV1.ReqRemoveOfflineData( data.Section, data.Owner ); }, 
                delegate { mPenCommV2.ReqRemoveOfflineData( data.Section, data.Owner, new int[] { data.Note } );
            } );
        }

        private void nmPowerOffTime_ValueChanged( object sender, EventArgs e )
        { 
            Request(
                delegate { mPenCommV1.ReqSetupPenAutoShutdownTime( (short)nmPowerOffTime.Value ); },
                delegate { mPenCommV2.ReqSetupPenAutoShutdownTime( (short)nmPowerOffTime.Value ); }
                );
        }

        private void tbFsrStep_ValueChanged( object sender, EventArgs e )
        {
            Request(
                delegate { mPenCommV1.ReqSetupPenSensitivity( (short)tbFsrStep.Value ); },
                delegate { mPenCommV2.ReqSetupPenSensitivity( (short)tbFsrStep.Value ); }
                );
        }

        private void cbPenCapPower_CheckedChanged( object sender, EventArgs e )
        {
            Request(
                delegate { },
                delegate { mPenCommV2.ReqSetupPenCapPower( cbPenCapPower.Checked ); }
                );
        }

        private void cbPenTipPowerOn_CheckedChanged( object sender, EventArgs e )
        {
            Request(
                delegate { mPenCommV1.ReqSetupPenAutoPowerOn( cbPenTipPowerOn.Checked ); },
                delegate { mPenCommV2.ReqSetupPenAutoPowerOn( cbPenTipPowerOn.Checked ); }
                );
        }

        private void cbBeep_CheckedChanged( object sender, EventArgs e )
        {
            Request(
                delegate { mPenCommV1.ReqSetupPenBeep( cbBeep.Checked ); },
                delegate { mPenCommV2.ReqSetupPenBeep( cbBeep.Checked ); }
                );
        }

        private void cbOfflineData_CheckedChanged( object sender, EventArgs e )
        {
            Request(
                delegate { },
                delegate { mPenCommV2.ReqSetupOfflineData( cbOfflineData.Checked ); }
                );
        }

        private void btnPwdChange_Click( object sender, EventArgs e )
        {
            Request(
                delegate { mPenCommV1.ReqSetUpPassword( tbOldPassword.Text, tbNewPassword.Text ); },
                delegate { mPenCommV2.ReqSetUpPassword( tbOldPassword.Text, tbNewPassword.Text ); }
                );
        }

        #endregion

        private void ToggleOption(bool enabled)
        {
            groupBox3.Enabled = enabled;
            groupBox5.Enabled = enabled;
            groupBox4.Enabled = enabled;
            groupBox6.Enabled = enabled;
            groupBox7.Enabled = enabled;
        }

        private void btnUpgrade_Click( object sender, EventArgs e )
        {
            if ( tbFirmwarePath.Text == "" || tbFirmwareVersion.Text == "" )
            {
                MessageBox.Show( "Select Firmware File and enter firmware version." );
                return;
            }

            Request( delegate { mPenCommV1.ReqPenSwUpgrade( tbFirmwarePath.Text ); }, delegate { mPenCommV2.ReqPenSwUpgrade( tbFirmwarePath.Text, tbFirmwareVersion.Text ); } );
        }

        private void tbFirmwarePath_Click( object sender, EventArgs e )
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Firmware Files|*._v_";
            openFileDialog1.Title = "Select a Firmware File";

            if ( openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK )
            {
                tbFirmwarePath.Text = openFileDialog1.FileName;
            }
        }
    }
}
