using Windows.Foundation;

namespace Neosmartpen.Net
{
    internal interface IPenControllerEvent
	{
		event TypedEventHandler<IPenClient, ConnectedEventArgs> Connected;
		event TypedEventHandler<IPenClient, object> Disconnected;
		event TypedEventHandler<IPenClient, SimpleResultEventArgs> OfflineDownloadFinished;
		event TypedEventHandler<IPenClient, object> Authenticated;
        event TypedEventHandler<IPenClient, object> AvailableNoteAdded;
        event TypedEventHandler<IPenClient, SimpleResultEventArgs> AutoPowerOnChanged;
		event TypedEventHandler<IPenClient, SimpleResultEventArgs> AutoPowerOffTimeChanged;
		event TypedEventHandler<IPenClient, SimpleResultEventArgs> BeepSoundChanged;
		event TypedEventHandler<IPenClient, SimpleResultEventArgs> PenCapPowerOnOffChanged;
		event TypedEventHandler<IPenClient, SimpleResultEventArgs> PenColorChanged;
		event TypedEventHandler<IPenClient, SimpleResultEventArgs> HoverChanged;
		event TypedEventHandler<IPenClient, SimpleResultEventArgs> OfflineDataChanged;
		event TypedEventHandler<IPenClient, PasswordRequestedEventArgs> PasswordRequested;
		event TypedEventHandler<IPenClient, SimpleResultEventArgs> PasswordChanged;
		event TypedEventHandler<IPenClient, SimpleResultEventArgs> SensitivityChanged;
		event TypedEventHandler<IPenClient, SimpleResultEventArgs> RtcTimeChanged;
		event TypedEventHandler<IPenClient, BatteryAlarmReceivedEventArgs> BatteryAlarmReceived;
		event TypedEventHandler<IPenClient, DotReceivedEventArgs> DotReceived;
        event TypedEventHandler<IPenClient, object> FirmwareInstallationStarted;
        event TypedEventHandler<IPenClient, SimpleResultEventArgs> FirmwareInstallationFinished;
		event TypedEventHandler<IPenClient, ProgressChangeEventArgs> FirmwareInstallationStatusUpdated;
		event TypedEventHandler<IPenClient, OfflineDataListReceivedEventArgs> OfflineDataListReceived;
		event TypedEventHandler<IPenClient, OfflineStrokeReceivedEventArgs> OfflineStrokeReceived;
		event TypedEventHandler<IPenClient, PenStatusReceivedEventArgs> PenStatusReceived;
		event TypedEventHandler<IPenClient, SimpleResultEventArgs> OfflineDataRemoved;
		event TypedEventHandler<IPenClient, object> OfflineDataDownloadStarted;
		event TypedEventHandler<IPenClient, PenProfileReceivedEventArgs> PenProfileReceived;
	}
}
