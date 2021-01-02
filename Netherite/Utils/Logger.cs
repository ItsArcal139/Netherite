using Netherite.Texts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Netherite.Utils
{
    public static class Logger
    {
        private const string DefaultName = "Netherite";

        public static readonly TranslateText PrefixFormat = TranslateText.Of("{2} - {0} {1}");

        private static SemaphoreSlim logLock = new SemaphoreSlim(1, 1);

        private static void Log(Text t, TextColor color, string name = DefaultName)
        {
            TranslateText f = PrefixFormat.Clone();
            LiteralText tag = LiteralText.Of($"[{name}]").SetColor(color);

            logLock.Wait();
            ConsoleHelper.WriteAsciiLine(f.AddWith(tag, t, LiteralText.Of(DateTime.Now.ToString())).ToAscii());
            logLock.Release();
        }

        public static void Log(Text t, string name = DefaultName)
        {
            Log(t, TextColor.DarkGray, name);
        }

        public static void Log(string msg, string name = DefaultName)
        {
            Log(LiteralText.Of(msg), name);
        }

        public static void LogPacket(string msg, string name = DefaultName)
        {
            LogPacket(LiteralText.Of(msg), name);
        }

        public static void LogPacket(Text t, string name = DefaultName)
        {
            if (Server.Instance.Config.DebugPacket)
            {
                Log(t, TextColor.DarkGray, name);
            }
        }

        public static void Info(Text t, string name = DefaultName)
        {
            Log(t, TextColor.Green, name);
        }

        public static void Warn(Text t, string name = DefaultName)
        {
            Log(t, TextColor.Gold, name);
        }

        public static void Warn(string msg, string name = DefaultName)
        {
            Warn(LiteralText.Of(msg), name);
        }

        public static void Error(Text t, string name = DefaultName)
        {
            Log(t, TextColor.Red, name);
        }
    }
}
