using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace Shrimp.Models
{
    public static class Util
    {
        public const int PaletteGridSize = 32;
        public const int PaletteHorizontalCount = 8;

        public static void CopyDirectory(string src, string dst)
        {
            if (!Directory.Exists(dst))
            {
                Directory.CreateDirectory(dst);
                File.SetAttributes(dst, File.GetAttributes(src));
            }
            foreach (string file in Directory.GetFiles(src))
            {
                string dstFile = Path.Combine(dst, Path.GetFileName(file));
                File.Copy(file, dstFile);
                File.SetAttributes(dstFile, File.GetAttributes(file));
            }
            foreach (string dir in Directory.GetDirectories(src))
            {
                CopyDirectory(dir, Path.Combine(dst, Path.GetFileName(dir)));
            }
        }

        public static int GetNewId(IEnumerable<int> ids)
        {
            int id = 1;
            if (0 < ids.Count())
            {
                int maxId = ids.Max();
                for (int i = id; i <= maxId + 1; i++)
                {
                    if (!ids.Contains(i))
                    {
                        id = i;
                        break;
                    }
                }
            }
            return id;
        }

        public static Bitmap CreateScaledBitmap(Bitmap srcBitmap, ScaleMode scaleMode)
        {
            int srcWidth = srcBitmap.Width;
            int srcHeight = srcBitmap.Height;
            Bitmap dstBitmap = null;
            BitmapData srcBD = null;
            BitmapData dstBD = null;
            try
            {
                srcBD = srcBitmap.LockBits(new Rectangle
                {
                    Width = srcWidth,
                    Height = srcHeight,
                }, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                switch (scaleMode)
                {
                case ScaleMode.Scale1:
                    {
                        int dstWidth = srcWidth * 2;
                        int dstHeight = srcHeight * 2;
                        dstBitmap = new Bitmap(dstWidth, dstHeight);
                        dstBD = dstBitmap.LockBits(new Rectangle
                        {
                            Width = dstWidth,
                            Height = dstHeight,
                        }, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                        int srcPadding = srcBD.Stride - srcWidth * 4;
                        int dstPadding = dstBD.Stride - dstWidth * 4;
                        unsafe
                        {
                            byte* src = (byte*)srcBD.Scan0;
                            byte* dst = (byte*)dstBD.Scan0;
                            int dstPadding2 = dstWidth * 4 + dstPadding * 2;
                            int dWidthX4 = dstWidth * 4;
                            for (int j = 0; j < srcHeight;
                                j++, src += srcPadding, dst += dstPadding2)
                            {
                                for (int i = 0; i < srcWidth; i++, dst += 4)
                                {
                                    for (int k = 0; k < 4; k++, src++, dst++)
                                    {
                                        dst[0] = *src;
                                        dst[4] = *src;
                                        dst[dWidthX4] = *src;
                                        dst[dWidthX4 + 4] = *src;
                                    }
                                }
                            }
                        }
                    }
                    return dstBitmap;
                case ScaleMode.Scale2:
                    {
                        int dstWidth = srcWidth;
                        int dstHeight = srcHeight;
                        dstBitmap = new Bitmap(dstWidth, dstHeight);
                        dstBD = dstBitmap.LockBits(new Rectangle
                        {
                            Width = dstWidth,
                            Height = dstHeight,
                        }, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                        int srcPadding = srcBD.Stride - srcWidth * 4;
                        int dstPadding = dstBD.Stride - dstWidth * 4;
                        unsafe
                        {
                            byte* src = (byte*)srcBD.Scan0;
                            byte* dst = (byte*)dstBD.Scan0;
                            for (int j = 0; j < dstHeight;
                                j++, src += srcPadding, dst += dstPadding)
                            {
                                for (int i = 0; i < dstWidth; i++, src += 4, dst += 4)
                                {
                                    dst[0] = src[0];
                                    dst[1] = src[1];
                                    dst[2] = src[2];
                                    dst[3] = src[3];
                                }
                            }
                        }
                    }
                    return dstBitmap;
                case ScaleMode.Scale4:
                    {
                        int dstWidth = srcWidth / 2;
                        int dstHeight = srcHeight / 2;
                        dstBitmap = new Bitmap(dstWidth, dstHeight);
                        dstBD = dstBitmap.LockBits(new Rectangle
                        {
                            Width = dstWidth,
                            Height = dstHeight,
                        }, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                        int srcPadding = (srcBD.Stride - srcWidth * 4) + srcBD.Stride;
                        int dstPadding = dstBD.Stride - dstWidth * 4;
                        unsafe
                        {
                            byte* src = (byte*)srcBD.Scan0;
                            byte* dst = (byte*)dstBD.Scan0;
                            for (int j = 0; j < dstHeight;
                                j++, src += srcPadding, dst += dstPadding)
                            {
                                for (int i = 0; i < dstWidth; i++, src += 4 * 2, dst += 4)
                                {
                                    dst[0] = src[0];
                                    dst[1] = src[1];
                                    dst[2] = src[2];
                                    dst[3] = src[3];
                                }
                            }
                        }
                    }
                    return dstBitmap;
                case ScaleMode.Scale8:
                    {
                        int dstWidth = srcWidth / 4;
                        int dstHeight = srcHeight / 4;
                        dstBitmap = new Bitmap(dstWidth, dstHeight);
                        dstBD = dstBitmap.LockBits(new Rectangle
                        {
                            Width = dstWidth,
                            Height = dstHeight,
                        }, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                        int srcPadding = (srcBD.Stride - srcWidth * 4) + srcBD.Stride * 3;
                        int dstPadding = dstBD.Stride - dstWidth * 4;
                        unsafe
                        {
                            byte* src = (byte*)srcBD.Scan0;
                            byte* dst = (byte*)dstBD.Scan0;
                            for (int j = 0; j < dstHeight;
                                j++, src += srcPadding, dst += dstPadding)
                            {
                                for (int i = 0; i < dstWidth; i++, src += 4 * 4, dst += 4)
                                {
                                    dst[0] = src[0];
                                    dst[1] = src[1];
                                    dst[2] = src[2];
                                    dst[3] = src[3];
                                }
                            }
                        }
                    }
                    return dstBitmap;
                default:
                    Debug.Fail("Invalid scale mode");
                    return null;
                }
            }
            finally
            {
                if (dstBitmap != null && dstBD != null)
                {
                    dstBitmap.UnlockBits(dstBD);
                }
                if (srcBD != null)
                {
                    srcBitmap.UnlockBits(srcBD);
                }
            }
        }

        private static bool ModifyRectangle(Size dstSize,
            ref int dstX, ref int dstY, ref int width, ref int height,
            Size srcSize, ref int srcX, ref int srcY)
        {
            if (width <= 0 || height <= 0)
            {
                return false;
            }
            if (dstX < 0)
            {
                width += dstX;
                srcX -= dstX;
                dstX = 0;
            }
            if (srcSize.Width <= srcX)
            {
                return false;
            }
            if (dstSize.Width <= dstX + width)
            {
                width -= dstX + width - dstSize.Width;
            }
            if (srcSize.Width <= srcX + width)
            {
                width -= srcX + width - srcSize.Width;
            }
            if (dstY < 0)
            {
                height += dstY;
                srcY -= dstY;
                dstY = 0;
            }
            if (srcSize.Height <= srcY)
            {
                return false;
            }
            if (dstSize.Height <= dstY + height)
            {
                height -= dstY + height - dstSize.Height;
            }
            if (srcSize.Height <= srcY + height)
            {
                height -= srcY + height - srcSize.Height;
            }
            if (width <= 0 || height <= 0 ||
                dstX + width <= 0 || dstY + height <= 0 ||
                srcX + width <= 0 || srcY + height <= 0)
            {
                return false;
            }
            Debug.Assert(0 <= dstX);
            Debug.Assert(dstX + width <= dstSize.Width);
            Debug.Assert(0 <= dstY);
            Debug.Assert(dstY + height <= dstSize.Height);
            Debug.Assert(0 <= srcX);
            Debug.Assert(srcX + width <= srcSize.Width);
            Debug.Assert(0 <= srcY);
            Debug.Assert(srcY + height <= srcSize.Height);
            return true;
        }

        public static void BltBitmap(IntPtr dstPixels, Size dstSize,
            int dstX, int dstY, int width, int height,
            BitmapData srcBD, int srcX, int srcY)
        {
            Debug.Assert(srcBD.PixelFormat == PixelFormat.Format32bppArgb);
            Size srcSize = new Size(srcBD.Width, srcBD.Height);
            if (!ModifyRectangle(dstSize, ref dstX, ref dstY, ref width, ref height, srcSize, ref srcX, ref srcY))
            {
                return;
            }
            unsafe
            {
                int dstStride = (dstSize.Width * 4 + 3) / 4 * 4;
                Debug.Assert(srcBD.Stride % 4 == 0);
                int* dst = (int*)dstPixels + dstX + (dstY * dstStride / 4);
                int* src = (int*)srcBD.Scan0 + srcX + (srcY * srcBD.Stride / 4);
                int paddingDst = dstStride / 4 - width;
                int paddingSrc = srcBD.Stride / 4 - width;
                Debug.Assert(0 <= paddingDst);
                Debug.Assert(0 <= paddingSrc);
                for (int j = 0; j < height; j++, dst += paddingDst, src += paddingSrc)
                {
                    for (int i = 0; i < width; i++, dst++, src++)
                    {
                        *dst = *src;
                    }
                }
            }
        }

        public static void DrawBitmap(IntPtr dstPixels, Size dstSize,
            int dstX, int dstY, int width, int height,
            BitmapData srcBD, int srcX, int srcY, byte alpha)
        {
            Debug.Assert(srcBD.PixelFormat == PixelFormat.Format32bppArgb);
            Size srcSize = new Size(srcBD.Width, srcBD.Height);
            if (!ModifyRectangle(dstSize, ref dstX, ref dstY, ref width, ref height, srcSize, ref srcX, ref srcY))
            {
                return;
            }
            unsafe
            {
                int dstStride = (dstSize.Width * 4 + 3) / 4 * 4;
                byte* dst = (byte*)dstPixels + (dstX * 4) + (dstY * dstStride);
                byte* src = (byte*)srcBD.Scan0 + (srcX * 4) + (srcY * srcBD.Stride);
                int paddingDst = dstStride - width * 4;
                int paddingSrc = srcBD.Stride - width * 4;
                Debug.Assert(0 <= paddingDst);
                Debug.Assert(0 <= paddingSrc);
                if (alpha == 255)
                {
                    for (int j = 0; j < height; j++, dst += paddingDst, src += paddingSrc)
                    {
                        for (int i = 0; i < width; i++, dst += 4, src += 4)
                        {
                            byte a = src[3];
                            byte dst0 = dst[0];
                            byte dst1 = dst[1];
                            byte dst2 = dst[2];
                            dst[0] = (byte)(((dst0 << 8) - dst0 + (src[0] - dst0) * a + 255) >> 8);
                            dst[1] = (byte)(((dst1 << 8) - dst1 + (src[1] - dst1) * a + 255) >> 8);
                            dst[2] = (byte)(((dst2 << 8) - dst2 + (src[2] - dst2) * a + 255) >> 8);
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < height; j++, dst += paddingDst, src += paddingSrc)
                    {
                        for (int i = 0; i < width; i++, dst += 4, src += 4)
                        {
                            byte a = (byte)((src[3] * alpha + 255) >> 8);
                            byte dst0 = dst[0];
                            byte dst1 = dst[1];
                            byte dst2 = dst[2];
                            dst[0] = (byte)(((dst0 << 8) - dst0 + (src[0] - dst0) * a + 255) >> 8);
                            dst[1] = (byte)(((dst1 << 8) - dst1 + (src[1] - dst1) * a + 255) >> 8);
                            dst[2] = (byte)(((dst2 << 8) - dst2 + (src[2] - dst2) * a + 255) >> 8);
                        }
                    }
                }
            }
        }

        public static void DrawFrame(Graphics g, Rectangle rect)
        {
            Pen framePen1 = new Pen(Color.Black, 1);
            g.DrawRectangle(framePen1, new Rectangle
            {
                X = rect.X,
                Y = rect.Y,
                Width = rect.Width - 1,
                Height = rect.Height - 1,
            });
            if (4 < rect.Width)
            {
                g.DrawRectangle(new Pen(Color.White, 2), new Rectangle
                {
                    X = rect.X + 2,
                    Y = rect.Y + 2,
                    Width = rect.Width - 4,
                    Height = rect.Height - 4,
                });
                g.DrawRectangle(framePen1, new Rectangle
                {
                    X = rect.X + 3,
                    Y = rect.Y + 3,
                    Width = rect.Width - 7,
                    Height = rect.Height - 7,
                });
            }
            else
            {
                g.DrawRectangle(new Pen(Color.White, 1), new Rectangle
                {
                    X = rect.X + 1,
                    Y = rect.Y + 1,
                    Width = rect.Width - 3,
                    Height = rect.Height - 3,
                });
            }
        }
    }
}
