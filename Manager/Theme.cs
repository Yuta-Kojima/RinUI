using System;
using System.Collections.Generic;
using System.Text;
using DxLibDLL;

namespace RinUI.Manager
{
    public class Theme
    {
        private static Theme instance = null;

        public static Theme Instance(uint mainColor = 0x111111, uint? secondColor = null, uint? borderColor = null,
            uint? accentColor = null, uint? fontColor = null, uint? accentFontColor = null)
        {
            if (instance != null)
            {
                return instance;
            }

            instance = new Theme(mainColor, secondColor, borderColor, accentColor, fontColor, accentFontColor);
            return instance;
        }
        public static Theme I(uint color = 0x111111) { return Instance(color); }

        // Main
        public uint MainColor { get; }
        public uint MainColorDark { get; }
        public uint MainColorLight { get; }

        // Second
        public uint SecondColor { get; }
        public uint SecondColorDark { get; }
        public uint SecondColorLight { get; }

        // Border
        public uint BorderColor { get; }
        public uint BorderColorDark { get; }
        public uint BorderColorLight { get; }

        // Accent
        public uint AccentColor { get; }
        public uint AccentColorDark { get; }
        public uint AccentColorLight { get; }
        public uint ErrorColor { get; }

        // Font
        public uint FontColor { get; }
        public uint AccentFontColor { get; }
        public int MenuFontHandle { get; }
        public int MenuFontHandleSize { get; }
        public int MainFontHandle { get; }
        public int MainFontHandleSize { get; }
        public int SecondFontHandle { get; }
        public int SecondFontHandleSize { get; }

        private Theme(uint mainColor, uint? secondColor = null, uint? borderColor = null,
            uint? accentColor = null, uint? fontColor = null, uint? accentFontColor = null,
            int? menuFontHandleSize = null, int? mainFontHandleSize = null, int? secondFontHandleSize = null)
        {
            int range = 20;

            MainColor = mainColor;
            MainColorLight = new Color(mainColor)[range];
            MainColorDark = new Color(mainColor)[-range];
            SecondColor = secondColor ?? new Color(MainColor)[range];
            SecondColorLight = new Color(SecondColorDark)[range];
            SecondColorDark = new Color(SecondColorDark)[-range];
            BorderColor = borderColor ?? new Color(MainColor)[100];
            BorderColorLight = new Color(BorderColor)[range];
            BorderColorDark = new Color(BorderColor)[-range];
            AccentColor = accentColor ?? Color.PINK[range];
            FontColor = fontColor ?? Color.WHITE[0];
            AccentFontColor = accentFontColor ?? new Color(AccentColor)[100];
            MenuFontHandle = DX.CreateFontToHandle("ＭＳ ゴシック", menuFontHandleSize ?? 14, 1, DX.DX_FONTTYPE_ANTIALIASING);
            MenuFontHandleSize = menuFontHandleSize ?? 10;
            MainFontHandle = DX.CreateFontToHandle("ＭＳ ゴシック", mainFontHandleSize ?? 18, 1, DX.DX_FONTTYPE_ANTIALIASING);
            MainFontHandleSize = mainFontHandleSize ?? 18;
            SecondFontHandle = DX.CreateFontToHandle("micross", secondFontHandleSize ?? 16, 1, DX.DX_FONTTYPE_ANTIALIASING);
            SecondFontHandleSize = secondFontHandleSize ?? 16;
            ErrorColor = Color.RED[0];
        }
    }
}
