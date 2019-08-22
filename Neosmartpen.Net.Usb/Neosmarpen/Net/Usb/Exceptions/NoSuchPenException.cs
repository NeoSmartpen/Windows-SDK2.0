using System;

namespace Neosmartpen.Net.Usb.Exceptions
{
    /// <summary>
    /// Exception thrown when pen doesn't exist
    /// </summary>
    public class NoSuchPenException : Exception
    {
        internal NoSuchPenException()
        {
        }
    }
}
