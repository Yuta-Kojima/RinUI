using System;
using System.Collections.Generic;
using System.Text;

namespace RinUI
{
    public struct Vector2x2
    {
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }

        public int Height => Y2 - Y1;
        public int Width => X2 - X1;

        public Vector2x2(int x1, int y1, int x2, int y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        public static bool operator <(Vector2x2 lValue, Vector2 rValue)
        {
            return (lValue.X1 <= rValue.X && rValue.X <= lValue.X2 &&
                    lValue.Y1 <= rValue.Y && rValue.Y <= lValue.Y2);
        }

        public static bool operator >(Vector2x2 lValue, Vector2 rValue)
        {
            return (lValue.X1 <= rValue.X && rValue.X <= lValue.X2 &&
                    lValue.Y1 <= rValue.Y && rValue.Y <= lValue.Y2);
        }
    }
}
