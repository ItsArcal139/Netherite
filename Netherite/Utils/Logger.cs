using Netherite.Texts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Netherite.Utils
{
    public enum LogLevel
    {
        Verbose, Log, Info, Warn, Error, Fatal
    }

    public static class Logger
    {
        public static LogLevel Level { get; set; }

        private const string DefaultName = null;

        public static readonly TranslateText PrefixFormat = TranslateText.Of("{2} - {0} {1}");

        private static SemaphoreSlim logLock = new SemaphoreSlim(1, 1);

        private static string GetCallSourceName()
        {
            string name;
            StackTrace stack = new StackTrace();
            MethodBase method = stack.GetFrame(2)?.GetMethod();
            if (method == null)
            {
                name = "Netherite";
            }
            else
            {
                name = GetRootType(method.DeclaringType).FullName;
            }
            return name;
        }

        private static Type GetRootType(Type t)
        {
            if (t.Name.StartsWith("<"))
            {
                return GetRootType(t.DeclaringType);
            }
            return t;
        }

        private static void Log(LogLevel level, Text t, TextColor color, string name = DefaultName)
        {
            if (Level > level) return;
            TranslateText f = PrefixFormat.Clone();
            LiteralText tag = LiteralText.Of($"[{name}]").SetColor(color);

            logLock.Wait();
            ConsoleHelper.WriteAsciiLine(f.AddWith(tag, t, LiteralText.Of(DateTime.Now.ToString())).ToAscii());
            logLock.Release();
        }

        public static void Log(Text t, string name = DefaultName)
        {
            if (name == null) name = GetCallSourceName();
            Log(LogLevel.Log, t, TextColor.DarkGray, name);
        }

        public static void Log(string msg, string name = DefaultName)
        {
            if (name == null) name = GetCallSourceName();
            Log(LiteralText.Of(msg), name);
        }

        public static void Verbose(Text t, string name = DefaultName)
        {
            if (Server.Instance.Config.DebugPacket)
            {
                if (name == null) name = GetCallSourceName();
                Log(LogLevel.Verbose, t, TextColor.DarkGray, name);
            }
        }

        public static void Verbose(string msg, string name = DefaultName)
        {
            if (name == null) name = GetCallSourceName();
            Verbose(LiteralText.Of(msg), name);
        }

        public static void Info(Text t, string name = DefaultName)
        {
            if (name == null) name = GetCallSourceName();
            Log(LogLevel.Info, t, TextColor.Green, name);
        }

        public static void Info(string msg, string name = DefaultName)
        {
            if (name == null) name = GetCallSourceName();
            Info(LiteralText.Of(msg), name);
        }

        public static void Warn(Text t, string name = DefaultName)
        {
            if (name == null) name = GetCallSourceName();
            Log(LogLevel.Warn, t, TextColor.Gold, name);
        }

        public static void Warn(string msg, string name = DefaultName)
        {
            if (name == null) name = GetCallSourceName();
            Warn(LiteralText.Of(msg), name);
        }

        public static void Error(Text t, string name = DefaultName)
        {
            if (name == null) name = GetCallSourceName();
            Log(LogLevel.Error, t, TextColor.Red, name);
        }

        public static void Error(string msg, string name = DefaultName)
        {
            if (name == null) name = GetCallSourceName();
            Error(LiteralText.Of(msg), name);
        }
    }
}
