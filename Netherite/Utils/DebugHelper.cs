using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Utils
{
    public static class DebugHelper
    {
        public static string HexDump(byte[] buffer)
        {
            string dump = "";
            foreach(byte b in buffer)
            {
                dump += $"{b:x2} ";
            }
            return dump.Trim();
        }
    }
}
