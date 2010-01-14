using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Shrimp.Models;

namespace Shrimp.IViews
{
    public interface IMapEditor
    {
        event EventHandler AfterLayout;
        event ScrollEventHandler HScrollBarScroll;
        event MouseEventHandler MouseDown;
        event MouseEventHandler MouseMove;
        event EventHandler MouseLeave;
        event LayoutEventHandler Layout;
        event MouseEventHandler MouseUp;
        event MouseEventHandler MouseWheel;
        event PaintEventHandler Paint;
        event ScrollEventHandler VScrollBarScroll;

        void AdjustScrollBars(EditorState editorState, Map map, int gridSize);
        IMapEditorOffscreen CreateOffscreen();
        void Invalidate();
        void Invalidate(Rectangle rect);
        void InvalidateScrolling(int dx, int dy);
        void RenderOffscreen(IMapEditorOffscreen offscreen, Graphics g, Rectangle rect);
        void Update();

        Point CurrentMousePosition { get; }
        int HScrollBarSmallChange { get; }
        int HScrollBarWidth { get; }
        int VScrollBarSmallChange { get; }
        int VScrollBarHeight { get; }
    }
}
