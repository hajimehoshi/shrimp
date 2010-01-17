using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Shrimp.Models;

namespace Shrimp.IViews
{
    public interface ITileSetPalette
    {
        event MouseEventHandler MouseDown;
        event MouseEventHandler MouseMove;
        event MouseEventHandler MouseUp;
        event PaintEventHandler Paint;

        void Invalidate();
        void Invalidate(Rectangle rect);

        Size AutoScrollMinSize { get; set; }
        Point AutoScrollPosition { get; set; }
        Size ClientSize { get; }
        Point MousePosition { get; }
    }
}
