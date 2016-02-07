using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ttaenc
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts to a string in the Internet Date/Time Format as defined in RFC3339
        /// </summary>
        /// https://www.ietf.org/rfc/rfc3339.txt, 5.6. Internet Date/Time Format
        public static string ToInternetDateTimeFormat(this DateTime dateTime)
        {
            return System.Xml.XmlConvert.ToString(dateTime, System.Xml.XmlDateTimeSerializationMode.Local);
        }

        public static DateTime FromInternetDateTimeFormat(string dateTimeString)
        {
            return System.Xml.XmlConvert.ToDateTime(dateTimeString, System.Xml.XmlDateTimeSerializationMode.Local);
        }
    }
}
