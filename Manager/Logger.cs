using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace RinUI.Manager
{
    public static class Logger
    {
        public static void Log<T>(T log, [CallerMemberName] string callerMemberName = "")
        {
            Console.WriteLine($"{callerMemberName}: {log}");
        }
    }
}
