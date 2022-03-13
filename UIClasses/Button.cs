using System;
using DxLibDLL;
using RinUI.Manager;

namespace RinUI.UIClasses
{
    public class Button : IRinui
    {
        public Vector2x2 Vector { get; private set; }
        public string Text { get; private set; }
        public byte Layer { get; private set; }
        public Style Style { get; private set; }

        public event EventHandler ClickHandler;
        private readonly State state = State.Instance();
        private readonly Theme theme = Theme.Instance();
        private bool OnCursol => Vector.X1 <= state.MouseX && state.MouseX <= Vector.X2 && Vector.Y1 <= state.MouseY && state.MouseY <= Vector.Y2;
        private int TextX
        {
            get
            {
                switch (Style.TextAlign)
                {
                    case Align.START:
                        return Vector.X1 + (Style.Margin ?? 0);
                    case Align.CENTER:
                        return Vector.X1 + (Vector.X2 - Vector.X1 - DX.GetDrawStringWidthToHandle(Text, (int)DX.strlenDx(Text), (int)Style.FontHandle)) / 2;
                    case Align.END:
                        return Vector.X1 + (Vector.X2 - Vector.X1 - DX.GetDrawStringWidthToHandle(Text, (int)DX.strlenDx(Text), (int)Style.FontHandle)) - (Style.Padding ?? 0);
                    default:
                        return 0;
                }
            }
        }

        private int TextY => Style.VerticalAlign == Align.CENTER
            ? Vector.Y1 + (Vector.Y2 - Vector.Y1 - Font.GetFontSize((int)Style.FontHandle)) / 2
            : Vector.Y1;

        public Button(string key, Vector2x2 vector, string text, byte layer = 0, Style style = null)
        {
            Vector = vector;
            Text = text;
            Layer = layer;
            Style = style ?? new Style();

            ComponentManager.Instance.Add(key, this);
        }

        public static void Create(string key, Vector2x2 vector, string text, byte layer = 0, Style style = null)
        {
            _ = new Button(key, vector, text, layer, style);
        }

        public void Draw()
        {
            if (OnCursol)
            {
                DX.DrawBox(Vector.X1, Vector.Y1, Vector.X2, Vector.Y2, theme.MainColorLight, DX.TRUE);
                DX.DrawBox(Vector.X1, Vector.Y1, Vector.X2, Vector.Y2, theme.BorderColorLight, DX.FALSE);
                DX.DrawStringToHandle(TextX, TextY, Text, theme.FontColor, theme.MenuFontHandle);
            }
            else
            {
                DX.DrawBox(Vector.X1, Vector.Y1, Vector.X2, Vector.Y2, theme.MainColor, DX.TRUE);
                DX.DrawBox(Vector.X1, Vector.Y1, Vector.X2, Vector.Y2, theme.BorderColor, DX.FALSE);
                DX.DrawStringToHandle(TextX, TextY, Text, theme.FontColor, theme.MenuFontHandle);
            }
        }

        public void Update()
        {
            if (state.JudgeInAreaStartToEnd(Vector) && state.IsClicked)
            {
                Click();
            }
        }

        public void Click()
        {
            ClickHandler?.Invoke(this, EventArgs.Empty);
        }

        public void AddEvent(EventHandler eve)
        {
            ClickHandler += eve;
        }
    }
}
