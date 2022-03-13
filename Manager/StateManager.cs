using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace RinUI.Manager
{
    public class StateManager
    {
        private static readonly StateManager stateManager = new();
        public static Dictionary<string, IStateObject> states = new Dictionary<string, IStateObject>();



        public static StateManager Instance()
        {
            return stateManager;
        }

        private StateManager()
        {

        }

        public static void SetState(string key, IStateObject state) => states.Add(key, state);

        public IStateObject this[string key]
        {
            get
            {
                if (states.ContainsKey(key))
                {
                    return states[key];

                }
                return null;
            }
        }

    }

    internal class StateManager<T>
    {
        public static Dictionary<string, T> StateDic { get; } = new();
    }

    public class RinState
    {
        public static Dictionary<Type, Type> TypeDic = new();
        private static int seed = 20;

        public static string Add<T>(T state, string key = null)
        {
            string objectId = key ?? CreateKey();
            Type type = typeof(T);
            if (TypeDic.ContainsKey(type))
            {
                if (StateManager<T>.StateDic.ContainsKey(objectId))
                {
                    StateManager<T>.StateDic[objectId] = state;
                }
                else
                {
                    StateManager<T>.StateDic.Add(objectId, state);
                }
            }
            else
            {
                TypeDic.Add(type, type);
                // StateManager<T>の型を生成
                Type genericStateManager = typeof(StateManager<>).MakeGenericType(type);
                // Dictionatyの型を生成
                Type genericStateDic = typeof(Dictionary<,>).MakeGenericType(typeof(string), type);
                // StateManager<T> の StateDic のインスタンスを取得
                var instance = Activator.CreateInstance(genericStateDic);
                // StateDicプロパティを取得
                Dictionary<string, T> stateDic = (Dictionary<string, T>)genericStateManager
                    .GetProperty("StateDic").GetValue(instance);
                if (stateDic.ContainsKey(objectId))
                {
                    stateDic[objectId] = state;
                }
                else
                {
                    stateDic.Add(objectId, state);
                }
            }

            //Console.WriteLine($"({typeof(T)}){objectId} : [{state?.ToString()}]");

            return objectId;
        }

        public static void Set<T>(string id, T state)
        {
            if (StateManager<T>.StateDic.ContainsKey(id))
            {
                StateManager<T>.StateDic[id] = state;
            }
        }

        public static T Get<T>(string id)
        {
            return StateManager<T>.StateDic.ContainsKey(id) ? StateManager<T>.StateDic[id] : default;
        }

        public static void Change<T>(string stateId, Func<T, T> changeFunc)
        {
            Set(stateId, changeFunc(Get<T>(stateId)));
        }

        public static List<string> Regex(string regexText)
        {
            var regex = new Regex(regexText);
            var list = new List<string>();
            foreach (var x in StateManager<string>.StateDic.Where(item => regex.IsMatch(item.Value)))
            {
                list.Add(x.Value);
            }
            return list;
        }

        public static List<string> EndsWith(string text)
        {
            var list = new List<string>();
            foreach (var x in StateManager<string>.StateDic.Where(item => item.Key.EndsWith(text)).OrderBy(text => text.Key))
            {
                list.Add(x.Value);
            }
            return list;
        }

        private static string CreateKey()
        {
            char[] chars = "0123456789abcdfghijklmnopqrstuvwxyzABCDFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            int count = 30;
            StringBuilder sb = new(count);
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
}
