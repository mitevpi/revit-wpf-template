using System;
using System.Diagnostics;
using System.Threading;

namespace RevitTemplate
{
    public static class Util
    {
        public static void LogThreadInfo(string name = "")
        {
            Thread th = Thread.CurrentThread;
            Debug.WriteLine($"Task Thread ID: {th.ManagedThreadId}, Thread Name: {th.Name}, Process Name: {name}");
        }

        public static void HandleError(Exception ex)
        {
            Debug.WriteLine(ex.Message);
            Debug.WriteLine(ex.Source);
            Debug.WriteLine(ex.StackTrace);
        }
    }
}