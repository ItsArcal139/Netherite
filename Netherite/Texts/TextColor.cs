using Netherite.Data;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Netherite.Texts
{
    public class TextColor
    {
        public const char ColorChar = '\u00a7';

        private string name;
        private int ordinal;
        private string toString;
        private Color color;

        private static Dictionary<char, TextColor> byChar = new();
        private static Dictionary<string, TextColor> byName = new();

        private static int count = 0;

        public static readonly TextColor Black      = new('0', "black",       new Color(0));
        public static readonly TextColor DarkBlue   = new('1', "dark_blue",   new Color(0xaa));
        public static readonly TextColor DarkGreen  = new('2', "dark_green",  new Color(0xaa00));
        public static readonly TextColor DarkAqua   = new('3', "dark_aqua",   new Color(0xaaaa));
        public static readonly TextColor DarkRed    = new('4', "dark_red",    new Color(0xaa0000));
        public static readonly TextColor DarkPurple = new('5', "dark_purple", new Color(0xaa00aa));
        public static readonly TextColor Gold       = new('6', "gold",        new Color(0xffaa00));
        public static readonly TextColor Gray       = new('7', "gray",        new Color(0xaaaaaa));
        public static readonly TextColor DarkGray   = new('8', "dark_gray",   new Color(0x555555));
        public static readonly TextColor Blue       = new('9', "blue",        new Color(0x5555ff));
        public static readonly TextColor Green      = new('a', "green",       new Color(0x55ff55));
        public static readonly TextColor Aqua       = new('b', "aqua",        new Color(0x55ffff));
        public static readonly TextColor Red        = new('c', "red",         new Color(0xff5555));
        public static readonly TextColor Purple     = new('d', "purple",      new Color(0xff55ff));
        public static readonly TextColor Yellow     = new('e', "yellow",      new Color(0xffff55));
        public static readonly TextColor White      = new('f', "white",       new Color(0xffffff));

        private TextColor(char code, string name, Color color)
        {
            this.name = name;
            toString = ColorChar + "" + code;
            ordinal = count++;
            this.color = color;

            byChar.Add(code, this);
            byName.Add(name, this);
        }

        private TextColor(string name, string toString, int rgb)
        {
            this.name = name;
            this.toString = toString;
            ordinal = -1;
            color = new Color(rgb);
        }

        public static TextColor Of(Color color)
        {
            return Of("#" + $"{color.RGB:x6}");
        }

        public static TextColor Of(string name)
        {
            if(name == null)
                throw new ArgumentNullException(nameof(name), "name cannot be null");

            if(name.StartsWith("#") && name.Length == 7)
            {
                int rgb;
                try
                {
                    rgb = int.Parse(name[1..], NumberStyles.HexNumber);
                } catch(FormatException)
                {
                    throw new ArgumentException("Illegal hex string " + name);
                }

                string magic = ColorChar + "x";
                foreach(char c in name[1..])
                {
                    magic += ColorChar + "" + c;
                }

                return new TextColor(name, magic, rgb);
            }

            if(byName.TryGetValue(name, out TextColor defined))
            {
                return defined;
            }

            throw new ArgumentException("Could not parse TextColor " + name);
        }

        public override string ToString() => toString;

        public string Name => name;

        public Color Color => color;

        public TextColor ToNearestPredefinedColor()
        {
            char c = toString[1];
            if (c != 'x')
            {
                return this;
            }

            TextColor closest = null;
            Color cl = Color;

            TextColor[] defined = new TextColor[]
            {
                Black,
                DarkBlue,
                DarkGreen,
                DarkAqua,
                DarkRed,
                DarkPurple,
                Gold,
                Gray,
                DarkGray,
                Blue,
                Green,
                Aqua,
                Red,
                Purple,
                Yellow,
                White
            };

            int smallestDiff = 0;
            foreach (TextColor tc in defined)
            {
                int rAverage = (tc.Color.R + cl.R) / 2;
                int rDiff = tc.Color.R - cl.R;
                int gDiff = tc.Color.G - cl.G;
                int bDiff = tc.Color.B - cl.B;

                int diff = ((2 + (rAverage >> 8)) * rDiff * rDiff)
                    + (4 * gDiff * gDiff)
                    + ((2 + ((255 - rAverage) >> 8)) * bDiff * bDiff);

                if (closest == null || diff < smallestDiff)
                {
                    closest = tc;
                    smallestDiff = diff;
                }
            }

            return closest;
        }

        public static char[] McCodes()
        {
            return "0123456789abcdef".ToCharArray();
        }
    }
}
