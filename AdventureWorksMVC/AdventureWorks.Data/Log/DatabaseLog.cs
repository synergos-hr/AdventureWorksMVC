using System.Diagnostics;
//using NLog;

namespace AdventureWorks.Data.Log
{
    internal static class DatabaseLog
    {
        //private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        internal static void Write(string message)
        {
            Debug.Write(message);

            // Log.Trace(message);
        }
    }
}
