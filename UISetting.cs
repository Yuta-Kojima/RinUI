using System;
using System.Collections.Generic;
using DxLibDLL;
using RinUI.Manager;

namespace RinUI
{
    public static class UISetting
    {
        private static string width;
        private static string height;
        public static int WINDOW_W { get{ return RinState.Get<int>(width); } } 
        public static int WINDOW_H { get{ return RinState.Get<int>(height); } } 
        public static void Init(int windowW, int windowH, bool debug = false)
        {
            width = RinState.Add(windowW, "WINDOW_W");
            height = RinState.Add(windowH, "WINDOW_H");
        }
    }

}
