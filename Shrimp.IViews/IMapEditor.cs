using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Shrimp.IViews
{
    public interface IMapEditor
    {
        event MouseEventHandler MouseDown;
        event MouseEventHandler MouseMove;
        event EventHandler MouseLeave;
        event MouseEventHandler MouseUp;
        event MouseEventHandler MouseWheel;

        void AdjustScrollBars();
        void Invalidate();
        void Invalidate(Rectangle rect);
        void InvalidateScrolling(int dx, int dy);
        void Update();
        void UpdateOffscreen();
        void UpdateOffscreen(Rectangle rect);

        Size OffscreenSize { get; }
        int CursorOffsetX { get; set; }
        int CursorOffsetY { get; set; }
    }
}
