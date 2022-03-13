using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;

namespace RinUI
{
    public unsafe class ComponentManager
    {
        private static readonly ComponentManager instance = new();
        //private List<IRinui> Rinuis { get; set; } = new List<IRinui>();
        public Dictionary<string, IRinui> Rinuis { get; private set; } = new Dictionary<string, IRinui>();
        private List<Action> EndActions { get; set; } = new();

        public static ComponentManager Instance
        {
            get { return instance; }
        }

        public void Update()
        {
            foreach (IRinui ui in Rinuis.Select(item => item.Value))
            {
                ui.Update();
            }
            EndAction();
        }

        public void Draw()
        {
            foreach(IRinui ui in Rinuis.Select(item => item.Value))
            {
                ui?.Draw();
            }
        }

        public void Add(string key, IRinui rinui)
        {
            Rinuis.Add(key, rinui);
            var q = Rinuis.OrderBy(ui => ui.Value.Layer);
            Dictionary<string, IRinui> dic = new Dictionary<string, IRinui>();
            foreach(var item in q)
            {
                dic.Add(item.Key, item.Value);
            }
            Rinuis = dic;
        }

        private void EndAction()
        {
            foreach(Action action in CollectionsMarshal.AsSpan(EndActions))
            {
                action();
            }
            EndActions.Clear();
        }

        public void AddEndAction(Action action)
        {
            EndActions.Add(action);
        }
    }
}
