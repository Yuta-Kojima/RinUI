using System;
using System.Text;
using DxLibDLL;
using RinUI.Manager;

namespace RinUI.UIClasses
{
    public unsafe class TextBox : IRinui
    {
        public Vector2x2 Vector { get; private set; }
        public StringBuilder Text { get; private set; }
        public Style Style { get; private set; }
        public byte Layer { get; private set; }
        public string StateId { get; private set; }
        public Action<string> Action { get; private set; }
        public Func<string, bool> ValidationFunc { get; private set; }
        public string NextKey { get; private set; }
        public bool IsError { get; set; }

        public int GetKey => keyData;


        private int keyData;
        private bool isInput = false;
        private ulong textLength;
        private int singleCharOnly;
        private int numCharOnly;
        private readonly State state = State.Instance();
        private readonly Theme theme = Theme.Instance();
        private string backupText = "";

        private int BoxX1 => Vector.X1;
        private int BoxY1 => Vector.Y1;
        private int BoxX2 => Style.Width == null ? Vector.X2 : Vector.X1 + (int)Style.Width;
        private int BoxY2 => Style.Height == null ? Vector.Y2 : Vector.Y1 + (int)Style.Height;
        private int TextX1 => Style.Padding == null ? Vector.X1 : Vector.X1 + (int)Style.Padding;
        private int TextY1 => Style.VerticalAlign == Align.CENTER ? Vector.Y1 + (BoxY2 - BoxY1 - Font.GetFontSize((int)Style.FontHandle)) / 2 : Vector.Y1 + 1;


        public TextBox(string key, Vector2x2 vector, string defaultText = "", byte layer = 0, int textLength = 100,
            int singleCharOnly = DX.FALSE, int numCharOnly = DX.FALSE, Style style = null, Action<string> action = null, 
            Action<string> initAction = null, Func<string, bool> func = null, string nextKey = "")
        {
            Vector = vector;
            StateId = RinState.Add(new StringBuilder(defaultText));
            Text = new StringBuilder(4096);
            Text.Append(defaultText);
            string text = RinState.Get<StringBuilder>(StateId).ToString();
            Layer = layer;
            Style = style ?? new Style(padding: 5);
            Action = action;
            ValidationFunc = func;
            NextKey = nextKey;

            this.textLength = (ulong)textLength;
            this.singleCharOnly = singleCharOnly;
            this.numCharOnly = numCharOnly;

            keyData = DX.MakeKeyInput((ulong)textLength, DX.FALSE, singleCharOnly, numCharOnly);

            ComponentManager.Instance.Add(key, this);

            initAction?.Invoke(Text.ToString());
        }

        public static void Create(string key, Vector2x2 vector, string defaultText = "", byte layer = 0, int textLength = 100, 
            int singleCharOnly = DX.FALSE, int numCharOnly = DX.FALSE, Style style = null, Action<string> action = null, 
            Action<string> initAction = null, Func<string, bool> func = null, string nextKey = "")
        {
            _ = new TextBox(key, vector, defaultText, layer, textLength, singleCharOnly, numCharOnly, style, action, initAction, func, nextKey);
        }

        public void Draw()
        {
            DX.DrawBox(BoxX1, BoxY1, BoxX2, BoxY2, theme.MainColor, DX.TRUE);
            DX.DrawBox(BoxX1, BoxY1, BoxX2, BoxY2, theme.BorderColor, DX.FALSE);


            if (isInput)
            {
                DX.DrawKeyInputString(TextX1, TextY1, keyData);
            }
            else
            {
                DX.DrawStringToHandle(TextX1, TextY1, Text.ToString(), IsError ? theme.ErrorColor : theme.FontColor, theme.MenuFontHandle);
            }
        }

        public void Update()
        {
            JudgeClickedTextBox();

        }

        private void JudgeClickedTextBox()
        {
            // 入力中
            if (state.JudgeInAreaStartToEnd(Vector) && state.IsClicked && State.Instance().SelectedLayer < Layer)
            {
                WriteText();
            }
            // 入力を終了したとき
            else if ((!state.JudgeInAreaStartToEnd(Vector) && state.IsClicked || DX.CheckKeyInput(keyData) == 1 && state.IsPushedEnter) && isInput)
            {
                SaveText();
            }
            // Tabを押したとき
            if(isInput && state.IsPushedTab)
            {
                if (ComponentManager.Instance.Rinuis.ContainsKey(NextKey))
                {
                    SaveText();
                    (ComponentManager.Instance.Rinuis[NextKey] as TextBox).WriteText();
                }

            }

        }

        public void ChangeText(string text)
        {
            Text.Clear();
            Text.Append(text);
        }

        public void WriteText()
        {
            backupText = Text.ToString();
            DX.SetActiveKeyInput(keyData);
            DX.SetKeyInputString(Text.ToString(), keyData);
            DX.SetKeyInputStringFont(theme.MenuFontHandle);
            isInput = true;
            state.EditState = Edit.Input;
            ComponentManager.Instance.AddEndAction(() => state.IsInputString = true);
        }
        public void SaveText()
        {
            isInput = false;
            ComponentManager.Instance.AddEndAction(() => state.IsInputString = false);
            ComponentManager.Instance.AddEndAction(() => state.EditState = Edit.Neutral);
            DX.GetKeyInputString(Text, keyData);
            if (ValidationFunc == null || ValidationFunc(Text.ToString()))
            {
                Action?.Invoke(Text.ToString());
            }
            else
            {
                ChangeText(backupText);
            }
        }
    }
}
