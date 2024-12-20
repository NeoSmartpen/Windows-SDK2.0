﻿using System;

namespace Neosmartpen.Net
{
    /// <exclude />
    public class OfflineDataInfo
    {
        public int Section { protected set; get; }

        public int Owner { protected set; get; }

        public int Note { protected set; get; }

        public int[] Pages { protected set; get; }

        public OfflineDataInfo( int sectionId, int ownerId, int noteId, int[] pages = null )
        {
            Section = sectionId;
            Owner = ownerId;
            Note = noteId;
            Pages = pages;
        }

        public override string ToString()
        {
            return String.Format( "sec:{0}, owner:{1}, note:{2}", Section, Owner, Note );
        }
    }

    /// <exclude />
    public class OfflineDataFile
    {
        public int Section, Owner, Note;
		public int totalDataSize, receiveDataSize;
        public string FilePath;

        public OfflineDataFile( int sectionId, int ownerId, int noteId, string filePath, int totalSize, int receiveDataSize )
        {
            Section = sectionId;
            Owner = ownerId;
            Note = noteId;
            FilePath = filePath;
			totalDataSize = totalSize;
			this.receiveDataSize = receiveDataSize;
        }

        public void Delete()
        {
            if ( System.IO.Directory.Exists( FilePath ) )
            {
                System.IO.Directory.Delete( FilePath, true );
            }
        }
    }
}
