using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlusCP.Extensions
{
    public static class StringExtensions
    {
        public static DateTime? ToErpDateTime(this string s)
        {
            if (DateTime.TryParse(s, out DateTime dt))
                return dt;
            return null;
        }

        public static string ToErpTime(this int time)
        {
            return TimeSpan.FromSeconds(time).ToString(@"hh\:mm");
        }
    }
}