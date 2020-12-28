using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Utils
{
    public static class IOHelper
    {
        public static short FromSignedByte(byte b)
        {
            return b > 127 ? (short)(b - 256) : b;
        }

        public static byte ToEmulatedSignedByte(short b)
        {
            return b < 0 ? (byte) (256 + b) : (byte)b;
        }
    }
}
