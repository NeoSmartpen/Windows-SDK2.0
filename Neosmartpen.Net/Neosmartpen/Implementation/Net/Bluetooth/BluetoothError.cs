using System;

namespace Neosmartpen.Net
{
    public enum BluetoothError
    {
        //
        // 요약:
        //     The operation was successfully completed or serviced.
        Success,
        //
        // 요약:
        //     The Bluetooth radio was not available. This error occurs when the Bluetooth radio
        //     has been turned off.
        RadioNotAvailable,
        //
        // 요약:
        //     The operation cannot be serviced because the necessary resources are currently
        //     in use.
        ResourceInUse,
        //
        // 요약:
        //     The operation cannot be completed because the remote device is not connected.
        DeviceNotConnected,
        //
        // 요약:
        //     An unexpected error has occurred.
        OtherError,
        //
        // 요약:
        //     The operation is disabled by policy.
        DisabledByPolicy,
        //
        // 요약:
        //     The operation is not supported on the current Bluetooth radio hardware.
        NotSupported,
        //
        // 요약:
        //     The operation is disabled by the user.
        DisabledByUser,
        //
        // 요약:
        //     The operation requires consent.
        ConsentRequired,
        //
        // 요약:
        //     The transport is not supported.
        TransportNotSupported

    }
}
