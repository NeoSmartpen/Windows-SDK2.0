using System;
using System.Collections.Generic;
using System.IO;

namespace Neosmartpen.Net.Usb.Demo.OfflineData
{
    class OfflineDataReader
    {
        private List<Dot> mDots = new List<Dot>();
        private DrawingViewLog mLog = null;

        FileStream mStrokeInputStream = null;
        FileStream mDotInputStream = null;

        StrokeFileHeader strokeFileHeader = new StrokeFileHeader();
        List<StrokeRef> strokeRefList = new List<StrokeRef>();
        DotDataHeader dotDataHeader = new DotDataHeader();

        private bool isLittleEndian = true;

        public OfflineDataReader(DrawingViewLog log)
        {
            mLog = log;
        }

        public Dot[] Parse(string strokeFile, string statusFile, string dotFile)
        {
            try
            {
                mStrokeInputStream = new FileStream(strokeFile, FileMode.Open);
                //mStatusInputStream = new FileStream(statusFile, FileMode.Open);
                mDotInputStream = new FileStream(dotFile, FileMode.Open);

                ParseHeader();
                ParseBody();
            }
            catch (Exception e)
            {
                mLog("[OfflineDataParser] parsing exception occured." +  e.ToString());
            }

            try { mDotInputStream.Close(); } catch (Exception e) {}
            //try { mStatusInputStream.Close(); } catch (Exception e) {}
            try { mStrokeInputStream.Close(); } catch (Exception e) {}

            if (mDots == null || mDots.Count <= 0)
            {
                return null;
            }
            else
            {
                return mDots.ToArray();
            }
        }

        private void ParseHeader()
        {
            if( strokeFileHeader.readFromStream(mStrokeInputStream) == false )
                throw new Exception("Could not load the stroke file header!");

            if (dotDataHeader.readFromStream(mDotInputStream) == false)
                throw new Exception("Could not load the dot file header!");

            isLittleEndian = strokeFileHeader.isLittleEndian;
        }

        private void ParseBody()
        {
            mDots.Clear();
            strokeRefList.Clear();

            while (true)
            {
                StrokeRef strokeRef = new StrokeRef(isLittleEndian);

                if (strokeRef.readFromStream(mStrokeInputStream))
                    strokeRefList.Add(strokeRef);
                else
                    break;
            }

            for (int i = 0; i < strokeRefList.Count; i++)
            {
                StrokeRef strokeRef = strokeRefList[i];

                for (int n = 0; n < strokeRef.codeCount; n++)
                {
                    DotData dotData = new DotData(isLittleEndian);

                    if (dotData.readFromStream(mDotInputStream) == false)
                        break;

                    //if (status == 0x00 || status == 0x01)
                    {
                        DotTypes type = (n == 0) ? DotTypes.PEN_DOWN : DotTypes.PEN_MOVE;

                        Dot dot = new Dot(strokeFileHeader.ownerId, strokeFileHeader.sectionId, strokeFileHeader.noteId, strokeRef.pageId,
                                            strokeRef.downTime + dotData.timestampDelta, dotData.x, dotData.y, dotData.fx, dotData.fy, dotData.force,
                                            type, strokeRef.penTipColor);

                        mDots.Add(dot);
                    }
                }
            }
        }
    }
}
