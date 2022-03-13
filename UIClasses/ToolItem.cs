using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxLibDLL;

namespace RinUI.UIClasses
{
    public abstract class ToolItem
    {
        public abstract string Text { get; set; }
        public abstract int TextWidth();
        public abstract void DrawItem();
        public abstract void ClickAction();
    }
}
