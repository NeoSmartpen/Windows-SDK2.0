using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Neosmartpen.Net.Support;

namespace Neosmartpen.Net
{
    /// <exclude />
    public class OfflineDataSerializer : OfflineData
    {
        public Dictionary<int, byte[]> chunks;
	
	    private int mPacketCount;

	    private bool IsCompressed = false;

        public int sectionId = 0, ownerId = 0, noteId = 0, pageId = 0;

        public static string DEFAULT_FILE_FORMAT = "{0}_{1}_{2}_{3}_{4}.{5}";
	    
        public OfflineDataSerializer( string filepath, int packetCount, bool isCompressed )
	    {
            chunks = new Dictionary<int, byte[]>();

            string[] arr = filepath.Split( '\\' );

            int sectionOwner = int.Parse( arr[2] );

            byte[] bso = ByteConverter.IntToByte( sectionOwner );

            sectionId = (int)( bso[3] & 0xFF );
            ownerId = ByteConverter.ByteToInt( new byte[] { bso[0], bso[1], bso[2], (byte)0x00 } );

		    noteId = int.Parse( arr[3] );
            pageId = int.Parse( arr[4] );
				
		    mPacketCount  = packetCount;
            IsCompressed = isCompressed;

            base.SetupFileSystem();
	    }
	
	    public string MakeFile() 
	    {
		    lock ( OfflineData.DEFAULT_PATH )
		    {
                ByteUtil buff = new ByteUtil();
		
			    for ( int i=0; i < chunks.Count(); i++ )
			    {
                    buff.Put( chunks[i] );
			    }

                string filename = String.Format( DEFAULT_FILE_FORMAT, sectionId, ownerId, noteId, pageId, Time.GetUtcTimeStamp(), IsCompressed ? "zip" : "pen" ); 

                string fullpath = OfflineData.DEFAULT_PATH + "\\" + filename;

                if ( ByteToFile( buff.ToArray(), fullpath ) )
			    {
                    return fullpath;
			    }
			    else
			    {
				    return null;
			    }
		    }
	    }
	
	    public void Put( byte[] data, int index ) 
	    {
            chunks.Add(index, data);
	    }

        private bool ByteToFile( byte[] bytes, String filepath ) 
	    {
            try
            {
				using (System.IO.FileStream fs = new System.IO.FileStream(filepath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
				{
					fs.Write(bytes, 0, bytes.Length);
					// 필요하다면...
					//fs.Flush();
				}

                return true;
            }
            catch ( Exception e )
            {
                Debug.WriteLine( "[OfflineDataSerializer] Exception caught in process: {0}", e.StackTrace );
            }

            // error occured, return false
            return false;
	    }
    }
}
