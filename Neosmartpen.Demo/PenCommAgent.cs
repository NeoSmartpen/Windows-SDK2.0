using Neosmartpen.Net.Bluetooth;
using Neosmartpen.Net.Protocol.v1;
using Neosmartpen.Net.Protocol.v2;
using System;
using System.IO;

namespace Neosmartpen.Net
{
    /// <summary>
    /// PenCommAgent class provides fuctions that can handle pen.
    /// 
    /// In order to use the various N2 thechnologies, you first have to obtain an instance of the PenCommAgent class by invoking the getInstance static method.
    /// If you obtain an instance, you have to call Init method.
    /// </summary>
    public class PenCommAgent : IDisposable
    {
        private BluetoothAdapter mBtAdapter;

        private PenCommV1Callbacks mCallbackV1;

        private PenCommV2Callbacks mCallbackV2;

        private PenCommV1 mCommV1;
            
        private PenCommV2 mCommV2;

        private bool IsInitilized = false;

        private static PenCommAgent mInstance;

        private static string BASE_DIRECTORY;

        private bool IsV1Comm = false;

        /// <summary>
        /// Static method that can creates and return a new PenCommAgent object.
        /// In order to listen message from a pen, you have to implement and register PenSignal callback interface.
        /// </summary>
        /// <param name="callback">an object implementing the PenSignal interface</param>
        /// <returns>a PenCommAgent object</returns>
        public static PenCommAgent GetInstance( PenCommV1Callbacks callbackForV1, PenCommV2Callbacks callbackForV2 )
        {
            if ( mInstance == null )
            {
                mInstance = new PenCommAgent( callbackForV1, callbackForV2 );
            }

            return mInstance;
        }

        private PenCommAgent( PenCommV1Callbacks callbackForV1, PenCommV2Callbacks callbackForV2 )
        {
            mCallbackV1 = callbackForV1;
            mCallbackV2 = callbackForV2;

            mBtAdapter = new BluetoothAdapter();
        }

        /// <summary>
        /// Launches the PenCommAgent at specific location.
        /// </summary>
        /// <param name="baseDirectory">absolute path of base directory</param>
        public void Init( string baseDirectory )
        {
            if ( IsInitilized )
            {
                return;
            }

            BASE_DIRECTORY = baseDirectory;

            if ( BASE_DIRECTORY == null || BASE_DIRECTORY == "" )
            {
                BASE_DIRECTORY = Directory.GetCurrentDirectory();
            }

            if ( !mBtAdapter.Enabled )
            {
                IsInitilized = false;
                return;
            }

            IsInitilized = true;
        }

        /// <summary>
        /// Launches the PenCommAgent at default location.
        /// </summary>
        public void Init()
        {
            Init( null );
        }

        public PenDevice[] FindAllDevices()
        {
            return mBtAdapter.FindAllDevices();
        }

        /// <summary>
        /// Attempt to connect to a pen device.
        /// This method will block until a connection is made or the connection fails.
        /// If you do not want block UI thread, you have to invoke method in another thread.
        /// </summary>
        /// <param name="btAddr">MAC address of pen</param>
        /// <returns>true if connection is made, otherwise false.</returns>
        public bool Connect( string btAddr )
        {
            return mBtAdapter.Connect( btAddr, delegate( uint deviceClass ) 
            {
                mCommV1 = new PenCommV1( mCallbackV1 );
                mCommV2 = new PenCommV2( mCallbackV2 );

                if ( deviceClass == 0x0500 )
                {
                    IsV1Comm = true;
                    System.Console.WriteLine( "bind socket with v1 {0}", deviceClass );
                    mBtAdapter.Bind( mCommV1 );
                }
                else if ( deviceClass == 0x2510 )
                {
                    IsV1Comm = false;
                    System.Console.WriteLine( "bind socket with v2 {0}", deviceClass );
                    mBtAdapter.Bind( mCommV2 );
                }
            } );
        }

        /// <summary>
        /// Attempt to disconnect to a pen device.
        /// </summary>
        /// <returns>true if disconnected, otherwise false.</returns>
        public bool Disconnect()
        {
            return mBtAdapter.Disconnect();
        }

        /// <summary>
        /// Get the connection status of PenCommAgent, ie, whether there is an active connection with remote device.
        /// </summary>
        /// <returns>true if connected false if not connected</returns>
        public bool isConnected()
        {
            return mBtAdapter.Connected;
        }

        /// <summary>
        /// To remove paired pen
        /// </summary>
        /// <param name="addr">MAC address of pen</param>
        public void RemovePairedDevice( string addr )
        {
            mBtAdapter.RemovePairedDevice( addr );
        }

        /// <summary>
        /// Sets the value of the pen's sensitivity property that controls the force sensor of pen.
        /// </summary>
        /// <param name="level">the value of sensitivity. (0~4, 0 means maximum sensitivity)</param>
        public void ReqSetupPenSensitivity( short level )
        {
            if ( IsV1Comm )
            {
                mCommV1.ReqSetupPenSensitivity( level );
            }
            else
            {
                mCommV2.ReqSetupPenSensitivity( level );
            }
        }

        /// <summary>
        /// Sets the value of the auto shutdown time property that if pen stay idle, shut off the pen.
        /// </summary>
        /// <param name="minute">minute of maximum idle time, staying power on (0~)</param>
        public void ReqSetupPenAutoShutdownTime( short minute )
        {
            if ( IsV1Comm )
            {
                mCommV1.ReqSetupPenAutoShutdownTime( minute );
            }
            else
            {
                mCommV2.ReqSetupPenAutoShutdownTime( minute );
            }
        }

        /// <summary>
        /// Sets the status of the auto power on property that if write the pen, turn on when pen is down.
        /// </summary>
        /// <param name="seton">true if you want to use, otherwise false.</param>
        public void ReqSetupPenAutoPowerOn( bool seton )
        {
            if ( IsV1Comm )
            {
                mCommV1.ReqSetupPenAutoPowerOn( seton );
            }
            else
            {
                mCommV2.ReqSetupPenAutoPowerOn( seton );
            }
        }

        /// <summary>
        /// Sets the status of the beep property.
        /// </summary>
        /// <param name="seton">true if you want to listen sound of pen, otherwise false.</param>
        public void ReqSetupPenBeep( bool seton )
        {
            if ( IsV1Comm )
            {
                mCommV1.ReqSetupPenBeep( seton );
            }
            else
            {
                mCommV2.ReqSetupPenBeep( seton );
            }
        }

        /// <summary>
        /// Sets the color of pen ink.
        /// If you want to change led color of pen, you should choose one among next preset values.
        /// 
        /// violet = 0x9C3FCD
        /// blue = 0x3c6bf0
        /// gray = 0xbdbdbd
        /// yellow = 0xfbcb26
        /// pink = 0xff2084
        /// mint = 0x27e0c8
        /// red = 0xf93610
        /// black = 0x000000
        /// </summary>
        /// <param name="rgbcolor">integer type color formatted 0xRRGGBB</param>
        public void ReqSetupPenColor( int rgbcolor )
        {
            if ( IsV1Comm )
            {
                mCommV1.ReqSetupPenColor( rgbcolor );
            }
            else
            {
                mCommV2.ReqSetupPenColor( rgbcolor );
            }
        }

        public void ReqSetupEnableOfflineData( bool enable )
        {
            if ( IsV1Comm )
            {
            }
            else
            {
                mCommV2.ReqSetupOfflineData( enable );
            }
        }

        public void ReqSetupPenCapPower( bool enable )
        {
            if ( IsV1Comm )
            {
            }
            else
            {
                mCommV2.ReqSetupPenCapPower( enable );
            }
        }

        /// <summary>
        /// Request the status of pen.
        /// If you requested, you can receive result by PenSignal.onReceivedPenStatus method.
        /// </summary>
        public void ReqPenStatus()
        {
            if ( IsV1Comm )
            {
                mCommV1.ReqPenStatus();
            }
            else
            {
                mCommV2.ReqPenStatus();
            }
        }

        /// <summary>
        /// When pen requested password, you can response password by this method. 
        /// </summary>
        /// <param name="password">password</param>
        public void InputPassword( string password )
        {
            if ( IsV1Comm )
            {
                mCommV1.ReqInputPassword( password );
            }
            else
            {
                mCommV2.ReqInputPassword( password );
            }
        }

        /// <summary>
        /// Change the password of pen.
        /// </summary>
        /// <param name="oldPassword">current password</param>
        /// <param name="newPassword">new password</param>
        public void ReqSetupPassword( string oldPassword, string newPassword )
        {
            if ( IsV1Comm )
            {
                mCommV1.ReqSetUpPassword( oldPassword, newPassword );
            }
            else
            {
                mCommV2.ReqSetUpPassword( oldPassword, newPassword );
            }
        }

        /// <summary>
        /// Notify pen of using note.
        /// </summary>
        /// <param name="sectionId">section id of note</param>
        /// <param name="ownerId">owner id of note</param>
        /// <param name="noteId">note id</param>
        public void AddUsingNote( int sectionId, int ownerId, int noteId )
        {
            if ( IsV1Comm )
            {
                mCommV1.ReqAddUsingNote( sectionId, ownerId, noteId );
            }
            else
            {
                mCommV2.ReqAddUsingNote( sectionId, ownerId, new int[] { noteId } );
            }
        }

        /// <summary>
        /// Notify pen of using note.
        /// </summary>
        /// <param name="sectionId">section id of note</param>
        /// <param name="ownerId">owner id of note</param>
        /// <param name="noteIds">array of note id</param>
        public void AddUsingNote( int sectionId, int ownerId, int[] noteIds )
        {
            if ( IsV1Comm )
            {
                mCommV1.ReqAddUsingNote( sectionId, ownerId, noteIds );
            }
            else
            {
                mCommV2.ReqAddUsingNote( sectionId, ownerId, noteIds );
            }
        }

        /// <summary>
        /// Notify pen of using note.
        /// </summary>
        /// <param name="sectionId">section id of note</param>
        /// <param name="ownerId">owner id of note</param>
        public void AddUsingNote( int sectionId, int ownerId )
        {
            if ( IsV1Comm )
            {
                mCommV1.ReqAddUsingNote( sectionId, ownerId );
            }
            else
            {
                mCommV2.ReqAddUsingNote( sectionId, ownerId );
            }
        }

        /// <summary>
        /// Notify pen of using note.
        /// </summary>
        /// <param name="sectionId">section id list of note</param>
        /// <param name="ownerId">owner id list of note</param>
        public void AddUsingNote( int[] sectionId, int[] ownerId )
        {
            if ( IsV1Comm )
            {
                mCommV1.ReqAddUsingNote( sectionId, ownerId );
            }
            else
            {
                mCommV2.ReqAddUsingNote( sectionId, ownerId );
            }
        }

        /// <summary>
        /// Notify pen that use every note.
        /// </summary>
        public void AddUsingNoteAll()
        {
            if ( IsV1Comm )
            {
                mCommV1.ReqAddUsingNote();
            }
            else
            {
                mCommV2.ReqAddUsingNote();
            }

        }

        /// <summary>
        /// Request offline data list of pen.
        /// If you requested, you can receive result by PenSignal.onOfflineDataList method.
        /// </summary>
        public void ReqOfflineDataList()
        {
            if ( IsV1Comm )
            {
                mCommV1.ReqOfflineDataList();
            }
            else
            {
                mCommV2.ReqOfflineDataList();
            }
        }

        /// <summary>
        /// Request offline data of pen.
        /// When you request offline data, your request is inserted to queue.
        /// </summary>
        /// <param name="notes">array of OfflineNote object</param>
        public void ReqOfflineData( OfflineDataInfo note )
        {
            System.Console.WriteLine( "ReqOfflineData ( result : {0} )", note.ToString() );

            if ( note == null )
            {
                return;
            }

            if ( IsV1Comm )
            {
                mCommV1.ReqOfflineData( note );
            }
            else
            {
                mCommV2.ReqOfflineData( note.Section, note.Owner, note.Note, false, note.Pages );
            }
        }

        /// <summary>
        /// Request update firmware of pen.
        /// </summary>
        /// <param name="filepath">absolute path of firmware file</param>
        public bool ReqFirmwareUpdate( string filepath, string version )
        {
            if ( IsV1Comm )
            {
                return mCommV1.ReqPenSwUpgrade( filepath );
            }
            else
            {
                return mCommV2.ReqPenSwUpgrade( filepath, version );
            }
        }

        /// <summary>
        /// To suspend updating task.
        /// </summary>
        public bool SuspendFirmwareUpdate()
        {
            if ( IsV1Comm )
            {
                return mCommV1.SuspendSwUpgrade();
            }
            else
            {
                return mCommV2.SuspendSwUpgrade();
            }
        }

        /// <summary>
        /// Request to delete offline data in pen.
        /// </summary>
        /// <param name="sectionId">section id of note</param>
        /// <param name="ownerId">owner id of note</param>
        public bool RemoveOfflineData( int sectionId, int ownerId, int noteId )
        {
            if ( IsV1Comm )
            {
                return mCommV1.ReqRemoveOfflineData( sectionId, ownerId );
            }
            else
            {
                return mCommV2.ReqRemoveOfflineData( sectionId, ownerId, new int[] { noteId } );
            }
        }

        /// <summary>
        /// If you want to stop working PenCommAgent, you can call this function.
        /// </summary>
        public void Dispose()
        {
            if ( isConnected() )
            {
                mCommV2.Clean();
                mCommV1.Clean();
            }

            IsInitilized = false;
        }
    }
}
