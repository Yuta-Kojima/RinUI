using DxLibDLL;
using RinUI.Manager;

namespace RinUI.UIClasses
{
    public class Box : IRinui
    {

        public Vector2x2 Vector { get; private set; }
        public byte Layer { get; private set; }
        public Style Style { get; private set; }
        public string TextId { get; private set; }
        public string Text => RinState.Get<string>(TextId);

        private int TextX
        {
            get
            {
                switch (Style.TextAlign)
                {
                    case Align.START:
                        return Vector.X1 + (Style.Padding ?? 0);
                    case Align.CENTER:
                        return Vector.X1 + (Vector.X2 - Vector.X1 - DX.GetDrawStringWidthToHandle(Text, (int)DX.strlenDx(Text), theme.MenuFontHandle)) / 2;
                    case Align.END:
                        return Vector.X1 + (Vector.X2 - Vector.X1 - DX.GetDrawStringWidthToHandle(Text, (int)DX.strlenDx(Text), theme.MenuFontHandle)) - (Style.Padding ?? 0);
                    default:
                        return 0;
                }
            }
        }

        private readonly Theme theme = Theme.Instance();

        private int TextY => Style.VerticalAlign == Align.CENTER
            ? Vector.Y1 + (Vector.Y2 - Vector.Y1 - Font.GetFontSize((int)Style.FontHandle)) / 2
            : Vector.Y1;

        public Box(string key, Vector2x2 vector, string text = null, byte layer = 0, Style style = null)
        {
            Vector = vector;
            Layer = layer;
            Style = style ?? new Style();
            TextId = RinState.Add(text);

            ComponentManager.Instance.Add(key, this);
        }

        public static void Create(string key, Vector2x2 vector, string text = null, byte layer = 0, Style style = null)
        {
            _ = new Box(key, vector, text, layer, style);
        }
        public void Draw()
        {
            DX.DrawBox(
                Vector.X1,
                Vector.Y1,
                Style.Width is null ? Vector.X2 : Vector.X1 + (int)Style.Width,
                Vector.Y2,
                theme.MainColor,
                DX.TRUE);

            DX.DrawBox(
                Vector.X1,
                Vector.Y1,
                Style.Width is null ? Vector.X2 : Vector.X1 + (int)Style.Width,
                Vector.Y2,
                theme.BorderColor,
                DX.FALSE);


            if (Text != null)
            {
                DX.DrawStringToHandle(
                    TextX,
                    TextY,
                    Text,
                    theme.FontColor,
                    theme.MenuFontHandle);
            }
        }

        public void Update()
        {

        }
    }
}
