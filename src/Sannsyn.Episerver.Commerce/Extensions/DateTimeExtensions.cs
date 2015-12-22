using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sannsyn.Episerver.Commerce.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime JavaTimeStampToDateTime(double unixTimeStamp)
        {

            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static double ToJavaTimeStamp(this DateTime dateTime)
        {
            TimeSpan span = TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                            new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return span.TotalMilliseconds;
        }
    }
}
