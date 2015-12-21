using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace MusicMinkAppLayer.Helpers
{
    public static class ColorHelper
    {
        public static Color GetColorFromHEX(object value)
        {
            uint color = System.Convert.ToUInt32(value.ToString(), fromBase: 16);
            byte A = (byte)((color & 0xFF000000) >> 24);
            byte R = (byte)((color & 0x00FF0000) >> 16);
            byte G = (byte)((color & 0x0000FF00) >> 8);
            byte B = (byte)((color & 0x000000FF) >> 0);
            Color col = Color.FromArgb(A, R, G, B);
            if (col.A == 0)
            {
                col.A = 255;
            }

            return col;
        }

        public static Brush GetBrushFromHEX(object value)
        {
            Color cc = ColorHelper.GetColorFromHEX(value);
            SolidColorBrush Result = new SolidColorBrush(cc);
            return Result;

        }

        public static Color GetLighterColor(this Color origin, int buff)
        {
            Color result = Windows.UI.ColorHelper.FromArgb(origin.A, GetValue(origin.R, buff, true), GetValue(origin.G, buff, true), GetValue(origin.B, buff, true));
            return result;
        }
        public static Color GetDarkerColor(this Color origin, int buff)
        {
            Color result = Windows.UI.ColorHelper.FromArgb(origin.A, GetValue(origin.R, buff, false), GetValue(origin.G, buff, false), GetValue(origin.B, buff, false));
            return result;
        }

        

        private static byte GetValue(int originvalue, int buff, bool isLighter)
        {
            int result = isLighter ? originvalue + buff : originvalue - buff;
            if (result > 255)
            {
                result = 255;
            }
            if (result < 0)
            {
                result = 0;
            }
            return Convert.ToByte(result);
        }
    }
}
