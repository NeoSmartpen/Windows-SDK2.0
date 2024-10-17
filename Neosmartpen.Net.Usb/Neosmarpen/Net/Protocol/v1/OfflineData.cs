﻿using System.Collections.Generic;
using System.IO;

namespace Neosmartpen.Net.Protocol.v1
{
    public abstract class OfflineData
    {
        public static string CURRENT_DIR;
        public static string DEFAULT_PATH;
        public static string DEFAULT_ERROR_PATH;

        public readonly string EXT_ZIP   = ".zip";
        public readonly string EXT_ERROR = ".err"; 
        public readonly string EXT_DATA  = ".pen";
        public readonly string DIR_ROOT  = "offline";
        public readonly string DIR_ERROR = "err";
        public readonly string DIR_TEMP  = "temp_";

        // 기본 디렉토리 생성
        public void SetDefaultPath( string basepath )
        {
            basepath = basepath == null || basepath == "" ? Directory.GetCurrentDirectory() : basepath;

            OfflineData.DEFAULT_PATH = basepath + "\\" + DIR_ROOT;
            OfflineData.DEFAULT_ERROR_PATH = DEFAULT_PATH + "\\" + DIR_ERROR;
        }

        public void SetupFileSystem()
        {
            if ( !System.IO.Directory.Exists( DEFAULT_PATH ) )
            {
                System.IO.Directory.CreateDirectory( DEFAULT_PATH );
            }

            // 에러 파일 저장 위치 설정
            string errorFilePath = DEFAULT_PATH + "\\" + DIR_ERROR;

            if ( !System.IO.Directory.Exists( errorFilePath ) )
            {
                System.IO.Directory.CreateDirectory( errorFilePath );
            }
        }

        public string[] GetOfflineFiles()
        {
            string[] filePaths = Directory.GetFiles( DEFAULT_PATH );
            return filePaths;
        }

        public string[] GetErrorFiles()
        {
            string[] filePaths = Directory.GetFiles( DEFAULT_PATH + "\\" + DIR_ERROR );
            return filePaths;
        }

        public static string GetFileFromFullPath( string fullpath )
        {
            string[] arr = fullpath.Split( '\\' );
            
            if ( arr.Length <= 1 )
            {
                return null;
            }

            return arr[arr.Length - 1];
        }

        public static string GetFileNameFromFullPath( string fullpath )
        {
            string[] arr = GetFileFromFullPath( fullpath ).Split( '.' );

            if ( arr.Length < 2 )
            {
                return null;
            }

            return arr[0];
        }

        public static string GetFileExtFromFullPath( string fullpath )
        {
            string[] arr = GetFileFromFullPath( fullpath ).Split( '.' );

            if ( arr.Length < 2 )
            {
                return null;
            }
            
            string ext = "";

            for ( int i=1; i<arr.Length; i++)
            {
                ext += arr[i];
            }

            return ext;
        }

        public static Stroke[] DotArrayToStrokeArray( Dot[] dots )
        {
            List<Stroke> slist = new List<Stroke>();

            Stroke temp = null;

            foreach ( Dot dot in dots )
            {
                if ( dot.DotType == DotTypes.PEN_DOWN )
                {
                    temp = new Stroke( dot.Section, dot.Owner, dot.Note, dot.Page );
                    slist.Add( temp );
                }

                temp.Add( dot );
            }

            return slist.ToArray();
        }
    }
}
