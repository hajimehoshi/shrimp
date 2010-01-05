using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using Shrimp.IViews;
using Shrimp.Models;

namespace Shrimp.Presenters
{
    internal class MapEditorPresenter
    {
        public MapEditorPresenter(IMapEditor mapEditor, ViewModel viewModel)
        {
            this.MapEditor = mapEditor;
            this.ViewModel = viewModel;

            this.ViewModel.IsOpenedChanged += delegate
            {
                this.Map = this.ViewModel.EditorState.Map;
            };
            var previousMapOffsets = new Dictionary<int, Point>();
            this.ViewModel.EditorState.Updated += (sender, e) =>
            {
                EditorState editorState = this.ViewModel.EditorState;
                if (e.Property == editorState.GetProperty(_ => _.MapId))
                {
                    this.Map = editorState.Map;
                }
                else if (e.Property == editorState.GetProperty(_ => _.LayerMode))
                {
                    this.MapEditor.Invalidate();
                    this.MapEditor.UpdateOffscreen();
                    this.MapEditor.Update();
                }
                else if (e.Property == editorState.GetProperty(_ => _.ScaleMode))
                {
                    this.MapEditor.AdjustScrollBars();
                    this.MapEditor.Invalidate();
                    this.MapEditor.UpdateOffscreen();
                    this.MapEditor.Update();
                }
                else if (e.Property == editorState.GetProperty(_ => _.MapOffsets))
                {
                    if (this.Map == null)
                    {
                        return;
                    }
                    this.MapEditor.AdjustScrollBars();
                    int mapId = this.Map.Id;
                    Point offset = editorState.GetMapOffset(mapId);
                    if (!previousMapOffsets.ContainsKey(mapId))
                    {
                        previousMapOffsets.Add(mapId, Point.Empty);
                    }
                    int dx = offset.X - previousMapOffsets[mapId].X;
                    int dy = offset.Y - previousMapOffsets[mapId].Y;
                    if (dx != 0)
                    {
                        this.MapEditor.UpdateOffscreen(new Rectangle
                        {
                            X = (0 < dx) ? this.MapEditor.OffscreenSize.Width - dx : 0,
                            Y = 0,
                            Width = Math.Abs(dx),
                            Height = this.MapEditor.OffscreenSize.Height,
                        });
                    }
                    if (dy != 0)
                    {
                        this.MapEditor.UpdateOffscreen(new Rectangle
                        {
                            X = 0,
                            Y = (0 < dy) ? this.MapEditor.OffscreenSize.Height - dy : 0,
                            Width = this.MapEditor.OffscreenSize.Width,
                            Height = Math.Abs(dy),
                        });
                    }
                    this.MapEditor.InvalidateScrolling(dx, dy);
                    this.MapEditor.Update();
                    previousMapOffsets[mapId] = offset;
                }
                else if (e.Property == editorState.GetProperty(_ => _.SelectedTiles))
                {
                    if (editorState.SelectedTiles.SelectedTilesType != SelectedTilesType.Picker)
                    {
                        this.MapEditor.CursorOffsetX = 0;
                        this.MapEditor.CursorOffsetY = 0;
                    }
                }
            };
            this.Map = this.ViewModel.EditorState.Map;
        }

        private IMapEditor MapEditor;
        private ViewModel ViewModel;

        private Rectangle PreviousFrameRect = Rectangle.Empty;

        private Map Map
        {
            get { return this.map; }
            set
            {
                if (this.map != value)
                {
                    this.PreviousFrameRect = Rectangle.Empty;
                    if (this.map != null)
                    {
                        this.map.Updated -= this.Map_Updated;
                    }
                    this.map = value;
                    if (this.map != null)
                    {
                        this.map.Updated += this.Map_Updated;
                    }
                    this.MapEditor.AdjustScrollBars();
                    this.MapEditor.Invalidate();
                    this.MapEditor.UpdateOffscreen();
                    this.MapEditor.Update();
                }
            }
        }
        private Map map;

        private void Map_Updated(object sender, UpdatedEventArgs e)
        {
            Map map = (Map)sender;
            if (e.Property == map.GetProperty(_ => _.Width) ||
                e.Property == map.GetProperty(_ => _.Height))
            {
                this.MapEditor.AdjustScrollBars();
                this.MapEditor.Invalidate();
                this.MapEditor.UpdateOffscreen();
                this.MapEditor.Update();
            }
            else if (e.Property == map.GetProperty(_ => _.Tiles))
            {
                Rectangle updatedTilesRect = (Rectangle)e.Bounds;
                Point offset = this.ViewModel.EditorState.GetMapOffset(this.Map.Id);
                Rectangle updatedRect = new Rectangle
                {
                    X = updatedTilesRect.X * this.GridSize + offset.X,
                    Y = updatedTilesRect.Y * this.GridSize + offset.Y,
                    Width = updatedTilesRect.Width * this.GridSize,
                    Height = updatedTilesRect.Height * this.GridSize,
                };
                this.MapEditor.Invalidate(updatedRect);
                this.MapEditor.UpdateOffscreen(updatedRect);
                this.MapEditor.Update();
            }
        }

        private int GridSize
        {
            get
            {
                switch (this.ViewModel.EditorState.ScaleMode)
                {
                case ScaleMode.Scale1:
                    return 32;
                case ScaleMode.Scale2:
                    return 16;
                case ScaleMode.Scale4:
                    return 8;
                case ScaleMode.Scale8:
                    return 4;
                default:
                    Debug.Fail("Invalid scale");
                    return 0;
                }
            }
        }
    }
}
