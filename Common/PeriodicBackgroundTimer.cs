using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gongchengshi
{
    /// <summary>
    /// This timer encapsulates the functionality frequently used for timers:
    ///   * Periodic.
    ///   * Run on a background timer thread.
    ///   * Start immediately (i.e., the timer action is called almost 
    ///     immediately followed by the period delay).
    ///   * Don't reenter the timer method (if the timer method happens to take
    ///     longer than the timer period).
    /// </summary>
    public class PeriodicBackgroundTimer : IDisposable
    {
        /// <summary>
        /// Construct a periodic timer with the given period and timer action.
        /// </summary>
        /// <param name="period">Period.</param>
        /// <param name="action">Action to call when timer fires.</param>
        public PeriodicBackgroundTimer(TimeSpan period, Action action)
        {
            _period = period;
            _action = action;

            // Use a short interval to get the timer going immediately.
            _timer = new System.Timers.Timer { Interval = 1, AutoReset = false };
            _timer.Elapsed += _timer_Elapsed;
            _timer.Enabled = true;
        }

        /// <summary>
        /// Called when timer elapses.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                _action();
            }
            finally
            {
                // This may fail because the timer has been disposed.  No problem,
                // just ignore the exception thrown.
                try
                {
                    _timer.Interval = _period.TotalMilliseconds;
                }
                catch { }
            }
        }

        /// <summary>
        /// The internal timer.
        /// </summary>
        System.Timers.Timer _timer;
        /// <summary>
        /// The period of the timer.
        /// </summary>
        TimeSpan _period;
        /// <summary>
        /// The action to call when the timer expires.
        /// </summary>
        Action _action;

        public void Dispose()
        {
            _timer.Dispose();
        }
    }

}
