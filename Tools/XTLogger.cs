using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XGE3D.Tools
{
    enum LogLevelColor
    {
        Message = ConsoleColor.White,
        Trace = ConsoleColor.Cyan,
        Warn = ConsoleColor.DarkYellow,
        Error = ConsoleColor.Red,
        Critical = ConsoleColor.DarkRed
    }

    public static class XTLogger
    {
        public static void Message(string message)
        {
            ConstructMessage(message, LogLevelColor.Message);
        }

        public static void Message(Object sender, string message)
        {
            ConstructMessage($"<{sender}> {message}", LogLevelColor.Message);
        }

        public static void Trace(string message)
        {
            ConstructMessage(message, LogLevelColor.Trace);
        }

        public static void Trace(Object sender, string message)
        {
            ConstructMessage($"<{sender}> {message}", LogLevelColor.Trace);
        }

        public static void Warn(string message)
        {
            ConstructMessage(message, LogLevelColor.Warn);
        }

        public static void Warn(Object sender, string message)
        {
            ConstructMessage($"<{sender}> {message}", LogLevelColor.Warn);
        }

        public static void Error(string message)
        {
            ConstructMessage(message, LogLevelColor.Error);
        }

        public static void Error(Object sender, string message)
        {
            ConstructMessage($"<{sender}> {message}", LogLevelColor.Error);
        }

        public static void Critical(string message)
        {
            ConstructMessage(message, LogLevelColor.Critical);
        }

        public static void Critical(Object sender, string message)
        {
            ConstructMessage($"<{sender}> {message}", LogLevelColor.Critical);
        }

        private static void ConstructMessage(string message, LogLevelColor color)
        {
            Console.ForegroundColor = (ConsoleColor)color;
            Console.WriteLine($"[{DateTime.Now} {color}] " + message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
