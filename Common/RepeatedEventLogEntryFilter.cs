using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Gongchengshi
{
    public interface IEventLog
    {
        void WriteEntry(string message, EventLogEntryType type, short catagory);
    }

    /// <summary>
    /// This class filters out repeated Windows Event Log Entries to avoid filling the log too rapidly.
    /// Algorithm:
    /// 1. Report event on first call.
    /// 2. If repeated, report again after 1-2 minutes, with count of the number of occurrences.  
    ///    This report will mention that further messages will be update only once per hour.
    /// 3. If still repeating, report every hour, with count of number of occurrences.
    /// 4. After > 1 minute of non-repeat, report the count of # of times message occurred.
    /// </summary>
    public class RepeatedEventLogEntryFilter : IEventLog
    {
        /// <summary>
        /// Construct a filter that forwards messages to the given eventLog.
        /// </summary>
        /// <param name="eventLog">Event log to forward messages to.</param>
        public RepeatedEventLogEntryFilter(EventLog eventLog)
        {
            _eventLog = eventLog;

            _repeatedEntryTimer =
                new System.Timers.Timer() 
                { 
                    AutoReset = false,
                    Interval = (ulong)_repeatedEntryUpdateSeconds * Constants.MilliSecPerSec 
                };
            _repeatedEntryTimer.Elapsed += (o, e) => ManageRepeatedEntries();
        }
        EventLog _eventLog;

        /// <summary>
        /// Wait 1 minute before first repeated message.
        /// </summary>
        static int _repeatedEntryUpdateSeconds = 60;

        /// <summary>
        /// After one minute, report every hour.
        /// </summary>
        static TimeSpan _repeatedEntryReminderPeriod = new TimeSpan(1, 0, 0);

        /// <summary>
        /// RepeatedEntry keeps track of an entry that is (or may be) repeating.
        /// </summary>
        class RepeatedEntry
        {
            public short EventLogCatagory;
            public EventLogEntryType Severity;
            public int CountSinceReported = 0;
            public DateTime LastReport = DateTime.MinValue;
            public DateTime LastOccurance = DateTime.MinValue;
            public bool OverOneMinute = false;
        }

        /// <summary>
        /// List of active repeating entries.
        /// </summary>
        Dictionary<string, RepeatedEntry> _repeatedEntries = new Dictionary<string, RepeatedEntry>();

  

        /// <summary>
        /// Write a message to the event log, filtering if the same message has been
        /// written recently.
        /// </summary>
        /// <param name="message">Message to write.</param>
        /// <param name="severity">Severity level of message.</param>
        public void WriteEntry(String message, EventLogEntryType severity, short catagory)
        {
            lock (_repeatedEntries)
            {
                var now = DateTime.Now;

                if (_repeatedEntries.ContainsKey(message))
                {
                    _repeatedEntries[message].CountSinceReported++;
                    _repeatedEntries[message].LastOccurance = now;
                }
                else
                {
                    _eventLog.WriteEntry(message, severity, 0, catagory);
                    _repeatedEntries.Add(message,
                        new RepeatedEntry() {EventLogCatagory = catagory, Severity = severity, CountSinceReported = 0, LastOccurance = now, LastReport = now });
                }
            }
            _repeatedEntryTimer.Start();
        }

        /// <summary>
        /// The timer that updates status of repeated entries.
        /// </summary>
        System.Timers.Timer _repeatedEntryTimer;

        /// <summary>
        /// Every minute, update the status of repeated entries, clearing out old ones and 
        /// generating log messages as necessary.
        /// </summary>
        void ManageRepeatedEntries()
        {
            lock (_repeatedEntries)
            {
                var now = DateTime.Now;
                foreach (var message in _repeatedEntries.Keys.ToArray())
                {
                    var repeatedEntry = _repeatedEntries[message];
                    if (!repeatedEntry.OverOneMinute)
                    {
                        if (now - repeatedEntry.LastReport > new TimeSpan(0, 0, _repeatedEntryUpdateSeconds))
                        {
                            if (repeatedEntry.CountSinceReported == 0)
                            {
                                // The message only occurred once within a minute interval.  Remove from list.
                                _repeatedEntries.Remove(message);
                            }
                            else
                            {
                                // The message occurred multiple times over the minute.  Note this in the log, and
                                // mark the message as OverOneMinute.
                                _eventLog.WriteEntry(String.Format(
                                    "The following message was repeated {0} times.  This message will not be reported again for 1 hour.",
                                    repeatedEntry.CountSinceReported) +
                                    Environment.NewLine + Environment.NewLine + message, repeatedEntry.Severity, 0, repeatedEntry.EventLogCatagory);
                                repeatedEntry.CountSinceReported = 0;
                                repeatedEntry.LastReport = now;
                                repeatedEntry.OverOneMinute = true;
                            }
                        }
                    }
                    else
                    {
                        if (now - repeatedEntry.LastOccurance > new TimeSpan(0, 0, _repeatedEntryUpdateSeconds))
                        {
                            // The message has not happened within the one minute interval.  Problem is likely
                            // resolved.  Go ahead and remove from the list.
                            if (repeatedEntry.CountSinceReported != 0)
                            {
                                // Report the number of times this message occurred since last report.
                                _eventLog.WriteEntry(String.Format(
                                    "The following message was repeated {0} more times before its last occurrence at {1}.",
                                    repeatedEntry.CountSinceReported,
                                    repeatedEntry.LastOccurance.ToString()) +
                                    Environment.NewLine + Environment.NewLine + message,
                                    repeatedEntry.Severity);
                            }
                            _repeatedEntries.Remove(message);
                        }
                        else if (now - repeatedEntry.LastReport > _repeatedEntryReminderPeriod)
                        {
                            // The message is still happenening, and it has been a long time since reported.  Go
                            // ahead and create a reminder.
                            _eventLog.WriteEntry(String.Format(
                                "The following message was repeated {0} times.  This message will not be reported again for 1 hour.",
                                repeatedEntry.CountSinceReported) +
                                Environment.NewLine + Environment.NewLine + message, repeatedEntry.Severity);
                            repeatedEntry.CountSinceReported = 0;
                            repeatedEntry.LastReport = now;
                        }
                    }
                }

                if (_repeatedEntries.Count > 0)
                    _repeatedEntryTimer.Start();
            }
        }

// This method is not used, so disabling it to save test time.
#if false
        /// <summary>
        /// Clear out repeated entries without generating a log message.
        /// </summary>
        public void Clear()
        {
            lock (_repeatedEntries)
            {
                _repeatedEntries.Clear();
                _repeatedEntryTimer.Stop();
            }
        }
#endif
    }
}
