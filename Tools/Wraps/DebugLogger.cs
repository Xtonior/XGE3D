using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XGE3D.Tools.Wraps
{
    internal static class DebugLogger
    {
        public static void Message(string message) => XTLogger.Message(message);
        public static void Message(object sender, string message) => XTLogger.Message(sender, message);
        public static void Trace(string message) => XTLogger.Trace(message);
        public static void Trace(object sender, string message) => XTLogger.Trace(sender, message);
        public static void Warn(string message) => XTLogger.Warn(message);
        public static void Warn(object sender, string message) => XTLogger.Warn(sender, message);
        public static void Error(string message) => XTLogger.Error(message);
        public static void Error(object sender, string message) => XTLogger.Error(sender, message);
        public static void Critical(string message) => XTLogger.Critical(message);
        public static void Critical(object sender, string message) => XTLogger.Critical(sender, message);
    }
}
