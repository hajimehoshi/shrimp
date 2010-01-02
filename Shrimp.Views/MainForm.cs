using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Shrimp.IViews;
using Shrimp.Models;

namespace Shrimp.Views
{
    public partial class MainForm : Form, IMainForm
    {
        static MainForm()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }

        public event EventHandler CloseButtonClicked;
        protected virtual void OnCloseButtonClicked(EventArgs e)
        {
            if (this.CloseButtonClicked != null)
            {
                this.CloseButtonClicked(this, e);
            }
        }

        public event EventHandler<DrawingModeSwitcherClickedEventArgs> DrawingModeSwitcherClicked;
        protected virtual void OnDrawingModeSwitcherClicked(DrawingModeSwitcherClickedEventArgs e)
        {
            if (this.DrawingModeSwitcherClicked != null)
            {
                this.DrawingModeSwitcherClicked(this, e);
            }
        }

        public event EventHandler<LayerModeSwitcherClickedEventArgs> LayerModeSwitcherClicked;
        protected virtual void OnLayerModeSwitcherClicked(LayerModeSwitcherClickedEventArgs e)
        {
            if (this.LayerModeSwitcherClicked != null)
            {
                this.LayerModeSwitcherClicked(this, e);
            }
        }

        public event EventHandler NewButtonClicked;
        protected virtual void OnNewButtonClicked(EventArgs e)
        {
            if (this.NewButtonClicked != null)
            {
                this.NewButtonClicked(this, e);
            }
        }

        public event EventHandler OpenButtonClicked;
        protected virtual void OnOpenButtonClicked(EventArgs e)
        {
            if (this.OpenButtonClicked != null)
            {
                this.OpenButtonClicked(this, e);
            }
        }

        public event EventHandler PassageButtonClicked;
        protected virtual void OnPassageButtonClicked(EventArgs e)
        {
            if (this.PassageButtonClicked != null) { this.PassageButtonClicked(this, e); }
        }

        public event EventHandler SaveButtonClicked;
        protected virtual void OnSaveButtonClicked(EventArgs e)
        {
            if (this.SaveButtonClicked != null)
            {
                this.SaveButtonClicked(this, e);
            }
        }

        public event EventHandler<ScaleModeSwitcherClickedEventArgs> ScaleModeSwitcherClicked;
        protected virtual void OnScaleModeSwitcherClicked(ScaleModeSwitcherClickedEventArgs e)
        {
            if (this.ScaleModeSwitcherClicked != null)
            {
                this.ScaleModeSwitcherClicked(this, e);
            }
        }

        public event EventHandler SelectedTileSetChanged;
        protected virtual void OnSelectedTileSetChanged(EventArgs e)
        {
            if (this.SelectedTileSetChanged != null) { this.SelectedTileSetChanged(this, e); }
        }
        
        public event EventHandler UndoButtonClicked;
        protected virtual void OnUndoButtonClicked(EventArgs e)
        {
            if (this.UndoButtonClicked != null)
            {
                this.UndoButtonClicked(this, e);
            }
        }

        public INewProjectDialog CreateNewProjectDialog()
        {
            return new NewProjectDialog();
        }

        public bool GetDrawingModeSwitcherEnabled(DrawingMode drawingMode)
        {
            switch (drawingMode)
            {
            case DrawingMode.Pen:
                return this.PenToolStripButton.Enabled;
            default:
                Debug.Fail("Invalid DrawingMode value");
                return false;
            }
        }

        public bool GetLayerModeSwitcherEnabled(LayerMode layerMode)
        {
            switch (layerMode)
            {
            case LayerMode.Layer1:
                return this.Layer1ToolStripButton.Enabled;
            case LayerMode.Layer2:
                return this.Layer2ToolStripButton.Enabled;
            case LayerMode.Event:
                return this.EventToolStripButton.Enabled;
            default:
                Debug.Fail("Invalid LayerMode value");
                return false;
            }
        }

        public bool GetScaleModeSwitcherEnabled(ScaleMode scaleMode)
        {
            switch (scaleMode)
            {
            case ScaleMode.Scale1:
                return this.Scale1ToolStripButton.Enabled;
            case ScaleMode.Scale2:
                return this.Scale2ToolStripButton.Enabled;
            case ScaleMode.Scale4:
                return this.Scale4ToolStripButton.Enabled;
            case ScaleMode.Scale8:
                return this.Scale8ToolStripButton.Enabled;
            default:
                Debug.Fail("Invalid ScaleMode value");
                return false;
            }
        }

        public void Run()
        {
            Application.Run(this);
        }

        public void SetDrawingModeSwitcherEnabled(DrawingMode drawingMode, bool enabled)
        {
            switch (drawingMode)
            {
            case DrawingMode.Pen:
                this.PenToolStripButton.Enabled = enabled;
                break;
            }
        }

        public void SetLayerModeSwitcherEnabled(LayerMode layerMode, bool enabled)
        {
            switch (layerMode)
            {
            case LayerMode.Layer1:
                this.Layer1ToolStripButton.Enabled = enabled;
                break;
            case LayerMode.Layer2:
                this.Layer2ToolStripButton.Enabled = enabled;
                break;
            case LayerMode.Event:
                this.EventToolStripButton.Enabled = enabled;
                break;
            }
        }

        public void SetScaleModeSwitcherEnabled(ScaleMode scaleMode, bool enabled)
        {
            switch (scaleMode)
            {
            case ScaleMode.Scale1:
                this.Scale1ToolStripButton.Enabled = enabled;
                break;
            case ScaleMode.Scale2:
                this.Scale2ToolStripButton.Enabled = enabled;
                break;
            case ScaleMode.Scale4:
                this.Scale4ToolStripButton.Enabled = enabled;
                break;
            case ScaleMode.Scale8:
                this.Scale8ToolStripButton.Enabled = enabled;
                break;
            }
        }

        public void SetTileSetSelectorItems(IEnumerable<string> items)
        {
            this.TileSetsToolStripComboBox.BeginUpdate();
            this.TileSetsToolStripComboBox.Items.Clear();
            this.TileSetsToolStripComboBox.Items.AddRange(items.ToArray());
            this.TileSetsToolStripComboBox.EndUpdate();
        }

        public OpenFileDialog OpenFileDialog
        {
            get { return this.openFileDialog; }
        }

        public bool IsCloseButtonEnabled
        {
            get { return this.CloseToolStripButton.Enabled; }
            set { this.CloseToolStripButton.Enabled = value; }
        }

        public bool IsMapTreeViewEnabled
        {
            get { return this.MapTreeView.Enabled; }
            set { this.MapTreeView.Enabled = value; }
        }

        public bool IsNewButtonEnabled
        {
            get { return this.NewToolStripButton.Enabled; }
            set { this.NewToolStripButton.Enabled = value; }
        }

        public bool IsOpenButtonEnabled
        {
            get { return this.OpenToolStripButton.Enabled; }
            set { this.OpenToolStripButton.Enabled = value; }
        }

        public bool IsPassageButtonChecked
        {
            get { return this.PassageToolStripButton.Checked; }
            set { this.PassageToolStripButton.Checked = value; }
        }

        public bool IsPassageButtonEnabled
        {
            get { return this.PassageToolStripButton.Enabled; }
            set { this.PassageToolStripButton.Enabled = value; }
        }

        public bool IsSaveButtonEnabled
        {
            get { return this.SaveToolStripButton.Enabled; }
            set { this.SaveToolStripButton.Enabled = value; }
        }

        public bool IsTileSetSelectorEnabled
        {
            get { return this.TileSetsToolStripComboBox.Enabled; }
            set { this.TileSetsToolStripComboBox.Enabled = value; }
        }

        public bool IsTileSetPaletteEnabled
        {
            get { return this.TileSetPalette.Enabled; }
            set { this.TileSetPalette.Enabled = value; }
        }

        public bool IsUndoButtonEnabled
        {
            get { return this.UndoToolStripButton.Enabled; }
            set { this.UndoToolStripButton.Enabled = value; }
        }

        public int TileSetSelectorSelectedIndex
        {
            get { return this.TileSetsToolStripComboBox.SelectedIndex; }
            set { this.TileSetsToolStripComboBox.SelectedIndex = value; }
        }

        private class CustomToolStripSystemRenderer : ToolStripSystemRenderer
        {
            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
            {
                // Do nothing
            }
        }

        public MainForm()
        {
            this.InitializeComponent();
            this.SuspendLayout();

            this.MapEditor = new MapEditor();
            this.MapEditor.BorderStyle = BorderStyle.Fixed3D;
            this.MapEditor.Dock = DockStyle.Fill;
            this.MainSplitContainer.Panel2.Controls.Add(this.MapEditor);

            this.ToolStrip.Renderer = new CustomToolStripSystemRenderer();
            this.TileSetPaletteToolStrip.Renderer = new CustomToolStripSystemRenderer();
            this.TileSetPalette.Size = new Size
            {
                Width = Util.PaletteGridSize * Util.PaletteHorizontalCount
                    + SystemInformation.VerticalScrollBarWidth,
                Height = this.TileSetPalette.Parent.ClientSize.Height
                    - this.TileSetPaletteToolStrip.Height,
            };
            this.MainSplitContainer.SplitterDistance -=
                this.TileSetPalette.Parent.ClientSize.Width - this.TileSetPalette.Width;

            this.Layer1ToolStripButton.Tag = LayerMode.Layer1;
            this.Layer2ToolStripButton.Tag = LayerMode.Layer2;
            this.EventToolStripButton.Tag = LayerMode.Event;
            foreach (var item in this.LayerModeSwitchers)
            {
                item.Click += (sender, e) =>
                {
                    LayerMode layerMode = (LayerMode)((ToolStripButton)sender).Tag;
                    this.OnLayerModeSwitcherClicked(new LayerModeSwitcherClickedEventArgs(layerMode));
                };
            }

            this.PenToolStripButton.Tag = DrawingMode.Pen;
            foreach (var item in this.DrawingModeSwitchers)
            {
                item.Click += (sender, e) =>
                {
                    DrawingMode drawingMode = (DrawingMode)((ToolStripButton)sender).Tag;
                    this.OnDrawingModeSwitcherClicked(new DrawingModeSwitcherClickedEventArgs(drawingMode));
                };
            }

            this.Scale1ToolStripButton.Tag = ScaleMode.Scale1;
            this.Scale2ToolStripButton.Tag = ScaleMode.Scale2;
            this.Scale4ToolStripButton.Tag = ScaleMode.Scale4;
            this.Scale8ToolStripButton.Tag = ScaleMode.Scale8;
            foreach (var item in this.ScaleModeSwitchers)
            {
                item.Click += (s, e) =>
                {
                    ScaleMode scaleMode = (ScaleMode)((ToolStripButton)s).Tag;
                    this.OnScaleModeSwitcherClicked(new ScaleModeSwitcherClickedEventArgs(scaleMode));
                };
            }

            this.ViewModel = new ViewModel();
            this.ViewModel.IsOpenedChanged += delegate
            {
                this.IsOpenedChanged();
            };
            this.ViewModel.EditorState.Updated += (s, e) =>
            {
                EditorState editorState = (EditorState)s;
                if (e.Property == editorState.GetProperty(_ => _.MapId))
                {
                    this.MapIdChanged();
                }
                else if (e.Property == editorState.GetProperty(_ => _.SelectedTileSetIds))
                {
                    this.SelectedTileSetIdsChanged();
                }
                else if (e.Property == editorState.GetProperty(_ => _.LayerMode))
                {
                    this.LayerModeChanged();
                }
                else if (e.Property == editorState.GetProperty(_ => _.DrawingMode))
                {
                    this.DrawingModeChanged();
                }
                else if (e.Property == editorState.GetProperty(_ => _.ScaleMode))
                {
                    this.ScaleModeChanged();
                }
                else if (e.Property == editorState.GetProperty(_ => _.TileSetMode))
                {
                    this.TileSetModeChanged();
                }
            };
            this.MapTreeView.ViewModel = this.ViewModel;
            this.MapEditor.ViewModel = this.ViewModel;
            this.TileSetPalette.ViewModel = this.ViewModel;

            this.IsOpenedChanged();
            this.ResumeLayout(false);
        }

        // TODO: remove ViewModel
        public ViewModel ViewModel { get; private set; }

        private MapEditor MapEditor;

        private IEnumerable<ToolStripButton> LayerModeSwitchers
        {
            get
            {
                yield return this.Layer1ToolStripButton;
                yield return this.Layer2ToolStripButton;
                yield return this.EventToolStripButton;
            }
        }

        private IEnumerable<ToolStripButton> DrawingModeSwitchers
        {
            get
            {
                yield return this.PenToolStripButton;
            }
        }

        private IEnumerable<ToolStripButton> ScaleModeSwitchers
        {
            get
            {
                yield return this.Scale1ToolStripButton;
                yield return this.Scale2ToolStripButton;
                yield return this.Scale4ToolStripButton;
                yield return this.Scale8ToolStripButton;
            }
        }

        private void IsOpenedChanged()
        {
            bool isOpened = this.ViewModel.IsOpened;
            Debug.Assert((isOpened == true && !this.ViewModel.IsDirty) || isOpened == false);

            if (isOpened)
            {
                this.SetTileSetSelectorItems(this.ViewModel.TileSetCollection.Items.Select(t => t.Id.ToString()));
            }
            else
            {
                this.SetTileSetSelectorItems(Enumerable.Empty<string>());
            }

            this.IsMapTreeViewEnabled = isOpened;
            this.IsNewButtonEnabled = !isOpened;
            this.IsOpenButtonEnabled = !isOpened;
            this.IsCloseButtonEnabled = isOpened;
            this.IsSaveButtonEnabled = isOpened;
            this.IsUndoButtonEnabled = isOpened;
            foreach (LayerMode layerMode in Enum.GetValues(typeof(LayerMode)))
            {
                this.SetLayerModeSwitcherEnabled(layerMode, isOpened);
            }
            foreach (DrawingMode drawingMode in Enum.GetValues(typeof(DrawingMode)))
            {
                this.SetDrawingModeSwitcherEnabled(drawingMode, isOpened);
            }
            foreach (ScaleMode scaleMode in Enum.GetValues(typeof(ScaleMode)))
            {
                this.SetScaleModeSwitcherEnabled(scaleMode, isOpened);
            }
            this.IsTileSetPaletteEnabled = isOpened;
            this.IsTileSetSelectorEnabled = isOpened;
            this.IsPassageButtonEnabled = isOpened;

            // this.IsUndoableChanged(); // TODO: あとでふっかつさせる
            // this.GameTitleChanged(); // TODO: あとでふっかつさせる
            this.MapIdChanged();
            this.SelectedTileSetIdsChanged();
            this.LayerModeChanged();
            this.DrawingModeChanged();
            this.ScaleModeChanged();
            this.TileSetModeChanged();

            // To prevent the map editor from being edited wrongly
            Application.DoEvents();
            this.MapEditor.Enabled = isOpened;
        }

        private void MapIdChanged()
        {
            this.AdjustTileSetsToolStripComboBox();
            this.TileSetsToolStripComboBox.Enabled =
                this.ViewModel.EditorState.Map != null;
        }

        private void SelectedTileSetIdsChanged()
        {
            this.AdjustTileSetsToolStripComboBox();
        }

        private void AdjustTileSetsToolStripComboBox()
        {
            if (this.ViewModel.IsOpened)
            {
                int tileSetId = this.ViewModel.EditorState.SelectedTileSetId;
                int index = 0;
                for (int i = 0; i < this.TileSetsToolStripComboBox.Items.Count; i++)
                {
                    if (int.Parse((string)this.TileSetsToolStripComboBox.Items[i]) == tileSetId)
                    {
                        index = i;
                        break;
                    }
                }
                this.TileSetSelectorSelectedIndex = index;
            }
        }

        private void LayerModeChanged()
        {
            if (this.ViewModel.IsOpened)
            {
                foreach (var item in this.LayerModeSwitchers)
                {
                    item.Checked =
                        ((LayerMode)item.Tag == this.ViewModel.EditorState.LayerMode);
                }
            }
            else
            {
                foreach (var item in this.LayerModeSwitchers)
                {
                    item.Checked = false;
                }
            }
        }

        private void DrawingModeChanged()
        {
            if (this.ViewModel.IsOpened)
            {
                foreach (var item in this.DrawingModeSwitchers)
                {
                    item.Checked =
                        ((DrawingMode)item.Tag == this.ViewModel.EditorState.DrawingMode);
                }
            }
            else
            {
                foreach (var item in this.DrawingModeSwitchers)
                {
                    item.Checked = false;
                }
            }
        }

        private void ScaleModeChanged()
        {
            if (this.ViewModel.IsOpened)
            {
                foreach (var item in this.ScaleModeSwitchers)
                {
                    item.Checked =
                        ((ScaleMode)item.Tag == this.ViewModel.EditorState.ScaleMode);
                }
            }
            else
            {
                foreach (var item in this.ScaleModeSwitchers)
                {
                    item.Checked = false;
                }
            }
        }

        private void TileSetModeChanged()
        {
            if (this.ViewModel.IsOpened)
            {
                this.IsPassageButtonChecked =
                    (this.ViewModel.EditorState.TileSetMode == TileSetMode.Passage);
            }
            else
            {
                this.IsPassageButtonChecked = false;
            }
        }

        private void NewToolStripButton_Click(object sender, EventArgs e)
        {
            this.OnNewButtonClicked(EventArgs.Empty);
        }

        private void OpenToolStripButton_Click(object sender, EventArgs e)
        {
            this.OnOpenButtonClicked(EventArgs.Empty);
        }

        private void CloseToolStripButton_Click(object sender, EventArgs e)
        {
            this.OnCloseButtonClicked(EventArgs.Empty);
        }

        private void SaveToolStripButton_Click(object sender, EventArgs e)
        {
            this.OnSaveButtonClicked(EventArgs.Empty);
        }

        private void UndoToolStripButton_Click(object sender, EventArgs e)
        {
            this.OnUndoButtonClicked(EventArgs.Empty);
        }

        private void PassageToolStripButton_Click(object sender, EventArgs e)
        {
            this.OnPassageButtonClicked(EventArgs.Empty);
        }

        private void TileSetsToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = this.TileSetSelectorSelectedIndex;
            if (selectedIndex != -1)
            {
                Map map = this.ViewModel.EditorState.Map;
                if (map != null)
                {
                    int mapId = map.Id;
                    int tileSetId = int.Parse((string)this.TileSetsToolStripComboBox.SelectedItem);
                    this.ViewModel.EditorState.SetSelectedTileSetId(mapId, tileSetId);
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.ViewModel.IsDirty)
            {
                DialogResult result = MessageBox.Show("Save?", "",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1);
                switch (result)
                {
                case DialogResult.Yes:
                    this.ViewModel.Save();
                    break;
                case DialogResult.No:
                    break;
                case DialogResult.Cancel:
                    e.Cancel = true;
                    return;
                }
            }
        }
    }
}
