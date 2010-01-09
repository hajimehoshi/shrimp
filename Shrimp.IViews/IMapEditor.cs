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
        event ScrollEventHandler HScrollBarScroll;
        event MouseEventHandler MouseDown;
        event MouseEventHandler MouseMove;
        event EventHandler MouseLeave;
        event LayoutEventHandler Layout;
        event MouseEventHandler MouseUp;
        event MouseEventHandler MouseWheel;
        event ScrollEventHandler VScrollBarScroll;

        void AdjustScrollBars(EditorState editorState, Map map);
        void Invalidate();
        void Invalidate(Rectangle rect);
        void InvalidateScrolling(int dx, int dy);
        void RecreateOffscreen();
        void Update();
        void UpdateOffscreen();
        void UpdateOffscreen(Rectangle rect);

        Size OffscreenSize { get; }
        int CursorOffsetX { get; set; }
        int CursorOffsetY { get; set; }
        int CursorTileX { get; set; }
        int CursorTileY { get; set; }
        Rectangle FrameRect { get; }
        int HScrollBarSmallChange { get; }
        int HScrollBarWidth { get; }
        bool IsPickingTiles { get; set; }
        int PickerStartX { get; set; }
        int PickerStartY { get; set; }
        int RenderingTileStartX { get; set; }
        int RenderingTileStartY { get; set; }
        int VScrollBarSmallChange { get; }
        int VScrollBarHeight { get; }
    }
}
