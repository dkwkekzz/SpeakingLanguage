using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace SpeakingLanguage.Library
{
    public class Tracer
    {
        public static void AddTextListener(string logPath)
        {
            Trace.Listeners.Add(new TextTraceListner(logPath));
        }

        public static void AddConsoleListener()
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
        }

        public static string GetCaller(
            [CallerFilePath] string filePath = null, // __FILE__
            [CallerLineNumber] int n = 0, // __LINE__
            [CallerMemberName] string name = null) //__FUNC__
        {
            return $"file: {filePath}, line: {n.ToString()}, func: {name}";
        }

        public static void Assert(bool condition)
        {
            Trace.Assert(condition);
        }

        public static void Assert(bool condition, string msg)
        {
            Trace.Assert(condition, msg);
        }

        public static void Write(string msg)
        {
            Trace.WriteLine(msg);
        }

        public static void Write(string tag, string msg)
        {
            Trace.WriteLine($"[{tag}] {msg}");
        }

        public static void Write(object caller, string msg)
        {
            Trace.WriteLine($"[{caller.GetType().Name}] {msg}");
        }

        public static void Error(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Trace.TraceError(msg);
            Console.ResetColor();
        }
        
        public static void Error(string tag, string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Trace.TraceError($"[{tag}] {msg}");
            Console.ResetColor();
        }

        public static void Exception(Type caller, Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Trace.TraceError($"[{caller.Name}]\n{e.GetType().Name}\n{e.Message}\n{e.StackTrace}");
            Console.ResetColor();
        }

        public static void Exception(Type caller, Exception e, string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Trace.TraceError($"[{caller.Name}]\n{e.Message}\n{e.StackTrace}\n=====\n{msg}");
            Console.ResetColor();
        }
    }
}
