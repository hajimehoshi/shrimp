using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Shrimp.IViews
{
    public interface IMapEditor
    {
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
