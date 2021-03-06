﻿using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace QuickFix.Fields.Converters
{
    /// <summary>
    /// Convert DateTime to/from String
    /// </summary>
    public static class DateTimeConverter
    {
        public const string DATE_TIME_FORMAT_WITH_MICROSECONDS = "{0:yyyyMMdd-HH:mm:ss.ffffff}";
        public const string DATE_TIME_FORMAT_WITH_MILLISECONDS = "{0:yyyyMMdd-HH:mm:ss.fff}";
        public const string DATE_TIME_FORMAT_WITHOUT_MILLISECONDS = "{0:yyyyMMdd-HH:mm:ss}";
        public const string DATE_ONLY_FORMAT = "{0:yyyyMMdd}";
        public const string TIME_ONLY_FORMAT_WITH_MICROSECONDS = "{0:HH:mm:ss.ffffff}";
        public const string TIME_ONLY_FORMAT_WITH_MILLISECONDS = "{0:HH:mm:ss.fff}";
        public const string TIME_ONLY_FORMAT_WITHOUT_MILLISECONDS = "{0:HH:mm:ss}";
        public static string[] DATE_TIME_FORMATS = { "yyyyMMdd-HH:mm:ss.ffffff", "yyyyMMdd-HH:mm:ss.fff", "yyyyMMdd-HH:mm:ss" };
        public static string[] DATE_ONLY_FORMATS = { "yyyyMMdd" };
        public static string[] TIME_ONLY_FORMATS = { "HH:mm:ss.ffffff", "HH:mm:ss.fff", "HH:mm:ss" };
        public static DateTimeStyles DATE_TIME_STYLES = DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal;
        public static CultureInfo DATE_TIME_CULTURE_INFO = CultureInfo.InvariantCulture;
        private static IDictionary<TimeStampPrecision, string> DATE_TIME_PRECISION_TO_FORMAT = new Dictionary<TimeStampPrecision, string>
        {
            {TimeStampPrecision.Second, DATE_TIME_FORMAT_WITHOUT_MILLISECONDS},
            {TimeStampPrecision.Millisecond, DATE_TIME_FORMAT_WITH_MILLISECONDS},
            {TimeStampPrecision.Microsecond, DATE_TIME_FORMAT_WITH_MICROSECONDS},
        };

        private static IDictionary<TimeStampPrecision, string> TIME_ONLY_PRECISION_TO_FORMAT = new Dictionary<TimeStampPrecision, string>
        {
            {TimeStampPrecision.Second, TIME_ONLY_FORMAT_WITHOUT_MILLISECONDS},
            {TimeStampPrecision.Millisecond, TIME_ONLY_FORMAT_WITH_MILLISECONDS},
            {TimeStampPrecision.Microsecond, TIME_ONLY_FORMAT_WITH_MICROSECONDS},
        };


        /// <summary>
        /// Convert string to DateTime
        /// </summary>
        /// <exception cref="FieldConvertError"/>
        public static System.DateTime ConvertToDateTime(string str)
        {
            try
            {
                return System.DateTime.ParseExact(str, DATE_TIME_FORMATS, DATE_TIME_CULTURE_INFO, DATE_TIME_STYLES);
            }
            catch (System.Exception e)
            {
                throw new FieldConvertError("Could not convert string (" + str + ") to DateTime: " + e.Message, e);
            }
        }

        /// <summary>
        /// Check if string is DateOnly and, if yes, convert to DateTime
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <exception cref="FieldConvertError"/>
        public static System.DateTime ConvertToDateOnly(string str)
        {
            try
            {
                return System.DateTime.ParseExact(str, DATE_ONLY_FORMATS, DATE_TIME_CULTURE_INFO, DATE_TIME_STYLES);
            }
            catch (System.Exception e)
            {
                throw new FieldConvertError("Could not convert string (" + str + ") to DateOnly: " + e.Message, e);
            }
        }

        /// <summary>
        /// Check if string is TimeOnly and, if yes, convert to DateTime
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <exception cref="FieldConvertError"/>
        public static System.DateTime ConvertToTimeOnly(string str)
        {
            try
            {
                System.DateTime d = System.DateTime.ParseExact(str, TIME_ONLY_FORMATS, DATE_TIME_CULTURE_INFO, DATE_TIME_STYLES);
                return new System.DateTime(1980, 1, 1) + d.TimeOfDay;
            }
            catch (System.Exception e)
            {
                throw new FieldConvertError("Could not convert string (" + str + ") to TimeOnly: " + e.Message, e);
            }
        }

        /// <summary>
        /// Check if string is TimeOnly and, if yes, convert to TimeSpan
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        /// <exception cref="FieldConvertError"/>
        public static System.TimeSpan ConvertToTimeSpan(string str)
        {
            try
            {
                System.DateTime d = ConvertToTimeOnly(str);
                return d.TimeOfDay;
            }
            catch (System.Exception e)
            {
                throw new FieldConvertError("Could not convert string (" + str + ") to TimeSpan: " + e.Message, e);
            }
        }

        /// <summary>
        /// Convert DateTime to string in FIX Format
        /// </summary>
        /// <param name="dt">the DateTime to convert</param>
        /// <param name="includeMilliseconds">if true, include milliseconds in the result</param>
        /// <returns>FIX-formatted DataTime</returns>
        public static string Convert( System.DateTime dt, bool includeMilliseconds )
        {
            return includeMilliseconds ? Convert(dt, TimeStampPrecision.Millisecond): Convert( dt, TimeStampPrecision.Second );
        }


        /// <summary>
        /// Converts the specified dt.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="precision">The precision.</param>
        /// <returns></returns>
        public static string Convert(System.DateTime dt, TimeStampPrecision precision )
        {
            var format = DATE_TIME_PRECISION_TO_FORMAT[precision];
            return string.Format(format, dt);
        }

        /// <summary>
        /// Convert DateTime to string in FIX Format, with milliseconds
        /// </summary>
        /// <param name="dt">the DateTime to convert</param>
        /// <returns>FIX-formatted DateTime</returns>
        public static string Convert(System.DateTime dt)
        {
            return DateTimeConverter.Convert(dt, true);
        }

        public static string ConvertDateOnly(System.DateTime dt)
        {
            return string.Format(DATE_ONLY_FORMAT, dt);
        }

        public static string ConvertTimeOnly(System.DateTime dt)
        {
            return DateTimeConverter.ConvertTimeOnly(dt, true);
        }

        public static string ConvertTimeOnly( System.DateTime dt, bool includeMilliseconds )
        {
            return includeMilliseconds ? ConvertTimeOnly( dt, TimeStampPrecision.Millisecond ) : ConvertTimeOnly( dt, TimeStampPrecision.Second );
        }

        public static string ConvertTimeOnly(System.DateTime dt, TimeStampPrecision precision)
        {
            var format = TIME_ONLY_PRECISION_TO_FORMAT[precision];
            return string.Format(format, dt);
        }
    }
}
