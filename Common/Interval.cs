using System;

namespace Gongchengshi
{
    /// <summary>
    /// An Interval represents an interval of time, like one hour, one day, etc.
    /// This can be used to find things like the beginning of a day, given any DateTime,
    /// or determine if two DateTimes are within the same hour, etc.
    /// </summary>
    public struct Interval
    {
        // Instead of constructing Interval instances in code, use one of the static 
        // ones defined here.  For example:
        //  var startOfDay = Interval.Day.Truncate(dateTime);
        public static Interval Second = new Interval(TimeSpan.FromSeconds(1));
        public static Interval Minute = new Interval(TimeSpan.FromMinutes(1));
        public static Interval TenMinute = new Interval(TimeSpan.FromMinutes(10));
        public static Interval Hour = new Interval(TimeSpan.FromHours(1));
        public static Interval Day = new Interval(TimeSpan.FromDays(1));

        /// <summary>
        /// The TimeSpan of this Interval.
        /// </summary>
        public readonly TimeSpan Span;

        /// <summary>
        /// Returns true if the given DateTime is at the top of the interval (top of the hour, for example).
        /// </summary>
        public bool IsAtTop(DateTime time)
        {
            return Truncate(time) == time;
        }

        /// <summary>
        /// If time is already at the tope of an interview this will return time
        /// </summary>
        public DateTime TopOfCurrentOrNext(DateTime time)
        {
            var mod = time.Ticks % Span.Ticks;
            return mod == 0 ? time : new DateTime((time.Ticks - mod) + Span.Ticks);
        }

        /// <summary>
        /// Truncate (i.e, round down) to the beginning of an interval.
        /// </summary>
        public DateTime Truncate(DateTime dateTime)
        {
            if (Span.Seconds != 0)
            {
                var newSecond = dateTime.Second / Span.Seconds * Span.Seconds;
                return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute,
                                    newSecond, dateTime.Kind);
            }
            else if (Span.Minutes != 0)
            {
                var newMinute = dateTime.Minute / Span.Minutes * Span.Minutes;
                return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, newMinute, 
                    0, dateTime.Kind);
            }
            else if (Span.Hours != 0)
            {
                var newHour = dateTime.Hour / Span.Hours * Span.Hours;
                return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, newHour, 0, 0, dateTime.Kind);
            }
            else
            {
                var newDay = dateTime.Day / Span.Days * Span.Days;
                return new DateTime(dateTime.Year, dateTime.Month, newDay, 0, 0, 0, dateTime.Kind);
            }
        }

        /// <summary>
        /// Returns true if the two dates given are within the same interval.
        /// </summary>
        public bool AreWithinSameInterval(DateTime left, DateTime right)
        {
            return Truncate(left) == Truncate(right);
        }

        /// <summary>
        /// Consider using the static instances for common intervals.
        /// </summary>
        /// <param name="span">Timespan for interval.  Currently only supports having one
        ///   quantity (hour, minute, etc.) non-zero.</param>
        public Interval(TimeSpan span)
        {
            int hasTotal = 0;
            if (span.Seconds != 0) hasTotal++;
            if (span.Minutes != 0) hasTotal++;
            if (span.Hours != 0) hasTotal++;
            if (span.Days != 0) hasTotal++;

            if (hasTotal != 1)
                throw new ArgumentException("Span must have one and only one quantity non-zero.");

            Span = span;
        }
    }
}