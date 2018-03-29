namespace Neosmartpen.Net.Protocol.v2
{
    /// <summary>
    /// The PenCommV2Callbacks interface models a callback used when the the Neo smartpen needs to notify the client side.
    /// Please note that the implementation of the PenCommV2Callbacks run on main read thread. so if you want to do heavy work in callback method block, you have to run your code asyncronously.
    /// </summary>
    public interface PenCommV2Callbacks
    {
        /// <summary>
        /// Fired when a connection is made, override to handle in your own code.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="macAddress">Gets the device identifier.</param>
        /// <param name="deviceName">Gets a name of a device.</param>
        /// <param name="fwVersion">current version of pen's firmware.</param>
        /// <param name="protocolVersion">Gets a version of a protocol.</param>
        /// <param name="subName">Gets a subname of a device.</param>
        /// <param name="maxForce">Gets the maximum level of force sensor.</param>
        void onConnected( IPenComm sender, string macAddress, string deviceName, string fwVersion, string protocolVersion, string subName, int maxForce );

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
        /// Fired when receive a dot successfully, override to handle in your own code.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="dot">model object of dot</param>
        /// <param name="info">model object of image processing. if data transmission type is request-response, you can get image processing info</param>
        void onReceiveDot( IPenComm sender, Dot dot, ImageProcessingInfo info );

        /// <summary>
        /// Fired when receive offline data list in Neo smartpen.
        /// When you received this signal, you can request offline data to PenCommV2 by ReqOfflineData method.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="offlineNotes">array of OfflineNote object</param>
        void onReceiveOfflineDataList( IPenComm sender, params OfflineDataInfo[] offlineNotes );

        /// <summary>
        /// Fired when started downloading, override to handle in your own code.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        void onStartOfflineDownload( IPenComm sender );

        /// <summary>
        /// This method is invoked by the PenCommV2 when it needs to notify the client side about the status of an download operation being performed.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="total">amount of total work</param>
        /// <param name="amountDone">amount of work done</param>
        /// <param name="strokes">array of stroke object</param>
        void onReceiveOfflineStrokes( IPenComm sender, int total, int amountDone, Stroke[] strokes );

        /// <summary>
        /// Fired when finished downloading, override to handle in your own code.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="result">result of downloading</param>
        void onFinishedOfflineDownload( IPenComm sender, bool result );
        
        /// <summary>
        /// Fired when removed offline data, override to handle in your own code.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="result">true if your request is successfully applied, otherwise false</param>
        void onRemovedOfflineData( IPenComm sender, bool result );

        /// <summary>
        /// Fired when received status of pen, override to handle in your own code.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="locked">true if pen is locked, otherwise false</param>
        /// <param name="passwdMaxReTryCount">maximum password input count</param>
        /// <param name="passwdRetryCount">current password input count</param>
        /// <param name="timestamp">timestamp, pen knew</param>
        /// <param name="autoShutdownTime">the status of the auto shutdown time property</param>
        /// <param name="maxForce">maximum level of pressure sensor</param>
        /// <param name="battery">battery status of pen</param>
        /// <param name="usedmem">memory status of pen</param>
        /// <param name="useOfflineData">true if offline data available, otherwise false</param>
        /// <param name="autoPowerOn">the status of the auto power on property that if write the unpowered pen, power on.</param>
        /// <param name="penCapPower">true if enable to control power by cap, otherwise false</param>
        /// <param name="hoverMode">the status of the hover mode property</param>
        /// <param name="beep">the status of the beep property</param>
        /// <param name="penSensitivity">the status of pen's sensitivity property</param>
        /// <param name="usbmode">the status of the usb mode</param>
        /// <param name="downsampling">true if enable to down sampling, otherwise false</param>
        /// <param name="btLocalName">the local name of device</param>
        /// <param name="dataTransmissionType">the type of data transmission</param>
        void onReceivePenStatus( IPenComm sender, bool locked, int passwdMaxReTryCount, int passwdRetryCount, long timestamp, short autoShutdownTime, int maxForce, int battery, int usedmem, bool useOfflineData, bool autoPowerOn, bool penCapPower, bool hoverMode, bool beep, short penSensitivity, PenCommV2.UsbMode usbmode, bool downsampling, string btLocalName, PenCommV2.DataTransmissionType dataTransmissionType );

        /// <summary>
        /// Fired when pen request a password to client side.
        /// When you received this signal, you have to enter password by PenCommV2.InputPassword method.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="retryCount">count of enter password</param>
        /// <param name="resetCount">if retry count reached reset count, delete all data in pen</param>
        void onPenPasswordRequest( IPenComm sender, int retryCount, int resetCount );

        /// <summary>
        /// Fired when pen response to your request that change password by PenCommV2.InputPassword method.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="result">true if your request is successfully applied, otherwise false</param>
        void onPenPasswordSetUpResponse( IPenComm sender, bool result );

        /// <summary>
        /// Fired when pen response to your request that enable offline data.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="result">true if your request is successfully applied, otherwise false</param>
        void onPenOfflineDataSetUpResponse( IPenComm sender, bool result );

        /// <summary>
        /// Fired when pen response to your request that sets timestamp.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="result">true if your request is successfully applied, otherwise false</param>
        void onPenTimestampSetUpResponse( IPenComm sender, bool result );

        /// <summary>
        /// Fired when pen response to your request that sets the value of the sensitivity property by PenCommV2.ReqSetupPenSensitivity method.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="result">true if your request is successfully applied, otherwise false</param>
        void onPenSensitivitySetUpResponse( IPenComm sender, bool result );

        /// <summary>
        /// Fired when pen response to your request that sets the value of the auto shutdown time by PenCommV2.ReqSetupPenAutoShutdownTime method.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="result">true if your request is successfully applied, otherwise false</param>
        void onPenAutoShutdownTimeSetUpResponse( IPenComm sender, bool result );

        /// <summary>
        /// Fired when pen response to your request that sets the status of the auto power on property by PenCommV2.ReqSetupPenBeep method.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="result">true if your request is successfully applied, otherwise false</param>
        void onPenAutoPowerOnSetUpResponse( IPenComm sender, bool result );

        /// <summary>
        /// Fired when pen response to your request that sets enabling control by cap.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="result">true if your request is successfully applied, otherwise false</param>
        void onPenCapPowerOnOffSetupResponse( IPenComm sender, bool result );

        /// <summary>
        /// Fired when pen response to your request that Sets the status of the beep property by PenCommV2.ReqSetupPenBeep method.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="result">true if your request is successfully applied, otherwise false</param>
        void onPenBeepSetUpResponse( IPenComm sender, bool result );

        /// <summary>
        /// Fired when pen response to your request that sets the status of the hover mode on property by PenCommV2.ReqSetupHoverMode method.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="result">true if your request is successfully applied, otherwise false</param>
        void onPenHoverSetUpResponse( IPenComm sender, bool result );

        /// <summary>
        /// Fired when pen response to your request that Sets the color of pen ink by PenCommV2.ReqSetupPenColor method.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="result">true if your request is successfully applied, otherwise false</param>
        void onPenColorSetUpResponse( IPenComm sender, bool result );

        /// <summary>
        /// Fired when pen response to your request that Sets the status of usb mode by PenCommV2.ReqSetupUsbMode method.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="result">true if your request is successfully applied, otherwise false</param>
        void onPenUsbModeSetUpResponse( IPenComm sender, bool result );

        /// <summary>
        /// Fired when pen response to your request that Sets the state of down sampling by PenCommV2.ReqSetupDownSampling method.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="result">true if your request is successfully applied, otherwise false</param>
        void onPenDownSamplingSetUpResponse(IPenComm sender, bool result);

        /// <summary>
        /// Fired when pen response to your request that Sets the local name of device by PenCommV2.ReqSetupBtLocalName method.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="result">true if your request is successfully applied, otherwise false</param>
        void onPenBtLocalNameSetUpResponse(IPenComm sender, bool result);

        /// <summary>
        /// Fired when pen response to your request that Sets the level of sensitivity(force sensor c-type) by PenCommV2.ReqSetupPenFscSensitivity method.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="result">true if your request is successfully applied, otherwise false</param>
        void onPenFscSensitivitySetUpResponse(IPenComm sender, bool result);

        /// <summary>
        /// Fired when pen response to your request that Sets the type of data transmission by PenCommV2.ReqSetupDataTransmissionType method.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="result">true if your request is successfully applied, otherwise false</param>
        void onPenDataTransmissionTypeSetUpResponse(IPenComm sender, bool result);

        /// <summary>
        /// This method is invoked by the PenCommV2 when it needs to notify the client side about the status of an firmware update operation being performed.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="total">amount of total work</param>
        /// <param name="amountDone">amount of work done</param>
        void onReceiveFirmwareUpdateStatus( IPenComm sender, int total, int amountDone );

        /// <summary>
        /// Fired when finished updating firmware, override to handle in your own code.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="result">true if updating is successfully finished, otherwise false</param>
        void onReceiveFirmwareUpdateResult( IPenComm sender, bool result );

        /// <summary>
        /// Fired when changed status of battery, override to handle in your own code.
        /// </summary>
        /// <param name="sender">sender refers to the object that invoked the callback method</param>
        /// <param name="battery">percentage of battery</param>
        void onReceiveBatteryAlarm( IPenComm sender, int battery );

		/// <summary>
		/// Occur when error received
		/// </summary>
		/// <param name="sender">sender refers to the object that invoked the callback method</param>
		/// <param name="errorType">Error Message Type</param>
		/// <param name="timestamp">Timestamp</param>
		void onErrorDetected(IPenComm sender, ErrorType errorType, long timestamp, Dot dot, string extraData, ImageProcessErrorInfo imageProcessErrorInfo);
	}
}
