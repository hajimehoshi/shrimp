using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shrimp.Models;

namespace Shrimp.IViews
{
    public interface ITileSetPalette
    {
        void Invalidate();

        TileSet TileSet { get; set; }
    }
}
