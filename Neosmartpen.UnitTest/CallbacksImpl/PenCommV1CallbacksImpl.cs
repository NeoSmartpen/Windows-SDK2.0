using Neosmartpen.Net;
using Neosmartpen.Net.Metadata.Model;
using Neosmartpen.Net.Protocol.v1;
using Neosmartpen.Net.Protocol.v2;
using System;
using System.Collections.Generic;

namespace Neosmartpen.UnitTest
{
    public class PenCommV1CallbacksImpl : PenCommV1Callbacks
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
        public CallbackDele PenBeepChanged;
        public CallbackDele PenColorChanged;
        public CallbackDele PenHoverChanged;
        public CallbackDele PenSensitivityChanged;
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
            PenBeepChanged = null;
            PenColorChanged = null;
            PenHoverChanged = null;
            PenSensitivityChanged = null;
            OfflineDataListReceived = null;
            OfflineDataDownloadingStarted = null;
            OfflineDataDownloadingFinished = null;
            OfflineStrokesReceived = null;
            FirmwareUpdateProgressReceived = null;
            FirmwareUpdateResultReceived = null;
        }

        void PenCommV1Callbacks.onReceiveDot(IPenComm sender, Dot dot)
        {
        }

        void PenCommV1Callbacks.onConnected(IPenComm sender, int maxForce, string firmwareVersion)
        {
            Connected?.Invoke(sender, maxForce, firmwareVersion);
        }

        void PenCommV1Callbacks.onPenAuthenticated(IPenComm sender)
        {
            Authenticated?.Invoke(sender);
        }

        void PenCommV1Callbacks.onDisconnected(IPenComm sender)
        {
            Disconnected?.Invoke(sender);
        }

        void PenCommV1Callbacks.onAvailableNoteAccepted(IPenComm sender, bool result)
        {
            AvailableNoteAccepted?.Invoke(sender, result);
        }

        void PenCommV1Callbacks.onUpDown(IPenComm sender, bool isUp)
        {
        }

        void PenCommV1Callbacks.onOfflineDataList(IPenComm sender, OfflineDataInfo[] offlineNotes)
        {
            OfflineDataListReceived?.Invoke(sender, offlineNotes);
        }

        void PenCommV1Callbacks.onStartOfflineDownload(IPenComm sender)
        {
            OfflineDataDownloadingStarted?.Invoke(sender);
        }

        void PenCommV1Callbacks.onUpdateOfflineDownload(IPenComm sender, int total, int amountDone)
        {
            OfflineStrokesReceived?.Invoke(sender, total, amountDone);
        }

        void PenCommV1Callbacks.onFinishedOfflineDownload(IPenComm sender, bool result)
        {
            OfflineDataDownloadingFinished?.Invoke(sender, result);
        }

        void PenCommV1Callbacks.onReceiveOfflineStrokes(IPenComm sender, Stroke[] strokes)
        {
            OfflineStrokesReceived?.Invoke(sender, strokes);
        }

        void PenCommV1Callbacks.onReceivedPenStatus(IPenComm sender, int timeoffset, long timetick, int maxForce, int battery, int usedmem, int pencolor, bool autopowerMode, bool accelerationMode, bool hoverMode, bool beep, short autoshutdownTime, short penSensitivity, string modelName)
        {
            PenStatusReceived?.Invoke(sender);
        }

        void PenCommV1Callbacks.onPenPasswordRequest(IPenComm sender, int retryCount, int resetCount)
        {
            PasswordRequest?.Invoke(sender, retryCount, resetCount);
        }

        void PenCommV1Callbacks.onPenPasswordSetUpResponse(IPenComm sender, bool result)
        {
            PasswordChanged?.Invoke(sender, result);
        }

        void PenCommV1Callbacks.onPenSensitivitySetUpResponse(IPenComm sender, bool result)
        {
            PenSensitivityChanged?.Invoke(sender, result);
        }

        void PenCommV1Callbacks.onPenAutoShutdownTimeSetUpResponse(IPenComm sender, bool result)
        {
            AutoShutdownTimeChanged?.Invoke(sender, result);
        }

        void PenCommV1Callbacks.onPenBeepSetUpResponse(IPenComm sender, bool result)
        {
            PenBeepChanged?.Invoke(sender, result);
        }

        void PenCommV1Callbacks.onPenAutoPowerOnSetUpResponse(IPenComm sender, bool result)
        {
            AutoPowerOnChanged?.Invoke(sender, result);
        }

        void PenCommV1Callbacks.onPenHoverSetUpResponse(IPenComm sender, bool result)
        {
            PenHoverChanged?.Invoke(sender, result);
        }

        void PenCommV1Callbacks.onPenColorSetUpResponse(IPenComm sender, bool result)
        {
            PenColorChanged?.Invoke(sender, result);
        }

        void PenCommV1Callbacks.onReceivedFirmwareUpdateStatus(IPenComm sender, int total, int amountDone)
        {
            FirmwareUpdateProgressReceived?.Invoke(sender, total, amountDone);
        }

        void PenCommV1Callbacks.onReceivedFirmwareUpdateResult(IPenComm sender, bool result)
        {
            FirmwareUpdateResultReceived?.Invoke(sender, result);
        }

        void PenCommV1Callbacks.onErrorDetected(IPenComm sender, ErrorType errorType, long timestamp, Dot dot, string extraData)
        {
        }

        void PenCommV1Callbacks.onSymbolDetected(IPenComm sender, List<Symbol> symbols)
        {
        }
    }
}
