using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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

            var tempCommands = new List<ICommand>();

            this.MapEditor.MouseDown += (sender, e) =>
            {
                if (this.Map == null)
                {
                    return;
                }
                Point offset = this.ViewModel.EditorState.GetMapOffset(this.Map.Id);
                Point mousePosition = new Point
                {
                    X = e.X - offset.X,
                    Y = e.Y - offset.Y,
                };
                Rectangle oldFrameRect = this.MapEditor.FrameRect;
                this.MapEditor.CursorTileX =
                    Math.Min(Math.Max(mousePosition.X / this.GridSize, 0), this.Map.Width - 1);
                this.MapEditor.CursorTileY =
                    Math.Min(Math.Max(mousePosition.Y / this.GridSize, 0), this.Map.Height - 1);
                tempCommands.Clear();
                if (this.ViewModel.EditorState.LayerMode != LayerMode.Event)
                {
                    if ((e.Button & MouseButtons.Left) != 0)
                    {
                        int layer = 0;
                        switch (this.ViewModel.EditorState.LayerMode)
                        {
                        case LayerMode.Layer1: layer = 0; break;
                        case LayerMode.Layer2: layer = 1; break;
                        default: Debug.Fail("Invalid layer"); break;
                        }
                        int x = this.MapEditor.CursorTileX + this.MapEditor.CursorOffsetX;
                        int y = this.MapEditor.CursorTileY + this.MapEditor.CursorOffsetY;
                        this.MapEditor.RenderingTileStartX = x;
                        this.MapEditor.RenderingTileStartY = y;
                        Command command =
                            this.Map.CreateSettingTilesCommand(layer, x, y, this.ViewModel.EditorState.SelectedTiles, 0, 0);
                        command.Do();
                        tempCommands.Add(command);
                    }
                    else if ((e.Button & MouseButtons.Right) != 0)
                    {
                        this.MapEditor.CursorOffsetX = 0;
                        this.MapEditor.CursorOffsetY = 0;
                        this.MapEditor.PickerStartX = this.MapEditor.CursorTileX;
                        this.MapEditor.PickerStartY = this.MapEditor.CursorTileY;
                        this.MapEditor.IsPickingTiles = true;
                        this.MapEditor.Invalidate(oldFrameRect);
                        this.MapEditor.Update();
                    }
                }
                else
                {
                    this.MapEditor.Invalidate(oldFrameRect);
                    this.MapEditor.Invalidate(this.MapEditor.FrameRect);
                    this.MapEditor.Update();
                }
            };
            Point mapOffsetOnSavingFrameRect = Point.Empty;
            this.MapEditor.MouseMove += (sender, e) =>
            {
                if (this.Map != null)
                {
                    if (this.ViewModel.EditorState.LayerMode != LayerMode.Event)
                    {
                        Point offset = this.ViewModel.EditorState.GetMapOffset(this.Map.Id);
                        Point mousePosition = new Point
                        {
                            X = e.X - offset.X,
                            Y = e.Y - offset.Y,
                        };
                        this.MapEditor.CursorTileX =
                            Math.Min(Math.Max(mousePosition.X / this.GridSize, 0), this.Map.Width - 1);
                        this.MapEditor.CursorTileY =
                            Math.Min(Math.Max(mousePosition.Y / this.GridSize, 0), this.Map.Height - 1);
                        if (!this.MapEditor.IsPickingTiles)
                        {
                            if ((e.Button & MouseButtons.Left) != 0)
                            {
                                if (0 < tempCommands.Count)
                                {
                                    SelectedTiles selectedTiles = this.ViewModel.EditorState.SelectedTiles;
                                    int layer = 0;
                                    switch (this.ViewModel.EditorState.LayerMode)
                                    {
                                    case LayerMode.Layer1: layer = 0; break;
                                    case LayerMode.Layer2: layer = 1; break;
                                    default: Debug.Fail("Invalid layer"); break;
                                    }
                                    int x = this.MapEditor.CursorTileX + this.MapEditor.CursorOffsetX;
                                    int y = this.MapEditor.CursorTileY + this.MapEditor.CursorOffsetY;
                                    Command command = this.Map.CreateSettingTilesCommand(
                                        layer, x, y, selectedTiles,
                                        x - this.MapEditor.RenderingTileStartX, y - this.MapEditor.RenderingTileStartY);
                                    command.Do();
                                    tempCommands.Add(command);
                                }
                            }
                        }
                    }
                }
                if (this.PreviousFrameRect != this.MapEditor.FrameRect)
                {
                    this.MapEditor.Invalidate(this.PreviousFrameRect);
                    this.MapEditor.Invalidate(this.MapEditor.FrameRect);
                    this.MapEditor.Update();
                }
                this.PreviousFrameRect = this.MapEditor.FrameRect;
                if (this.Map != null)
                {
                    mapOffsetOnSavingFrameRect = this.ViewModel.EditorState.GetMapOffset(this.Map.Id);
                }
                else
                {
                    mapOffsetOnSavingFrameRect = Point.Empty;
                }
            };
            this.MapEditor.MouseUp += (sender, e) =>
            {
                if (this.Map == null)
                {
                    return;
                }
                if (this.MapEditor.IsPickingTiles)
                {
                    if ((e.Button & MouseButtons.Right) != 0)
                    {
                        int pickedRegionX = Math.Min(this.MapEditor.CursorTileX, this.MapEditor.PickerStartX);
                        int pickedRegionY = Math.Min(this.MapEditor.CursorTileY, this.MapEditor.PickerStartY);
                        int pickedRegionWidth = Math.Abs(this.MapEditor.CursorTileX - this.MapEditor.PickerStartX) + 1;
                        int pickedRegionHeight = Math.Abs(this.MapEditor.CursorTileY - this.MapEditor.PickerStartY) + 1;
                        this.MapEditor.CursorOffsetX = pickedRegionX - this.MapEditor.CursorTileX;
                        this.MapEditor.CursorOffsetY = pickedRegionY - this.MapEditor.CursorTileY;
                        int layer = 0;
                        switch (this.ViewModel.EditorState.LayerMode)
                        {
                        case LayerMode.Layer1: layer = 0; break;
                        case LayerMode.Layer2: layer = 1; break;
                        default: Debug.Fail("Invalid layer"); break;
                        }
                        Map map = this.Map;
                        List<Tile> tiles = new List<Tile>();
                        for (int j = 0; j < pickedRegionHeight; j++)
                        {
                            for (int i = 0; i < pickedRegionWidth; i++)
                            {
                                tiles.Add(map.GetTile(layer, i + pickedRegionX, j + pickedRegionY));
                            }
                        }
                        if (tiles.Count == 1)
                        {
                            this.ViewModel.EditorState.SelectedTiles = SelectedTiles.Single(tiles[0]);
                        }
                        else
                        {
                            this.ViewModel.EditorState.SelectedTiles =
                                SelectedTiles.Picker(tiles, pickedRegionWidth, pickedRegionHeight);
                        }
                    }
                    this.MapEditor.IsPickingTiles = false;
                }
                if (0 < tempCommands.Count)
                {
                    IEnumerable<ICommand> commands = tempCommands.ToArray(); // Copy
                    var command = new Command();
                    command.Doing += delegate
                    {
                        foreach (var c in commands) { c.Do(); }
                    };
                    command.Undoing += delegate
                    {
                        this.Map.Updated -= this.Map_Updated;
                        foreach (var c in commands.Reverse()) { c.Undo(); }
                        this.Map.Updated += this.Map_Updated;
                        this.MapEditor.Invalidate();
                        this.MapEditor.UpdateOffscreen();
                        this.MapEditor.Update();
                    };
                    this.ViewModel.EditorState.AddCommand(command);
                    tempCommands.Clear();
                }
            };

            this.MapEditor.MouseLeave += (sender, e) =>
            {
                if (this.Map == null)
                {
                    return;
                }
                Rectangle previousFrameRect = this.PreviousFrameRect;
                Point offset = this.ViewModel.EditorState.GetMapOffset(this.Map.Id);
                previousFrameRect.X += -mapOffsetOnSavingFrameRect.X + offset.X;
                previousFrameRect.Y += -mapOffsetOnSavingFrameRect.Y + offset.Y;
                if (previousFrameRect != this.MapEditor.FrameRect)
                {
                    this.MapEditor.Invalidate(previousFrameRect);
                }
                this.MapEditor.Invalidate(this.MapEditor.FrameRect);
                this.MapEditor.Update();
                this.PreviousFrameRect = Rectangle.Empty;
            };

            this.MapEditor.MouseWheel += (sender, e) =>
            {
                if (this.Map == null)
                {
                    return;
                }
                Point offset = this.ViewModel.EditorState.GetMapOffset(this.Map.Id);
                if ((Control.ModifierKeys & Keys.Shift) != 0)
                {
                    offset.X += (int)((e.Delta / 120.0) * this.MapEditor.HScrollBarSmallChange);
                    offset.X = Math.Max(Math.Min(0, offset.X),
                        -(this.Map.Width * this.GridSize - this.MapEditor.HScrollBarWidth));
                }
                else
                {
                    offset.Y += (int)((e.Delta / 120.0) * this.MapEditor.VScrollBarSmallChange);
                    offset.Y = Math.Max(Math.Min(0, offset.Y),
                        -(this.Map.Height * this.GridSize - this.MapEditor.VScrollBarHeight));
                }
                this.ViewModel.EditorState.SetMapOffset(this.Map.Id, offset);
            };

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
