
namespace Neosmartpen.Net.Protocol.v1
{
    /// <summary>
    /// The PenCommV1Callbacks interface models a callback used when the the N2 smart pen needs to notify the client side.
    /// Please note that the implementation of the PenSignal run on main read thread. so if you want to do heavy work in callback method block, you have to run your code asyncronously.
    /// </summary>
    public interface PenCommV1Callbacks
    {
        /// <summary>
        /// Fired when receive a dot successfully, override to handle in your own code.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="dot">model object of dot</param>
        void onReceiveDot( IPenComm sender, Dot dot );
        
        /// <summary>
        /// Fired when a connection is made, override to handle in your own code.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="maxForce">maximum level of force</param>
        /// <param name="firmwareVersion">current version of pen's firmware</param>
        void onConnected( IPenComm sender, int maxForce, string firmwareVersion );

        /// <summary>
        /// Fired when your connection is authenticated.
        /// When it fired, you can use all function of pen.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        void onPenAuthenticated( IPenComm sender );

        /// <summary>
        /// Fired when a connection is destroyed, override to handle in your own code.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        void onDisconnected( IPenComm sender );

        /// <summary>
        /// Fired when receive up or down signal from your force sensor of pen, override to handle in your own code.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="isUp">false if N2 detected pressure, otherwise false.</param>
        void onUpDown( IPenComm sender, bool isUp );

        /// <summary>
        /// Fired when receive offline data list in N2 smartpen.
        /// When you received this signal, you can request offline data to PenCommV1 by ReqOfflineData method.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="offlineNotes">array of OfflineNote object</param>
        void onOfflineDataList( IPenComm sender, OfflineDataInfo[] offlineNotes );

        /// <summary>
        /// Fired when started downloading, override to handle in your own code.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        void onStartOfflineDownload( IPenComm sender );

        /// <summary>
        /// This method is invoked by the PenCommV1 when it needs to notify the client side about the status of an download operation being performed.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="total">amount of total work</param>
        /// <param name="amountDone">amount of work done</param>
        void onUpdateOfflineDownload( IPenComm sender, int total, int amountDone );

        /// <summary>
        /// Fired when finished downloading, override to handle in your own code.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="result">result of downloading</param>
        void onFinishedOfflineDownload( IPenComm sender, bool result );

        /// <summary>
        /// Fired when received one in all offline data.
        /// Array of stroke is consist of single note.
        /// This method can be invoked several time as number of offline note.  
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="strokes">array of stroke object</param>
        void onReceiveOfflineStrokes( IPenComm sender, Stroke[] strokes );

        /// <summary>
        /// Fired when received status of pen, override to handle in your own code.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="timeoffset">timestamp offset, you should ignore it</param>
        /// <param name="timetick">timestamp, pen knew</param>
        /// <param name="maxForce">maximum level of pressure sensor</param>
        /// <param name="battery">battery status of pen</param>
        /// <param name="usedmem">memory status of pen</param>
        /// <param name="pencolor">color status of pen</param>
        /// <param name="autopowerMode">the status of the auto power on property that if write the unpowered pen, power on.</param>
        /// <param name="accelerationMode">the status of the acceleration sensor property</param>
        /// <param name="hoverMode">the status of the hover mode property</param>
        /// <param name="beep">the status of the beep property</param>
        /// <param name="autoshutdownTime">the status of the auto shutdown time property</param>
        /// <param name="penSensitivity">the status of pen's sensitivity property</param>
        void onReceivedPenStatus( IPenComm sender, int timeoffset, long timetick, int maxForce, int battery, int usedmem, int pencolor, bool autopowerMode, bool accelerationMode, bool hoverMode, bool beep, short autoshutdownTime, short penSensitivity, string modelName );

        /// <summary>
        /// Fired when pen request a password to client side.
        /// When you received this signal, you have to enter password by PenCommV1.InputPassword method.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="retryCount">count of enter password</param>
        /// <param name="resetCount">if retry count reached reset count, delete all data in pen</param>
        void onPenPasswordRequest( IPenComm sender, int retryCount, int resetCount );

        /// <summary>
        /// Fired when pen response to your request that change password by PenCommV1.InputPassword method.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="result">true if your request is successfully applied, otherwise false</param>
        void onPenPasswordSetUpResponse( IPenComm sender, bool result );

        /// <summary>
        /// Fired when pen response to your request that sets the value of the sensitivity property by PenCommV1.ReqSetupPenSensitivity method.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="result">true if your request is successfully applied, otherwise false</param>
        void onPenSensitivitySetUpResponse( IPenComm sender, bool result );

        /// <summary>
        /// Fired when pen response to your request that sets the value of the auto shutdown time by PenCommV1.ReqSetupPenAutoShutdownTime method.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="result">true if your request is successfully applied, otherwise false</param>
        void onPenAutoShutdownTimeSetUpResponse( IPenComm sender, bool result );

        /// <summary>
        /// Fired when pen response to your request that Sets the status of the beep property by PenCommV1.ReqSetupPenBeep method.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="result">true if your request is successfully applied, otherwise false</param>
        void onPenBeepSetUpResponse( IPenComm sender, bool result );

        /// <summary>
        /// Fired when pen response to your request that Sets the status of the auto power on property by PenCommV1.ReqSetupPenBeep method.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="result">true if your request is successfully applied, otherwise false</param>
        void onPenAutoPowerOnSetUpResponse( IPenComm sender, bool result );

        /// <summary>
        /// Fired when pen response to your request that sets the status of the hover mode on property by PenCommV1.ReqSetupHoverMode method.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="result">true if your request is successfully applied, otherwise false</param>
        void onPenHoverSetUpResponse( IPenComm sender, bool result );

        /// <summary>
        /// Fired when pen response to your request that Sets the color of pen ink by PenCommV1.ReqSetupPenColor method.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="result">true if your request is successfully applied, otherwise false</param>
        void onPenColorSetUpResponse( IPenComm sender, bool result );

        /// <summary>
        /// This method is invoked by the PenCommV1 when it needs to notify the client side about the status of an firmware update operation being performed.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="total">amount of total work</param>
        /// <param name="amountDone">amount of work done</param>
        void onReceivedFirmwareUpdateStatus( IPenComm sender, int total, int amountDone );

        /// <summary>
        /// Fired when finished updating firmware, override to handle in your own code.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="result">true if updating is successfully finished, otherwise false</param>
        void onReceivedFirmwareUpdateResult( IPenComm sender, bool result );
    }
}
