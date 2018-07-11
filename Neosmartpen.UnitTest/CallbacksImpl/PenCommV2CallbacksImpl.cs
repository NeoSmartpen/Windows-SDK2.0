using Neosmartpen.Net;
using Neosmartpen.Net.Protocol.v2;
using System;

namespace Neosmartpen.UnitTest
{
    public class PenCommV2CallbacksImpl : PenCommV2Callbacks
    {
        public delegate void CallbackDele(object sender, params object[] args);

        public CallbackDele Connected;
        public CallbackDele Authenticated;
        public CallbackDele Disconnected;
        public CallbackDele AvailableNoteAccepted;
        public CallbackDele PasswordRequest;
        public CallbackDele PasswordChanged;
        public CallbackDele PenStatusReceived;
        public CallbackDele AutoPowerOnChanged;
        public CallbackDele AutoShutdownTimeChanged;
        public CallbackDele PenBeepAndLightChanged;
        public CallbackDele PenBeepChanged;
        public CallbackDele PenBtLocalNameChanged;
        public CallbackDele PenCapPowerOnOffChanged;
        public CallbackDele PenColorChanged;
        public CallbackDele PenDataTransmissionTypeChanged;
        public CallbackDele PenDownSamplingChanged;
        public CallbackDele PenFscSensitivityChanged;
        public CallbackDele PenHoverChanged;
        public CallbackDele PenOfflineDataChanged;
        public CallbackDele PenSensitivityChanged;
        public CallbackDele PenTimestampChanged;
        public CallbackDele PenUsbModeChanged;
        public CallbackDele OfflineDataRemoved;
        public CallbackDele OfflineDataListReceived;
        public CallbackDele OfflineDataDownloadingStarted;
        public CallbackDele OfflineDataDownloadingFinished;
        public CallbackDele OfflineStrokesReceived;
        public CallbackDele FirmwareUpdateProgressReceived;
        public CallbackDele FirmwareUpdateResultReceived;

        public void Dispose()
        {
            Connected = null;
            Authenticated = null;
            Disconnected = null;
            AvailableNoteAccepted = null;
            PasswordRequest = null;
            PasswordChanged = null;
            PenStatusReceived = null;
            AutoPowerOnChanged = null;
            AutoShutdownTimeChanged = null;
            PenBeepAndLightChanged = null;
            PenBeepChanged = null;
            PenBtLocalNameChanged = null;
            PenCapPowerOnOffChanged = null;
            PenColorChanged = null;
            PenDataTransmissionTypeChanged = null;
            PenDownSamplingChanged = null;
            PenFscSensitivityChanged = null;
            PenHoverChanged = null;
            PenOfflineDataChanged = null;
            PenSensitivityChanged = null;
            PenTimestampChanged = null;
            PenUsbModeChanged = null;
            OfflineDataRemoved = null;
            OfflineDataListReceived = null;
            OfflineDataDownloadingStarted = null;
            OfflineDataDownloadingFinished = null;
            OfflineStrokesReceived = null;
            FirmwareUpdateProgressReceived = null;
            FirmwareUpdateResultReceived = null;
        }

        void PenCommV2Callbacks.onConnected(IPenComm sender, string macAddress, string deviceName, string fwVersion, string protocolVersion, string subName, int maxForce)
        {
            Connected?.Invoke(sender, macAddress, deviceName, fwVersion, protocolVersion, subName, maxForce);
        }

        void PenCommV2Callbacks.onDisconnected(IPenComm sender)
        {
            Disconnected?.Invoke(sender);
        }

        void PenCommV2Callbacks.onErrorDetected(IPenComm sender, ErrorType errorType, long timestamp, Dot dot, string extraData, ImageProcessErrorInfo imageProcessErrorInfo)
        {
        }

        void PenCommV2Callbacks.onPenAuthenticated(IPenComm sender)
        {
            Authenticated?.Invoke(sender);
        }

        void PenCommV2Callbacks.onAvailableNoteAccepted(IPenComm sender, bool result)
        {
            AvailableNoteAccepted?.Invoke(sender, result);
        }

        void PenCommV2Callbacks.onPenPasswordRequest(IPenComm sender, int retryCount, int resetCount)
        {
            PasswordRequest?.Invoke(sender, retryCount, resetCount);
        }

        void PenCommV2Callbacks.onPenPasswordSetUpResponse(IPenComm sender, bool result)
        {
            PasswordChanged?.Invoke(sender, result);
        }

        void PenCommV2Callbacks.onPenAutoPowerOnSetUpResponse(IPenComm sender, bool result)
        {
            AutoPowerOnChanged?.Invoke(sender, result);
        }

        void PenCommV2Callbacks.onPenAutoShutdownTimeSetUpResponse(IPenComm sender, bool result)
        {
            AutoShutdownTimeChanged?.Invoke(sender, result);
        }

        void PenCommV2Callbacks.onPenBeepAndLightResponse(IPenComm sender, bool result)
        {
            PenBeepAndLightChanged?.Invoke(sender, result);
        }

        void PenCommV2Callbacks.onPenBeepSetUpResponse(IPenComm sender, bool result)
        {
            PenBeepChanged?.Invoke(sender, result);
        }

        void PenCommV2Callbacks.onPenBtLocalNameSetUpResponse(IPenComm sender, bool result)
        {
            PenBtLocalNameChanged?.Invoke(sender, result);
        }

        void PenCommV2Callbacks.onPenCapPowerOnOffSetupResponse(IPenComm sender, bool result)
        {
            PenCapPowerOnOffChanged?.Invoke(sender, result);
        }

        void PenCommV2Callbacks.onPenColorSetUpResponse(IPenComm sender, bool result)
        {
            PenColorChanged?.Invoke(sender, result);
        }

        void PenCommV2Callbacks.onPenDataTransmissionTypeSetUpResponse(IPenComm sender, bool result)
        {
            PenDataTransmissionTypeChanged?.Invoke(sender, result);
        }

        void PenCommV2Callbacks.onPenDownSamplingSetUpResponse(IPenComm sender, bool result)
        {
            PenDownSamplingChanged?.Invoke(sender, result);
        }

        void PenCommV2Callbacks.onPenFscSensitivitySetUpResponse(IPenComm sender, bool result)
        {
            PenFscSensitivityChanged?.Invoke(sender, result);
        }

        void PenCommV2Callbacks.onPenHoverSetUpResponse(IPenComm sender, bool result)
        {
            PenHoverChanged?.Invoke(sender, result);
        }

        void PenCommV2Callbacks.onPenOfflineDataSetUpResponse(IPenComm sender, bool result)
        {
            PenOfflineDataChanged?.Invoke(sender, result);
        }

        void PenCommV2Callbacks.onPenSensitivitySetUpResponse(IPenComm sender, bool result)
        {
            PenSensitivityChanged?.Invoke(sender, result);
        }

        void PenCommV2Callbacks.onPenTimestampSetUpResponse(IPenComm sender, bool result)
        {
            PenTimestampChanged?.Invoke(sender, result);
        }

        void PenCommV2Callbacks.onPenUsbModeSetUpResponse(IPenComm sender, bool result)
        {
            PenUsbModeChanged?.Invoke(sender, result);
        }

        void PenCommV2Callbacks.onReceiveBatteryAlarm(IPenComm sender, int battery)
        {
        }

        void PenCommV2Callbacks.onReceiveDot(IPenComm sender, Dot dot, ImageProcessingInfo info)
        {
        }

        void PenCommV2Callbacks.onReceiveFirmwareUpdateResult(IPenComm sender, bool result)
        {
            FirmwareUpdateResultReceived?.Invoke(sender, result);
        }

        void PenCommV2Callbacks.onReceiveFirmwareUpdateStatus(IPenComm sender, int total, int amountDone)
        {
            FirmwareUpdateProgressReceived?.Invoke(sender, total, amountDone);
        }

        void PenCommV2Callbacks.onReceiveOfflineDataList(IPenComm sender, params OfflineDataInfo[] offlineNotes)
        {
            OfflineDataListReceived?.Invoke(sender, offlineNotes);
        }

        void PenCommV2Callbacks.onReceiveOfflineStrokes(IPenComm sender, int total, int amountDone, Stroke[] strokes)
        {
            OfflineStrokesReceived?.Invoke(sender, total, amountDone);
        }

        void PenCommV2Callbacks.onReceivePenStatus(IPenComm sender, bool locked, int passwdMaxReTryCount, int passwdRetryCount, long timestamp, short autoShutdownTime, int maxForce, int battery, int usedmem, bool useOfflineData, bool autoPowerOn, bool penCapPower, bool hoverMode, bool beep, short penSensitivity, PenCommV2.UsbMode usbmode, bool downsampling, string btLocalName, PenCommV2.DataTransmissionType dataTransmissionType)
        {
            PenStatusReceived?.Invoke(sender);
        }

        void PenCommV2Callbacks.onRemovedOfflineData(IPenComm sender, bool result)
        {
            OfflineDataRemoved?.Invoke(sender, result);
        }

        void PenCommV2Callbacks.onStartOfflineDownload(IPenComm sender)
        {
            OfflineDataDownloadingStarted?.Invoke(sender);
        }

        void PenCommV2Callbacks.onFinishedOfflineDownload(IPenComm sender, bool result)
        {
            OfflineDataDownloadingFinished?.Invoke(sender, result);
        }
    }
}
