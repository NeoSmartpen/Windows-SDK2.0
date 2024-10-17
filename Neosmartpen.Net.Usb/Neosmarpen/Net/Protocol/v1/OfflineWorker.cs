﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Neosmartpen.Net.Protocol.v1
{
    public class OfflineWorker : OfflineData
    {
	    // 처리해야 할 노트 목록
	    private Queue<OfflineDataInfo> mOfflineRequestQueue = null;

        private Queue<OfflineDataFile> mOfflineProcessQueue = null;

        private AutoResetEvent lockHandleRequest, lockHandleProcess;

        private Thread tOfflineRequestThread, tOfflineProcessThread;

	    // 현재 처리중인 노트
	    private OfflineDataInfo currentNote = null;

        private OfflineWorkResponseHandler mHandler;

        private object noteLock = new object();

        private bool isRun = true;

        public int PenMaxForce = 0;

        public OfflineWorker( OfflineWorkResponseHandler handler )
	    {
		    mOfflineRequestQueue = new Queue<OfflineDataInfo>();
            mOfflineProcessQueue = new Queue<OfflineDataFile>();
            
            mHandler = handler;
	    }

        public void Startup( string basedir = null )
        {
            base.SetDefaultPath( basedir );

            lockHandleRequest = new AutoResetEvent( false );
            lockHandleProcess = new AutoResetEvent( false );

            base.SetupFileSystem();

            tOfflineRequestThread = new Thread( new ThreadStart( RequestNote ) );
            tOfflineRequestThread.Name = "tOfflineRequestThread";
            tOfflineRequestThread.IsBackground = true;
            tOfflineRequestThread.Start();
            
            tOfflineProcessThread = new Thread( new ThreadStart( ProcessNote ) );
            tOfflineProcessThread.Name = "tOfflineProcessThread";
            tOfflineProcessThread.IsBackground = true;
            tOfflineProcessThread.Start();
        }

        public void Dispose()
        {
            isRun = false;

            lock ( lockHandleRequest )
            {
                lockHandleRequest.Set();
            }

            lock ( lockHandleProcess )
            {
                lockHandleProcess.Set();
            }

            tOfflineRequestThread.Join();
            tOfflineProcessThread.Join();
        }

	    public void Reset()
	    {
		    mOfflineRequestQueue.Clear();

            lock ( noteLock )
		    {
		    	currentNote = null;
		    }
	    }

        public void Put( OfflineDataInfo note )
        {
            Put( new OfflineDataInfo[] { note } );
        }

        public void Put( OfflineDataInfo[] notes )
        {
            bool isEmpty = mOfflineRequestQueue.Count() <= 0;

            foreach ( OfflineDataInfo n in notes )
            {
                mOfflineRequestQueue.Enqueue( n );
            }

            if ( isEmpty )
            {
                lock ( lockHandleRequest )
                {
                    lockHandleRequest.Set();
                }
            }
        }

        public void onFinishDownload()
	    {
		    if ( currentNote == null )
		    {
			    return;
		    }

            if ( currentNote.Section != 4 )
            {
                mHandler.onRequestRemoveOfflineData( 4, currentNote.Note );
            }

		    lock ( noteLock )
		    {
			    currentNote = null;
		    }

            mOfflineRequestQueue.Dequeue();

            lock ( lockHandleRequest )
            {
                lockHandleRequest.Set();
            }
	    }

        public void onCreateFile( int sectionId, int ownerId, int noteId, String filepath )
	    {
            System.Console.WriteLine( "[OfflineWorker] onCreateFile => sectionId : " + sectionId + ", ownerId : " + ownerId + ", noteId : " + noteId + ", filepath : " + filepath );
            EnqueueProcessQueue( new OfflineDataFile( sectionId, ownerId, noteId, filepath ) );
	    }

        private void EnqueueProcessQueue( OfflineDataFile dataFile )
        {
            System.Console.WriteLine( "[OfflineWorker] enqueueProcessQueue => sectionId : " + dataFile.Section + ", ownerId : " + dataFile.Owner + ", noteId : " + dataFile.Note );

            mOfflineProcessQueue.Enqueue( dataFile );

            lock ( lockHandleProcess )
            {
                lockHandleProcess.Set();
            }
        }

		private void RequestNote()
		{
            while ( isRun )
			{
				if ( mOfflineRequestQueue.Count() > 0 )
				{
                    lock ( noteLock )
                    {
                        currentNote = mOfflineRequestQueue.Peek();

                        System.Console.WriteLine( "[OfflineWorker] start download offline note => sectionId : " + currentNote.Section + ", ownerId : " + currentNote.Owner + ", noteId : " + currentNote.Note );

                        // 데이터 전송을 요청한다.
                        mHandler.onRequestDownloadOfflineData( currentNote.Section, currentNote.Owner, currentNote.Note );
                    }
				}
                    
                {
                    lockHandleRequest.WaitOne();
                }
			}
		}

        private void ProcessNote()
        {
            while ( isRun )
            {
				while( mOfflineProcessQueue.Count() > 0 )
                {
                    OfflineDataFile oFile = mOfflineProcessQueue.Dequeue();

                    if ( oFile.Section != 4 )
                    {
                        ProcessData( oFile );
                    }
				}

                {
                    lockHandleProcess.WaitOne();
                }
            }
        }

        private Stroke[] DataFileToStroke( OfflineDataFile sfile, int penMaxForce)
	    {
            if ( sfile == null )
		    {
			    return null;
		    }

            OfflineDataParser parser;

			try
			{
				if ( !System.IO.File.Exists( sfile.FilePath ) )
				{
                    System.Console.WriteLine( "[OfflineWorker] file not found : " + sfile.FilePath );
					return null;
				}

				parser = new OfflineDataParser( sfile.FilePath );

                Dot[] dots = parser.Parse(penMaxForce);

                parser.Delete();
 
				if ( dots != null && dots.Length > 0 )
				{
                    return DotArrayToStrokeArray(dots);
				}
                else
                {
                    System.Console.WriteLine( "[OfflineWorker] parseError" );
                }
			}
			catch ( Exception e )
			{
                System.Console.WriteLine( "[OfflineWorker] parse file exeption occured. => {0}", e );
			}
            finally
            {
                parser = null;
            }

		    return null;
	    }

        private void ProcessData( OfflineDataFile currentFile )
	    {
            System.Console.WriteLine( "[OfflineWorker] processData() - begin" );
            
            // 노트 스트로크
            Stroke[] sarr = DataFileToStroke( currentFile, PenMaxForce );

            if ( sarr == null || sarr.Length <= 0 )
            {
                return;
            }

            mHandler.onReceiveOfflineStrokes(sarr);

            System.Console.WriteLine( "[OfflineWorker] processData() - finish" );
	    }
    }

    public interface OfflineWorkResponseHandler
    {
        void onReceiveOfflineStrokes( Stroke[] strokes );

        void onRequestDownloadOfflineData( int sectionId, int ownerId, int noteId );

        void onRequestRemoveOfflineData( int sectionId, int ownerId );
    }
}
