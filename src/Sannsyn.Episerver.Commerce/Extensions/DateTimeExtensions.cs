using System;

namespace Sannsyn.Episerver.Commerce.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts a java timestamp(number of milliseconds since January 1st, 1970) to DateTime
        /// </summary>
        /// <param name="javaTimeStamp">double to convert to DateTime</param>
        /// <returns>Converted Datetime</returns>
        public static DateTime JavaTimeStampToDateTime(double javaTimeStamp)
        {

            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        /// <summary>
        /// Convert a DateTime to Java timestamp(number of milliseconds since January 1st, 1970)
        /// </summary>
        /// <param name="dateTime">DateTime to convert to Java timestamp(double)</param>
        /// <returns>A double(milliseconds since January 1st, 1970)</returns>
        public static double ToJavaTimeStamp(this DateTime dateTime)
        {
            TimeSpan span = TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                            new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return span.TotalMilliseconds;
        }
    }
}
