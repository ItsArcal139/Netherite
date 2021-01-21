using System;
using System.Collections.Generic;
using System.Text;
using Netherite.Utils;

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

        public Color(double r, double g, double b)
        {
            CommonOperations.ToPercent(ref r, "r");
            CommonOperations.ToPercent(ref g, "g");
            CommonOperations.ToPercent(ref b, "b");

            R = (byte)(r * 255);
            G = (byte)(g * 255);
            B = (byte)(b * 255);
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

        public (double, double, double) Normalized => (R / 255.0, G / 255.0, B / 255.0);

        public (double, double, double) ToHsv()
        {
            var (r, g, b) = Normalized;

            double max = MathHelper.Max(r, g, b);
            double min = MathHelper.Min(r, g, b);
            double delta = max - min;

            double hue, saturation, value;
            double d60 = MathHelper.DegToRad * 60;

            // Calculate the hue
            if (delta == 0)
            {
                hue = 0;
            }
            else if (max == r)
            {
                hue = d60 * ((g - b) / delta % 6);
            }
            else if (max == g)
            {
                hue = d60 * ((b - r) / delta + 2);
            }
            else if (max == b)
            {
                hue = d60 * ((r - g) / delta + 4);
            }
            else
            {
                throw new Exception("Shouldn't reach here");
            }

            // Calculate the saturation
            if(max == 0)
            {
                saturation = 0;
            } else
            {
                saturation = delta / max;
            }

            value = max;
            return (hue, saturation, value);
        }

        public double Hue => ToHsv().Item1;

        public static Color FromHsv(double hue, double saturation, double value)
        {
            // Normalize the hue input.
            if (hue < 0)
            {
                hue *= -1;
                hue %= Math.PI * 2;
                hue *= -1;
                hue += 360;
            }
            else
            {
                hue %= Math.PI * 2;
            }

            CommonOperations.ToPercent(ref saturation, "Saturation");
            CommonOperations.ToPercent(ref value, "Value");

            double d60 = MathHelper.DegToRad * 60;
            double d120 = d60 * 2;
            double d180 = d60 * 3;
            double d240 = d60 * 4;
            double d300 = d60 * 5;

            double c = value * saturation;
            double x = c * (1 - Math.Abs((hue / d60 % 2) - 1));
            double m = value - c;

            double r = 0, g = 0, b = 0;

            // Calculate the colors.
            if (0 <= hue && hue < d60)
            {
                r = c;
                g = x;
            } else if (d60 <= hue && hue < d120)
            {
                r = x;
                g = c;
            } else if (d120 <= hue && hue < d180)
            {
                g = c;
                b = x;
            } else if (d180 <= hue && hue < d240)
            {
                g = x;
                b = c;
            } else if (d240 <= hue && hue < d300)
            {
                b = c;
                r = x;
            } else
            {
                b = x;
                r = c;
            }

            return new Color(r + m, g + m, b + m);
        }
    }
}
