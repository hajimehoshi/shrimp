﻿using System;
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

        public event EventHandler NewButtonClicked;
        protected void OnNewButtonClicked(EventArgs e)
        {
            if (this.NewButtonClicked != null) { this.NewButtonClicked(this, e); }
        }

        public event EventHandler OpenButtonClicked;
        protected void OnOpenButtonClicked(EventArgs e)
        {
            if (this.OpenButtonClicked != null) { this.OpenButtonClicked(this, e); }
        }

        public event EventHandler CloseButtonClicked;
        protected void OnCloseButtonClicked(EventArgs e)
        {
            if (this.CloseButtonClicked != null) { this.CloseButtonClicked(this, e); }
        }

        public event EventHandler SaveButtonClicked;
        protected void OnSaveButtonClicked(EventArgs e)
        {
            if (this.SaveButtonClicked != null) { this.SaveButtonClicked(this, e); }
        }
        
        public event EventHandler UndoButtonClicked;
        protected void OnUndoButtonClicked(EventArgs e)
        {
            if (this.UndoButtonClicked != null) { this.UndoButtonClicked(this, e); }
        }

        public event EventHandler<LayerSwitcherClickedEventArgs> LayerSwitcherClicked;
        protected void OnLayerSwitcherClicked(LayerSwitcherClickedEventArgs e)
        {
            if (this.LayerSwitcherClicked != null) { this.LayerSwitcherClicked(this, e); }
        }

        public event EventHandler PassageButtonClicked;
        protected void OnPassageButtonClicked(EventArgs e)
        {
            if (this.PassageButtonClicked != null) { this.PassageButtonClicked(this, e); }
        }

        public event EventHandler SelectedTileSetChanged;
        protected void OnSelectedTileSetChanged(EventArgs e)
        {
            if (this.SelectedTileSetChanged != null) { this.SelectedTileSetChanged(this, e); }
        }

        public INewProjectDialog CreateNewProjectDialog()
        {
            return new NewProjectDialog();
        }

        public OpenFileDialog OpenFileDialog
        {
            get { return this.openFileDialog; }
        }

        public void Run()
        {
            Application.Run(this);
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
                item.Click += (s, e) =>
                {
                    LayerMode layerMode = (LayerMode)((ToolStripButton)s).Tag;
                    this.OnLayerSwitcherClicked(new LayerSwitcherClickedEventArgs(layerMode));
                };
            }

            this.PenToolStripButton.Tag = DrawingMode.Pen;
            foreach (var item in this.DrawingModeSwitchers)
            {
                item.Click += (s, e) =>
                {
                    this.ViewModel.EditorState.DrawingMode =
                        (DrawingMode)((ToolStripButton)s).Tag;
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
                    this.ViewModel.EditorState.ScaleMode =
                        (ScaleMode)((ToolStripButton)s).Tag;
                };
            }

            this.ViewModel = new ViewModel();
            this.ViewModel.IsOpenedChanged += delegate
            {
                this.IsOpenedChanged();
            };
            this.ViewModel.IsDirtyChanged += delegate
            {
                this.IsDirtyChanged();
            };
            this.ViewModel.IsUndoableChanged += delegate
            {
                this.IsUndoableChanged();
            };
            this.ViewModel.Project.Updated += (s, e) =>
            {
                Project project = (Project)s;
                if (e.Property == project.GetProperty(_ => _.GameTitle))
                {
                    this.GameTitleChanged();
                }
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

            this.TileSetsToolStripComboBox.Items.Clear();
            if (isOpened)
            {
                this.TileSetsToolStripComboBox.BeginUpdate();
                var tileSets = this.ViewModel.TileSetCollection.Items.ToArray();
                Dictionary<int, int> indexToId = new Dictionary<int, int>();
                for (int i = 0; i < tileSets.Length; i++)
                {
                    TileSet tileSet = tileSets[i];
                    indexToId.Add(i, tileSet.Id);
                    this.TileSetsToolStripComboBox.Items.Add(tileSet.Id.ToString());
                }
                this.TileSetsToolStripComboBox.Tag = indexToId;
                this.TileSetsToolStripComboBox.EndUpdate();
            }

            this.MapTreeView.Enabled = isOpened;
            this.NewToolStripButton.Enabled = !isOpened;
            this.OpenToolStripButton.Enabled = !isOpened;
            this.CloseToolStripButton.Enabled = isOpened;
            this.SaveToolStripButton.Enabled = isOpened;
            this.UndoToolStripButton.Enabled = isOpened;
            foreach (var item in this.LayerModeSwitchers)
            {
                item.Enabled = isOpened;
            }
            foreach (var item in this.DrawingModeSwitchers)
            {
                item.Enabled = isOpened;
            }
            foreach (var item in this.ScaleModeSwitchers)
            {
                item.Enabled = isOpened;
            }
            this.TileSetPalette.Enabled = isOpened;
            this.TileSetPaletteToolStrip.Enabled = isOpened;
            this.PassageToolStripButton.Enabled = isOpened;

            this.IsDirtyChanged();
            this.IsUndoableChanged();
            this.GameTitleChanged();
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

        private void IsDirtyChanged()
        {
        }

        private void IsUndoableChanged()
        {
            this.UndoToolStripButton.Enabled = this.ViewModel.IsOpened && this.ViewModel.IsUndoable;
        }

        private void GameTitleChanged()
        {
            if (this.ViewModel.IsOpened)
            {
                this.Text = this.ViewModel.Project.GameTitle + " - Shrimp";
            }
            else
            {
                this.Text = "Shrimp";
            }
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
                var indexToId = (Dictionary<int, int>)this.TileSetsToolStripComboBox.Tag;
                int index = (from p in indexToId
                             where p.Value == tileSetId
                             select p.Key).FirstOrDefault();
                this.TileSetsToolStripComboBox.SelectedIndex = index;
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
                this.PassageToolStripButton.Checked =
                    (this.ViewModel.EditorState.TileSetMode == TileSetMode.Passage);
            }
            else
            {
                this.PassageToolStripButton.Checked = false;
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
            switch (this.ViewModel.EditorState.TileSetMode)
            {
            case TileSetMode.Normal:
                this.ViewModel.EditorState.TileSetMode = TileSetMode.Passage;
                break;
            case TileSetMode.Passage:
                this.ViewModel.EditorState.TileSetMode = TileSetMode.Normal;
                break;
            }
        }

        private void TileSetsToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = this.TileSetsToolStripComboBox.SelectedIndex;
            if (selectedIndex != -1)
            {
                Map map = this.ViewModel.EditorState.Map;
                if (map != null)
                {
                    int mapId = map.Id;
                    var indexToId = (Dictionary<int, int>)this.TileSetsToolStripComboBox.Tag;
                    int tileSetId = indexToId[selectedIndex];
                    this.ViewModel.EditorState.SetSelectedTileSetId(mapId, tileSetId);
                }
                if (this.TileSetsToolStripComboBox.Focused)
                {
                    this.TileSetPalette.Focus();
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
