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
            switch (drawingMode)
            {
            case DrawingMode.Pen:
                this.PenToolStripButton.Checked = isChecked;
                break;
            }
        }

        public void SetDrawingModeSwitcherEnabled(DrawingMode drawingMode, bool isEnabled)
        {
            switch (drawingMode)
            {
            case DrawingMode.Pen:
                this.PenToolStripButton.Enabled = isEnabled;
                break;
            }
        }

        public void SetLayerModeSwitcherChecked(LayerMode layerMode, bool isChecked)
        {
            switch (layerMode)
            {
            case LayerMode.Layer1:
                this.Layer1ToolStripButton.Checked = isChecked;
                break;
            case LayerMode.Layer2:
                this.Layer2ToolStripButton.Checked = isChecked;
                break;
            case LayerMode.Event:
                this.EventToolStripButton.Checked = isChecked;
                break;
            }
        }

        public void SetLayerModeSwitcherEnabled(LayerMode layerMode, bool isEnabled)
        {
            switch (layerMode)
            {
            case LayerMode.Layer1:
                this.Layer1ToolStripButton.Enabled = isEnabled;
                break;
            case LayerMode.Layer2:
                this.Layer2ToolStripButton.Enabled = isEnabled;
                break;
            case LayerMode.Event:
                this.EventToolStripButton.Enabled = isEnabled;
                break;
            }
        }

        public void SetScaleModeSwitcherChecked(ScaleMode scaleMode, bool isChecked)
        {
            switch (scaleMode)
            {
            case ScaleMode.Scale1:
                this.Scale1ToolStripButton.Checked = isChecked;
                break;
            case ScaleMode.Scale2:
                this.Scale2ToolStripButton.Checked = isChecked;
                break;
            case ScaleMode.Scale4:
                this.Scale4ToolStripButton.Checked = isChecked;
                break;
            case ScaleMode.Scale8:
                this.Scale8ToolStripButton.Checked = isChecked;
                break;
            }
        }

        public void SetScaleModeSwitcherEnabled(ScaleMode scaleMode, bool isEnabled)
        {
            switch (scaleMode)
            {
            case ScaleMode.Scale1:
                this.Scale1ToolStripButton.Enabled = isEnabled;
                break;
            case ScaleMode.Scale2:
                this.Scale2ToolStripButton.Enabled = isEnabled;
                break;
            case ScaleMode.Scale4:
                this.Scale4ToolStripButton.Enabled = isEnabled;
                break;
            case ScaleMode.Scale8:
                this.Scale8ToolStripButton.Enabled = isEnabled;
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

        public bool IsMapEditorEnabled
        {
            get { return this.MapEditor.Enabled; }
            set { this.MapEditor.Enabled = value; }
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

        public MainForm()
        {
            this.InitializeComponent();

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
            
            this.MapTreeView.ViewModel = this.ViewModel;
            this.MapEditor.ViewModel = this.ViewModel;
            this.TileSetPalette.ViewModel = this.ViewModel;
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
