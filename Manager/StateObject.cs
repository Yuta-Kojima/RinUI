using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace RinUI.Manager
{
    public class StateObject<T> : IStateObject
    {
        public T State { get; set; }

        private static int seed = 0;

        public StateObject(string key, T state)
        {
            State = state;

            // Dictionary<string, StateObject>に登録される
            StateManager.SetState(key, this);
        }

        public static string CreateInstance(T state)
        {
            string key = CreateKey();
            new StateObject<T>(key, state);
            return key;
        }

        public void ChangeState(Func<T, T> function)
        {
            State = function(State);
        }

        public Type GetValueType()
        {
            return typeof(T);
        }
        public T GetValue()
        {
            return State;
        }

        private static string CreateKey()
        {
            char[] chars = "0123456789abcdfghijklmnopqrstuvwxyzABCDFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            int count = 30;
            StringBuilder sb = new StringBuilder(count);
            Random rand = new(seed);
            for (int i = 0; i < count; i++)
            {
                int randInt = rand.Next(0, chars.Length);
                sb.Append(chars[randInt]);
            }
            seed += rand.Next(100);
            return sb.ToString();
        }
    }

    public interface IStateObject
    {
        Type GetValueType();

    }
}
