using System;
using System.Collections.Generic;
using System.Linq;
using DxLibDLL;
using RinUI.Manager;

namespace RinUI.UIClasses
{
    public class RadioButton : IRinui
    {
        public Vector2x2 Vector { get; set; }
        public byte Layer { get; set; }
        public Style Style { get; set; }
        public List<string> Items { get; set; }
        public List<int?> ItemImgs { get; set; }
        public int SelectedIndex { get; set; } = 0;
        public Action<int> Action { get; private set; }

        private float ItemHeight => (float)Vector.Height / Items.Count;
        private float CircleCenter => ItemHeight / 2;
        private int TextX => Vector.X1 + circleWidth + (int)Style.Padding * 2 + circleWidth;
        private int TextY(int index) { return (int)(Vector.Y1 + (ItemHeight * index) + ((ItemHeight - theme.MenuFontHandleSize) / 2)); }
        private int MaxImgWidth => ItemImgs.Max(item => GetImgWidth((int)item));
        private int MaxImgHeight => ItemImgs.Max(item => GetImgHeight((int)item));


        private int circleWidth;
        private readonly Theme theme = Theme.Instance();
        private readonly State state = State.Instance();

        public RadioButton(string key, Vector2x2 vector, List<string> items, byte layer = 0, Style style = null, int selectedIndex = 0, int circleWidth = 6, List<int?> itemImgs = null, Action<int> action = null)
        {
            Vector = vector;
            Layer = layer;
            Style = style ?? new Style(padding: 6);
            Items = items;
            ItemImgs = itemImgs ?? new List<int?>();
            SelectedIndex = selectedIndex;
            Action = action;
            this.circleWidth = circleWidth;

            ComponentManager.Instance.Add(key, this);
        }

        public static void Create(string key, Vector2x2 vector, List<string> items, byte layer = 0, Style style = null, int selectedIndex = 0, int circleWidth = 6, List<int?> itemImgs = null, Action<int> action = null)
        {
            _ = new RadioButton(key, vector, items, layer, style, selectedIndex, circleWidth, itemImgs, action);
        }

        public void Draw()
        {
            DX.DrawBox(Vector.X1, Vector.Y1, Vector.X2, Vector.Y2, theme.MainColor, DX.TRUE);
            DX.DrawBox(Vector.X1, Vector.Y1, Vector.X2, Vector.Y2, theme.BorderColor, DX.FALSE);

            if (ItemImgs.Count == 0)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    DrawRadioButton(Vector.X1 + circleWidth + (int)Style.Padding, Vector.Y1 + (int)(ItemHeight * i) + (int)CircleCenter, i == SelectedIndex);
                    DX.DrawStringToHandle(TextX, TextY(i), Items[i], theme.FontColor, theme.MenuFontHandle);
                }
            }
            else
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (ItemImgs[i] != null)
                    {
                        DX.DrawExtendGraph(Vector.X1 + (int)Style.Padding,
                            Vector.Y1 + (int)(ItemHeight * i) + (int)((ItemHeight - MaxImgHeight) / 2),
                            Vector.X1 + (int)Style.Padding + MaxImgWidth,
                            Vector.Y1 + (int)(ItemHeight * (i + 1)) - (int)((ItemHeight - MaxImgHeight) / 2),
                            (int)ItemImgs[i],
                            DX.TRUE);
                    }
                    DX.DrawStringToHandle(
                        Vector.X1 + (int)Style.Padding * 2 + MaxImgWidth,
                        TextY(i),
                        Items[i],
                        theme.FontColor,
                        theme.MenuFontHandle);
                    if (i == SelectedIndex)
                    {
                        DX.DrawBox(Vector.X1 + 1, Vector.Y1 + (int)(ItemHeight * i) + 1, Vector.X2 - 1, Vector.Y1 + (int)(ItemHeight * (i + 1)) - 1, theme.AccentColor, DX.FALSE);
                    }
                }
            }
        }

        public void Update()
        {
            if (state.JudgeInAreaStartToEnd(Vector) && state.IsClicked && State.Instance().SelectedLayer <= Layer)
            {
                SelectedIndex = (int)((state.MouseY - Vector.Y1) / ItemHeight);
                Action(SelectedIndex);
            }
        }

        private void DrawRadioButton(int x, int y, bool isSelectedIndex)
        {
            DX.DrawCircle(x, y, circleWidth, theme.BorderColor);
            if (isSelectedIndex) { DX.DrawCircle(x, y, circleWidth / 2, theme.MainColorDark); }
        }

        private int GetImgWidth(int imgHandle)
        {
            DX.GetGraphSize(imgHandle, out int width, out _);
            return width;
        }

        private int GetImgHeight(int imgHandle)
        {
            DX.GetGraphSize(imgHandle, out _, out int height);
            return height;
        }
    }
}
