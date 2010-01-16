using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shrimp.IViews;
using Shrimp.Models;

namespace Shrimp.Presenters
{
    internal class TileSetPalettePresenter
    {
        private ITileSetPalette TileSetPalette;
        private ViewModel ViewModel;

        public TileSetPalettePresenter(ITileSetPalette tileSetPalette, ViewModel viewModel)
        {
            this.TileSetPalette = tileSetPalette;
            this.ViewModel = viewModel;
            this.ViewModel.IsOpenedChanged += (sender, e) =>
            {
                if (this.ViewModel.IsOpened)
                {
                    this.TileSetPalette.TileSet = this.ViewModel.EditorState.SelectedTileSet;
                }
                else
                {
                    this.TileSetPalette.TileSet = null;
                }
            };

            this.ViewModel.EditorState.Updated += (sender, e) =>
            {
                EditorState editorState = this.ViewModel.EditorState;
                if (e.Property == editorState.GetProperty(_ => _.MapId) ||
                    e.Property == editorState.GetProperty(_ => _.SelectedTileSetIds))
                {
                    this.TileSetPalette.TileSet = editorState.SelectedTileSet;
                }
                else if (e.Property == editorState.GetProperty(_ => _.SelectedTiles))
                {
                    this.TileSetPalette.Invalidate();
                }
                else if (e.Property == editorState.GetProperty(_ => _.TileSetMode))
                {
                    this.TileSetPalette.Invalidate();
                }
            };
        }
    }
}
