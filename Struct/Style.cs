using System;
using System.Collections.Generic;
using System.Text;

namespace RinUI
{
    public class Style
    {
        public int? Width { get; }
        public int? Height { get; }
        public int? Margin { get; }
        public int? Padding { get; }
        public Align TextAlign { get; }
        public Align VerticalAlign { get; }
        public int? Top { get; }
        public int? Bottom { get; }
        public int? Left { get; }
        public int? Right { get; }
        public uint? FontColor { get; }
        public uint? BackgroundColor { get; }
        public uint? BorderLineColor { get; }
        public uint? FontColorHover { get; }
        public uint? BackgroundColorHover { get; }
        public uint? BorderLineColorHover { get; }
        public int? FontHandle { get; }

        public Style(int? width = null, int? height = null, int? margin = null, int? padding = null,
            Align textAlign = Align.START, Align verticalAlign = Align.CENTER, int? top = null, int? bottom = null, int? left = null, int? right = null, 
            uint? fontColor = 0xFFFFFF, uint? borderLineColor = 0xFFFFFF, uint? backgroundColor = 0x000000,
            uint? fontColorHover = null, uint? borderLineColorHover = null, uint? backgroundColorHover = null,int? fontHandle = null)
        {
            Width = width;
            Height = height;
            Margin = margin;
            Padding = padding;
            TextAlign = textAlign;
            VerticalAlign = verticalAlign;
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
            FontColor = fontColor;
            BorderLineColor = borderLineColor;
            BackgroundColor = backgroundColor;
            FontHandle = fontHandle == null ? Font.DefaultFont : fontHandle;
            FontColorHover = fontColorHover;
            BackgroundColorHover = backgroundColorHover;
            BorderLineColorHover = borderLineColorHover;
        }
    }

    public enum Align
    {
        START = 0,
        END,
        CENTER
    }
}
