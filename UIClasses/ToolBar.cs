using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxLibDLL;
using RinUI.Manager;

namespace RinUI.UIClasses
{
    public class ToolBar
    {
        private List<ToolMenu> toolBars;
        private int height;

        public ToolBar(List<ToolMenu> toolBars = null, int height = 20)
        {
            this.toolBars = toolBars ?? new List<ToolMenu>();
            this.height = height;
        }

        public void Add(ToolMenu toolItem)
        {
            toolBars.Add(toolItem);
        }
        public void Draw()
        {
            DX.DrawBox(0, 0, RinState.Get<int>("WINDOW_W"), height, Theme.I().MainColorLight, DX.TRUE);

            foreach (var tool in toolBars)
            {
                tool.DrawTool();
            }
        }

        public void Update()
        {
            foreach (var tool in toolBars)
            {
                tool.Update();
            }
        }
    }
}
