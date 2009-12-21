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
    public class Map : Model
    {
        public Map(MapCollection mapCollection, int id)
        {
            this.MapCollection = mapCollection;
            this.Id = id;
            this.Clear();
        }

        private const int LayerCount = 2;

        public const int MinWidth = 20;
        public const int MaxWidth = 500;
        public const int MinHeight = 15;
        public const int MaxHeight = 500;

        public MapCollection MapCollection { get; private set; }

        public ViewModel ViewModel
        {
            get { return this.MapCollection.ViewModel; }
        }

        public int Id { get; private set; }

        public string Name
        {
            get { return this.name; }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.OnUpdated(new UpdatedEventArgs(this.GetProperty(_ => _.Name)));
                }
            }
        }
        private string name = "";

        public int Width
        {
            get { return this.width; }
            set
            {
                if (this.width != value)
                {
                    if (value < MinWidth || MaxWidth < value)
                    {
                        throw new ArgumentOutOfRangeException(this.GetProperty(_ => _.Width));
                    }
                    if (this.width < value)
                    {
                        foreach (var layer in this.Layers)
                        {
                            for (int j = 0; j < this.height; j++)
                            {
                                var newTiles = Enumerable.Repeat(new Tile(), value - this.width);
                                layer.InsertRange(j * value + this.width, newTiles);
                            }
                        }
                    }
                    else
                    {
                        foreach (var layer in this.Layers)
                        {
                            for (int j = 0; j < this.height; j++)
                            {
                                layer.RemoveRange(j * value + value, this.width - value);
                            }
                        }
                    }
                    this.width = value;
                    Debug.Assert(this.Layers.All(l => l.Count == this.Width * this.Height));
                    this.OnUpdated(new UpdatedEventArgs(this.GetProperty(_ => _.Width)));
                }
            }
        }
        private int width;

        public int Height
        {
            get { return this.height; }
            set
            {
                if (this.height != value)
                {
                    if (value < MinHeight || MaxHeight < value)
                    {
                        throw new ArgumentOutOfRangeException(this.GetProperty(_ => _.Height));
                    }
                    if (this.height < value)
                    {
                        foreach (var layer in this.Layers)
                        {
                            int size = (value - this.height) * this.Width;
                            layer.AddRange(Enumerable.Repeat(new Tile(), size));
                        }
                    }
                    else
                    {
                        foreach (var layer in this.Layers)
                        {
                            int size = (this.height - value) * this.Width;
                            layer.RemoveRange(layer.Count - size, size);
                        }
                    }
                    this.height = value;
                    Debug.Assert(this.Layers.All(l => l.Count == this.Width * this.Height));
                    this.OnUpdated(new UpdatedEventArgs(this.GetProperty(_ => _.Height)));
                }
            }
        }
        private int height;

        private List<List<Tile>> Layers = new List<List<Tile>>();

        public Tile GetTile(int layerNumber, int x, int y)
        {
            return this.Layers[layerNumber][y * this.Width + x];
        }

        public Command CreateSettingTilesCommand(int layerNumber, int x, int y, SelectedTiles selectedTiles,
            int dx, int dy)
        {
            Rectangle region = new Rectangle
            {
                X = x,
                Y = y,
                Width = selectedTiles.Width,
                Height = selectedTiles.Height,
            };
            int width = region.Width;
            int height = region.Height;
            if (dx < 0)
            {
                dx = -(-dx % width) + width;
            }
            if (dy < 0)
            {
                dy = -(-dy % height) + height;
            }
            Tile[] oldTiles = new Tile[region.Width * region.Height];
            int index = 0;
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    if (0 <= i + x && i + x < this.Width &&
                        0 <= j + y && j + y < this.Height)
                    {
                        oldTiles[index] = this.Layers[layerNumber][(y + j) * this.Width + (x + i)];
                    }
                    index++;
                }
            }
            Command command = new Command();
            command.Done += delegate
            {
                bool isChanged = false;
                List<Tile> layer = this.Layers[layerNumber];
                Tile[] newTiles = selectedTiles.Tiles.ToArray();
                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        if (0 <= i + x && i + x < this.Width &&
                            0 <= j + y && j + y < this.Height)
                        {
                            Tile tile = newTiles[((j + dy) % height) * width + ((i + dx) % width)];
                            int location = (j + y) * this.Width + (i + x);
                            if (layer[location] != tile)
                            {
                                layer[location] = tile;
                                isChanged = true;
                            }
                        }
                    }
                }
                if (isChanged)
                {
                    this.OnUpdated(new UpdatedEventArgs(this.GetProperty(_ => _.Tiles), region));
                }
            };
            command.Undone += delegate
            {
                bool isChanged = false;
                List<Tile> layer = this.Layers[layerNumber];
                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        if (0 <= i + x && i + x < this.Width &&
                            0 <= j + y && j + y < this.Height)
                        {
                            Tile tile = oldTiles[j * width + i];
                            int location = (j + y) * this.Width + (i + x);
                            if (layer[location] != tile)
                            {
                                layer[location] = tile;
                                isChanged = true;
                            }
                        }
                    }
                }
                if (isChanged)
                {
                    this.OnUpdated(new UpdatedEventArgs(this.GetProperty(_ => _.Tiles), region));
                }
            };
            return command;
        }

        public IEnumerable<Tile> Tiles
        {
            get
            {
                // TODO
                foreach (List<Tile> layer in this.Layers)
                {
                    foreach (Tile tile in layer)
                    {
                        yield return tile;
                    }
                }
            }
        }

        public override void Clear()
        {
            this.Width = MinWidth;
            this.Height = MinHeight;
            this.Layers.Clear();
            int size = this.Width * this.Height;
            for (int i = 0; i < LayerCount; i++)
            {
                this.Layers.Add(Enumerable.Repeat(new Tile(), size).ToList());
            }
        }

        public override JToken ToJson()
        {
            List<string> layerStrings = new List<string>();
            foreach (var layer in this.Layers)
            {
                int length = layer.Count;
                byte[] layerBytes = new byte[length * 4];
                for (int i = 0; i < length; i++)
                {
                    Tile tile = layer[i];
                    byte[] bytes = tile.ToBytes();
                    layerBytes[i * 4] = bytes[0];
                    layerBytes[i * 4 + 1] = bytes[1];
                    layerBytes[i * 4 + 2] = bytes[2];
                    layerBytes[i * 4 + 3] = bytes[3];
                }
                layerStrings.Add(Convert.ToBase64String(layerBytes));
            }
            return new JObject(
                new JProperty("Name", this.Name),
                new JProperty("Width", this.Width),
                new JProperty("Height", this.Height),
                new JProperty("Tiles",
                    new JArray(layerStrings)));
        }

        public override void LoadJson(JToken json)
        {
            this.Clear();
            JToken token;
            if ((token = json["Name"] as JValue) != null)
            {
                this.Name = token.Value<string>();
            }
            if ((token = json["Width"] as JValue) != null)
            {
                this.Width = token.Value<int>();
            }
            if ((token = json["Height"] as JValue) != null)
            {
                this.Height = token.Value<int>();
            }
            if ((token = json["Tiles"] as JArray) != null)
            {
                for (int i = 0; i < LayerCount; i++)
                {
                    JValue token2 = token[i] as JValue;
                    byte[] bytes = Convert.FromBase64String(token2.Value<string>());
                    int length = this.Width * this.Height;
                    List<Tile> layer = this.Layers[i];
                    for (int j = 0; j < length; j++)
                    {
                        Tile tile = new Tile();
                        tile.FromBytes(bytes, j * 4);
                        layer[j] = tile;
                    }
                }
            }
        }
    }
}
