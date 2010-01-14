using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Shrimp.IViews;
using Shrimp.Models;

namespace Shrimp.Views
{
    internal partial class MapEditor : UserControl, IMapEditor
    {
        public event EventHandler AfterLayout;
        protected void OnAfterLayout(EventArgs e)
        {
            if (this.AfterLayout != null)
            {
                this.AfterLayout(this, e);
            }
        }

        public event ScrollEventHandler HScrollBarScroll;
        protected void OnHScrollBarScroll(ScrollEventArgs e)
        {
            if (this.HScrollBarScroll != null)
            {
                this.HScrollBarScroll(this, e);
            }
        }

        public event ScrollEventHandler VScrollBarScroll;
        protected void OnVScrollBarScroll(ScrollEventArgs e)
        {
            if (this.VScrollBarScroll != null)
            {
                this.VScrollBarScroll(this, e);
            }
        }

        public void InvalidateScrolling(int dx, int dy)
        {
            NativeMethods.ScrollWindowEx(this.Handle, dx, dy,
                IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero,
                NativeMethods.SW_INVALIDATE);
        }

        public IMapEditorOffscreen CreateOffscreen()
        {
            return new MapEditorOffscreen(this.Handle,
                new Size(this.HScrollBarWidth, this.VScrollBarHeight));
        }

        public void RenderOffscreen(IMapEditorOffscreen offscreen, Graphics g, Rectangle rect)
        {
            IntPtr hDstDC = IntPtr.Zero;
            try
            {
                hDstDC = g.GetHdc();
                NativeMethods.BitBlt(
                    hDstDC, rect.Left, rect.Top, rect.Width, rect.Height,
                    offscreen.DeviceContext, rect.Left, rect.Top,
                    NativeMethods.TernaryRasterOperations.SRCCOPY);
            }
            finally
            {
                if (hDstDC != IntPtr.Zero)
                {
                    g.ReleaseHdc(hDstDC);
                }
            }
        }

        public Point CurrentMousePosition
        {
            get { return this.PointToClient(Control.MousePosition); }
        }

        public int HScrollBarSmallChange
        {
            get { return this.HScrollBar.SmallChange; }
        }

        public int HScrollBarWidth
        {
            get { return this.HScrollBar.Width; }
        }

        public int VScrollBarSmallChange
        {
            get { return this.VScrollBar.SmallChange; }
        }

        public int VScrollBarHeight
        {
            get { return this.VScrollBar.Height; }
        }

        public MapEditor()
        {
            this.InitializeComponent();

            this.SuspendLayout();
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.HScrollBar.Width = this.ClientSize.Width - SystemInformation.VerticalScrollBarWidth;
            this.HScrollBar.Height = SystemInformation.HorizontalScrollBarHeight;
            this.HScrollBar.Left = 0;
            this.HScrollBar.Top = this.ClientSize.Height - SystemInformation.HorizontalScrollBarHeight;
            this.VScrollBar.Width = SystemInformation.VerticalScrollBarWidth;
            this.VScrollBar.Height = this.ClientSize.Height - SystemInformation.HorizontalScrollBarHeight;
            this.VScrollBar.Left = this.ClientSize.Width - SystemInformation.VerticalScrollBarWidth;
            this.VScrollBar.Top = 0;
            this.ResumeLayout(false);

            this.HScrollBar.Scroll += (sender, e) => { this.OnHScrollBarScroll(e); };
            this.VScrollBar.Scroll += (sender, e) => { this.OnVScrollBarScroll(e); };
        }

        public void AdjustScrollBars(EditorState editorState, Map map, int gridSize)
        {
            if (map != null)
            {
                Point offset = editorState.GetMapOffset(map.Id);
                int hMax = map.Width * gridSize - this.HScrollBar.Width;
                if (0 < hMax)
                {
                    this.HScrollBar.Enabled = true;
                    this.HScrollBar.Minimum = 0;
                    this.HScrollBar.Maximum = hMax + this.HScrollBar.Width - 1;
                    this.HScrollBar.SmallChange = 32; // TODO
                    this.HScrollBar.LargeChange = this.HScrollBar.Width;
                    Debug.Assert(this.HScrollBar.LargeChange == this.HScrollBar.Width);
                    this.HScrollBar.Value = Math.Min(Math.Max(0, -offset.X), hMax);
                }
                else
                {
                    this.HScrollBar.Enabled = false;
                    this.HScrollBar.Value = 0;
                }
                int vMax = map.Height * gridSize - this.VScrollBar.Height;
                if (0 < vMax)
                {
                    this.VScrollBar.Enabled = true;
                    this.VScrollBar.Minimum = 0;
                    this.VScrollBar.Maximum = vMax + this.VScrollBar.Height - 1;
                    this.VScrollBar.SmallChange = 32; // TODO
                    this.VScrollBar.LargeChange = this.VScrollBar.Height;
                    Debug.Assert(this.VScrollBar.LargeChange == this.VScrollBar.Height);
                    this.VScrollBar.Value = Math.Min(Math.Max(0, -offset.Y), vMax);
                }
                else
                {
                    this.VScrollBar.Enabled = false;
                    this.VScrollBar.Value = 0;
                }
                editorState.SetMapOffset(map.Id, new Point
                {
                    X = -this.HScrollBar.Value,
                    Y = -this.VScrollBar.Value,
                });
            }
            else
            {
                this.HScrollBar.Enabled = false;
                this.VScrollBar.Enabled = false;
                this.HScrollBar.Value = 0;
                this.VScrollBar.Value = 0;
            }
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            this.OnAfterLayout(EventArgs.Empty);
        }
    }
}
