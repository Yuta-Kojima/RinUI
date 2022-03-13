using System.Collections.Generic;
using DxLibDLL;
using RinUI.Manager;

namespace RinUI.UIClasses
{
    public class StatusBar
    {
        public int Height { get; }
        public int Margin { get; }
        public List<string> ViewText { get; private set; }

        private int firstMargin = 20;
        private readonly Theme theme = Theme.Instance();

        public StatusBar(int height = 20, int margin = 15, List<string> viewText = null)
        {
            Height = height;
            Margin = margin;
            ViewText = viewText ?? new List<string>();
        }

        public static void Create(int height = 20, int margin = 15, List<string> viewText = null)
        {
            _ = new StatusBar(height, margin, viewText);
        }

        public void Update()
        {
            ViewText = RinState.EndsWith(".LOG");
        }

        public void Draw()
        {
            DX.DrawBox(0, RinState.Get<int>("WINDOW_H") - Height, RinState.Get<int>("WINDOW_W"), RinState.Get<int>("WINDOW_H"), theme.SecondColor, DX.TRUE);
            int x = 0;
            foreach (var text in ViewText)
            {
                DX.DrawStringFToHandle(Margin + x, RinState.Get<int>("WINDOW_H") - Height + (Height - theme.MenuFontHandleSize) / 2, text, theme.FontColor, theme.MenuFontHandle);

                x += Margin + DX.GetDrawStringWidthToHandle(text, (int)DX.strlenDx(text), theme.MenuFontHandle);
            }
        }
    }
}
