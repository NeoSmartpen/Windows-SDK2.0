using System;

namespace Neosmartpen.Net.Usb.Exceptions
{
    /// <summary>
    /// Exception raised when the pen does not support the function
    /// </summary>
    public class NotSupportedVersionException : Exception
    {
        internal NotSupportedVersionException()
        {
        }
    }
}
