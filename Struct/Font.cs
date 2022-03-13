using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Text;
using DxLibDLL;

namespace RinUI
{
    public partial class Font
    {
    
        private Dictionary<string,int> fontDictionary = new Dictionary<string,int>();
        private static readonly Font font = new();
        FontFamily[] fontFamilies;

        private Font()
        {
            //using (InstalledFontCollection fonts = new InstalledFontCollection())
            //{
            //    fontFamilies = fonts.Families;
            //}
        }

        public static Font Instance()
        {
            return font;
        }

        public void AddFont(string fontName, int fontSize = 16)
        {
            fontDictionary.Add(fontName, DX.CreateFontToHandle(fontName, fontSize, 1, DX.DX_FONTTYPE_ANTIALIASING));
        }

        public int this[string fontName]
        {
            get
            {
                return fontDictionary[fontName];
            }
        }

        public static int GetFontSize(int fontHandle)
        {
            DX.GetFontStateToHandle(new StringBuilder(0), out int size, out _, fontHandle, out _, out _, out _, out _);
            return size;
        }
    }

    public partial class Font { 
        public static int DefaultFont = DX.CreateFontToHandle("メイリオ", 12, 1, DX.DX_FONTTYPE_ANTIALIASING);
    }
}
