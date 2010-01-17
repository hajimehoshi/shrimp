using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Shrimp.IViews;
using Shrimp.Models;

namespace Shrimp.Presenters
{
    internal class TileSetPalettePresenter
    {
        private ITileSetPalette TileSetPalette;
        private ViewModel ViewModel;
        private Timer AutoScrollingTimer = new Timer();
        private bool IsSelectingTiles = false;
        private int SelectedTileStartX;
        private int SelectedTileStartY;

        private enum AutoScrollingState { Up, Down, }

        public TileSetPalettePresenter(ITileSetPalette tileSetPalette, ViewModel viewModel)
        {
            this.TileSetPalette = tileSetPalette;
            this.TileSetPalette.MouseDown += (sender, e) =>
            {
                if (this.TileSet == null)
                {
                    return;
                }
                Point point = new Point
                {
                    X = e.X - this.TileSetPalette.AutoScrollPosition.X,
                    Y = e.Y - this.TileSetPalette.AutoScrollPosition.Y,
                };
                this.IsSelectingTiles = true;
                this.SelectedTileStartX =
                    Math.Min(Math.Max(point.X / Util.PaletteGridSize, 0), Util.PaletteHorizontalCount - 1);
                this.SelectedTileStartY =
                    Math.Min(Math.Max(point.Y / Util.PaletteGridSize, 0), this.TileSet.Height - 1);
                int tileId = this.SelectedTileStartY * Util.PaletteHorizontalCount
                    + this.SelectedTileStartX;
                switch (this.ViewModel.EditorState.TileSetMode)
                {
                case TileSetMode.Normal:
                    this.ViewModel.EditorState.SelectedTiles = SelectedTiles.Single(new Tile
                    {
                        TileSetId = (short)this.ViewModel.EditorState.SelectedTileSetId,
                        TileId = (short)tileId,
                    });
                    break;
                case TileSetMode.Passage:
                    var tilePassageTypes = this.TileSet.TilePassageTypes;
                    if ((e.Button & MouseButtons.Left) != 0)
                    {
                        switch (tilePassageTypes[tileId])
                        {
                        case TilePassageType.Passable:
                            tilePassageTypes[tileId] = TilePassageType.Impassable;
                            break;
                        case TilePassageType.Impassable:
                            tilePassageTypes[tileId] = TilePassageType.Wall;
                            break;
                        case TilePassageType.Wall:
                            tilePassageTypes[tileId] = TilePassageType.Ceil;
                            break;
                        case TilePassageType.Ceil:
                            tilePassageTypes[tileId] = TilePassageType.Passable;
                            break;
                        }
                    }
                    else if ((e.Button & MouseButtons.Right) != 0)
                    {
                        switch (tilePassageTypes[tileId])
                        {
                        case TilePassageType.Passable:
                            tilePassageTypes[tileId] = TilePassageType.Ceil;
                            break;
                        case TilePassageType.Impassable:
                            tilePassageTypes[tileId] = TilePassageType.Passable;
                            break;
                        case TilePassageType.Wall:
                            tilePassageTypes[tileId] = TilePassageType.Impassable;
                            break;
                        case TilePassageType.Ceil:
                            tilePassageTypes[tileId] = TilePassageType.Wall;
                            break;
                        }
                    }
                    break;
                }
            };
            this.TileSetPalette.MouseMove += (sender, e) =>
            {
                if (this.ViewModel.EditorState.TileSetMode != TileSetMode.Normal)
                {
                    return;
                }
                if (this.IsSelectingTiles)
                {
                    this.SetSelectedTileEnd(e.Location);
                    if (e.Y < 0)
                    {
                        this.AutoScrollingTimer.Tag = AutoScrollingState.Up;
                        if (!this.AutoScrollingTimer.Enabled)
                        {
                            this.AutoScrollingTimer.Start();
                        }
                    }
                    else if (this.TileSetPalette.ClientSize.Height <= e.Y)
                    {
                        this.AutoScrollingTimer.Tag = AutoScrollingState.Down;
                        if (!this.AutoScrollingTimer.Enabled)
                        {
                            this.AutoScrollingTimer.Start();
                        }
                    }
                    else
                    {
                        if (this.AutoScrollingTimer.Enabled)
                        {
                            this.AutoScrollingTimer.Stop();
                        }
                    }
                }
            };
            this.TileSetPalette.MouseUp += (sender, e) =>
            {
                this.AutoScrollingTimer.Stop();
                this.IsSelectingTiles = false;
            };
            this.TileSetPalette.Paint += (sender, e) =>
            {
                if (this.TileSet == null)
                {
                    return;
                }
                Graphics g = e.Graphics;
                int baseX = (-this.TileSetPalette.AutoScrollPosition.X + e.ClipRectangle.X) % Util.PaletteGridSize;
                int baseY = (-this.TileSetPalette.AutoScrollPosition.Y + e.ClipRectangle.Y) % Util.PaletteGridSize;
                Brush lightBlueBrush = new SolidBrush(Color.FromArgb(0x00, 0x00, 0x80));
                Brush darkBlueBrush = new SolidBrush(Color.FromArgb(0x00, 0x00, 0x40));
                int halfGridSize = Util.PaletteGridSize >> 1;
                for (int j = 0; j <= (e.ClipRectangle.Height + baseY) / Util.PaletteGridSize; j++)
                {
                    for (int i = 0; i <= (e.ClipRectangle.Width + baseX) / Util.PaletteGridSize; i++)
                    {
                        int x = e.ClipRectangle.X - baseX + i * Util.PaletteGridSize;
                        int y = e.ClipRectangle.Y - baseY + j * Util.PaletteGridSize;
                        g.FillRectangle(lightBlueBrush, new Rectangle
                        {
                            X = x,
                            Y = y,
                            Width = halfGridSize,
                            Height = halfGridSize,
                        });
                        g.FillRectangle(darkBlueBrush, new Rectangle
                        {
                            X = x + halfGridSize,
                            Y = y,
                            Width = halfGridSize,
                            Height = halfGridSize,
                        });
                        g.FillRectangle(darkBlueBrush, new Rectangle
                        {
                            X = x,
                            Y = y + halfGridSize,
                            Width = halfGridSize,
                            Height = halfGridSize,
                        });
                        g.FillRectangle(lightBlueBrush, new Rectangle
                        {
                            X = x + halfGridSize,
                            Y = y + halfGridSize,
                            Width = halfGridSize,
                            Height = halfGridSize,
                        });
                    }
                }
                g.DrawImage(this.LargeBitmap,
                    e.ClipRectangle.X, e.ClipRectangle.Y,
                    new Rectangle
                    {
                        X = -this.TileSetPalette.AutoScrollPosition.X + e.ClipRectangle.X,
                        Y = -this.TileSetPalette.AutoScrollPosition.Y + e.ClipRectangle.Y,
                        Width = e.ClipRectangle.Width,
                        Height = e.ClipRectangle.Height,
                    },
                    GraphicsUnit.Pixel);
                if (this.ViewModel.EditorState.TileSetMode == TileSetMode.Passage)
                {
                    g.FillRectangle(new SolidBrush(Color.FromArgb(128, 0, 0, 0)), e.ClipRectangle);
                }

                switch (this.ViewModel.EditorState.TileSetMode)
                {
                case TileSetMode.Normal:
                    SelectedTiles selectedTiles = this.ViewModel.EditorState.SelectedTiles;
                    switch (selectedTiles.SelectedTilesType)
                    {
                    case SelectedTilesType.Single:
                    case SelectedTilesType.Rectangle:
                        Tile tile = selectedTiles.Tile;
                        if (tile.TileSetId == this.ViewModel.EditorState.SelectedTileSetId)
                        {
                            int tileId = tile.TileId;
                            Util.DrawFrame(g, new Rectangle
                            {
                                X = tileId % Util.PaletteHorizontalCount * Util.PaletteGridSize
                                    + this.TileSetPalette.AutoScrollPosition.X,
                                Y = tileId / Util.PaletteHorizontalCount * Util.PaletteGridSize
                                    + this.TileSetPalette.AutoScrollPosition.Y,
                                Width = Util.PaletteGridSize * selectedTiles.Width,
                                Height = Util.PaletteGridSize * selectedTiles.Height,
                            });
                        }
                        break;
                    }
                    break;
                case TileSetMode.Passage:
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    Pen passablePen = new Pen(Color.FromArgb(192, 64, 255, 64), 3);
                    Pen impassablePen = new Pen(Color.FromArgb(192, 255, 64, 64), 3);
                    Pen wallPen = new Pen(Color.FromArgb(192, 128, 128, 255), 3);
                    Pen ceilPen = new Pen(Color.FromArgb(192, 255, 255, 128), 3);
                    var tilePassageTypes = this.TileSet.TilePassageTypes;
                    for (int i = 0; i < tilePassageTypes.Length; i++)
                    {
                        Rectangle tileRect = new Rectangle
                        {
                            X = i % Util.PaletteHorizontalCount * Util.PaletteGridSize
                                + this.TileSetPalette.AutoScrollPosition.X,
                            Y = i / Util.PaletteHorizontalCount * Util.PaletteGridSize
                                + this.TileSetPalette.AutoScrollPosition.Y,
                            Width = Util.PaletteGridSize,
                            Height = Util.PaletteGridSize,
                        };
                        if (!tileRect.IntersectsWith(e.ClipRectangle))
                        {
                            continue;
                        }
                        switch (tilePassageTypes[i])
                        {
                        case TilePassageType.Passable:
                            g.DrawEllipse(passablePen,
                                tileRect.X + 10, tileRect.Y + 10, tileRect.Width - 20, tileRect.Height - 20);
                            break;
                        case TilePassageType.Impassable:
                            Point point11 = new Point(tileRect.X + 10, tileRect.Y + 10);
                            Point point22 = new Point
                            {
                                X = point11.X + tileRect.Width - 20,
                                Y = point11.Y + tileRect.Height - 20,
                            };
                            Point point12 = new Point(point11.X, point22.Y);
                            Point point21 = new Point(point22.X, point11.Y);
                            g.DrawLine(impassablePen, point11, point22);
                            g.DrawLine(impassablePen, point12, point21);
                            break;
                        case TilePassageType.Wall:
                            g.DrawRectangle(wallPen, new Rectangle(
                                tileRect.X + 10, tileRect.Y + 10, tileRect.Width - 20, tileRect.Height - 20));
                            break;
                        case TilePassageType.Ceil:
                            g.DrawLines(ceilPen, new[] {
                            new Point(tileRect.X + tileRect.Width / 2,  tileRect.Y + 10),
                            new Point(tileRect.X + 10,                  tileRect.Y + tileRect.Height - 10),
                            new Point(tileRect.X + tileRect.Width - 10, tileRect.Y + tileRect.Height - 10),
                            new Point(tileRect.X + tileRect.Width / 2,  tileRect.Y + 10),
                        });
                            break;
                        }
                    }
                    break;
                }
            };

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

            this.AutoScrollingTimer.Interval = 20;
            this.AutoScrollingTimer.Tick += delegate
            {
                Debug.Assert(this.IsSelectingTiles);
                int dy = 0;
                switch ((AutoScrollingState)this.AutoScrollingTimer.Tag)
                {
                case AutoScrollingState.Up:
                    dy = -8;
                    break;
                case AutoScrollingState.Down:
                    dy = +8;
                    break;
                }
                this.TileSetPalette.AutoScrollPosition = new Point
                {
                    X = -this.TileSetPalette.AutoScrollPosition.X,
                    Y = -this.TileSetPalette.AutoScrollPosition.Y + dy,
                };
                this.SetSelectedTileEnd(this.TileSetPalette.MousePosition);
            };
        }

        public TileSet TileSet
        {
            get { return tileSet; }
            set
            {
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

        private void SetSelectedTileEnd(Point mousePoint)
        {
            Debug.Assert(this.IsSelectingTiles);
            Debug.Assert(this.TileSet != null);
            Point point = new Point
            {
                X = mousePoint.X - this.TileSetPalette.AutoScrollPosition.X,
                Y = mousePoint.Y - this.TileSetPalette.AutoScrollPosition.Y,
            };
            int selectedTileEndX =
                Math.Min(Math.Max(point.X / Util.PaletteGridSize, 0), Util.PaletteHorizontalCount - 1);
            int selectedTileEndY =
                Math.Min(Math.Max(point.Y / Util.PaletteGridSize, 0), this.TileSet.Height - 1);
            int x = Math.Min(this.SelectedTileStartX, selectedTileEndX);
            int y = Math.Min(this.SelectedTileStartY, selectedTileEndY);
            int tileId = y * Util.PaletteHorizontalCount + x;
            int width = Math.Abs(this.SelectedTileStartX - selectedTileEndX) + 1;
            int height = Math.Abs(this.SelectedTileStartY - selectedTileEndY) + 1;
            this.ViewModel.EditorState.SelectedTiles =
                SelectedTiles.Rectangle(new Tile
                {
                    TileSetId = (short)this.ViewModel.EditorState.SelectedTileSetId,
                    TileId = (short)tileId
                }, width, height);
        }
    }
}
