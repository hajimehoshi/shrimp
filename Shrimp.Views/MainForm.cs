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

        public event EventHandler CloseButtonClick;
        protected virtual void OnCloseButtonClick(EventArgs e)
        {
            if (this.CloseButtonClick != null)
            {
                this.CloseButtonClick(this, e);
            }
        }

        public event EventHandler<DrawingModeSwitcherClickEventArgs> DrawingModeSwitcherClick;
        protected virtual void OnDrawingModeSwitcherClick(DrawingModeSwitcherClickEventArgs e)
        {
            if (this.DrawingModeSwitcherClick != null)
            {
                this.DrawingModeSwitcherClick(this, e);
            }
        }

        public event EventHandler<LayerModeSwitcherClickEventArgs> LayerModeSwitcherClick;
        protected virtual void OnLayerModeSwitcherClick(LayerModeSwitcherClickEventArgs e)
        {
            if (this.LayerModeSwitcherClick != null)
            {
                this.LayerModeSwitcherClick(this, e);
            }
        }

        public event EventHandler NewButtonClick;
        protected virtual void OnNewButtonClick(EventArgs e)
        {
            if (this.NewButtonClick != null)
            {
                this.NewButtonClick(this, e);
            }
        }

        public event EventHandler OpenButtonClick;
        protected virtual void OnOpenButtonClick(EventArgs e)
        {
            if (this.OpenButtonClick != null)
            {
                this.OpenButtonClick(this, e);
            }
        }

        public event EventHandler PassageButtonClick;
        protected virtual void OnPassageButtonClick(EventArgs e)
        {
            if (this.PassageButtonClick != null) { this.PassageButtonClick(this, e); }
        }

        public event EventHandler SaveButtonClick;
        protected virtual void OnSaveButtonClick(EventArgs e)
        {
            if (this.SaveButtonClick != null)
            {
                this.SaveButtonClick(this, e);
            }
        }

        public event EventHandler<ScaleModeSwitcherClickEventArgs> ScaleModeSwitcherClick;
        protected virtual void OnScaleModeSwitcherClick(ScaleModeSwitcherClickEventArgs e)
        {
            if (this.ScaleModeSwitcherClick != null)
            {
                this.ScaleModeSwitcherClick(this, e);
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
        
        public event EventHandler UndoButtonClick;
        protected virtual void OnUndoButtonClick(EventArgs e)
        {
            if (this.UndoButtonClick != null)
            {
                this.UndoButtonClick(this, e);
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
            get { return this.mapTreeView.Enabled; }
            set { this.mapTreeView.Enabled = value; }
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
            get { return this.tileSetPalette.Enabled; }
            set { this.tileSetPalette.Enabled = value; }
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

        public IMapTreeView MapTreeView
        {
            get { return this.mapTreeView; }
        }

        public OpenFileDialog OpenFileDialog
        {
            get { return this.openFileDialog; }
        }

        public ITileSetPalette TileSetPalette
        {
            get { return this.tileSetPalette; }
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

            this.SuspendLayout();

            this.mapTreeView = new MapTreeView();
            this.LeftSplitContainer.Panel2.Controls.Add(this.mapTreeView);
            this.mapTreeView.BorderStyle = BorderStyle.None;
            this.mapTreeView.Dock = DockStyle.Fill;
            this.mapTreeView.DrawMode = TreeViewDrawMode.OwnerDrawAll;
            this.mapTreeView.FullRowSelect = true;
            this.mapTreeView.HideSelection = false;
            this.mapTreeView.ImageKey = "PageWhite"; // unused?
            this.mapTreeView.ItemHeight = 21;
            this.mapTreeView.ShowLines = false;
            this.mapTreeView.ShowRootLines = false;

            this.mapEditor = new MapEditor();
            this.mapEditor.BorderStyle = BorderStyle.Fixed3D;
            this.mapEditor.Dock = DockStyle.Fill;
            this.MainSplitContainer.Panel2.Controls.Add(this.mapEditor);

            this.tileSetPalette = new Shrimp.Views.TileSetPalette();
            this.tileSetPalette.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            this.tileSetPalette.AutoScroll = true;
            this.LeftSplitContainer.Panel1.Controls.Add(this.tileSetPalette);

            this.ToolStrip.Renderer = new CustomToolStripSystemRenderer();
            this.TileSetPaletteToolStrip.Renderer = new CustomToolStripSystemRenderer();
            this.tileSetPalette.Size = new Size
            {
                Width = Util.PaletteGridSize * Util.PaletteHorizontalCount
                    + SystemInformation.VerticalScrollBarWidth,
                Height = this.tileSetPalette.Parent.ClientSize.Height
                    - this.TileSetPaletteToolStrip.Height,
            };
            this.MainSplitContainer.SplitterDistance -=
                this.tileSetPalette.Parent.ClientSize.Width - this.tileSetPalette.Width;

            this.ResumeLayout(false);

            this.Layer1ToolStripButton.Tag = LayerMode.Layer1;
            this.Layer2ToolStripButton.Tag = LayerMode.Layer2;
            this.LayerEventToolStripButton.Tag = LayerMode.Event;
            foreach (var item in this.LayerModeSwitchers)
            {
                item.Click += (sender, e) =>
                {
                    LayerMode layerMode = (LayerMode)((ToolStripButton)sender).Tag;
                    this.OnLayerModeSwitcherClick(new LayerModeSwitcherClickEventArgs(layerMode));
                };
            }

            this.PenToolStripButton.Tag = DrawingMode.Pen;
            foreach (var item in this.DrawingModeSwitchers)
            {
                item.Click += (sender, e) =>
                {
                    DrawingMode drawingMode = (DrawingMode)((ToolStripButton)sender).Tag;
                    this.OnDrawingModeSwitcherClick(new DrawingModeSwitcherClickEventArgs(drawingMode));
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
                    this.OnScaleModeSwitcherClick(new ScaleModeSwitcherClickEventArgs(scaleMode));
                };
            }
        }

        private MapEditor mapEditor;
        private MapTreeView mapTreeView;
        private TileSetPalette tileSetPalette;

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
            this.OnNewButtonClick(EventArgs.Empty);
        }

        private void OpenToolStripButton_Click(object sender, EventArgs e)
        {
            this.OnOpenButtonClick(EventArgs.Empty);
        }

        private void CloseToolStripButton_Click(object sender, EventArgs e)
        {
            this.OnCloseButtonClick(EventArgs.Empty);
        }

        private void SaveToolStripButton_Click(object sender, EventArgs e)
        {
            this.OnSaveButtonClick(EventArgs.Empty);
        }

        private void UndoToolStripButton_Click(object sender, EventArgs e)
        {
            this.OnUndoButtonClick(EventArgs.Empty);
        }

        private void PassageToolStripButton_Click(object sender, EventArgs e)
        {
            this.OnPassageButtonClick(EventArgs.Empty);
        }

        private void TileSetsToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.OnTileSetSelectorSelectedIndexChanged(EventArgs.Empty);
        }
    }
}
