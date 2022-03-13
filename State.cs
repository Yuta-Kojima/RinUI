using RinUI;
using System;
using System.Collections.Generic;
using System.Text;
using DxLibDLL;
using RinUI.Manager;

namespace RinUI
{
    public class State
    {
        private static readonly State instance = new();

        public static State Instance()
        {
            return instance;
        }

        private State()
        {

        }

        // Mouse State
        public int MouseX { get; private set; }
        public int MouseY { get; private set; }

        // Wheel State
        public int LaneWheelVol { get; private set; }

        // Click State
        public bool IsClicked { get; private set; }
        public bool IsRightClicked { get; private set; }
        public bool IsCenterClicked { get; private set; }

        private bool isClickedBefore = false;
        private bool isClickedAfter = false;
        private bool isRightClickedBefore = false;
        private bool isRightClickedAfter = false;
        private bool isCenterClickedBefore = false;
        private bool isCenterClickedAfter = false;
        private Vector2 clickedVectorStart;
        private Vector2 clickedVectorEnd;
        private Vector2 rightClickedVectorStart;
        private Vector2 rightClickedVectorEnd;
        private Vector2 centerClickedVectorStart;
        private Vector2 centerClickedVectorEnd;

        // EnterKey State
        public bool IsPushedEnter { get; private set; }
        private bool isPushedEnterBefore = false;
        private bool isPushedEnterAfter = false;

        // Selected Layer
        public byte SelectedLayer { get; set; } = 0;

        // Key Board Input State
        public bool IsInputString { get; set; } = false;

        // Ctrl + S
        public bool CtrlS => SState && DX.CheckHitKey(DX.KEY_INPUT_LCONTROL) == 1;
        private bool SState { get; set; } = false;
        private bool sB = false;
        private bool sA = false;

        // Tab
        public bool IsPushedTab { get; set; } = false;
        private bool isPushedTabB = false;
        private bool isPushedTabA = false;

        // EditState
        public Edit EditState { get; set; } = Edit.Neutral;

        public void Update()
        {
            JudgeClick();
            JudgeRightClick();
            JudgeMousePoint();
            JudgeMouseWheelVol();
            JudgePushEnter();
            JudgeCtrlS();
            JudgeTab();

            RinState.Add($"LAYER: {SelectedLayer}", "SELECTEDLAYER.LOG");
        }
        public void JudgeClick()
        {
            bool isClicked;
            isClickedAfter = (DX.GetMouseInput() & DX.MOUSE_INPUT_LEFT) != 0;
            isClicked = isClickedBefore && !isClickedAfter;
            if (!isClickedBefore && isClickedAfter)
            {
                clickedVectorStart = new Vector2(MouseX, MouseY);
            }
            else if (isClicked)
            {
                clickedVectorEnd = new Vector2(MouseX, MouseY);
            }
            isClickedBefore = isClickedAfter;
            IsClicked = isClicked;
        }

        public void JudgeRightClick()
        {
            bool isClicked;
            isRightClickedAfter = (DX.GetMouseInput() & DX.MOUSE_INPUT_RIGHT) != 0;
            isClicked = isRightClickedBefore && !isRightClickedAfter;
            if (!isRightClickedBefore && isRightClickedAfter)
            {
                rightClickedVectorStart = new Vector2(MouseX, MouseY);
            }
            else if (isClicked)
            {
                rightClickedVectorEnd = new Vector2(MouseX, MouseY);
            }
            isRightClickedBefore = isRightClickedAfter;
            IsRightClicked = isClicked;
        }

        public void JudgeCenterClick()
        {
            bool isClicked;
            isCenterClickedAfter = (DX.GetMouseInput() & 5) != 0;
            isClicked = isCenterClickedBefore && !isCenterClickedAfter;
            if (!isCenterClickedBefore && isCenterClickedAfter)
            {
                centerClickedVectorStart = new Vector2(MouseX, MouseY);
            }
            else if (isClicked)
            {
                centerClickedVectorEnd = new Vector2(MouseX, MouseY);
            }
            isCenterClickedBefore = isCenterClickedAfter;
            IsCenterClicked = isClicked;
        }

        private void JudgeMousePoint()
        {
            DX.GetMousePoint(out int x, out int y);
            MouseX = x;
            MouseY = y;
        }

        private void JudgeMouseWheelVol()
        {
            LaneWheelVol = DX.GetMouseWheelRotVol();
        }

        private void JudgePushEnter()
        {
            isPushedEnterAfter = DX.CheckHitKey(DX.KEY_INPUT_RETURN) != 0;
            IsPushedEnter = isPushedEnterBefore && !isPushedEnterAfter;
            isPushedEnterBefore = isPushedEnterAfter;
        }

        public bool JudgeInArea(Vector2x2 vector)
        {
            return (
                vector.X1 <= clickedVectorStart.X && clickedVectorStart.X <= vector.X2 &&
                vector.Y1 <= clickedVectorStart.Y && clickedVectorStart.Y <= vector.Y2 &&
                vector.X1 <= clickedVectorEnd.X && clickedVectorEnd.X <= vector.X2 &&
                vector.Y1 <= clickedVectorEnd.Y && clickedVectorEnd.Y <= vector.Y2
            );
        }

        public bool JudgeInAreaStartToEnd(Vector2x2 vector)
        {
            return (
                vector.X1 <= clickedVectorStart.X && clickedVectorStart.X <= vector.X2 &&
                vector.Y1 <= clickedVectorStart.Y && clickedVectorStart.Y <= vector.Y2 &&
                vector.X1 <= clickedVectorEnd.X && clickedVectorEnd.X <= vector.X2 &&
                vector.Y1 <= clickedVectorEnd.Y && clickedVectorEnd.Y <= vector.Y2
            );
        }

        public bool JudgeInAreaStartToEndRight(Vector2x2 vector)
        {
            return (
                vector.X1 <= rightClickedVectorStart.X && rightClickedVectorStart.X <= vector.X2 &&
                vector.Y1 <= rightClickedVectorStart.Y && rightClickedVectorStart.Y <= vector.Y2 &&
                vector.X1 <= rightClickedVectorEnd.X && rightClickedVectorEnd.X <= vector.X2 &&
                vector.Y1 <= rightClickedVectorEnd.Y && rightClickedVectorEnd.Y <= vector.Y2
            );
        }

        public bool JudgeInAreaStartToEndCenter(Vector2x2 vector)
        {
            return (
                vector.X1 <= centerClickedVectorStart.X && centerClickedVectorStart.X <= vector.X2 &&
                vector.Y1 <= centerClickedVectorStart.Y && centerClickedVectorStart.Y <= vector.Y2 &&
                vector.X1 <= centerClickedVectorEnd.X && centerClickedVectorEnd.X <= vector.X2 &&
                vector.Y1 <= centerClickedVectorEnd.Y && centerClickedVectorEnd.Y <= vector.Y2
            );
        }
        
        private void JudgeCtrlS()
        {
            sA = DX.CheckHitKey(DX.KEY_INPUT_S) == 1;
            SState = sA && !sB;
            sB = sA;
        }

        private void JudgeTab()
        {
            isPushedTabA = DX.CheckHitKey(DX.KEY_INPUT_TAB) == 1;
            IsPushedTab = isPushedTabA && !isPushedTabB;
            isPushedTabB = isPushedTabA;
        }

        public void DrawMouseVector()
        {
            var white = DX.GetColor(255, 255, 255);
            DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, 50);
            DX.DrawBox(0, 0, 130, 15, white, DX.TRUE);
            DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, 255);
            DX.DrawString(0, 0, $"X: {MouseX}  Y: {MouseY}", white);
        }
    }

    public enum Edit
    {
        Neutral = 0,
        Input
    }
}
