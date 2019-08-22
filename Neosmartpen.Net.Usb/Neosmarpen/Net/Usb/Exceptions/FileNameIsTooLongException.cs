using System;

namespace Neosmartpen.Net.Usb.Exceptions
{
    /// <summary>
    /// Exception thrown when the specified file name is too long
    /// </summary>
    public class FileNameIsTooLongException : Exception
    {
        internal FileNameIsTooLongException()
        {
        }
    }
}
