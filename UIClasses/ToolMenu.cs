using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxLibDLL;
using RinUI;
using RinUI.Manager;

namespace RinUI.UIClasses
{
    public class ToolMenu
    {
        int _drawX;
        string _text;

        bool _onCursol = false;
        bool _isOpen = false;
        int _width;
        int _height;
        int _minMenuWidth;

        List<ToolItem> _toolItems = new List<ToolItem>();

        const int PADDING_X = 10;
        const int PADDING_Y = 2;
        const int MARGIN_X = 4;
        const int MARGIN_Y = 1;

        const int ITEM_PADDING_X = 30;
        const int ITEM_PADDING_Y = 2;
        const int ITEM_MARGIN_X = 4;
        const int ITEM_MARGIN_Y = 1;

        private readonly State state = State.Instance();
        private readonly Theme theme = Theme.Instance();

        private int ItemTopY => PADDING_Y * 2 + MARGIN_Y * 2 + _height;
        private int ItemHeight => ITEM_PADDING_Y * 2 + ITEM_MARGIN_Y * 2 + _height;
        private int ItemHeightSum => ItemHeight * _toolItems.Count;

        private int X1 => _drawX + MARGIN_X;
        private int Y1 => PADDING_Y;
        private int X2 => _drawX + _width + PADDING_X * 2 + MARGIN_X;
        private int Y2 => _height + PADDING_Y * 2 + MARGIN_Y;

        private int TextX => MARGIN_X + PADDING_X + _drawX;
        private int TextY => MARGIN_Y + PADDING_Y;

        private int ItemBoxX1 => _drawX + ITEM_MARGIN_X;
        private int ItemBoxY1(int i) => ItemTopY + ItemHeight * i;
        private int ItemBoxX2 => _drawX + (MaxWidth > _minMenuWidth ? MaxWidth + ITEM_MARGIN_X + ITEM_PADDING_X * 2 : _minMenuWidth + ITEM_MARGIN_X + ITEM_PADDING_X * 2);
        private int ItemBoxY2(int i) => ItemTopY + ItemHeight * (i + 1);

        private int ItemTextX => ITEM_MARGIN_X + ITEM_PADDING_X + _drawX;
        private int ItemTextY(int i) => ItemTopY + ItemHeight * i + ITEM_PADDING_Y;

        private int MaxWidth => _toolItems.Select((x) => x.TextWidth()).Max();

        private int SelectedIndex => state.MouseY - ItemTopY > 0 ? (int)((state.MouseY - ItemTopY) / (double)ItemHeight) : -1;

        public ToolMenu(int drawX, string text, int minMenuWidth = 0)
        {
            _drawX = drawX;
            _text = text;
            _width = DX.GetDrawStringWidthToHandle(_text, (int)DX.strlenDx(_text), theme.MenuFontHandle);
            _height = DX.GetFontSizeToHandle(theme.MenuFontHandle);
            _minMenuWidth = minMenuWidth;
        }
        public ToolItem AddToolItem(ToolItem toolItem)
        {
            _toolItems.Add(toolItem);
            return toolItem;
        }

        public void DrawTool()
        {
            if (_onCursol && !_isOpen)
            {
                DX.DrawBoxAA(X1, Y1, X2, Y2, theme.MainColorDark, DX.TRUE);
            }
            else if (_isOpen)
            {
                DX.DrawBoxAA(X1, Y1, X2, Y2, theme.MainColorDark, DX.TRUE);
                DX.DrawBox(ItemBoxX1, ItemBoxY1(0), ItemBoxX2, ItemBoxY2(_toolItems.Count - 1), theme.MainColorDark, DX.TRUE);
                int i = 0;
                if (0 <= SelectedIndex && SelectedIndex < _toolItems.Count && IsInBox())
                {
                    DX.DrawBox(ItemBoxX1, ItemBoxY1(SelectedIndex), ItemBoxX2, ItemBoxY2(SelectedIndex), theme.MainColorLight, DX.TRUE);
                }

                foreach (ToolItem toolItem in _toolItems)
                {
                    DX.DrawStringFToHandle(ItemTextX, ItemTextY(i), toolItem.Text, theme.FontColor, theme.MenuFontHandle);
                    i++;
                }

            }
            DX.DrawStringToHandle(TextX, TextY, _text, theme.FontColor, theme.MenuFontHandle);

        }

        public void Update()
        {
            IsOnCursol();
            ClickItem();
            IsClicked();
        }

        private void IsOnCursol()
        {
            _onCursol = state.MouseX >= X1 && state.MouseX <= X2 && state.MouseY >= Y1 && state.MouseY <= Y2;
        }

        private void IsClicked()
        {
            //if (state.IsLeftClicked && state.JudgeInAreaStartToEnd(new Vector2x2(X1, Y1, X2, Y2)))
            if (state.IsLeftClicked && new Vector2x2(X1, Y1, X2, Y2) >= new Vector2(state.MouseX, state.MouseY))
            {

                _isOpen = !_isOpen;
                state.SelectedLayer = _isOpen ? 100 : 0;
                Console.WriteLine($"Clicked {_text}: {_isOpen}");
            }
            if (_isOpen && state.IsLeftClicked && !(new Vector2x2(X1, Y1, X2, Y2) >= new Vector2(state.MouseX, state.MouseY)))
            {
                _isOpen = false;
                Console.WriteLine($"Closed {_text}: {_isOpen}");
                state.SelectedLayer = 0;
            }
        }

        private void ClickItem()
        {

            //if (_isOpen && state.JudgeInAreaStartToEnd(new Vector2x2(ItemBoxX1, ItemTopY, ItemBoxX2, ItemTopY + ItemHeightSum)))
            if (state.IsLeftClicked && _isOpen && new Vector2x2(ItemBoxX1, ItemTopY, ItemBoxX2, ItemTopY + ItemHeightSum) >= new Vector2(state.MouseX, state.MouseY))
            {
                if (SelectedIndex == -1) return;
                _toolItems[SelectedIndex].ClickAction();

                if (RinState.Get<bool>("DEBUG"))
                {
                    Console.WriteLine($"Click in {_text} box => {_toolItems[SelectedIndex].Text}  [{(int)((state.MouseY - ItemTopY) / (double)ItemHeight)}]");
                }

            }
        }

        private bool IsInBox()
        {
            return (
                ItemBoxX1 <= state.MouseX && state.MouseX <= ItemBoxX2 &&
                ItemBoxY1(0) <= state.MouseY && state.MouseY <= ItemBoxY2(_toolItems.Count)
            );
        }
    }
}
