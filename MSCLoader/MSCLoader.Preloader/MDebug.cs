using System;
using System.Diagnostics;
using System.Reflection;

namespace MSCLoader.Preloader
{
    internal class MDebug
    {
        private static readonly TraceSource ts = new TraceSource("MSCLoader");
        private static readonly TextWriterTraceListener tw = new TextWriterTraceListener("MSCLoader_Preloader.txt");

        public static void Init()
        {
            ts.Switch.Level = SourceLevels.All;
            ts.Listeners.Add(tw);
            Log($"MSCLoader Preloader Log {DateTime.Now.ToString("u")}");
            Log($"Version {Assembly.GetExecutingAssembly().GetName().Version}", true);
        }

        public static void Log(string message, bool newline = false)
        {
            tw.WriteLine(message);
            if (newline) tw.WriteLine("");
            tw.Flush();
        }
    }
}