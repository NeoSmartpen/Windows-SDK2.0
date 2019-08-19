using Neosmartpen.Net.Protocol.v1;
using Neosmartpen.Net.Support;
using Neosmartpen.Net.Usb.Demo.OfflineData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Neosmartpen.Net.Usb.Demo
{
    delegate void DrawingViewLog(string logString);

    public partial class DrawingView : Form
    {
        Stroke[] mStrokes = null;

        private object mDrawLock = new object();
        private int width, height;
        private Bitmap mBitmap;
        PaperSize msize = PaperSize.A4W;

        string[] FileArray = null;
        enum PaperSize
        {
            A4W,
            A4H,
            A3W,
            A3H
        }

        public DrawingView(string[] fileNames)
        {
            InitializeComponent();
            this.Text = " DrawingView";
            this.Name = " DrawingView";
            width = pictureBoxDraw.Width;
            height = pictureBoxDraw.Height;
            mBitmap = new Bitmap(pictureBoxDraw.Width, pictureBoxDraw.Height);
            FileArray = fileNames;
        }

        private string getFileName(string path, bool includeExt = true)
        {
            FileInfo fi = new FileInfo(path);
            string name = fi.Name;

            if (!includeExt)
            {
                string ext = fi.Extension;
                if (ext == null)
                    ext = "";

                name = name.Substring(0, name.Length - ext.Length);
            }

            return name;
        }

        private bool isOfflineDataFile( string fileName )
        {
            //string sampleFileName = "data_31b267t";

            //if (fileName.StartsWith("data_") && fileName.EndsWith("t")) // 2017.10.13 commented out by ksshin
            if ( fileName.EndsWith("t") )
            {
                return true;
            }

            return false;
        }

        private void ViewWithStroke(string[] fileNames)
        {
            List<Stroke> mk = new List<Stroke>();
            foreach (string fileName in fileNames)
            {
                string fileNameOnly = getFileName(fileName);

                if (fileName != null)
                {
                    Stroke[] ss = null;

                    if (isOfflineDataFile(fileNameOnly))
                    {
                        FileInfo fi = new FileInfo(fileName);

                        string baseName = fileNameOnly.Substring(0, fileNameOnly.Length - 1);
                        string strokeFile, statusFile, dotFile;

                        strokeFile = fi.DirectoryName + "\\" + baseName + "t";
                        statusFile = fi.DirectoryName + "\\" + baseName + "o";
                        dotFile = fi.DirectoryName + "\\" + baseName + "c";

                        ss = ParseDataFileToStroke(strokeFile, statusFile, dotFile);
                    }

                    if (ss == null)
                    {
                        return;
                    }

                    foreach (Stroke tstroke in ss)
                    {
                        mk.Add(tstroke);
                    }
                }
                else
                {


                }
            }
            mStrokes = mk.ToArray();

        }

        public Stroke[] ParseDataFileToStroke(string strokeFile, string statusFile, string dotFile)
        {
            OfflineDataReader reader = new OfflineDataReader(Log);
            Dot[] dots = reader.Parse(strokeFile, statusFile, dotFile);

            if (dots != null && dots.Length > 0)
            {
                Stroke[] ss = OfflineDataParser.DotArrayToStrokeArray(dots);
                return ss;

            }
            return null;
        }


        private void setSize(PaperSize msize, ref float pdx, ref float pdy, ref float pwx, ref float phy)
        {
            switch (msize)
            {
                case PaperSize.A3H:
                    pdx = 0;
                    pdy = 0;
                    pwx = 90;
                    phy = 140;
                    break;
                case PaperSize.A3W:
                    pdx = 0;
                    pdy = 0;
                    pwx = 140;
                    phy = 90;
                    break;
                case PaperSize.A4H:
                    pdx = 0;
                    pdy = 0;
                    pwx = 140;
                    phy = 180;
                    break;
                case PaperSize.A4W:
                    pdx = 0;
                    pdy = 0;
                    pwx = 180;
                    phy = 140;
                    break;
                default:
                    pdx = 0;
                    pdy = 0;
                    pwx = 180;
                    phy = 180;
                    break;

            }
        }
        private void drawStroke(Stroke stroke)
        {
            lock (mDrawLock)
            {
                //603 Ring Note Height  5.52  5.41 	63.46 	88.88
                //A4 기준 Delta 추정 5,5 width height 86,126
                float pdx = 0;
                float pdy = 0;
                float pwx = 90;
                float phy = 140;
                setSize(msize,ref pdx,ref pdy,ref pwx,ref phy);
                int dx = (int)((pdx * width) / pwx);
                int dy = (int)((pdy * height) / phy);

                Renderer.draw(mBitmap, stroke, (float)(width / pwx), (float)(height / phy), -dx, -dy, 1, Color.FromArgb(200, Color.Blue));

                this.BeginInvoke(new MethodInvoker(delegate()
                {
                    pictureBoxDraw.Image = mBitmap;
                }));
            }
        }
        public void DrawStroke(Stroke[] sss)
        {
            foreach (Stroke ss in sss)
            {
                drawStroke(ss);
            }
        }

        private void DrawingView_Load(object sender, EventArgs e)
        {
            ViewWithStroke(FileArray);
            RefreshView();
        }

        private void DrawingView_SizeChanged(object sender, EventArgs e)
        {
            RefreshView();
        }

        private void RefreshView()
        {
            if (mStrokes != null)
            {
                width = pictureBoxDraw.Width;
                height = pictureBoxDraw.Height;
                mBitmap = new Bitmap(pictureBoxDraw.Width, pictureBoxDraw.Height);
                DrawStroke(mStrokes);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox obj = (ComboBox)sender;
            switch (obj.SelectedIndex)
            {
                case 0:
                    msize = PaperSize.A3H;
                    break;
                case 1:
                    msize = PaperSize.A3W;
                    break;
                case 2:
                    msize = PaperSize.A4H;
                    break;
                case 3:
                    msize = PaperSize.A4W;
                    break;
                default:
                    break;
         
            }
            Console.WriteLine(msize);
            width = pictureBoxDraw.Width;
            height = pictureBoxDraw.Height;
            mBitmap = new Bitmap(pictureBoxDraw.Width, pictureBoxDraw.Height);
            DrawStroke(mStrokes);
        }


        private void DrawingView_DragDrop(object sender, DragEventArgs e)
        {
            Console.WriteLine("DragDrop ");
            string[] strFullFilename = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            ViewWithStroke(strFullFilename);
            RefreshView();
        }

        private void DrawingView_DragEnter(object sender, DragEventArgs e)
        {
            Console.WriteLine("DragDrop Enter");
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }

        }

        private void Log(String log)
        {
            if (textBox1 == null) return;
            this.BeginInvoke(new MethodInvoker(delegate()
            {
                if (textBox1.Enabled)
                {
                    textBox1.AppendText(log);
                    textBox1.AppendText(Environment.NewLine);
                }
            }));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }
    }
}
