using System;
namespace VietCommerce.Core.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime ToVietnamTime(this DateTime utcDateTime)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, tz);
        }
        public static DateTime ToUtc(this DateTime vnDateTime)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            return TimeZoneInfo.ConvertTimeToUtc(vnDateTime, tz);
        }
        public static string ToVietnameseFormat(this DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy HH:mm:ss");
        }
        public static string ToDateOnly(this DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy");
        }
        public static bool IsToday(this DateTime dateTime) => dateTime.Date == DateTime.Now.Date;
        public static bool IsInRange(this DateTime dateTime, DateTime start, DateTime end)
        {
            return dateTime >= start && dateTime <= end;
        }
        public static DateTime StartOfDay(this DateTime dateTime) => dateTime.Date;
        public static DateTime EndOfDay(this DateTime dateTime) => dateTime.Date.AddDays(1).AddTicks(-1);
        public static DateTime StartOfMonth(this DateTime dateTime) => new DateTime(dateTime.Year, dateTime.Month, 1);
        public static DateTime EndOfMonth(this DateTime dateTime) => dateTime.StartOfMonth().AddMonths(1).AddTicks(-1);
    }
}
