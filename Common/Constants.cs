using System;

namespace Gongchengshi
{
    public static class Constants
    {
        public const UInt64 MilliSecPerDay = HourPerDay * MinPerHour * SecPerMin * MilliSecPerSec;
        public const UInt64 MilliSecPerSec = 1000;
        public const UInt64 USecPerSec = MilliSecPerSec * 1000;
        public const UInt64 SecPerMin = 60;
        public const int MinPerHour = 60;
        public const int HourPerDay = 24;
        public const UInt64 TicksPerUSec = TimeSpan.TicksPerSecond / USecPerSec;
        public const uint BytesPerMegaByte = 1024 * 1024;
        public const UInt64 BytesPerGigaByte = BytesPerMegaByte * 1024;

        // This magic number gets me a rough average number of ticks per year.
        // This is not quite exact, but does have the property that the error is always positive, 
        // so the code won't print the same year twice.
        public const string AverageYearString = "365.5:49:50";
        public static readonly TimeSpan AverageYearLength = TimeSpan.Parse(AverageYearString);
        public static readonly long TicksPerYear = AverageYearLength.Ticks;

        public const string AverageMonthString = "30.10:29:09.166666"; // Average Year / 12
        public static readonly TimeSpan AverageMonth = TimeSpan.Parse(AverageMonthString);
    }
}
