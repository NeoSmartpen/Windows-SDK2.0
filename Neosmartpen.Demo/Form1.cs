using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Diagnostics;
using InTheHand.Net;
using PenComm;


namespace PenDemo
{
    public partial class Form1 : Form, PenSignal, PenDemo.PenDataForm.PenDataResponseHandler
    {
        public PenCommAgent mPenAgent = null;

        public PressureFilter mFilter;

        public static int[] UsingNote = new int[] { 301, 302, 303 };

        public static string defaultPassword = "0000";

        public FirmwareUpgrade fuDlg;

        // form for downloading pen's data
        public PenDataForm penDataFrm;

        private Bitmap mBitmap;
        private Stroke stroke;

        private int width, height;

        private object mDrawLock = new object();

        public Form1()
        {
            InitializeComponent();

            //파일로 로그를 보려면 아래에서
            Log.FileLog();

            mPenAgent = PenCommAgent.GetInstance(this);
            mPenAgent.Init();



            fuDlg = new FirmwareUpgrade( mPenAgent );
            penDataFrm = new PenDataForm( this );

            width  = pictureBox1.Width;
            height = pictureBox1.Height;

            mBitmap = new Bitmap( pictureBox1.Width, pictureBox1.Height );

        }

        private void initImage()
        {
            Graphics g = Graphics.FromImage(mBitmap);
            g.Clear(Color.Transparent);
            g.Dispose();
            pictureBox1.Invalidate();
        }

        //===================Find N2 Pen =======================
        //
        //======================================================

        private void btnSearch_Click(object sender, EventArgs e)
        {
            btnSearch.Enabled = false;

            (new Thread(new ThreadStart(FindDevice))).Start();
        }

        public void FindDevice()
        {
            PenDevice[] devices = mPenAgent.FindAllDevices();

            this.BeginInvoke(new MethodInvoker(delegate()
            {
                lbDevices.Items.Clear();

                if (devices == null || devices.Length <= 0)
                {
                    MessageBox.Show("Searching fail");
                }
                else
                {
                    lbDevices.Items.AddRange(devices);
                }

                btnSearch.Enabled = true;
            }));
        }


        //==============Connect Process =======================
        // Step 1. Connect request(App to Pen)
        // Step 2. Connected(Pen to App)
        // Step 3. Request User Password(Pen to App)
        // Step 4. Input User Password(App to Pen)
        // Step 5. Authenticated(Pen to App)
        // Step 6. Using NCode Info(App to Pen)
        // Step 7. Get Dot(Pen to App)
        //======================================================

        // Step 1. Connect request(App to Pen)
        private void btnConnect_Click(object sender, EventArgs e)
        {
            (new Thread(new ThreadStart(connectToBT))).Start();
        }

        private void connectToBT(){

            if (btnConnect.Text == "Connect")
            {

                if (txtMacAddress.Text == "")
                {
                    MessageBox.Show("Select Mac Address of Pen!!");
                    return;
                }

                string macAddress = txtMacAddress.Text;

                bool connected = mPenAgent.Connect(macAddress);

                if (connected)
                {
                    this.BeginInvoke(new MethodInvoker(delegate()
                    {
                        btnConnect.Text = "Disconnect";
                    }));
                }
                else
                {
                    MessageBox.Show("Fail to connection");
                }
            }
            else
            {
                bool disconnected = mPenAgent.Disconnect();

                if (disconnected)
                {
                    this.BeginInvoke(new MethodInvoker(delegate()
                    {
                        btnConnect.Text = "Connected";
                    }));
                }
                else
                {
                    MessageBox.Show("Disconnecting fail");
                }
            }
        }

        /// <summary>
        // Step 2. Connected(Pen to App)
        /// 펜과 통신 가능한 세션이 성립된 상태
        /// </summary>
        void PenSignal.onConnected(int forceMax, string swVersion)
        {
            System.Console.WriteLine("onConnected forceMax={0}, swVersion={1}", forceMax, swVersion);

            mFilter = new PressureFilter( forceMax );

            this.BeginInvoke(new MethodInvoker(delegate()
            {
                btnConnect.Text = "Disconnect";
            }));
        }

        void PenSignal.onDisconnected()
        {
            System.Console.WriteLine( "onDisconnected" );

            this.BeginInvoke(new MethodInvoker(delegate()
            {
                btnConnect.Text = "Connect";
            }));
        }
        /// <summary>
        // Step 3. Request User Password(Pen to App)
        /// 펜이 사용자 비밀번호를 물어봄
        /// </summary>
        void PenSignal.onPenPasswordRequest(int retryCount, int resetCount)
        {
            System.Console.WriteLine("[UIForm] onPenPasswordRequest ( retryCount : {0}, resetCount : {1} )", retryCount, resetCount);
            
            // Step 4. Input User Password(App to Pen)
            mPenAgent.InputPassword(defaultPassword);
        }


        /// <summary>
        // Step 5. Authenticated(Pen to App)
        /// 펜에 인증되어 모든 기능을 이용 할 수 있는 상태
        /// </summary>
        void PenSignal.onPenAuthenticated()
        {
            // Step 6. Using NCode Info(App to Pen)
            // 사용할 노트 지정
            mPenAgent.AddUsingNote(3, 27, UsingNote);
            mPenAgent.AddUsingNoteAll();

        }

        // Step 7. Get Dot(Pen to App)
        void PenSignal.onReceiveDot(Dot dot)
        {
            processDot(dot);
        }

        void PenSignal.onUpDown(bool isUp)
        {
        }



        void processDot( Dot dot )
        {
            // 필터링 된 필압
            dot.force = mFilter.Filter( dot.force );

            // TODO: Drawing sample code
            if ( dot.type == DotType.PEN_DOWN )
            {
                stroke = new Stroke( dot.sectionId, dot.ownerId, dot.noteId, dot.pageId );
                stroke.Add( dot );
            }
            else if ( dot.type == DotType.PEN_MOVE )
            {
                stroke.Add( dot );
            }
            else if ( dot.type == DotType.PEN_UP )
            {
                stroke.Add( dot );

                drawStroke( stroke );
                
                mFilter.Reset();
            }

            this.BeginInvoke( new MethodInvoker( delegate()
            {
                if (dot.type==DotType.PEN_DOWN)
                {
                    txtPacket.Text += Time.GetDateTime(dot.timeLong).ToString() + " ";
                    txtPacket.Text += dot.ToString() + "\r\n";
                    txtPacket.SelectionStart = txtPacket.TextLength;
                    txtPacket.ScrollToCaret();
                }

            }));
        }

        //============== Rendering Example  ====================
        //
        //======================================================

        private void drawStroke( Stroke stroke )
        {
            lock ( mDrawLock )
            {
                //603 Ring Note Height  5.52  5.41 	63.46 	88.88 
                int dx = (int)( ( 5.52 * width ) / 63.46 );
                int dy = (int)( ( 5.41 * height ) / 88.88 );

                Renderer.draw( mBitmap, stroke, (float)( width / 63.46f ), (float)( height / 88.88f ), -dx, -dy, 1, Color.FromArgb( 200, Color.Blue ) );

                pictureBox1.Image = mBitmap;
            }
        }


        //============== USB DATA Example  ====================
        //
        //======================================================
        private void button1_Click_1(object sender, EventArgs e)
        {
            // to open form showing pen's data files
            if (!penDataFrm.Visible) //&& !mPenAgent.isConnected())
            {
                if (penDataFrm.IsDisposed) penDataFrm = new PenDataForm(this);
                penDataFrm.Show();
            }
        }

        void PenDemo.PenDataForm.PenDataResponseHandler.onReceivePenData( Stroke[] strokes )
        {
            this.BeginInvoke( new MethodInvoker( delegate() 
            {
                initImage();
                if ( strokes != null && strokes.Length > 0 )
                {         
                    txtPacket.Text += Time.GetDateTime(strokes[strokes.Length-1][0].timeLong).ToString() + "\r\n";
                    txtPacket.Text += strokes[0][0].ToString() + "\r\n";
                    txtPacket.SelectionStart = txtPacket.TextLength;
                    txtPacket.ScrollToCaret();
                    foreach ( Stroke str in strokes )
                    {
                        drawStroke( str );
                    }
                }
            }) );
        }







        /// <summary>
        /// Pen Status (F110)
        /// </summary>
        /// <param name="timeoffset">TimeZone Offset(millisecond)</param>
        /// <param name="timetick">RTC Time(UTC millisecond)</param>
        /// <param name="forcemax">forcemax (0-255)</param>
        /// <param name="battery">(0-5)</param>
        /// <param name="usedmem">The percentage usage(%)</param>
        /// <param name="pencolor">Pen color</param>
        /// <param name="autopower">Pen tip Power on using?</param>
        /// <param name="accel">3-axis acceleration sensor using?</param>
        /// <param name="hovermode">hovermode using?</param>
        /// <param name="beep">Beep using?</param>
        /// <param name="autoshutdownTime">autoshutdownTime set(min)</param>
        /// <param name="penSensitivity">pen Sensitivity</param>

        void PenSignal.onReceivedPenStatus(int timeoffset, long timetick, int forcemax, int battery, int usedmem, int pencolor, bool autopower, bool accel, bool hovermode, bool beep, short autoshutdownTime, short penSensitivity)
        {
            System.Console.WriteLine("[UIForm] onReceivedPenStatus ( timeoffset : {0} , timetick : {1}, forcemax : {2}, battery : {3}, usedmem : {4}, pencolor : {5}, autopower : {6}, accerater censor : {7}, hovermode : {8}, beep : {9}, autoshutdownTime : {10}, penSensitivity : {11} )", timeoffset, timetick, forcemax, battery, usedmem, pencolor, autopower, accel, hovermode, beep, autoshutdownTime, penSensitivity);
        }

        /// <summary>
        /// Pen Status (F100E)
        /// </summary>
        /// <param name="timetick"> RTC Time</param>
        /// <param name="battery">battery(0~100%)</param>
        /// <param name="totalmem">total mem(Mbyte)</param>
        /// <param name="usedmem">used mem(Mbyte)</param>
        void PenSignal.onReceivedPenStatus(long timetick, int battery, int totalmem, int usedmem)
        {
            System.Console.WriteLine("[UIForm] onReceivedPenStatus ( timetick : {0}, battery : {1}, totalmem : {2}, usedmem : {3} )", timetick, battery, totalmem, usedmem);
        }


        void PenSignal.onPenPasswordSetUpResponse(bool isSuccess)
        {
            System.Console.WriteLine("[UIForm] onPenPasswordSetUpResponse ( result : {0} )", isSuccess);
        }

        void PenSignal.onPenSensitivitySetUpResponse(bool isSuccess)
        {
            System.Console.WriteLine("[UIForm] onPenSensitivitySetUpResponse ( result : {0} )", isSuccess);

            mPenAgent.ReqPenStatus();
        }

        void PenSignal.onPenAutoShutdownTimeSetUpResponse(bool isSuccess)
        {
            System.Console.WriteLine("[UIForm] onPenAutoShutdownTimeSetUpResponse ( result : {0} )", isSuccess);
        }

        void PenSignal.onPenBeepSetUpResponse(bool isSuccess)
        {
            System.Console.WriteLine("[UIForm] onPenBeepSetUpResponse ( result : {0} )", isSuccess);
        }

        void PenSignal.onPenAutoPowerOnSetUpResponse(bool isSuccess)
        {
            System.Console.WriteLine("[UIForm] onPenAutoPowerOnSetUpResponse ( result : {0} )", isSuccess);
        }

        void PenSignal.onPenColorSetUpResponse(bool isSuccess)
        {
            System.Console.WriteLine("[UIForm] onPenColorSetUpResponse ( result : {0} )", isSuccess);
        }

        void PenSignal.onReceivedFirmwareUpdateResult(bool isSuccess)
        {
            System.Console.WriteLine("[UIForm] onReceivedFirmwareUpdateResult ( result : {0} )", isSuccess);

            if ( fuDlg == null || !fuDlg.Visible )
            {
                return;
            }

            if ( isSuccess )
            {
                fuDlg.SetStatusComplete();
            }
            else
            {
                fuDlg.SetStatusDismiss();
            }
        }

        void PenSignal.onReceivedFirmwareUpdateStatus(int total, int progress)
        {
            if ( fuDlg != null && fuDlg.Visible )
            {
                fuDlg.SetProgress(total, progress);
            }
        }

        private void lbDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox lbEvent = sender as ListBox;
            PenDevice dev = lbEvent.SelectedItem as PenDevice;
            txtMacAddress.Text = dev.address;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate()
            {
                this.txtPacket.Text = "";
                this.txtPacket.SelectionStart = txtPacket.Text.Length;
                this.txtPacket.ScrollToCaret();
                initImage();
            }
            ));
        }

        private void btnPenStatus_Click(object sender, EventArgs e)
        {
            mPenAgent.ReqPenStatus();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if ( !mPenAgent.isConnected() )
            {
                return;
            }

            string filepath = Directory.GetCurrentDirectory() + "\\fw\\N2_tt.zip";

            System.Console.WriteLine("[UIForm] ReqFirmwareUpdate ( result : {0} )", filepath);

            bool result = mPenAgent.ReqFirmwareUpdate(filepath);

            if (!result)
            {
                this.BeginInvoke(new MethodInvoker(delegate()
                {
                    MessageBox.Show("This firmware update request failed.");
                }
            ));
            }

            if ( !fuDlg.Visible )
            {
                fuDlg.init();
                fuDlg.ShowDialog();
            }
        }

        private void button1_Click( object sender, EventArgs e )
        {
            if ( txtMacAddress.Text == "" )
            {
                MessageBox.Show("Choose your device!");
                return;
            }

            mPenAgent.RemovePairedDevice( txtMacAddress.Text );
        }



        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (mPenAgent != null)
            {
                if (mPenAgent.isConnected()) mPenAgent.Disconnect();
                mPenAgent.Dispose();
            }

        }

        //==============Offline data Process =======================
        // Step 1. Search offline data in pen(App to Pen)
        // Step 2. Callback offline data list(Pen to App)
        // Step 3. Request offline data(App to Pen)
        // Step 4. Callback : Start download data(Pen to App)
        // Step 4-1. Callback : Progress Offline data(Pen to App)
        // Step 5. Callback : Finish download data.(Pen to App)
        // Step 6. Callback : Get Stroke Array from download data.(Pen to App)
        //======================================================

        // Step 1. Search offline data in pen(App to Pen)
        private void button4_Click(object sender, EventArgs e)
        {
            mPenAgent.ReqOfflineDataList();
        }

        // Step 2. Callback offline data list(Pen to App)
        void PenSignal.onOfflineDataList(OfflineNote[] notes)
        {
            foreach (OfflineNote d in notes)
            {
                System.Console.WriteLine(d.ToString());
            }

            if (notes != null && notes.Length > 0)
            {
                // Step 3. Request offline data(App to Pen)
                mPenAgent.ReqOfflineData(notes);
            }
        }
        // Step 4. Callback : Start download data(Pen to App)
        /// <summary>
        /// 오프라인 데이터 전송 시작
        /// </summary>
        void PenSignal.onStartOfflineDownload()
        {
            System.Console.WriteLine("[UIForm] onStartOfflineDownload");
        }

        // Step 4-1. Callback : Progress Offline data(Pen to App)
        /// <summary>
        /// Progress Offline data
        /// </summary>
        /// <param name="total">total byte</param>
        /// <param name="progress">pregress byte</param>
        void PenSignal.onUpdateOfflineDownload(int total, int progress)
        {
            System.Console.WriteLine("[UIForm] onUpdateOfflineDownload ({0} / {1})", progress, total);
        }

        // Step 5. Callback : Finish download data.(Pen to App)
        /// <summary>
        /// Finish download data
        /// </summary>
        /// <param name="status">Success/fail</param>
        void PenSignal.onFinishedOfflineDownload(bool status)
        {
            System.Console.WriteLine("[UIForm] onFinishedOfflineDownload status : {0}", status);
        }

        // Step 6. Callback : Get Stroke Array from download data.(Pen to App)
        void PenSignal.onReceiveOfflineStrokes(Stroke[] strokes)
        {
            this.BeginInvoke(new MethodInvoker(delegate()
            {
                if (strokes != null && strokes.Length > 0)
                {
                    System.Console.WriteLine("[UIForm] onReceiveOfflineStrokes sectionId : {0}, ownerId : {1}, noteId : {2}, length : {3}", strokes[0].sectionId, strokes[0].ownerId, strokes[0].noteId, strokes.Count());

                    foreach (Stroke str in strokes)
                    {
                        drawStroke(str);
                    }

                    Array.Clear(strokes, 0, strokes.Length);
                }
            }
            ));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //mPenAgent.ReqSetupPenAutoShutdownTime((short)20);

            //mPenAgent.ReqSetupPenSensitivity((short)5);

            int color_violet = 0x9C3FCD;
            int color_blue = 0x3c6bf0;
            int color_gray = 0xbdbdbd;
            int color_yellow = 0xfbcb26;
            int color_pink = 0xff2084;
            int color_mint = 0x27e0c8;
            int color_red = 0xf93610;
            int color_black = 0x000000;

            //mPenAgent.ReqSetupPenColor(color_yellow);

            //mPenAgent.ReqSetupPenBeep(true);

            //mPenAgent.ReqSetupPenAutoPowerOn(true);
        }



    }
}
