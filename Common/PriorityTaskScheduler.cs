using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gongchengshi
{
    /// <summary>
    /// This is a replacement for the default .NET TaskScheduler that allows setting
    /// the priority of threads.  It uses the Environment.ProcessorCount to determine
    /// the number of threads to use for running tasks.
    /// </summary>
    public class PriorityTaskScheduler : TaskScheduler
    {
        private ThreadPriority _threadPriority;
        public PriorityTaskScheduler(int numberOfThreads, ThreadPriority threadPriority)
        {
            _threadPriority = threadPriority;
            _threads = Enumerable.Range(0, numberOfThreads).Select(i =>
            {
                var thread = new Thread(() =>
                {
                    try
                    {
                        foreach (var task in tasks.GetConsumingEnumerable())
                        {
                            TryExecuteTask(task);
                        }
                    }
                    catch (Exception)
                    {
                        if (!_aborting)
                        {
                            throw;
                        }
                    }
                });
                thread.IsBackground = true;
                thread.Priority = _threadPriority;
                return thread;
            }).ToList();

            _threads.ForEach(t => t.Start());
        }
        private readonly List<Thread> _threads;
        private BlockingCollection<Task> tasks = new BlockingCollection<Task>();

        protected override void QueueTask(Task task)
        {
            tasks.Add(task);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            // Don't try to execute on same thread because it may be wrong priority.
            return false; 
        }

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return tasks.ToArray();
        }

        public void Dispose()
        {
            if (tasks != null)
            {
                tasks.CompleteAdding();
                foreach (var thread in _threads) thread.Join();
                tasks.Dispose();
                tasks = null;
            }
        }

        /// <summary>
        /// Aborts all tasks.  This is intended for testing purposes only.  The TaskScheduler
        /// is unusable after this call.
        /// </summary>
        public void AbortAllTasks()
        {
            _aborting = true;
            _threads.ForEach(t => t.Abort());
        }
        private bool _aborting = false;
    }
}
