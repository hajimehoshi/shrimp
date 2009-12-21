using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shrimp.Models
{
    public class ViewModel
    {
        public ViewModel()
        {
            this.Project = new Project();
            this.EditorState = new EditorState(this);
            this.MapCollection = new MapCollection(this);
            this.TileSetCollection = new TileSetCollection(this);

            foreach (IModel modelStore in this.Models)
            {
                modelStore.Updated += delegate { this.IsDirty = true; };
            }
            this.EditorState.IsUndoableChanged += delegate
            {
                this.OnIsUndoableChanged(EventArgs.Empty);
            };
        }

        public Project Project { get; private set; }

        public EditorState EditorState{get; private set;}

        public MapCollection MapCollection { get; private set; }

        public TileSetCollection TileSetCollection { get; private set; }

        private IEnumerable<IModel> Models
        {
            get
            {
                yield return this.Project;
                yield return this.EditorState;
                yield return this.MapCollection;
                yield return this.TileSetCollection;
            }
        }

        public string DirectoryPath { get; private set; }

        public void New(string directoryPath, string gameTitle)
        {
            foreach (IModel model in this.Models)
            {
                model.Clear();
            }
            this.DirectoryPath = directoryPath;
            Util.CopyDirectory(ProjectTemplatePath, this.DirectoryPath);
            this.Project.GameTitle = gameTitle;
            this.TileSetCollection.AddItemsFromImageFiles();
            this.Save();
            this.IsOpened = true;
        }

        private static readonly Encoding UTF8 = new UTF8Encoding(false);

        private static string ProjectTemplatePath
        {
            get
            {
                string location = Assembly.GetExecutingAssembly().Location;
                return Path.Combine(Path.GetDirectoryName(location), "ProjectTemplate");
            }
        }

        public void Open(string projectFilePath)
        {
            this.DirectoryPath = Path.GetDirectoryName(projectFilePath);
            string path = projectFilePath;
            using (var sr = new StreamReader(path, UTF8))
            using (var reader = new JsonTextReader(sr))
            {
                this.LoadJson(JToken.ReadFrom(reader));
            }
            this.IsDirty = false;
            this.TileSetCollection.AddItemsFromImageFiles();
            this.IsOpened = true;
            this.IsDirty = false;
        }

        public void Close()
        {
            foreach (IModel model in this.Models)
            {
                model.Clear();
            }
            this.DirectoryPath = null;
            this.IsOpened = false;
            this.IsDirty = false;
        }

        public void Save()
        {
            Debug.Assert(Directory.Exists(this.DirectoryPath));
            string path = Path.Combine(this.DirectoryPath, "Project.json");
            JToken token = this.ToJson();
            using (var sw = new StreamWriter(path, false, UTF8))
            using (var writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                token.WriteTo(writer);
            }
            this.IsDirty = false;
        }

        public void Undo()
        {
            Debug.Assert(this.IsUndoable);
            this.EditorState.Undo();
        }

        public bool IsOpened
        {
            get { return this.isOpened; }
            private set
            {
                if (this.isOpened != value)
                {
                    this.isOpened = value;
                    this.OnIsOpenedChanged(EventArgs.Empty);
                }
            }
        }
        private bool isOpened = false;
        public event EventHandler IsOpenedChanged;
        protected virtual void OnIsOpenedChanged(EventArgs e)
        {
            if (this.IsOpenedChanged != null) { this.IsOpenedChanged(this, e); }
        }

        public bool IsDirty
        {
            get { return this.isDirty; }
            set
            {
                if (this.isDirty != value)
                {
                    this.isDirty = value;
                    this.OnIsDirtyChanged(EventArgs.Empty);
                }
            }
        }
        private bool isDirty = false;
        public event EventHandler IsDirtyChanged;
        protected virtual void OnIsDirtyChanged(EventArgs e)
        {
            if (this.IsDirtyChanged != null) { this.IsDirtyChanged(this, e); }
        }

        public bool IsUndoable
        {
            get
            {
                return this.EditorState.IsUndoable;
            }
        }
        public event EventHandler IsUndoableChanged;
        protected virtual void OnIsUndoableChanged(EventArgs e)
        {
            if (this.IsUndoableChanged != null) { this.IsUndoableChanged(this, e); }
        }

        public Bitmap GetTilesBitmap()
        {
            string tilesBitmapPath = Path.Combine(this.DirectoryPath, "Graphics/Tiles.png");
            return Bitmap.FromFile(tilesBitmapPath) as Bitmap;
        }

        private JToken ToJson()
        {
            return new JObject(
                new JProperty("Project", this.Project.ToJson()),
                new JProperty("EditorState", this.EditorState.ToJson()),
                new JProperty("MapCollection", this.MapCollection.ToJson()),
                new JProperty("TileSetCollection", this.TileSetCollection.ToJson()));
        }

        private void LoadJson(JToken json)
        {
            foreach (IModel model in this.Models)
            {
                model.Clear();
            }
            JToken token;
            if ((token = json["Project"]) != null)
            {
                this.Project.LoadJson(token);
            }
            if ((token = json["EditorState"]) != null)
            {
                this.EditorState.LoadJson(token);
            }
            if ((token = json["MapCollection"]) != null)
            {
                this.MapCollection.LoadJson(token);
            }
            if ((token = json["TileSetCollection"]) != null)
            {
                this.TileSetCollection.LoadJson(token);
            }
        }
    }
}

