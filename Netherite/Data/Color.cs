using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Data
{
    public struct Color
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public Color(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public Color(int hex)
        {
            R = (byte)((hex >> 16) & 0xff);
            G = (byte)((hex >> 8) & 0xff);
            B = (byte)(hex & 0xff);
        }

        public int RGB
        {
            get
            {
                int result = R;
                result = result << 8 | G;
                result = result << 8 | B;
                return result;
            }
        }
    }
}
