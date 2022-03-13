using System;
using System.Collections.Generic;
using System.Linq;
using DxLibDLL;
using RinUI.Manager;

namespace RinUI.UIClasses
{
    public class DropDown<T> : IRinui
    {
        public Vector2x2 Vector { get; set; }
        public byte Layer { get; set; }
        public Style Style { get; set; }
        public List<DropDownItem<T>> ItemList { get; set; }
        public string ItemListId { get; }
        public int SelectedIndex { get; set; }
        public string SelectedItemId { get; private set; }
        public Action<T> Action { get; private set; }

        private int ItemHeight => Font.GetFontSize((int)Style.FontHandle) + (int)Style.Padding * 2;
        private int MaxHeight => ItemHeight * ItemList.Count;
        private int MaxWidth
        {
            get
            {
                int maxWidth = ItemList.Max(item => item.Text.Length);
                string maxWidthItem = ItemList.Where(item => item.Text.Length == maxWidth).FirstOrDefault().Text;
                return DX.GetDrawStringWidthToHandle(maxWidthItem, (int)DX.strlenDx(maxWidthItem), (int)Style.FontHandle) > Vector.X2 - Vector.X1 ?
                       DX.GetDrawStringWidthToHandle(maxWidthItem, (int)DX.strlenDx(maxWidthItem), (int)Style.FontHandle) + (int)Style.Padding :
                       Vector.X2 - Vector.X1 + (int)Style.Padding;
            }
        }
        private int TextX
        {
            get
            {
                switch (Style.TextAlign)
                {
                    case Align.START:
                        return Vector.X1 + (Style.Padding ?? 0);
                    case Align.CENTER:
                        return Vector.X1 + (Vector.X2 - Vector.X1 - DX.GetDrawStringWidthToHandle(ItemList[SelectedIndex].Text, (int)DX.strlenDx(ItemList[SelectedIndex].Text), (int)Style.FontHandle)) / 2;
                    case Align.END:
                        return Vector.X1 + (Vector.X2 - Vector.X1 - DX.GetDrawStringWidthToHandle(ItemList[SelectedIndex].Text, (int)DX.strlenDx(ItemList[SelectedIndex].Text), (int)Style.FontHandle)) - (Style.Padding ?? 0);
                    default:
                        return 0;
                }
            }
        }
        private int TextY => Style.VerticalAlign == Align.CENTER
            ? Vector.Y1 + (Vector.Y2 - Vector.Y1 - Font.GetFontSize((int)Style.FontHandle)) / 2
            : Vector.Y1;

        private readonly byte initLayer;
        private bool isOpen = false;
        private readonly State state = State.Instance();
        private readonly Vector2x2 dropDownItemVector;
        private readonly ComponentManager cm = ComponentManager.Instance;
        private readonly Theme theme = Theme.Instance();


        public DropDown(string key, Vector2x2 vector, List<DropDownItem<T>> itemList, byte layer = 0, Style style = null, int selectedIndex = 0, Action<T> action = null)
        {
            Vector = vector;
            Layer = layer;
            initLayer = layer;
            ItemList = itemList;
            ItemListId = RinState.Add(itemList);
            Style = style ?? new Style(padding: 4);
            SelectedIndex = selectedIndex;
            SelectedItemId = RinState.Add(ItemList[selectedIndex]);
            Action = action;

            ComponentManager.Instance.Add(key, this);

            dropDownItemVector = new Vector2x2(Vector.X1, Vector.Y2, Vector.X1 + MaxWidth, Vector.Y2 + MaxHeight);
        }

        public static void Create(string key, Vector2x2 vector, List<DropDownItem<T>> itemList, byte layer = 0, Style style = null, int selectedIndex = 0, Action<T> action = null)
        {
            _ = new DropDown<T>(key, vector, itemList, layer, style, selectedIndex, action);
        }

        public void Draw()
        {
            // 枠の描画
            DX.DrawBox(Vector.X1, Vector.Y1, Vector.X2, Vector.Y2, theme.MainColor, DX.TRUE);
            DX.DrawBox(Vector.X1, Vector.Y1, Vector.X2, Vector.Y2, theme.BorderColor, DX.FALSE);
            DX.DrawStringToHandle(
                TextX,
                TextY,
                ItemList[SelectedIndex].Text,
                theme.FontColor,
                Theme.Instance().MenuFontHandle);

            // ドロップダウンメニューリスト
            if (isOpen)
            {
                for (int i = 0; i < ItemList.Count; i++)
                {
                    DX.DrawBox(dropDownItemVector.X1,
                        dropDownItemVector.Y1 + ItemHeight * i,
                        dropDownItemVector.X2,
                        dropDownItemVector.Y1 + ItemHeight * (i + 1),
                        (uint)(i == (state.MouseY - dropDownItemVector.Y1) / ItemHeight ? Style.BackgroundColor + 0x222222 : Style.BackgroundColor),
                        DX.TRUE);

                    DX.DrawStringToHandle(dropDownItemVector.X1 + (int)Style.Padding,
                        dropDownItemVector.Y1 + ItemHeight * i + (int)Style.Padding,
                        ItemList[i].Text,
                        (uint)Style.FontColor,
                        Theme.Instance().MenuFontHandle);
                }
            }
        }

        public void Update()
        {
            // ドロップダウンのボタンを押したとき
            if (state.JudgeInAreaStartToEnd(Vector) && state.IsClicked && State.Instance().SelectedLayer < Layer)
            {
                Layer = 100;
                State.Instance().SelectedLayer = Layer;
                isOpen = true;
            }
            // ドロップボタンを押したとき
            else if (state.JudgeInAreaStartToEnd(dropDownItemVector) && state.IsClicked && isOpen)
            {
                SelectedIndex = (state.MouseY - dropDownItemVector.Y1) / ItemHeight;
                if (SelectedIndex > ItemList.Count - 1) return;
                RinState.Set(SelectedItemId, ItemList[SelectedIndex]);
                Action?.Invoke(ItemList[SelectedIndex].Value);
                isOpen = !isOpen;
                cm.AddEndAction(() =>
                {
                    Layer = initLayer;
                    State.Instance().SelectedLayer = 0;
                });


            }
            // ドロップボタン外を押したとき
            else if (!state.JudgeInAreaStartToEnd(dropDownItemVector) && state.IsClicked && isOpen)
            {
                isOpen = !isOpen;
                Layer = initLayer;
                State.Instance().SelectedLayer = 0;
            }
        }
    }

    public class DropDownItem<T>
    {
        public string Text { get; set; }
        public T Value { get; set; }

        public DropDownItem(string text, T value)
        {
            Text = text;
            Value = value;
        }
    }
}
