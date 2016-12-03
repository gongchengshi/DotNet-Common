using System;

namespace Gongchengshi
{
    public static class DateTimeHelper
    {
        public static DateTime Min(DateTime val1, DateTime val2)
        {
            return (val1 < val2) ? val1 : val2;
        }

        public static DateTime Max(DateTime val1, DateTime val2)
        {
            return (val1 > val2) ? val1 : val2;
        }

        public static TimeSpan Multiply(this TimeSpan span, int multiplier)
        {
            return new TimeSpan(span.Ticks * multiplier);
        }

        public static TimeSpan Divide(this TimeSpan span, int divisor)
        {
            return new TimeSpan(span.Ticks / divisor);
        }
    }
}
