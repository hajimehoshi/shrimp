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
        IList<ICommand> TempCommands { get; }
        int VScrollBarSmallChange { get; }
        int VScrollBarHeight { get; }
    }
}
