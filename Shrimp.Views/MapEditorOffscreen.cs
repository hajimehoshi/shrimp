using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Shrimp.IViews;
using Shrimp.Models;

namespace Shrimp.Views
{
    internal class MapEditorOffscreen : IMapEditorOffscreen
    {
        public Size Size { get; private set; }
        private IntPtr Handle = IntPtr.Zero;
        internal IntPtr DeviceContext { get; private set; }
        private unsafe IntPtr Pixels = IntPtr.Zero;

        public MapEditorOffscreen(IntPtr parentHandle, Size size)
        {
            this.Size = size;
            NativeMethods.BITMAPINFO bitmapInfo = new NativeMethods.BITMAPINFO
            {
                bmiHeader = new NativeMethods.BITMAPINFOHEADER
                {
                    biSize = (uint)Marshal.SizeOf(typeof(NativeMethods.BITMAPINFOHEADER)),
                    biWidth = this.Size.Width,
                    biHeight = -this.Size.Height,
                    biPlanes = 1,
                    biBitCount = 32,
                    biCompression = NativeMethods.BI_RGB,
                },
            };
            IntPtr hDC = IntPtr.Zero;
            try
            {
                this.Handle = NativeMethods.CreateDIBSection(IntPtr.Zero, ref bitmapInfo,
                    NativeMethods.DIB_RGB_COLORS, out this.Pixels, IntPtr.Zero, 0);
                this.DeviceContext = NativeMethods.CreateCompatibleDC(hDC);
                NativeMethods.SelectObject(this.DeviceContext, this.Handle);
            }
            finally
            {
                if (hDC != IntPtr.Zero)
                {
                    NativeMethods.ReleaseDC(parentHandle, hDC);
                    hDC = IntPtr.Zero;
                }
            }
        }

        public void Dispose()
        {
            if (this.Handle != IntPtr.Zero)
            {
                NativeMethods.DeleteDC(this.DeviceContext);
                NativeMethods.DeleteObject(this.Handle);
                this.Pixels = IntPtr.Zero;
                this.DeviceContext = IntPtr.Zero;
                this.Handle = IntPtr.Zero;
                this.Size = Size.Empty;
            }
        }

        public void Update(EditorState editorState, TileSetCollection tileSetCollection, Map map, int gridSize)
        {
            Rectangle rect = new Rectangle(new Point(0, 0), this.Size);
            this.Update(editorState, tileSetCollection, map, gridSize, rect);
        }

        public void Update(EditorState editorState, TileSetCollection tileSetCollection, Map map, int gridSize, Rectangle rect)
        {
            if (editorState == null || map == null)
            {
                return;
            }
            Debug.Assert(this.DeviceContext != IntPtr.Zero);
            Point offset = editorState.GetMapOffset(map.Id);
            int offscreenWidth = this.Size.Width;
            int offscreenHeight = this.Size.Height;
            Size offscreenSize = this.Size;

            int tileGridSize = gridSize;
            int tileStartI = Math.Max(-offset.X / tileGridSize, 0);
            int tileEndI = Math.Min((offscreenWidth - offset.X) / tileGridSize + 1, map.Width);
            int tileStartJ = Math.Max(-offset.Y / tileGridSize, 0);
            int tileEndJ = Math.Min((offscreenHeight - offset.Y) / tileGridSize + 1, map.Height);

            using (Graphics g = Graphics.FromHdc(this.DeviceContext))
            {
                if (0 < offscreenHeight - map.Height * gridSize)
                {
                    NativeMethods.RECT win32Rect = new NativeMethods.RECT
                    {
                        Left = 0,
                        Right = offscreenWidth,
                        Top = map.Height * gridSize,
                        Bottom = offscreenHeight,
                    };
                    NativeMethods.FillRect(this.DeviceContext, ref win32Rect, (IntPtr)(NativeMethods.COLOR_BTNFACE + 1));
                }
                if (0 < offscreenWidth - map.Width * gridSize)
                {
                    NativeMethods.RECT win32Rect = new NativeMethods.RECT
                    {
                        Left = map.Width * gridSize,
                        Right = offscreenWidth,
                        Top = 0,
                        Bottom = map.Height * gridSize,
                    };
                    NativeMethods.FillRect(this.DeviceContext, ref win32Rect, (IntPtr)(NativeMethods.COLOR_BTNFACE + 1));
                }
                Dictionary<Bitmap, BitmapData> bdHash = null;
                try
                {
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
                                        Util.DrawBitmap(this.Pixels, offscreenSize,
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



        private void DrawRectOffscreen(Rectangle rect, byte r, byte g, byte b)
        {
            Debug.Assert(this.Pixels != IntPtr.Zero);
            Size dstSize = this.Size;
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
                int* dst = (int*)this.Pixels + startI + startJ * stride / 4;
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
            Debug.Assert(this.Pixels != IntPtr.Zero);
            Size dstSize = this.Size;
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
                byte* dst = (byte*)this.Pixels + startI * 4 + startJ * stride;
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
            Size dstSize = this.Size;
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
                byte* dst = (byte*)this.Pixels + x1 * 4 + y1 * stride;
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
