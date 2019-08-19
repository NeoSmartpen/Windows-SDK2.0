using System;

namespace Neosmartpen.Net.Support
{
    public class Time
    {
        /// <summary>
        /// Time gap form 1970.1.1
        /// </summary>
        /// <returns>"long Time gap"</returns>
        public static long GetUtcTimeStamp()
        {
            TimeSpan t = ( DateTime.UtcNow - new DateTime( 1970, 1, 1 ) );
			long ts = (long)t.TotalMilliseconds;
			return ts;
        }

        public static long GetUtcTimeStamp(DateTime dateTime)
        {
            TimeSpan t = (dateTime.ToUniversalTime() - new DateTime(1970, 1, 1));
            long ts = (long)t.TotalMilliseconds;
            return ts;
        }

        /// <summary>
        /// Time offset 
        /// </summary>
        /// <returns></returns>
        public static int GetLocalTimeOffset()
        {
            TimeSpan offset = TimeZoneInfo.Local.GetUtcOffset( DateTime.UtcNow );
            int iofs = (int)offset.TotalSeconds * 1000;
            return iofs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime GetDateTime( long timestamp, int offset = 0 )
        {
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            //DateTime date = start.AddMilliseconds(timestamp).ToLocalTime();
            DateTime date = start.AddMilliseconds( timestamp + offset );
            return date;
        }

        public static DateTime GetLocalDateTimeFromUtcTimestamp( long timestamp )
        {
            DateTime start = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
            DateTime date = start.AddMilliseconds(timestamp).ToLocalTime();
            return date;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static long GetTimeGap()
        {
            // 30day gap
            return 2592000000;
        }
    }
}
