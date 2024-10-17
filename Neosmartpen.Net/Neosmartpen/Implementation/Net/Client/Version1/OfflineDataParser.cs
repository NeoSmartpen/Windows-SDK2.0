﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using Neosmartpen.Net.Support;
using Neosmartpen.Net.Filter;

namespace Neosmartpen.Net
{
    // 오프라인 데이터 파일을 파싱해서 도트묶음으로 반환
    public class OfflineDataParser : OfflineData
    {
        private byte[] mData, mBody;

        private List<Dot> mDots = new List<Dot>();

        private int mOwnerId = 0, mSectionId = 0, mNoteId = 0, mPageId = 0;
        private int mLineCount, mDataSize;

        //private byte headerCheckSum;

        private string mTarget = null;

        //private bool mIsCompressed = false;

        private const int LINE_MARK_1 = 0x4c;
        private const int LINE_MARK_2 = 0x4e;

        private const int BYTE_LINE_SIZE   = 28;
        private const int BYTE_DOT_SIZE    = 8;
        private const int BYTE_HEADER_SIZE = 64;

		private FilterForPaper offlineFilterForPaper;

        public OfflineDataParser( string fullpath )
        {
            mTarget = fullpath;
			offlineFilterForPaper = new FilterForPaper(AddOfflineFilteredDot);
        }

        public Dot[] Parse(int penMaxFoce)
        {
            if ( !mTarget.EndsWith( EXT_DATA ) && !mTarget.EndsWith( EXT_ZIP )  )
            {
                Debug.WriteLine( "[OfflineDataParser] this file is not data file" );
                return null;
            }

            try
            {
                LoadDataFromFile( mTarget );
                ParseHeader();
                ParseBody(penMaxFoce);
            }
            catch ( Exception e )
            {
                Debug.WriteLine( "[OfflineDataParser] parsing exception occured.", e );
            }

            if ( mDots == null || mDots.Count <= 0 )
            {
                return null;
            }
            else
            {
                return mDots.ToArray();
            }
        }

        public void Delete()
        {
            Debug.WriteLine( "[OfflineDataParser] delete file : " + mTarget );
            System.IO.File.Delete( mTarget );
        }

        private void LoadDataFromFile( string fileName )
        {
            if ( fileName.EndsWith( EXT_ZIP ) )
            {
                string datafile = Extract( fileName );

                mData = System.IO.File.ReadAllBytes( datafile );

                System.IO.File.Delete( datafile );
            }
            else 
            {
                mData = System.IO.File.ReadAllBytes( fileName );
            }
        }

        private string Extract( string file )
        {
            string newfile = null;

            //System.Console.WriteLine( "[OfflineDataParser] Extract {0}", file );

            string prefix = OfflineData.GetFileNameFromFullPath( file );

            string temp = DEFAULT_PATH + "\\" + DIR_TEMP + prefix;

            try
            {
                // 임시 폴더 생성
                System.IO.Directory.CreateDirectory( temp );

				// 임시 폴더에 압축 해제
				// TODO Check
				ZipArchive zipfile = ZipFile.OpenRead(file);
				zipfile.ExtractToDirectory(temp);
                zipfile.Dispose();

                // 압축 해제된 파일을 기본 폴더로 이동
                string[] infiles = Directory.GetFiles( temp );

                if ( infiles == null || infiles.Length != 1 )
                {
                    throw new Exception();
                }

                string infile = infiles[0];

                newfile = DEFAULT_PATH + "\\" + prefix + EXT_DATA;

                //System.Console.WriteLine( "[OfflineDataParser] Extract {0} to {1}", infile, newfile );

                System.IO.File.Move( infile, newfile );
            }
            finally
            {
                // 임시 폴더 삭제
                if ( System.IO.Directory.Exists( temp ) )
                {
                    System.IO.Directory.Delete( temp, true );
                }
            }

            return newfile;
        }

        public static byte[] CopyOfRange( byte[] datas, int start, int size )
        {
            byte[] result = new byte[size];
            Array.Copy( datas, start, result, 0, size );
            return result;
        }

        private void ParseHeader()
        {
            byte[] header = CopyOfRange( mData, mData.Length - BYTE_HEADER_SIZE, BYTE_HEADER_SIZE );

            byte[] osbyte = CopyOfRange( header, 6, 4 );

            mSectionId = (int)( osbyte[3] & 0xFF );
            mOwnerId = ByteConverter.ByteToInt( new byte[] { osbyte[0], osbyte[1], osbyte[2], (byte)0x00 } );

            mNoteId = ByteConverter.ByteToInt( CopyOfRange( header, 10, 4 ) );
            mPageId = ByteConverter.ByteToInt( CopyOfRange( header, 14, 4 ) );

            mLineCount = ByteConverter.ByteToInt( CopyOfRange( header, 22, 4 ) );
            mDataSize  = ByteConverter.ByteToInt( CopyOfRange( header, 26, 4 ) );
            //this.headerCheckSum = header[BYTE_HEADER_SIZE-1];

            mBody = CopyOfRange( mData, 0, mData.Length - BYTE_HEADER_SIZE );

            if ( mBody.Length != mDataSize )
            {
                throw new Exception( "data size is invalid" );
            }

            //System.Console.WriteLine( "[OfflineDataParser] noteId : " + noteId + ", pageId : " + pageId + ", lineCount : " + lineCount + ", fileSize : " + dataSize + "byte" );
        }

        public static byte CalcChecksum( byte[] bytes )
        {
            int CheckSum = 0;

            for ( int i = 0; i < bytes.Length; i++ )
            {
                CheckSum += (int)( bytes[i] & 0xFF );
            }

            return (byte)CheckSum;
        }

        private void ParseBody(int penMaxForce)
        {
            mDots.Clear();

            long penDownTime = 0, penUpTime = 0, prevTimestamp = 0;

            int  dotTotalCount = 0, dotCount = 0;
            
            byte lineCheckSum = 0;
            
            int  dotStartIndex = 0, dotSize = 0;
            
            byte[] lineColorBytes = new byte[4];
            
            int lineColor = 0x000000;

            // 현재 라인의 도트
            offlineDots = new List<Dot>();

            int i = 0;

            while ( i < mBody.Length && mBody.Length > 0 )
            {
                if ( ByteConverter.SingleByteToInt( mBody[i] ) == LINE_MARK_1 && ByteConverter.SingleByteToInt( mBody[i + 1] ) == LINE_MARK_2 )
                {
                    offlineDots = new List<Dot>();

                    penDownTime = ByteConverter.ByteToLong( CopyOfRange( mBody, i + 2, 8 ) );
                    penUpTime = ByteConverter.ByteToLong( CopyOfRange( mBody, i + 10, 8 ) );
                    dotTotalCount = ByteConverter.ByteToInt( CopyOfRange( mBody, i + 18, 4 ) );
                    lineColorBytes = CopyOfRange( mBody, i + 23, 4 );

                    lineColor = ByteConverter.ByteToInt( new byte[] { lineColorBytes[2], lineColorBytes[1], lineColorBytes[0], (byte)0 } );

                    //System.Console.WriteLine( "[OfflineDataParser] penDownTime : {0}, penUpTime : {1}, dotTotalCount : {2}, lineColor : {3}", penDownTime, penUpTime, dotTotalCount, lineColor );

                    lineCheckSum = mBody[i + 27];

                    i += BYTE_LINE_SIZE;

                    dotStartIndex = i;
                    dotSize  = 0;
                    dotCount = 0;
                }
                else
                {
                    dotCount++;

                    // 스트로크 헤더에 정의된 도트갯수보다 넘어가면 LN이 나올때까지 한바이트씩 포인터를 이동한다.
                    if ( dotCount > dotTotalCount )
                    {
                        i++;
                        continue;
                    }

                    long timeGap = ByteConverter.SingleByteToInt( mBody[i] );

                    short x = ByteConverter.ByteToShort( CopyOfRange( mBody, i + 1, 2 ) );
                    short y = ByteConverter.ByteToShort( CopyOfRange( mBody, i + 3, 2 ) );

                    int fx = ByteConverter.SingleByteToInt( mBody[i + 5] );
                    int fy = ByteConverter.SingleByteToInt( mBody[i + 6] );

                    int force = ByteConverter.SingleByteToInt( mBody[i + 7] );

                    int color = lineColor;

                    bool isPenUp = false;

                    long timestamp = -1L;
                    
                    DotTypes dotType;

                    if ( dotSize == 0 )
                    {
                        dotType = DotTypes.PEN_DOWN;
                        timestamp = penDownTime + timeGap;
                        prevTimestamp = timestamp;
                    }
                    else if ( dotTotalCount > dotCount )
                    {
                        dotType = DotTypes.PEN_MOVE;
                        timestamp = prevTimestamp + timeGap;
                        prevTimestamp = timestamp;
                    }
                    else
                    {
                        dotType = DotTypes.PEN_UP;
                        timestamp = penUpTime;
                        isPenUp = true;
                    }

					Dot.Builder builder;
					if (penMaxForce == 0) builder = new Dot.Builder();
					else builder = new Dot.Builder(penMaxForce);
                    offlineFilterForPaper.Put(
							builder
                            .section( mSectionId )
                            .owner( mOwnerId )
                            .note( mNoteId )
                            .page( mPageId )
                            .coord( x + fx * 0.01f, y + fy * 0.01f )
                            .force( force )
                            .color( color )
                            .timestamp( timestamp )
                            .dotType( dotType ).Build(), null
                     );

                    dotSize += 8;

                    if ( isPenUp )
                    {
                        byte dotCalcCs = CalcChecksum( CopyOfRange( mBody, dotStartIndex, dotSize ) );

                        if ( dotCalcCs == lineCheckSum )
                        {
                            for ( int j = 0; j < offlineDots.Count; j++ )
                            {
                                mDots.Add( offlineDots[j] );
                            }
                        }
                        else
                        {
                            Debug.WriteLine( "[OfflineDataParser] invalid CheckSum cs : " + lineCheckSum + ", calc : " + dotCalcCs );
                        }

                        offlineDots = new List<Dot>();
                    }

                    i += BYTE_DOT_SIZE;
                }
            }
        }

		private List<Dot> offlineDots;
		private void AddOfflineFilteredDot(Dot dot, object obj)
		{
			offlineDots.Add(dot);
		}
    }
}
