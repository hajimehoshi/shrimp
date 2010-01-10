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

        public Rectangle GetFrameRect(EditorState editorState, Map map)
        {
            if (map != null)
            {
                Point offset = editorState.GetMapOffset(map.Id);
                int gridSize = this.GridSize;
                if (!this.IsPickingTiles)
                {
                    SelectedTiles selectedTiles = editorState.SelectedTiles;
                    int cursorHorizontalCount, cursorVerticalCount;
                    bool isEvent = editorState.LayerMode == LayerMode.Event;
                    if (!isEvent)
                    {
                        cursorHorizontalCount = this.CursorTileX + this.CursorOffsetX;
                        cursorVerticalCount = this.CursorTileY + this.CursorOffsetY;
                    }
                    else
                    {
                        cursorHorizontalCount = this.CursorTileX;
                        cursorVerticalCount = this.CursorTileY;
                    }
                    return new Rectangle
                    {
                        X = cursorHorizontalCount * gridSize + offset.X,
                        Y = cursorVerticalCount * gridSize + offset.Y,
                        Width = gridSize * (!isEvent ? selectedTiles.Width : 1),
                        Height = gridSize * (!isEvent ? selectedTiles.Height : 1),
                    };
                }
                else
                {
                    int pickedRegionX = Math.Min(this.CursorTileX, this.PickerStartX);
                    int pickedRegionY = Math.Min(this.CursorTileY, this.PickerStartY);
                    int pickedRegionWidth = Math.Abs(this.CursorTileX - this.PickerStartX) + 1;
                    int pickedRegionHeight = Math.Abs(this.CursorTileY - this.PickerStartY) + 1;
                    return new Rectangle
                    {
                        X = pickedRegionX * gridSize + offset.X,
                        Y = pickedRegionY * gridSize + offset.Y,
                        Width = gridSize * pickedRegionWidth,
                        Height = gridSize * pickedRegionHeight,
                    };
                }
            }
            else
            {
                return Rectangle.Empty;
            }
        }

        public void InvalidateScrolling(int dx, int dy)
        {
            NativeMethods.ScrollWindowEx(this.Handle, dx, dy,
                IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero,
                NativeMethods.SW_INVALIDATE);
        }

        public void RecreateOffscreen()
        {
            if (this.HOffscreen != IntPtr.Zero)
            {
                NativeMethods.DeleteDC(this.HOffscreenDC);
                NativeMethods.DeleteObject(this.HOffscreen);
                this.OffscreenPixels = IntPtr.Zero;
                this.HOffscreenDC = IntPtr.Zero;
                this.HOffscreen = IntPtr.Zero;
                this.OffscreenSize = Size.Empty;
            }
            this.OffscreenSize = new Size
            {
                Width = this.HScrollBar.Width,
                Height = this.VScrollBar.Height,
            };
            NativeMethods.BITMAPINFO bitmapInfo = new NativeMethods.BITMAPINFO
            {
                bmiHeader = new NativeMethods.BITMAPINFOHEADER
                {
                    biSize = (uint)Marshal.SizeOf(typeof(NativeMethods.BITMAPINFOHEADER)),
                    biWidth = this.OffscreenSize.Width,
                    biHeight = -this.OffscreenSize.Height,
                    biPlanes = 1,
                    biBitCount = 32,
                    biCompression = NativeMethods.BI_RGB,
                },
            };
            IntPtr hDC = IntPtr.Zero;
            try
            {
                hDC = NativeMethods.GetDC(this.Handle);
                this.HOffscreen = NativeMethods.CreateDIBSection(hDC, ref bitmapInfo,
                    NativeMethods.DIB_RGB_COLORS, out this.OffscreenPixels, IntPtr.Zero, 0);
                this.HOffscreenDC = NativeMethods.CreateCompatibleDC(hDC);
                NativeMethods.SelectObject(this.HOffscreenDC, this.HOffscreen);
            }
            finally
            {
                if (hDC != IntPtr.Zero)
                {
                    NativeMethods.ReleaseDC(this.Handle, hDC);
                    hDC = IntPtr.Zero;
                }
            }
        }

        public void RenderOffscreen(Graphics g, Rectangle rect)
        {
            IntPtr hDstDC = IntPtr.Zero;
            try
            {
                hDstDC = g.GetHdc();
                NativeMethods.BitBlt(
                    hDstDC, rect.Left, rect.Top, rect.Width, rect.Height,
                    this.HOffscreenDC, rect.Left, rect.Top,
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

        public int HScrollBarSmallChange
        {
            get { return this.HScrollBar.SmallChange; }
        }

        public int HScrollBarWidth
        {
            get { return this.HScrollBar.Width; }
        }

        public Point MousePosition
        {
            get { return this.PointToClient(Control.MousePosition); }
        }

        public int VScrollBarSmallChange
        {
            get { return this.VScrollBar.SmallChange; }
        }

        public int VScrollBarHeight
        {
            get { return this.VScrollBar.Height; }
        }

        public MapEditor(ViewModel viewModel)
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

            this.ViewModel = viewModel;
        }

        private ViewModel ViewModel;

        private EditorState EditorState
        {
            get { return this.ViewModel != null ? this.ViewModel.EditorState : null; }
        }

        public void AdjustScrollBars(EditorState editorState, Map map)
        {
            if (map != null)
            {
                Point offset = editorState.GetMapOffset(map.Id);
                int hMax = map.Width * this.GridSize - this.HScrollBar.Width;
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
                int vMax = map.Height * this.GridSize - this.VScrollBar.Height;
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

        public int CursorTileX { get; set; }
        public int CursorTileY { get; set; }
        public int CursorOffsetX { get; set; }
        public int CursorOffsetY { get; set; }
        public bool IsPickingTiles { get; set; }
        public int PickerStartX { get; set; }
        public int PickerStartY { get; set; }
        public int RenderingTileStartX { get; set; }
        public int RenderingTileStartY { get; set; }

        // TODO: Remove this later
        private int GridSize
        {
            get
            {
                switch (this.EditorState.ScaleMode)
                {
                case ScaleMode.Scale1:
                    return 32;
                case ScaleMode.Scale2:
                    return 16;
                case ScaleMode.Scale4:
                    return 8;
                case ScaleMode.Scale8:
                    return 4;
                default:
                    Debug.Fail("Invalid scale");
                    return 0;
                }
            }
        }

        public Size OffscreenSize { get; private set; }
        private IntPtr HOffscreen = IntPtr.Zero;
        private IntPtr HOffscreenDC = IntPtr.Zero;
        private unsafe IntPtr OffscreenPixels = IntPtr.Zero;

        public void UpdateOffscreen(EditorState editorState, Map map)
        {
            this.UpdateOffscreen(editorState, map, new Rectangle(new Point(0, 0), this.OffscreenSize));
        }

        public void UpdateOffscreen(EditorState editorState, Map map, Rectangle rect)
        {
            if (editorState == null || map == null)
            {
                return;
            }
            Debug.Assert(this.HOffscreenDC != IntPtr.Zero);
            Point offset = editorState.GetMapOffset(map.Id);
            int offscreenWidth = this.OffscreenSize.Width;
            int offscreenHeight = this.OffscreenSize.Height;
            Size offscreenSize = this.OffscreenSize;

            int tileGridSize = this.GridSize;
            int tileStartI = Math.Max(-offset.X / tileGridSize, 0);
            int tileEndI = Math.Min((offscreenWidth - offset.X) / tileGridSize + 1, map.Width);
            int tileStartJ = Math.Max(-offset.Y / tileGridSize, 0);
            int tileEndJ = Math.Min((offscreenHeight - offset.Y) / tileGridSize + 1, map.Height);

            using (Graphics g = Graphics.FromHdc(this.HOffscreenDC))
            {
                if (0 < offscreenHeight - map.Height * this.GridSize)
                {
                    NativeMethods.RECT win32Rect = new NativeMethods.RECT
                    {
                        Left = 0,
                        Right = offscreenWidth,
                        Top = map.Height * this.GridSize,
                        Bottom = offscreenHeight,
                    };
                    NativeMethods.FillRect(this.HOffscreenDC, ref win32Rect, (IntPtr)(NativeMethods.COLOR_BTNFACE + 1));
                }
                if (0 < offscreenWidth - map.Width * this.GridSize)
                {
                    NativeMethods.RECT win32Rect = new NativeMethods.RECT
                    {
                        Left = map.Width * this.GridSize,
                        Right = offscreenWidth,
                        Top = 0,
                        Bottom = map.Height * this.GridSize,
                    };
                    NativeMethods.FillRect(this.HOffscreenDC, ref win32Rect, (IntPtr)(NativeMethods.COLOR_BTNFACE + 1));
                }
                Dictionary<Bitmap, BitmapData> bdHash = null;
                try
                {
                    TileSetCollection tileSetCollection = this.ViewModel.TileSetCollection;
                    bdHash = new Dictionary<Bitmap, BitmapData>();
                    LayerMode layerMode = editorState.LayerMode;
                    ScaleMode scaleMode = editorState.ScaleMode;
                    int reductionRatio = 0;
                    switch (scaleMode)
                    {
                    case ScaleMode.Scale2: reductionRatio = 1; break;
                    case ScaleMode.Scale4: reductionRatio = 2; break;
                    case ScaleMode.Scale8: reductionRatio = 4; break;
                    }
                    int halfTileGridSize = tileGridSize >> 1;
                    for (int layer = -1; layer < 2; layer++)
                    {
                        byte alpha = 255;
                        if (layerMode == LayerMode.Layer1 && layer == 1)
                        {
                            alpha = 128;
                        }
                        for (int j = tileStartJ; j < tileEndJ; j++)
                        {
                            int y = j * tileGridSize + offset.Y;
                            for (int i = tileStartI; i < tileEndI; i++)
                            {
                                int x = i * tileGridSize + offset.X;
                                if (0 <= layer)
                                {
                                    Tile tile = map.GetTile(layer, i, j);
                                    if (tileSetCollection.ContainsId(tile.TileSetId))
                                    {
                                        int tileId = tile.TileId;
                                        TileSet tileSet = tileSetCollection.GetItem(tile.TileSetId);
                                        Bitmap bitmap = tileSet.GetBitmap(scaleMode);
                                        BitmapData srcBD;
                                        if (!bdHash.ContainsKey(bitmap))
                                        {
                                            srcBD = bdHash[bitmap] = bitmap.LockBits(new Rectangle
                                            {
                                                Size = bitmap.Size,
                                            }, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                                        }
                                        else
                                        {
                                            srcBD = bdHash[bitmap];
                                        }
                                        int srcX = (tileId % Util.PaletteHorizontalCount) * tileGridSize;
                                        int srcY = (tileId / Util.PaletteHorizontalCount) * tileGridSize;
                                        Util.DrawBitmap(this.OffscreenPixels, offscreenSize,
                                            x, y, tileGridSize, tileGridSize, srcBD, srcX, srcY, alpha);
                                    }
                                }
                                else
                                {
                                    if (scaleMode == ScaleMode.Scale1)
                                    {
                                        this.DrawRectOffscreen(new Rectangle
                                        {
                                            X = x,
                                            Y = y,
                                            Width = halfTileGridSize,
                                            Height = halfTileGridSize,
                                        }, 0x00, 0x00, 0x80);
                                        this.DrawRectOffscreen(new Rectangle
                                        {
                                            X = x + halfTileGridSize,
                                            Y = y,
                                            Width = halfTileGridSize,
                                            Height = halfTileGridSize,
                                        }, 0x00, 0x00, 0x40);
                                        this.DrawRectOffscreen(new Rectangle
                                        {
                                            X = x,
                                            Y = y + halfTileGridSize,
                                            Width = halfTileGridSize,
                                            Height = halfTileGridSize,
                                        }, 0x00, 0x00, 0x40);
                                        this.DrawRectOffscreen(new Rectangle
                                        {
                                            X = x + halfTileGridSize,
                                            Y = y + halfTileGridSize,
                                            Width = halfTileGridSize,
                                            Height = halfTileGridSize,
                                        }, 0x00, 0x00, 0x80);
                                    }
                                    else
                                    {
                                        byte blue = (byte)(((i / reductionRatio + j / reductionRatio) % 2) == 0 ? 0x80 : 0x40);
                                        this.DrawRectOffscreen(new Rectangle
                                        {
                                            X = x,
                                            Y = y,
                                            Width = tileGridSize,
                                            Height = tileGridSize,
                                        }, 0x00, 0x00, blue);
                                    }
                                }
                            }
                        }
                        if (layerMode == LayerMode.Event)
                        {
                            int yMin = tileStartJ * tileGridSize + offset.Y;
                            int yMax = tileEndJ * tileGridSize + offset.Y;
                            for (int i = tileStartI; i < tileEndI; i++)
                            {
                                int x1 = i * tileGridSize + offset.X;
                                int x2 = i * tileGridSize + offset.X + tileGridSize - 1;
                                this.DrawGrayLineOnOffscreen(x1, x1, yMin, yMax);
                                this.DrawGrayLineOnOffscreen(x2, x2, yMin, yMax);
                            }
                            int xMin = tileStartI * tileGridSize + offset.X;
                            int xMax = tileEndI * tileGridSize + offset.X;
                            for (int j = tileStartJ; j < tileEndJ; j++)
                            {
                                int y1 = j * tileGridSize + offset.Y;
                                int y2 = j * tileGridSize + offset.Y + tileGridSize - 1;
                                this.DrawGrayLineOnOffscreen(xMin, xMax, y1, y1);
                                this.DrawGrayLineOnOffscreen(xMin, xMax, y2, y2);
                            }
                        }
                        if (editorState.LayerMode == LayerMode.Layer2 && layer == 0)
                        {
                            this.DarkenOffscreen(new Rectangle
                            {
                                X = tileStartI * tileGridSize + offset.X,
                                Y = tileStartJ * tileGridSize + offset.Y,
                                Width = (tileEndI - tileStartI) * tileGridSize,
                                Height = (tileEndJ - tileStartJ) * tileGridSize,
                            });
                        }
                    }
                }
                finally
                {
                    if (bdHash != null)
                    {
                        foreach (var pair in bdHash)
                        {
                            pair.Key.UnlockBits(pair.Value);
                        }
                        bdHash.Clear();
                        bdHash = null;
                    }
                }
            }
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            this.OnAfterLayout(EventArgs.Empty);
        }

        private void DrawRectOffscreen(Rectangle rect, byte r, byte g, byte b)
        {
            Debug.Assert(this.OffscreenPixels != IntPtr.Zero);
            Size dstSize = this.OffscreenSize;
            rect.X = Math.Max(rect.X, 0);
            rect.Y = Math.Max(rect.Y, 0);
            if (dstSize.Width < rect.Right)
            {
                rect.Width -= rect.Right - dstSize.Width;
            }
            if (dstSize.Height < rect.Bottom)
            {
                rect.Height -= rect.Bottom - dstSize.Height;
            }
            int stride = (dstSize.Width * 4 + 3) / 4 * 4;
            int padding = stride / 4 - rect.Width;
            int startI = rect.Left;
            int startJ = rect.Top;
            int endI = rect.Right;
            int endJ = rect.Bottom;
            unsafe
            {
                int color = (0xff << 24) | (r << 16) | (g << 8) | b;
                int* dst = (int*)this.OffscreenPixels + startI + startJ * stride / 4;
                for (int j = startJ; j < endJ; j++, dst += padding)
                {
                    for (int i = startI; i < endI; i++, dst++)
                    {
                        *dst = color;
                    }
                }
            }
        }

        private void DarkenOffscreen(Rectangle rect)
        {
            Debug.Assert(this.OffscreenPixels != IntPtr.Zero);
            Size dstSize = this.OffscreenSize;
            rect.X = Math.Max(rect.X, 0);
            rect.Y = Math.Max(rect.Y, 0);
            if (dstSize.Width < rect.Right)
            {
                rect.Width -= rect.Right - dstSize.Width;
            }
            if (dstSize.Height < rect.Bottom)
            {
                rect.Height -= rect.Bottom - dstSize.Height;
            }
            int stride = (dstSize.Width * 4 + 3) / 4 * 4;
            int padding = stride - rect.Width * 4;
            int startI = rect.Left;
            int startJ = rect.Top;
            int endI = rect.Right;
            int endJ = rect.Bottom;
            unsafe
            {
                byte* dst = (byte*)this.OffscreenPixels + startI * 4 + startJ * stride;
                for (int j = startJ; j < endJ; j++, dst += padding)
                {
                    for (int i = startI; i < endI; i++, dst += 4)
                    {
                        dst[0] >>= 1;
                        dst[1] >>= 1;
                        dst[2] >>= 1;
                    }
                }
            }
        }

        private void DrawGrayLineOnOffscreen(int x1, int x2, int y1, int y2)
        {
            Debug.Assert(x1 == x2 || y1 == y2);
            Size dstSize = this.OffscreenSize;
            Debug.Assert(x1 <= x2);
            Debug.Assert(y1 <= y2);
            if (dstSize.Width <= x1 || dstSize.Height <= y1 || x2 <= 0 || y2 <= 0)
            {
                return;
            }
            x1 = Math.Min(Math.Max(x1, 0), dstSize.Width);
            x2 = Math.Min(Math.Max(x2, 0), dstSize.Width);
            y1 = Math.Min(Math.Max(y1, 0), dstSize.Height);
            y2 = Math.Min(Math.Max(y2, 0), dstSize.Height);
            Debug.Assert(x1 <= x2);
            Debug.Assert(y1 <= y2);
            int stride = (dstSize.Width * 4 + 3) / 4 * 4;
            unsafe
            {
                byte* dst = (byte*)this.OffscreenPixels + x1 * 4 + y1 * stride;
                if (x1 == x2)
                {
                    for (int j = y1; j < y2; j++, dst += stride)
                    {
                        dst[0] -= (byte)(dst[0] >> 3);
                        dst[1] -= (byte)(dst[1] >> 3);
                        dst[2] -= (byte)(dst[2] >> 3);

                    }
                }
                else
                {
                    Debug.Assert(y1 == y2);
                    if (y1 < dstSize.Height)
                    {
                        for (int i = x1; i < x2; i++, dst += 4)
                        {
                            dst[0] -= (byte)(dst[0] >> 3);
                            dst[1] -= (byte)(dst[1] >> 3);
                            dst[2] -= (byte)(dst[2] >> 3);
                        }
                    }
                }
            }
        }
    }
}
