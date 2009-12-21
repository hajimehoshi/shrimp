using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shrimp.Models
{
    public class TileSet : Model
    {
        public TileSet(TileSetCollection tileSetCollection, int id)
        {
            this.TileSetCollection = tileSetCollection;
            this.Id = id;
            this.OriginalBitmap = new Bitmap(this.ImageFileFullPath);
            this.SetTilePassageTypes(new ObservedArray<TilePassageType>(this.Width * this.Height));
            this.Clear();
        }

        private TileSetCollection TileSetCollection;

        public int Id { get; private set; }

        public string ImageFileName
        {
            get { return this.Id.ToString() + ".png"; }
        }

        public string ImageFileFullPath
        {
            get
            {
                if (this.ImageFileName != null)
                {
                    string path = this.TileSetCollection.ViewModel.DirectoryPath;
                    path = Path.Combine(path, this.TileSetCollection.TilesGraphicsDirectory);
                    path = Path.Combine(path, this.ImageFileName);
                    return path;
                }
                else
                {
                    return null;
                }
            }
        }

        public int Width
        {
            get
            {
                if (this.OriginalBitmap != null)
                {
                    // TODO
                    return this.OriginalBitmap.Width / 16;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int Height
        {
            get
            {
                if (this.OriginalBitmap != null)
                {
                    // TODO
                    return this.OriginalBitmap.Height / 16;
                }
                else
                {
                    return 0;
                }
            }
        }

        private Bitmap OriginalBitmap;

        private Dictionary<ScaleMode, Bitmap> Bitmaps = new Dictionary<ScaleMode, Bitmap>();

        public Bitmap GetBitmap(ScaleMode scale)
        {
            if (!this.Bitmaps.ContainsKey(scale))
            {
                Bitmap bitmap = Util.CreateScaledBitmap(this.OriginalBitmap, scale);
                this.Bitmaps.Add(scale, bitmap);
                return bitmap;
            }
            else
            {
                return this.Bitmaps[scale];
            }
        }

        public IIndexable<TilePassageType> TilePassageTypes
        {
            get { return this.tilePassageTypes; }
        }
        private ObservedArray<TilePassageType> GetTilePassageTypes()
        {
            return this.tilePassageTypes;
        }
        private void SetTilePassageTypes(ObservedArray<TilePassageType> tilePassageTypes)
        {
            if (this.tilePassageTypes != tilePassageTypes)
            {
                if (this.tilePassageTypes != null)
                {
                    this.tilePassageTypes.Updated -= this.TilePassageTypes_Updated;
                }
                this.tilePassageTypes = tilePassageTypes;
                if (this.tilePassageTypes != null)
                {
                    this.tilePassageTypes.Updated += this.TilePassageTypes_Updated;
                }
            }
        }
        private ObservedArray<TilePassageType> tilePassageTypes;

        protected void TilePassageTypes_Updated(object sender, UpdatedEventArgs e)
        {
            this.OnUpdated(new UpdatedEventArgs(this.GetProperty(_ => _.TilePassageTypes), e));
        }

        public override void Clear()
        {
        }

        public override JToken ToJson()
        {
            return new JObject(
                new JProperty("ImageFileName", this.ImageFileName),
                new JProperty("TilePassageTypes", this.GetTilePassageTypes().ToJson()));
        }

        public override void LoadJson(JToken json)
        {
            this.Clear();
            JToken token;
            if (this.TilePassageTypes != null)
            {
                if ((token = json["TilePassageTypes"]) != null)
                {
                    this.GetTilePassageTypes().LoadJson(token);
                }
            }
        }
    }

    public enum TilePassageType
    {
        Passable,
        Impassable,
        Wall,
        Ceil,
    }
}
