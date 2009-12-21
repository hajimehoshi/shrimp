using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shrimp.Models
{
    public enum LayerMode
    {
        Layer1,
        Layer2,
        Event,
    }

    public enum DrawingMode
    {
        Pen,
    }

    public enum ScaleMode
    {
        Scale1,
        Scale2,
        Scale4,
        Scale8,
    }

    public enum TileSetMode
    {
        Normal,
        Passage,
    }

    public enum SelectedTilesType
    {
        Single,
        Rectangle,
        Picker,
    }
    
    public class SelectedTiles
    {
        public static SelectedTiles Single(Tile tile)
        {
            return new SelectedTiles(SelectedTilesType.Single, tile, 1, 1);
        }

        public static SelectedTiles Rectangle(Tile tile, int width, int height)
        {
            Debug.Assert(0 < width);
            Debug.Assert(tile.TileId % Util.PaletteHorizontalCount + width <= Util.PaletteHorizontalCount);
            Debug.Assert(0 < height);
            var tiles = Enumerable.Empty<Tile>();
            for (int j = 0; j < height; j++)
            {
                var line = from i in Enumerable.Range(tile.TileId + j * Util.PaletteHorizontalCount, width)
                           select new Tile
                           {
                               TileSetId = tile.TileSetId,
                               TileId = (short)i,
                           };
                tiles = tiles.Concat(line);
            }
            return new SelectedTiles(SelectedTilesType.Rectangle, tiles, width, height);
        }

        public static SelectedTiles Picker(IEnumerable<Tile> tiles, int width, int height)
        {
            return new SelectedTiles(SelectedTilesType.Picker, tiles, width, height);
        }

        private SelectedTiles(SelectedTilesType type, Tile tile, int width, int height)
            : this(type, new[] { tile }, width, height)
        {
        }

        private SelectedTiles(SelectedTilesType type, IEnumerable<Tile> tiles, int width, int height)
        {
            this.SelectedTilesType = type;
            this.Tiles = tiles;
            this.Width = width;
            this.Height = height;
        }

        public SelectedTilesType SelectedTilesType { get; private set; }
        public Tile Tile { get { return this.Tiles.First(); } }
        public IEnumerable<Tile> Tiles { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
    }

    public class EditorState : Model
    {
        public EditorState(ViewModel viewModel)
        {
            this.ViewModel = viewModel;
            this.Clear();
        }

        public ViewModel ViewModel { get; private set; }

        public Map Map
        {
            get
            {
                Map map;
                if (this.ViewModel.MapCollection.TryGetMap(this.MapId, out map))
                {
                    return map;
                }
                else
                {
                    return null;
                }
            }
        }

        public int MapId
        {
            get { return this.mapId; }
            set
            {
                if (this.mapId != value)
                {
                    this.mapId = value;
                    this.OnUpdated(new UpdatedEventArgs(this.GetProperty(_ => _.MapId)));
                    this.Commands.Clear();
                    this.OnIsUndoableChanged(EventArgs.Empty);
                }
            }
        }
        private int mapId;

        public Point GetMapOffset(int id)
        {
            if (!this.mapOffsets.ContainsKey(id))
            {
                this.mapOffsets.Add(id, new Point(0, 0));
            }
            return this.mapOffsets[id];
        }
        public void SetMapOffset(int id, Point point)
        {
            if (this.mapOffsets[id] != point)
            {
                point.X = Math.Min(point.X, 0);
                point.Y = Math.Min(point.Y, 0);
                this.mapOffsets[id] = point;
                this.OnUpdated(new UpdatedEventArgs(this.GetProperty(_ => _.MapOffsets), null));
            }
        }
        private Dictionary<int, Point> mapOffsets = new Dictionary<int, Point>();

        public object MapOffsets
        {
            get
            {
                // TODO
                throw new InvalidOperationException("Dummy property");
            }
        }

        public int GetSelectedTileSetId(int mapId)
        {
            if (this.ViewModel.MapCollection.Roots.Contains(mapId))
            {
                return -1;
            }
            if (!this.selectedTileSetIds.ContainsKey(mapId))
            {
                if (0 < this.ViewModel.TileSetCollection.ItemCount)
                {
                    int minId = this.ViewModel.TileSetCollection.ItemIds.Min();
                    this.selectedTileSetIds.Add(mapId, minId);
                }
                else
                {
                    this.selectedTileSetIds.Add(mapId, 0);
                }
            }
            return this.selectedTileSetIds[mapId];
        }
        public void SetSelectedTileSetId(int mapId, int tileSetId)
        {
            if (this.ViewModel.MapCollection.Roots.Contains(mapId))
            {
                throw new ArgumentException("Invalid map ID", "mapId");
            }
            if (this.selectedTileSetIds[mapId] != tileSetId)
            {
                this.selectedTileSetIds[mapId] = tileSetId;
                this.OnUpdated(new UpdatedEventArgs(this.GetProperty(_ => _.SelectedTileSetIds), null));
            }
        }
        private Dictionary<int, int> selectedTileSetIds = new Dictionary<int, int>();

        public object SelectedTileSetIds
        {
            get
            {
                // TODO
                throw new InvalidOperationException("Dummy property");
            }
        }

        public LayerMode LayerMode
        {
            get { return this.layerMode; }
            set
            {
                if (this.layerMode != value)
                {
                    this.layerMode = value;
                    this.OnUpdated(new UpdatedEventArgs(this.GetProperty(_ => _.LayerMode)));
                }
            }
        }
        private LayerMode layerMode;

        public DrawingMode DrawingMode
        {
            get { return this.drawingMode; }
            set
            {
                if (this.drawingMode != value)
                {
                    this.drawingMode = value;
                    this.OnUpdated(new UpdatedEventArgs(this.GetProperty(_ => _.DrawingMode)));
                }
            }
        }
        private DrawingMode drawingMode = DrawingMode.Pen;

        public ScaleMode ScaleMode
        {
            get { return this.scaleMode; }
            set
            {
                if (this.scaleMode != value)
                {
                    this.scaleMode = value;
                    this.OnUpdated(new UpdatedEventArgs(this.GetProperty(_ => _.ScaleMode)));
                }
            }
        }
        private ScaleMode scaleMode;

        public TileSetMode TileSetMode
        {
            get { return this.tileSetMode; }
            set
            {
                if (this.tileSetMode != value)
                {
                    this.tileSetMode = value;
                    this.OnUpdated(new UpdatedEventArgs(this.GetProperty(_ => _.TileSetMode)));
                }
            }
        }
        private TileSetMode tileSetMode = TileSetMode.Normal;

        public TileSet SelectedTileSet
        {
            get
            {
                int tileSetId = this.SelectedTileSetId;
                if (this.ViewModel.TileSetCollection.ContainsId(tileSetId))
                {
                    return this.ViewModel.TileSetCollection.GetItem(tileSetId);
                }
                else
                {
                    return null;
                }
            }
        }

        public int SelectedTileSetId
        {
            get
            {
                return this.GetSelectedTileSetId(this.MapId);
            }
        }

        public SelectedTiles SelectedTiles
        {
            get { return this.selectedTiles; }
            set
            {
                if (this.selectedTiles != value)
                {
                    this.selectedTiles = value;
                    this.OnUpdated(new UpdatedEventArgs(this.GetProperty(_ => _.SelectedTiles)));
                }
            }
        }
        private SelectedTiles selectedTiles;

        public override void Clear()
        {
            this.MapId = 0;
            this.mapOffsets.Clear();
            this.selectedTileSetIds.Clear();
            this.DrawingMode = DrawingMode.Pen;
            this.SelectedTiles = SelectedTiles.Single(new Tile
            {
                TileSetId = 0,
                TileId = 0,
            });
        }

        private Stack<ICommand> Commands = new Stack<ICommand>();

        public void AddCommand(ICommand command)
        {
            this.Commands.Push(command);
            this.OnIsUndoableChanged(EventArgs.Empty);
        }

        public void Undo()
        {
            Debug.Assert(this.IsUndoable);
            this.Commands.Pop().Undo();
            this.OnIsUndoableChanged(EventArgs.Empty);
        }

        public bool IsUndoable
        {
            get
            {
                return 0 < this.Commands.Count;
            }
        }
        public event EventHandler IsUndoableChanged;
        protected virtual void OnIsUndoableChanged(EventArgs e)
        {
            if (this.IsUndoableChanged != null) { this.IsUndoableChanged(this, e); }
        }

        public override JToken ToJson()
        {
            return new JObject(
                new JProperty("MapId", this.MapId),
                new JProperty("MapOffsets",
                    new JArray(
                        from p in this.mapOffsets
                        select new JObject(
                            new JProperty("MapId", p.Key),
                            new JProperty("Offset", new JArray(p.Value.X, p.Value.Y))))),
                new JProperty("SelectedTileSetIds",
                    new JArray(
                        from p in this.selectedTileSetIds
                        select new JObject(
                            new JProperty("MapId", p.Key),
                            new JProperty("TileSetId", p.Value)))),
                new JProperty("ScaleMode", this.ScaleMode.ToString()));
        }

        public override void LoadJson(JToken json)
        {
            this.Clear();
            JToken token;
            if ((token = json["MapId"]) != null)
            {
                this.MapId = token.Value<int>();
            }
            if ((token = json["MapOffsets"]) != null)
            {
                foreach (JObject j in token as JArray)
                {
                    int id = j.Value<int>("MapId");
                    int[] jOffset = (j["Offset"] as JArray).Values<int>().ToArray();
                    Point point = new Point()
                    {
                        X = jOffset[0],
                        Y = jOffset[1],
                    };
                    this.mapOffsets.Add(id, point);
                }
            }
            if ((token = json["SelectedTileSetIds"]) != null)
            {
                foreach (JObject j in token as JArray)
                {
                    int mapId = j.Value<int>("MapId");
                    int tileSetId = j.Value<int>("TileSetId");
                    this.selectedTileSetIds[mapId] = tileSetId;
                }
            }
            if ((token = json["ScaleMode"]) != null)
            {
                this.ScaleMode = (ScaleMode)Enum.Parse(typeof(ScaleMode), token.Value<string>());
            }
        }
    }
}
