using RinUI;
using System;
using System.Collections.Generic;
using System.Text;
using DxLibDLL;
using RinUI.Manager;
using System.Diagnostics;

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


        // Propatyの整理
        /// <summary>
        /// 左クリックを押して戻った時にTrueになる。
        /// </summary>
        public bool IsLeftClicked { get; private set; }
        /// <summary>
        /// 左ドラッグや左クリックを開始した時にTrueになる。
        /// </summary>
        public bool IsLeftDragedStart { get; private set; }
        /// <summary>
        /// 左ドラッグを終了したときにTrueになる。
        /// </summary>
        public bool IsLeftDragedEnd { get; private set; }
        public bool _isLeftClickedB;
        public bool _isLeftClickedA;
        /// <summary>
        /// 左ドラックをしている最中である時にTrueになる。
        /// </summary>
        public bool IsLeftDraging { get; private set; }

        private bool isCenterClickedAfter = false;
        private Vector2 leftClickedVectorStart;
        private Vector2 leftClickedVectorEnd;

        /// <summary>
        /// 右クリックを押して戻った時にTrueになる。
        /// </summary>
        public bool IsRightClicked { get; private set; }
        /// <summary>
        /// 右ドラッグや右クリックを開始した時にTrueになる。
        /// </summary>
        public bool IsRightDragedStart { get; private set; }
        /// <summary>
        /// 右ドラッグを終了したときにTrueになる。
        /// </summary>
        public bool IsRightDragedEnd { get; private set; }
        public bool _isRightClickedB;
        public bool _isRightClickedA;
        /// <summary>
        /// 右ドラックをしている最中である時にTrueになる。
        /// </summary>
        public bool IsRightDraging { get; private set; }

        private readonly long DRAG_TIME = 300;
        private readonly Stopwatch _LeftDragStopWatch = new();
        private readonly Stopwatch _RightDragStopWatch = new();
        private Dictionary<int, bool> KeyStates { get; set; } = new();
        private Dictionary<int, bool> KeyStatesLong { get; set; } = new();
        private Dictionary<int, KeyState> _keyStates = new();


        public bool GetKeyState(int key)
        {
            if (KeyStates.ContainsKey(key))
            {
                return KeyStates[key];
            }
            return false;
        }

        public bool GetKeyStatesLong(int key)
        {
            if (KeyStatesLong.ContainsKey(key))
            {
                return KeyStatesLong[key];
            }
            return false;
        }

        public void PushLeftClick()
        {
            _isLeftClickedA = (DX.GetMouseInput() & DX.MOUSE_INPUT_LEFT) != 0;

            if (!_isLeftClickedB && _isLeftClickedA)
            {
                _LeftDragStopWatch.Reset();
                _LeftDragStopWatch.Start();
                IsLeftDragedStart = true;
                leftClickedVectorStart = new Vector2(MouseX, MouseY);
            }
            else
            {
                IsLeftDragedStart = false;
            }


            // Draging
            if (_isLeftClickedB && _isLeftClickedA && _LeftDragStopWatch.ElapsedMilliseconds > DRAG_TIME)
            {
                IsLeftDraging = true;
            }
            else
            {
                IsLeftDraging = false;
            }

            // Left Click
            if (_isLeftClickedB && !_isLeftClickedA && _LeftDragStopWatch.ElapsedMilliseconds <= DRAG_TIME)
            {
                _LeftDragStopWatch.Stop();
                IsLeftClicked = true;
                leftClickedVectorEnd = new Vector2(MouseX, MouseY);
            }
            else
            {
                IsLeftClicked = false;
            }

            // Left Drag Finished Flag
            if (_isLeftClickedB && !_isLeftClickedA && _LeftDragStopWatch.ElapsedMilliseconds > DRAG_TIME)
            {
                _LeftDragStopWatch.Stop();
                IsLeftDragedEnd = true;
                leftClickedVectorEnd = new Vector2(MouseX, MouseY);
            }
            else
            {
                IsLeftDragedEnd = false;
            }

            _isLeftClickedB = _isLeftClickedA;
        }

        public void PushRightClick()
        {
            _isRightClickedA = (DX.GetMouseInput() & DX.MOUSE_INPUT_RIGHT) != 0;

            if (!_isRightClickedB && _isRightClickedA)
            {
                _RightDragStopWatch.Reset();
                _RightDragStopWatch.Start();
                IsRightDragedStart = true;
            }
            else
            {
                IsRightDragedStart = false;
            }


            // Draging
            if (_isRightClickedB && _isRightClickedA && _RightDragStopWatch.ElapsedMilliseconds > DRAG_TIME)
            {
                IsRightDraging = true;
            }
            else
            {
                IsRightDraging = false;
            }

            // Right Click
            if (_isRightClickedB && !_isRightClickedA && _RightDragStopWatch.ElapsedMilliseconds <= DRAG_TIME)
            {
                _RightDragStopWatch.Stop();
                IsRightClicked = true;
            }
            else
            {
                IsRightClicked = false;
            }

            // Right Drag Finished Flag
            if (_isRightClickedB && !_isRightClickedA && _RightDragStopWatch.ElapsedMilliseconds > DRAG_TIME)
            {
                _RightDragStopWatch.Stop();
                IsRightDragedEnd = true;
            }
            else
            {
                IsRightDragedEnd = false;
            }

            _isRightClickedB = _isRightClickedA;
        }


        private void JudgeKey(int key)
        {
            bool keyState = DX.CheckHitKey(key) == 1;
            // Key登録されていない場合_keystatesに追加
            if (_keyStates.ContainsKey(key))
            {
                _keyStates[key] = new KeyState(_keyStates[key].Before, keyState);
            }
            else
            {
                _keyStates.Add(key, new(false, false));
            }


            if (KeyStates.ContainsKey(key))
            {
                KeyStates[key] = !_keyStates[key].Before && _keyStates[key].After;
            }
            else
            {
                KeyStates.Add(key, !_keyStates[key].Before && _keyStates[key].After);
            }

            _keyStates[key] = new KeyState(_keyStates[key].After, _keyStates[key].After);
        }

        private void JudgeKeyLong(int key)
        {
            bool keyState = DX.CheckHitKey(key) == 1;
            if (_keyStates.ContainsKey(key))
            {
                _keyStates[key] = new KeyState(_keyStates[key].Before, keyState);
            }
            else
            {
                _keyStates.Add(key, new(false, false));
            }

            if (KeyStatesLong.ContainsKey(key))
            {
                KeyStatesLong[key] = _keyStates[key].Before && _keyStates[key].After;
            }
            else
            {
                KeyStatesLong.Add(key, _keyStates[key].Before && _keyStates[key].After);
            }
        }

        /*
        // Click State
        public bool _IsLeftClicked { get; private set; }
        public bool _IsLeftClickedStart { get; private set; }
        public bool _IsLeftClickedEnd { get; private set; }
        public bool _IsRightClicked { get; private set; }
        public bool _IsRightClicking => isRightClickedAfter;
        public bool _IsCenterClicked { get; private set; }

        public bool _IsDrag =>
            isClickedBefore && isClickedAfter && clickStopWatch.ElapsedMilliseconds > DRAG_TIME;
        private bool isDragB;
        private bool isDragA;
        public bool _IsDragNextFrame => isDragB && !isDragA;

        private readonly Stopwatch clickStopWatch = new();
        private bool isClickedBefore = false;
        private bool isClickedAfter = false;
        private bool isRightClickedBefore = false;
        private bool isRightClickedAfter = false;
        private bool isCenterClickedBefore = false;
        private Vector2 rightClickedVectorStart;
        private Vector2 rightClickedVectorEnd;
        private Vector2 centerClickedVectorStart;
        private Vector2 centerClickedVectorEnd;
        */
        // EnterKey State
        public bool IsPushedEnter { get; private set; }
        private bool isPushedEnterBefore = false;
        private bool isPushedEnterAfter = false;

        // Selected Layer
        public int SelectedLayer { get; set; } = 0;

        // Key Board Input State
        public bool IsInputString { get; set; } = false;

        // Ctrl + S
        public bool CtrlS => SState && DX.CheckHitKey(DX.KEY_INPUT_LCONTROL) == 1;
        private bool SState { get; set; } = false;
        private bool csB = false;
        private bool csA = false;

        // WASD
        public bool W { get; set; } = false;
        private bool wB = false;
        private bool wA = false;
        public bool A { get; set; } = false;
        private bool aB = false;
        private bool aA = false;
        public bool S { get; set; } = false;
        private bool sB = false;
        private bool sA = false;
        public bool D { get; set; } = false;
        private bool dB = false;
        private bool dA = false;

        // Tab
        public bool IsPushedTab { get; set; } = false;
        private bool isPushedTabB = false;
        private bool isPushedTabA = false;

        // Shift
        public bool PushedShift => DX.CheckHitKey(DX.KEY_INPUT_LSHIFT) == 1;

        // EditState
        public Edit EditState { get; set; } = Edit.Neutral;

        public void Update()
        {
            PushLeftClick();
            PushRightClick();
            JudgeKey(DX.KEY_INPUT_A);  // Ａキー
            JudgeKey(DX.KEY_INPUT_B);  // Ｂキー
            JudgeKey(DX.KEY_INPUT_C);  // Ｃキー
            JudgeKey(DX.KEY_INPUT_D);  // Ｄキー
            JudgeKey(DX.KEY_INPUT_E);  // Ｅキー
            JudgeKey(DX.KEY_INPUT_F);  // Ｆキー
            JudgeKey(DX.KEY_INPUT_G);  // Ｇキー
            JudgeKey(DX.KEY_INPUT_H);  // Ｈキー
            JudgeKey(DX.KEY_INPUT_I);  // Ｉキー
            JudgeKey(DX.KEY_INPUT_J);  // Ｊキー
            JudgeKey(DX.KEY_INPUT_K);  // Ｋキー
            JudgeKey(DX.KEY_INPUT_L);  // Ｌキー
            JudgeKey(DX.KEY_INPUT_M);  // Ｍキー
            JudgeKey(DX.KEY_INPUT_N);  // Ｎキー
            JudgeKey(DX.KEY_INPUT_O);  // Ｏキー
            JudgeKey(DX.KEY_INPUT_P);  // Ｐキー
            JudgeKey(DX.KEY_INPUT_Q);  // Ｑキー
            JudgeKey(DX.KEY_INPUT_R);  // Ｒキー
            JudgeKey(DX.KEY_INPUT_S);  // Ｓキー
            JudgeKey(DX.KEY_INPUT_T);  // Ｔキー
            JudgeKey(DX.KEY_INPUT_U);  // Ｕキー
            JudgeKey(DX.KEY_INPUT_V);  // Ｖキー
            JudgeKey(DX.KEY_INPUT_W);  // Ｗキー
            JudgeKey(DX.KEY_INPUT_X);  // Ｘキー
            JudgeKey(DX.KEY_INPUT_Y);  // Ｙキー
            JudgeKey(DX.KEY_INPUT_Z);  // Ｚキー
            JudgeKey(DX.KEY_INPUT_0);  // ０キー
            JudgeKey(DX.KEY_INPUT_1);  // １キー
            JudgeKey(DX.KEY_INPUT_2);  // ２キー
            JudgeKey(DX.KEY_INPUT_3);  // ３キー
            JudgeKey(DX.KEY_INPUT_4);  // ４キー
            JudgeKey(DX.KEY_INPUT_5);  // ５キー
            JudgeKey(DX.KEY_INPUT_6);  // ６キー
            JudgeKey(DX.KEY_INPUT_7);  // ７キー
            JudgeKey(DX.KEY_INPUT_8);  // ８キー
            JudgeKey(DX.KEY_INPUT_9);  // ９キー
            JudgeKey(DX.KEY_INPUT_SPACE);
            JudgeKey(DX.KEY_INPUT_RETURN);
            JudgeKey(DX.KEY_INPUT_TAB);
            /*
            JudgeKey(DX.KEY_INPUT_W);
            JudgeKey(DX.KEY_INPUT_A);
            JudgeKey(DX.KEY_INPUT_S);
            JudgeKey(DX.KEY_INPUT_D);
            */

            JudgeMousePoint();
            JudgeMouseWheelVol();


            /*
            JudgeClick();
            JudgeRightClick();
            JudgeCtrlS();
            JudgePushEnter();
            JudgeTab();
            JudgeKey();
            */
            RinState.Add($"LAYER: {SelectedLayer}", "SELECTEDLAYER.LOG");
            RinState.Add($"IS_DRAG: {IsLeftDraging}", "_.LOG");
        }

        /*
        public void JudgeClick()
        {
            bool isClicked;
            isDragA = _IsDrag;
            isClickedAfter = (DX.GetMouseInput() & DX.MOUSE_INPUT_LEFT) != 0;
            isClicked = isClickedBefore && !isClickedAfter;
            // クリックを始めた
            if (!isClickedBefore && isClickedAfter)
            {
                leftClickedVectorStart = new Vector2(MouseX, MouseY);
                _IsLeftClickedStart = true;
                _IsLeftClickedEnd = false;
                clickStopWatch.Start();
            }
            // クリックを終えた
            else if (isClicked)
            {
                leftClickedVectorEnd = new Vector2(MouseX, MouseY);
                clickStopWatch.Stop();
                _IsLeftClickedEnd = true;
                clickStopWatch.Reset();
                _IsLeftClickedStart = false;
            }
            else
            {
                _IsLeftClickedStart = false;
                _IsLeftClickedEnd = false;
            }
            isClickedBefore = isClickedAfter;
            // ?
            IsLeftClicked = isClicked;
            //IsLeftClicked = DRAG_TIME >= clickStopWatch.ElapsedMilliseconds;
            isDragB = _IsDrag;
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
            _IsRightClicked = isClicked;
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
            _IsCenterClicked = isClicked;
        }*/

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
                vector.X1 <= leftClickedVectorStart.X && leftClickedVectorStart.X <= vector.X2 &&
                vector.Y1 <= leftClickedVectorStart.Y && leftClickedVectorStart.Y <= vector.Y2 &&
                vector.X1 <= leftClickedVectorEnd.X && leftClickedVectorEnd.X <= vector.X2 &&
                vector.Y1 <= leftClickedVectorEnd.Y && leftClickedVectorEnd.Y <= vector.Y2
            );
        }

        public bool JudgeClickInAreaVec(Vector2x2 areaVector)
        {
            return areaVector <= leftClickedVectorEnd;
        }

        public bool JudgeInAreaStartToEnd(Vector2x2 vector)
        {
            return (
                vector.X1 <= leftClickedVectorStart.X && leftClickedVectorStart.X <= vector.X2 &&
                vector.Y1 <= leftClickedVectorStart.Y && leftClickedVectorStart.Y <= vector.Y2 &&
                vector.X1 <= leftClickedVectorEnd.X && leftClickedVectorEnd.X <= vector.X2 &&
                vector.Y1 <= leftClickedVectorEnd.Y && leftClickedVectorEnd.Y <= vector.Y2
            );
        }

        /*
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
        */
        private void JudgeCtrlS()
        {
            csA = DX.CheckHitKey(DX.KEY_INPUT_S) == 1;
            SState = csA && !csB;
            csB = csA;
        }
        /*
        private void JudgeKey()
        {
            wA = DX.CheckHitKey(DX.KEY_INPUT_W) == 1;
            W = wA && !wB;
            wB = wA;

            aA = DX.CheckHitKey(DX.KEY_INPUT_A) == 1;
            A = aA && !aB;
            aB = aA;

            sA = DX.CheckHitKey(DX.KEY_INPUT_S) == 1;
            S = sA && !sB;
            sB = sA;
            
            dA = DX.CheckHitKey(DX.KEY_INPUT_D) == 1;
            D = dA && !dB;
            dB = dA;
            

        }*/

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

    public struct KeyState
    {
        public bool Before { get; set; }
        public bool After { get; set; }
        public KeyState(bool before, bool after)
        {
            Before = before;
            After = after;
        }
    }
}
