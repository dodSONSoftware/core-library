using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Threading
{
    /// <summary>
    /// Common threading methods.
    /// </summary>
    public static class ThreadingHelper
    {
        /// <summary>
        /// Suspends the current thread for the specified time.
        /// </summary>
        /// <param name="waitTime">The amount of time to suspend the current thread.</param>
        public static void Sleep(TimeSpan waitTime)
        {
            using (var waiter = new System.Threading.AutoResetEvent(false))
            {
                waiter.WaitOne(waitTime);
            }
        }
        /// <summary>
        /// Suspends the current thread for a specified number of milliseconds.
        /// </summary>
        /// <param name="milliseconds">The number of milliseconds for which the thread is suspended.</param>
        public static void Sleep(double milliseconds) => ThreadingHelper.Sleep(TimeSpan.FromMilliseconds(milliseconds));
    }
}
