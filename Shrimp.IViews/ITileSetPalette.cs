using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Shrimp.Models;

namespace Shrimp.IViews
{
    public interface ITileSetPalette
    {
        void Invalidate();
        void Invalidate(Rectangle rect);

        Size AutoScrollMinSize { get; set; }
        Point AutoScrollPosition { get; set; }

        // TODO: Remove it
        TileSet TileSet { get; set; }
    }
}
