using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                    this.TileSet = this.ViewModel.EditorState.SelectedTileSet;
                }
                else
                {
                    this.TileSet = null;
                }
            };

            this.ViewModel.EditorState.Updated += (sender, e) =>
            {
                EditorState editorState = this.ViewModel.EditorState;
                if (e.Property == editorState.GetProperty(_ => _.MapId) ||
                    e.Property == editorState.GetProperty(_ => _.SelectedTileSetIds))
                {
                    this.TileSet = editorState.SelectedTileSet;
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

        public TileSet TileSet
        {
            get { return tileSet; }
            set
            {
                this.TileSetPalette.TileSet = value;
                if (this.tileSet != value)
                {
                    if (this.tileSet != null)
                    {
                        this.tileSet.Updated -= this.TileSet_Updated;
                    }
                    this.tileSet = value;
                    if (this.tileSet != null)
                    {
                        this.tileSet.Updated += this.TileSet_Updated;
                    }
                    this.AdjustSize();
                    this.TileSetPalette.Invalidate();
                }
            }
        }
        private TileSet tileSet;

        private void TileSet_Updated(object sender, UpdatedEventArgs e)
        {
            this.AdjustSize();
            if (e.Property == this.TileSet.GetProperty(_ => _.TilePassageTypes))
            {
                UpdatedEventArgs e2 = e.InnerUpdatedEventArgs;
                if (e2 != null && e2.Property != null)
                {
                    Match match = Regex.Match(e2.Property, @"^Index(\d+)$");
                    if (match.Success)
                    {
                        int tileId = int.Parse(match.Groups[1].Value);
                        this.TileSetPalette.Invalidate(new Rectangle
                        {
                            X = this.TileSetPalette.AutoScrollPosition.X + (tileId % Util.PaletteHorizontalCount) * Util.PaletteGridSize,
                            Y = this.TileSetPalette.AutoScrollPosition.Y + (tileId / Util.PaletteHorizontalCount) * Util.PaletteGridSize,
                            Width = Util.PaletteGridSize,
                            Height = Util.PaletteGridSize,
                        });
                    }
                }
                else
                {
                    this.TileSetPalette.Invalidate();
                }
            }
            else
            {
                this.TileSetPalette.Invalidate();
            }
        }

        private void AdjustSize()
        {
            if (this.TileSet != null)
            {
                this.TileSetPalette.AutoScrollMinSize = new Size
                {
                    Width = Util.PaletteGridSize * Util.PaletteHorizontalCount,
                    Height = this.LargeBitmap.Height,
                };
            }
            else
            {
                this.TileSetPalette.AutoScrollMinSize = new Size(0, 0);
            }
        }

        private Bitmap LargeBitmap
        {
            get
            {
                TileSet tileSet = this.TileSet;
                if (tileSet != null)
                {
                    return tileSet.GetBitmap(ScaleMode.Scale1);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
