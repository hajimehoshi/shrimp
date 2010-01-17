using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Shrimp.IViews;

namespace Shrimp.Views
{
    partial class TileSetPalette : UserControl, ITileSetPalette
    {
        public new Point MousePosition
        {
            get { return this.PointToClient(Control.MousePosition); }
        }

        public TileSetPalette()
        {
            this.InitializeComponent();
            this.VScroll = true;
            this.VerticalScroll.SmallChange = Shrimp.Models.Util.PaletteGridSize;
        }
    }
}
