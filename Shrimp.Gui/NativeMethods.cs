using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Shrimp
{
    internal static class NativeMethods
    {
        public const int AC_SRC_OVER = 0x00;

        public const int AC_SRC_ALPHA = 0x01;

        public const int BI_RGB = 0x00;
        public const int DIB_RGB_COLORS = 0x00;

        public const int
            COLOR_SCROLLBAR = 0,
            COLOR_BACKGROUND = 1,
            COLOR_DESKTOP = 1,
            COLOR_ACTIVECAPTION = 2,
            COLOR_INACTIVECAPTION = 3,
            COLOR_MENU = 4,
            COLOR_WINDOW = 5,
            COLOR_WINDOWFRAME = 6,
            COLOR_MENUTEXT = 7,
            COLOR_WINDOWTEXT = 8,
            COLOR_CAPTIONTEXT = 9,
            COLOR_ACTIVEBORDER = 10,
            COLOR_INACTIVEBORDER = 11,
            COLOR_APPWORKSPACE = 12,
            COLOR_HIGHLIGHT = 13,
            COLOR_HIGHLIGHTTEXT = 14,
            COLOR_BTNFACE = 15,
            COLOR_3DFACE = 15,
            COLOR_BTNSHADOW = 16,
            COLOR_3DSHADOW = 16,
            COLOR_GRAYTEXT = 17,
            COLOR_BTNTEXT = 18,
            COLOR_INACTIVECAPTIONTEXT = 19,
            COLOR_BTNHIGHLIGHT = 20,
            COLOR_3DHIGHLIGHT = 20,
            COLOR_3DHILIGHT = 20,
            COLOR_BTNHILIGHT = 20,
            COLOR_3DDKSHADOW = 21,
            COLOR_3DLIGHT = 22,
            COLOR_INFOTEXT = 23,
            COLOR_INFOBK = 24;

        public const uint SW_SCROLLCHILDREN = 0x0001;
        public const uint SW_INVALIDATE = 0x0002;
        public const uint SW_ERASE = 0x0004;
        public const uint SW_SMOOTHSCROLL = 0x0010;

        public const int WM_PAINT = 0x000F;

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFO
        {
            public BITMAPINFOHEADER bmiHeader;
            public RGBQUAD bmiColors;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFOHEADER
        {
            public UInt32 biSize;
            public Int32 biWidth;
            public Int32 biHeight;
            public Int16 biPlanes;
            public Int16 biBitCount;
            public UInt32 biCompression;
            public UInt32 biSizeImage;
            public Int32 biXPelsPerMeter;
            public Int32 biYPelsPerMeter;
            public UInt32 biClrUsed;
            public UInt32 biClrImportant;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;

            public BLENDFUNCTION(byte op, byte flags, byte alpha, byte format)
            {
                BlendOp = op;
                BlendFlags = flags;
                SourceConstantAlpha = alpha;
                AlphaFormat = format;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PAINTSTRUCT
        {
            public IntPtr hdc;
            public bool fErase;
            public RECT rcPaint;
            public bool fRestore;
            public bool fIncUpdate;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] rgbReserved;
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public RECT(int left_, int top_, int right_, int bottom_)
            {
                Left = left_;
                Top = top_;
                Right = right_;
                Bottom = bottom_;
            }

            public int Height { get { return Bottom - Top; } }
            public int Width { get { return Right - Left; } }
            public Size Size { get { return new Size(Width, Height); } }

            public Point Location { get { return new Point(Left, Top); } }

            // Handy method for converting to a System.Drawing.Rectangle
            public Rectangle ToRectangle()
            {
                return Rectangle.FromLTRB(Left, Top, Right, Bottom);
            }

            public static RECT FromRectangle(Rectangle rectangle)
            {
                return new RECT(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
            }

            public override int GetHashCode()
            {
                return Left ^ ((Top << 13) | (Top >> 0x13))
                  ^ ((Width << 0x1a) | (Width >> 6))
                  ^ ((Height << 7) | (Height >> 0x19));
            }

            #region Operator overloads

            public static implicit operator Rectangle(RECT rect)
            {
                return rect.ToRectangle();
            }

            public static implicit operator RECT(Rectangle rect)
            {
                return FromRectangle(rect);
            }

            #endregion
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RGBQUAD
        {
            public byte rgbBlue;
            public byte rgbGreen;
            public byte rgbRed;
            public byte rgbReserved;
        }

        public enum TernaryRasterOperations : uint
        {
            SRCCOPY = 0x00CC0020, /* dest = source*/
            SRCPAINT = 0x00EE0086, /* dest = source OR dest*/
            SRCAND = 0x008800C6, /* dest = source AND dest*/
            SRCINVERT = 0x00660046, /* dest = source XOR dest*/
            SRCERASE = 0x00440328, /* dest = source AND (NOT dest )*/
            NOTSRCCOPY = 0x00330008, /* dest = (NOT source)*/
            NOTSRCERASE = 0x001100A6, /* dest = (NOT src) AND (NOT dest) */
            MERGECOPY = 0x00C000CA, /* dest = (source AND pattern)*/
            MERGEPAINT = 0x00BB0226, /* dest = (NOT source) OR dest*/
            PATCOPY = 0x00F00021, /* dest = pattern*/
            PATPAINT = 0x00FB0A09, /* dest = DPSnoo*/
            PATINVERT = 0x005A0049, /* dest = pattern XOR dest*/
            DSTINVERT = 0x00550009, /* dest = (NOT dest)*/
            BLACKNESS = 0x00000042, /* dest = BLACK*/
            WHITENESS = 0x00FF0062, /* dest = WHITE*/
        };

        [DllImport("user32.dll")]
        public static extern IntPtr BeginPaint(IntPtr hwnd, out PAINTSTRUCT lpPaint);

        [DllImport("user32.dll")]
        public static extern bool EndPaint(IntPtr hWnd, [In] ref PAINTSTRUCT lpPaint);

        [DllImport("user32.dll")]
        public static extern int FillRect(IntPtr hDC, [In] ref RECT lprc, IntPtr hbr);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern uint GetSysColor(int nIndex);

        [DllImport("user32.dll")]
        public static extern bool GetUpdateRect(IntPtr hWnd, out RECT lpRect, bool bErase);

        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        public static extern int ScrollWindowEx(IntPtr hWnd, int dx, int dy, IntPtr prcScroll,
           IntPtr prcClip, IntPtr hrgnUpdate, IntPtr prcUpdate, uint flags);

        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
            int nWidth, int nHeight, IntPtr hObjSource, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateDIBSection(IntPtr hdc, [In] ref BITMAPINFO pbmi,
           uint iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        public static extern IntPtr GetStockObject(int fnObject);

        [DllImport("gdi32.dll", ExactSpelling = true, PreserveSig = true, SetLastError = true)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        public static extern bool StretchBlt(IntPtr hdcDest, int nXOriginDest, int nYOriginDest,
            int nWidthDest, int nHeightDest,
            IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc,
            TernaryRasterOperations dwRop);
    }
}
