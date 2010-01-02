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

        public event EventHandler<QuittingEventArgs> Quitting;
        protected virtual void OnQuitting(QuittingEventArgs e)
        {
            if (this.Quitting != null)
            {
                this.Quitting(this, e);
            }
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
            if (this.SelectedTileSetChanged != null)
            {
                this.SelectedTileSetChanged(this, e);
            }
        }

        public event EventHandler TileSetSelectorSelectedIndexChanged;
        protected virtual void OnTileSetSelectorSelectedIndexChanged(EventArgs e)
        {
            if (this.TileSetSelectorSelectedIndexChanged != null)
            {
                this.TileSetSelectorSelectedIndexChanged(this, e);
            }
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

        public IEnumerable<string> GetTileSetSelectorItems()
        {
            return this.TileSetsToolStripComboBox.Items.Cast<string>();
        }

        public void Run()
        {
            Application.Run(this);
        }

        public void SetDrawingModeSwitcherChecked(DrawingMode drawingMode, bool isChecked)
        {
            this.DrawingModeSwitchers.Where(s => (DrawingMode)s.Tag == drawingMode).First().Checked = isChecked;
        }

        public void SetDrawingModeSwitcherEnabled(DrawingMode drawingMode, bool isEnabled)
        {
            this.DrawingModeSwitchers.Where(s => (DrawingMode)s.Tag == drawingMode).First().Enabled = isEnabled;
        }

        public void SetLayerModeSwitcherChecked(LayerMode layerMode, bool isChecked)
        {
            this.LayerModeSwitchers.Where(s => (LayerMode)s.Tag == layerMode).First().Checked = isChecked;
        }

        public void SetLayerModeSwitcherEnabled(LayerMode layerMode, bool isEnabled)
        {
            this.LayerModeSwitchers.Where(s => (LayerMode)s.Tag == layerMode).First().Enabled = isEnabled;
        }

        public void SetScaleModeSwitcherChecked(ScaleMode scaleMode, bool isChecked)
        {
            this.ScaleModeSwitchers.Where(s => (ScaleMode)s.Tag == scaleMode).First().Checked = isChecked;
        }

        public void SetScaleModeSwitcherEnabled(ScaleMode scaleMode, bool isEnabled)
        {
            this.ScaleModeSwitchers.Where(s => (ScaleMode)s.Tag == scaleMode).First().Enabled = isEnabled;
        }

        public void SetTileSetSelectorItems(IEnumerable<string> items)
        {
            this.TileSetsToolStripComboBox.BeginUpdate();
            this.TileSetsToolStripComboBox.Items.Clear();
            this.TileSetsToolStripComboBox.Items.AddRange(items.ToArray());
            this.TileSetsToolStripComboBox.EndUpdate();
        }

        public bool IsCloseButtonEnabled
        {
            get { return this.CloseToolStripButton.Enabled; }
            set { this.CloseToolStripButton.Enabled = value; }
        }

        public bool IsMapEditorEnabled
        {
            get { return this.mapEditor.Enabled; }
            set { this.mapEditor.Enabled = value; }
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

        public IMapEditor MapEditor
        {
            get { return this.mapEditor; }
        }

        public OpenFileDialog OpenFileDialog
        {
            get { return this.openFileDialog; }
        }

        public int TileSetSelectorSelectedIndex
        {
            get { return this.TileSetsToolStripComboBox.SelectedIndex; }
            set { this.TileSetsToolStripComboBox.SelectedIndex = value; }
        }

        public string TileSetSelectorSelectedItem
        {
            get { return (string)this.TileSetsToolStripComboBox.SelectedItem; }
        }

        private class CustomToolStripSystemRenderer : ToolStripSystemRenderer
        {
            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
            {
                // Do nothing
            }
        }

        public MainForm(ViewModel viewModel)
        {
            this.InitializeComponent();

            this.mapEditor = new MapEditor();
            this.mapEditor.BorderStyle = BorderStyle.Fixed3D;
            this.mapEditor.Dock = DockStyle.Fill;
            this.MainSplitContainer.Panel2.Controls.Add(this.mapEditor);

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
            this.LayerEventToolStripButton.Tag = LayerMode.Event;
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
                item.Click += (sender, e) =>
                {
                    ScaleMode scaleMode = (ScaleMode)((ToolStripButton)sender).Tag;
                    this.OnScaleModeSwitcherClicked(new ScaleModeSwitcherClickedEventArgs(scaleMode));
                };
            }

            this.MapTreeView.ViewModel = viewModel;
            this.mapEditor.ViewModel = viewModel;
            this.TileSetPalette.ViewModel = viewModel;
        }

        private MapEditor mapEditor;

        private IEnumerable<ToolStripButton> LayerModeSwitchers
        {
            get
            {
                yield return this.Layer1ToolStripButton;
                yield return this.Layer2ToolStripButton;
                yield return this.LayerEventToolStripButton;
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
            this.OnTileSetSelectorSelectedIndexChanged(EventArgs.Empty);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            QuittingEventArgs e2 = new QuittingEventArgs();
            this.OnQuitting(e2);
            e.Cancel = e2.Cancel;
        }
    }
}
