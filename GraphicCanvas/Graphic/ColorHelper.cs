using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace GraphicCanvas.Graphic
{
    static class ColorHelper
    {
        public static int ToInt(Color color)
        {
            int outColor = color.A;

            outColor = outColor << 8;
            outColor |= color.R;
            outColor = outColor << 8;
            outColor |= color.G;
            outColor = outColor << 8;
            outColor |= color.B;

            return outColor;
        }

        public static Color FromInt(int color)
        {
            return Color.FromArgb(color);
        }

        public static Color FromHEX(string hexString)
        {
            if (hexString[0] == '#')
            {
                hexString = hexString.Substring(1);
            }

            if (hexString.Length == 3)
            {
                hexString = "" + hexString[0] + hexString[0] + hexString[1] + hexString[1] + hexString[2] + hexString[2];
            }
            else if (hexString.Length == 6) { }
            else
            {
                throw new ArgumentException();
            }

            try
            {
                return Color.FromArgb((int)(0xFF000000 | Convert.ToInt64(hexString, 16)));
            }
            catch
            {
                throw new ArgumentException();
            }
        }

        public static Color FromRGB(int r, int g, int b)
        {
            return Color.FromArgb(r, g, b);
        }

        public static Color FromRGBA(int r, int g, int b, int a)
        {
            return Color.FromArgb(a, r, g, b);
        }

        public static Color FromCMYK(float c, float m, float y, float k)
        {
            return Color.FromArgb((int)(255 * (1 - c) * (1 - k)), (int)(255 * (1 - m) * (1 - k)), (int)(255 * (1 - y) * (1 - k)));
        }

        public static Color FromHSV(float h, float s, float v)
        {
            int hi = Convert.ToInt32(Math.Floor(h / 60)) % 6;
            double f = h / 60 - Math.Floor(h / 60);

            v = v * 255;
            int va = Convert.ToInt32(v);
            int p = Convert.ToInt32(v * (1 - s));
            int q = Convert.ToInt32(v * (1 - f * s));
            int t = Convert.ToInt32(v * (1 - (1 - f) * s));

            if (hi == 0)
                return Color.FromArgb(255, va, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, va, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, va, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, va);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, va);
            else
                return Color.FromArgb(255, va, p, q);
        }

        public static Color FromLAB(int l, int a, int b)
        {
            throw new NotImplementedException();
        }
    }
}
