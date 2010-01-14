using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Shrimp.Models;

namespace Shrimp.IViews
{
    public interface IMapEditorOffscreen : IDisposable
    {
        void Update(EditorState editorState, TileSetCollection tileSetCollection, Map map, int gridSize);
        void Update(EditorState editorState, TileSetCollection tileSetCollection, Map map, int gridSize, Rectangle rect);

        IntPtr DeviceContext { get; }
        Size Size { get; }
    }
}
