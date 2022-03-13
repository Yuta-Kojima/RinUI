using System;
using System.Collections.Generic;
using System.Text;

namespace RinUI
{
    public partial struct Color
    {
        public static readonly Color WHITE = new(255, 255, 255);
        public static readonly Color GRAY = new(0x777777);
        public static readonly Color BLACK = new Color(0, 0, 0);
        public static readonly Color RED = new(0xFF0000);
        public static readonly Color BLUE = new(0x0000FF);
        public static readonly Color GREEN = new(0x00FF00);
        public static readonly Color GOLD = new(0xFFD700);
        public static readonly Color ORANGE = new(0xFF4500);
        public static readonly Color PURPLE = new(0x800080);
        public static readonly Color PINK = new(0xFF69B4);
        public static readonly Color M_GREEN = new(0x3CB371); // MidiumSeaGreen
        public static readonly Color M_TURQUOISE = new(0x48D1CC); // MidiumTurquoise
        public static readonly Color M_BLUE = new(0x7B68EE); // MidiumSlateBlue
    }

    public partial struct Color
    {
        readonly byte r;
        readonly byte g;
        readonly byte b;

        public Color(byte r, byte g, byte b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }
        public Color(uint u)
        {
            r = (byte)(u / 0x10000);
            g = (byte)(u / 0x100 - u / 0x10000 * 0x100);
            b = (byte)(u % 0x100);

            Color color = new(r, g, b);
            (byte b1, byte b2, byte b3) = color.SplitHex(color[100]);
        }

        public uint this[int vol]
        {
            get
            {
                return this + vol;
            }
        }

        public (byte, byte, byte) SplitHex(uint u)
        {
            return ((byte)(u / 0x10000), (byte)(u / 0x100 - u / 0x10000 * 0x100), (byte)(u % 0x100));
        }

        public static uint operator +(Color z, int w)
        {
            uint r = PlByte(z.r, w);
            uint g = PlByte(z.g, w);
            uint b = PlByte(z.b, w);

            return ConvertRgbToUint(r, g, b);
        }

        public static uint operator -(Color z, int w)
        {
            uint r = PlByte(z.r, -w);
            uint g = PlByte(z.g, -w);
            uint b = PlByte(z.b, -w);

            return ConvertRgbToUint(r, g, b);
        }


        private static uint PlByte(uint z, int w)
        {
            if (0 > (int)z + w)
            {
                return 0;
            }
            else if (255 < (int)z + w)
            {
                return 255;
            }
            else
            {
                return (uint)((int)z + w);
            }
        }

        private static uint ConvertRgbToUint(uint r, uint g, uint b)
        {
            return r * 0x10000 + g * 0x100 + b;
        }
    }

}
