using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RinUI.Manager
{
    public class FpsTimer
    {
        public static FpsTimer Instance => instance;
        private static FpsTimer instance = new FpsTimer();
        private static long beforeTime = 0;
        private static long afterTime = 0;

        private static long sum = 0;

        private readonly Stopwatch sw;
        
        private FpsTimer()
        {
            sw = new Stopwatch();
            sw.Start();
        }
        public void UpdateForFps(Action action, int fps = 60)
        {
            afterTime = sw.ElapsedMilliseconds;
            long x = afterTime - beforeTime;
            sum += x;
            if (1000 / fps <= sum)
            {
                action();
                sum = 0;
            }
            beforeTime = afterTime;
            
        }
    }
}
