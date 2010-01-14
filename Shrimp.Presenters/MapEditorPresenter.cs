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
        private IMapEditor MapEditor;
        private ViewModel ViewModel;

        private IMapEditorOffscreen Offscreen;
        private Rectangle PreviousFrameRect = Rectangle.Empty;

        public MapEditorPresenter(IMapEditor mapEditor, ViewModel viewModel)
        {
            this.MapEditor = mapEditor;
            this.ViewModel = viewModel;

            this.MapEditor.AfterLayout += (sender, e) =>
            {
                this.MapEditor.AdjustScrollBars(this.ViewModel.EditorState, this.Map, this.GridSize);
                if (this.Offscreen != null)
                {
                    this.Offscreen.Dispose();
                }
                this.Offscreen = this.MapEditor.CreateOffscreen();
                this.MapEditor.Invalidate();
                this.Offscreen.Update(this.ViewModel.EditorState, this.ViewModel.TileSetCollection, this.Map, this.GridSize);
                this.MapEditor.Update();
            };

            bool isPickingTiles = false;
            Point renderingTileStart = Point.Empty;
            Point cursorTile = Point.Empty;
            Point cursorOffset = Point.Empty;
            Point pickerStart = Point.Empty;
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
                Rectangle oldFrameRect = this.GetFrameRect(isPickingTiles, cursorTile, cursorOffset, pickerStart);
                cursorTile = new Point
                {
                    X = Math.Min(Math.Max(mousePosition.X / this.GridSize, 0), this.Map.Width - 1),
                    Y = Math.Min(Math.Max(mousePosition.Y / this.GridSize, 0), this.Map.Height - 1),
                };
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
                        int x = cursorTile.X + cursorOffset.X;
                        int y = cursorTile.Y + cursorOffset.Y;
                        renderingTileStart = new Point(x, y);
                        Command command =
                            this.Map.CreateSettingTilesCommand(layer, x, y, this.ViewModel.EditorState.SelectedTiles, 0, 0);
                        command.Do();
                        tempCommands.Add(command);
                    }
                    else if ((e.Button & MouseButtons.Right) != 0)
                    {
                        cursorOffset = Point.Empty;
                        pickerStart = cursorTile;
                        isPickingTiles = true;
                        this.MapEditor.Invalidate(oldFrameRect);
                        this.MapEditor.Update();
                    }
                }
                else
                {
                    this.MapEditor.Invalidate(oldFrameRect);
                    Rectangle frameRect = this.GetFrameRect(isPickingTiles, cursorTile, cursorOffset, pickerStart);
                    this.MapEditor.Invalidate(frameRect);
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
                        cursorTile = new Point
                        {
                            X = Math.Min(Math.Max(mousePosition.X / this.GridSize, 0), this.Map.Width - 1),
                            Y = Math.Min(Math.Max(mousePosition.Y / this.GridSize, 0), this.Map.Height - 1),
                        };
                        if (!isPickingTiles)
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
                                    int x = cursorTile.X + cursorOffset.X;
                                    int y = cursorTile.Y + cursorOffset.Y;
                                    Command command = this.Map.CreateSettingTilesCommand(
                                        layer, x, y, selectedTiles,
                                        x - renderingTileStart.X, y - renderingTileStart.Y);
                                    command.Do();
                                    tempCommands.Add(command);
                                }
                            }
                        }
                    }
                }
                Rectangle frameRect = this.GetFrameRect(isPickingTiles, cursorTile, cursorOffset, pickerStart);
                if (this.PreviousFrameRect != frameRect)
                {
                    this.MapEditor.Invalidate(this.PreviousFrameRect);
                    this.MapEditor.Invalidate(frameRect);
                    this.MapEditor.Update();
                }
                this.PreviousFrameRect = frameRect;
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
                if (isPickingTiles)
                {
                    if ((e.Button & MouseButtons.Right) != 0)
                    {
                        int pickedRegionX = Math.Min(cursorTile.X, pickerStart.X);
                        int pickedRegionY = Math.Min(cursorTile.Y, pickerStart.Y);
                        int pickedRegionWidth = Math.Abs(cursorTile.X - pickerStart.X) + 1;
                        int pickedRegionHeight = Math.Abs(cursorTile.Y - pickerStart.Y) + 1;
                        cursorOffset = cursorTile;
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
                    isPickingTiles = false;
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
                        this.Offscreen.Update(this.ViewModel.EditorState, this.ViewModel.TileSetCollection, this.Map, this.GridSize);
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
                Rectangle frameRect = this.GetFrameRect(isPickingTiles, cursorTile, cursorOffset, pickerStart);
                if (previousFrameRect != frameRect)
                {
                    this.MapEditor.Invalidate(previousFrameRect);
                }
                this.MapEditor.Invalidate(frameRect);
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
            this.MapEditor.HScrollBarScroll += (sender, e) =>
            {
                Point offset = this.ViewModel.EditorState.GetMapOffset(this.Map.Id);
                this.ViewModel.EditorState.SetMapOffset(this.Map.Id, new Point
                {
                    X = -e.NewValue,
                    Y = offset.Y,
                });
            };
            this.MapEditor.VScrollBarScroll += (sender, e) =>
            {
                Point offset = this.ViewModel.EditorState.GetMapOffset(this.Map.Id);
                this.ViewModel.EditorState.SetMapOffset(this.Map.Id, new Point
                {
                    X = offset.X,
                    Y = -e.NewValue,
                });
            };

            this.MapEditor.Paint += (sender, e) =>
            {
                Graphics g = e.Graphics;
                Rectangle clipRect = e.ClipRectangle;
                if (this.Map != null)
                {
                    this.MapEditor.RenderOffscreen(this.Offscreen, g, clipRect);
                    Point mousePosition = this.MapEditor.CurrentMousePosition;
                    Size offscreenSize = this.Offscreen.Size;
                    if ((this.ViewModel.EditorState.LayerMode == LayerMode.Event) ||
                        (0 <= mousePosition.X && mousePosition.X < offscreenSize.Width &&
                         0 <= mousePosition.Y && mousePosition.Y < offscreenSize.Height))
                    {
                        Util.DrawFrame(g, this.GetFrameRect(isPickingTiles, cursorTile, cursorOffset, pickerStart));
                    }
                }
                else
                {
                    g.FillRectangle(SystemBrushes.Control, clipRect);
                }
            };

            this.ViewModel.IsOpenedChanged += delegate
            {
                this.Map = this.ViewModel.EditorState.Map;
            };
            var previousMapOffsets = new Dictionary<int, Point>();
            this.ViewModel.EditorState.Updated += (sender, e) =>
            {
                EditorState editorState = this.ViewModel.EditorState;
                TileSetCollection tileSetCollection = this.ViewModel.TileSetCollection;
                if (e.Property == editorState.GetProperty(_ => _.MapId))
                {
                    this.Map = editorState.Map;
                }
                else if (e.Property == editorState.GetProperty(_ => _.LayerMode))
                {
                    this.MapEditor.Invalidate();
                    this.Offscreen.Update(editorState, tileSetCollection, this.Map, this.GridSize);
                    this.MapEditor.Update();
                }
                else if (e.Property == editorState.GetProperty(_ => _.ScaleMode))
                {
                    this.MapEditor.AdjustScrollBars(editorState, this.Map, this.GridSize);
                    this.MapEditor.Invalidate();
                    this.Offscreen.Update(editorState, tileSetCollection, this.Map, this.GridSize);
                    this.MapEditor.Update();
                }
                else if (e.Property == editorState.GetProperty(_ => _.MapOffsets))
                {
                    if (this.Map == null)
                    {
                        return;
                    }
                    this.MapEditor.AdjustScrollBars(editorState, this.Map, this.GridSize);
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
                        this.Offscreen.Update(editorState, tileSetCollection, this.Map, this.GridSize, new Rectangle
                        {
                            X = (0 < dx) ? this.Offscreen.Size.Width - dx : 0,
                            Y = 0,
                            Width = Math.Abs(dx),
                            Height = this.Offscreen.Size.Height,
                        });
                    }
                    if (dy != 0)
                    {
                        this.Offscreen.Update(editorState, tileSetCollection, this.Map, this.GridSize, new Rectangle
                        {
                            X = 0,
                            Y = (0 < dy) ? this.Offscreen.Size.Height - dy : 0,
                            Width = this.Offscreen.Size.Width,
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
                        cursorOffset = Point.Empty;
                    }
                }
            };
            this.Map = this.ViewModel.EditorState.Map;
        }

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
                    this.MapEditor.AdjustScrollBars(this.ViewModel.EditorState, this.map, this.GridSize);
                    this.MapEditor.Invalidate();
                    this.Offscreen.Update(this.ViewModel.EditorState, this.ViewModel.TileSetCollection, this.map, this.GridSize);
                    this.MapEditor.Update();
                }
            }
        }
        private Map map;

        private void Map_Updated(object sender, UpdatedEventArgs e)
        {
            if (e.Property == this.Map.GetProperty(_ => _.Width) ||
                e.Property == this.Map.GetProperty(_ => _.Height))
            {
                this.MapEditor.AdjustScrollBars(this.ViewModel.EditorState, this.Map, this.GridSize);
                this.MapEditor.Invalidate();
                Debug.Assert(this.Offscreen != null);
                this.Offscreen.Update(this.ViewModel.EditorState, this.ViewModel.TileSetCollection, this.Map, this.GridSize);
                this.MapEditor.Update();
            }
            else if (e.Property == this.Map.GetProperty(_ => _.Tiles))
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
                Debug.Assert(this.Offscreen != null);
                this.Offscreen.Update(this.ViewModel.EditorState, this.ViewModel.TileSetCollection, this.Map, this.GridSize, updatedRect);
                this.MapEditor.Update();
            }
        }

        private Rectangle GetFrameRect(bool isPickingTiles, Point cursorTile, Point cursorOffset, Point pickerStart)
        {

            if (this.Map != null)
            {
                EditorState editorState = this.ViewModel.EditorState;
                int gridSize = this.GridSize;
                Point offset = editorState.GetMapOffset(map.Id);
                if (!isPickingTiles)
                {
                    SelectedTiles selectedTiles = editorState.SelectedTiles;
                    int cursorHorizontalCount, cursorVerticalCount;
                    bool isEvent = editorState.LayerMode == LayerMode.Event;
                    if (!isEvent)
                    {
                        cursorHorizontalCount = cursorTile.X + cursorOffset.X;
                        cursorVerticalCount = cursorTile.Y + cursorOffset.Y;
                    }
                    else
                    {
                        cursorHorizontalCount = cursorTile.X;
                        cursorVerticalCount = cursorTile.Y;
                    }
                    return new Rectangle
                    {
                        X = cursorHorizontalCount * gridSize + offset.X,
                        Y = cursorVerticalCount * gridSize + offset.Y,
                        Width = gridSize * (!isEvent ? selectedTiles.Width : 1),
                        Height = gridSize * (!isEvent ? selectedTiles.Height : 1),
                    };
                }
                else
                {
                    int pickedRegionX = Math.Min(cursorTile.X, pickerStart.X);
                    int pickedRegionY = Math.Min(cursorTile.Y, pickerStart.Y);
                    int pickedRegionWidth = Math.Abs(cursorTile.X - pickerStart.X) + 1;
                    int pickedRegionHeight = Math.Abs(cursorTile.Y - pickerStart.Y) + 1;
                    return new Rectangle
                    {
                        X = pickedRegionX * gridSize + offset.X,
                        Y = pickedRegionY * gridSize + offset.Y,
                        Width = gridSize * pickedRegionWidth,
                        Height = gridSize * pickedRegionHeight,
                    };
                }
            }
            else
            {
                return Rectangle.Empty;
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
