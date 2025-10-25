using System;
namespace VietCommerce.Core.Common.Utils;
public static class DateTimeHelper
{
    public static DateTime ToUtc(DateTime dateTime) => dateTime.Kind == DateTimeKind.Utc ? dateTime : DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
    public static DateTime ToLocal(DateTime utcDateTime, TimeZoneInfo timeZone) => TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZone);
}
