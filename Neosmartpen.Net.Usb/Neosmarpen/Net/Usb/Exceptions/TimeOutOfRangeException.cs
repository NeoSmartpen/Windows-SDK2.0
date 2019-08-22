using System;

namespace Neosmartpen.Net.Usb.Exceptions
{
    /// <summary>
    /// Occurs when the entered time value exceeds the range
    /// </summary>
    public class TimeOutOfRangeException : Exception
    {
        internal TimeOutOfRangeException()
        {
        }
    }
}
