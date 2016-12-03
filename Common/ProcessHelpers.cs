using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Gongchengshi
{
   public static class ProcessHelpers
   {
      public static void WaitForProcessesToClose(params string[] processNames)
      {
         var processList = new List<Process>();
         foreach(var processName in processNames)
         {
            processList.AddRange(Process.GetProcessesByName(processName));
         }

         WaitForProcessesToClose(processList);
      }

      public static void WaitForProcessesToClose(IEnumerable<Process> processes)
      {
         bool stillRunning = true;

         while (stillRunning)
         {
            Thread.Sleep(1000);
            foreach (var process in processes)
            {
               if (stillRunning ^= process.HasExited)
               {
                  break;
               }
            }
         }
      }
   }
}
