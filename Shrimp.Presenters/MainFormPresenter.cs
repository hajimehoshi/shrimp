﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Shrimp.IViews;
using Shrimp.Models;

namespace Shrimp.Presenters
{
    public class MainFormPresenter
    {
        public MainFormPresenter(IMainForm mainForm, ViewModel viewModel)
        {
            this.MainForm = mainForm;
            this.ViewModel = viewModel;
            this.MainForm.CloseButtonClick += this.MainForm_CloseButtonClick;
            this.MainForm.DrawingModeSwitcherClick += this.MainForm_DrawingModeSwitcherClick;
            this.MainForm.FormClosing += (sender, e) =>
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
            };
            this.MainForm.LayerModeSwitcherClick += this.MainForm_LayerModeSwitcherClick;
            this.MainForm.NewButtonClick += this.MainForm_NewButtonClick;
            this.MainForm.OpenButtonClick += this.MainForm_OpenButtonClick;
            this.MainForm.PassageButtonClick += this.MainForm_PassageButtonClick;
            this.MainForm.SaveButtonClick += this.MainForm_SaveButtonClick;
            this.MainForm.ScaleModeSwitcherClick += this.MainForm_ScaleModeSwitcherClick;
            this.MainForm.TileSetSelectorSelectedIndexChanged += this.MainForm_TileSetSelectorSelectedIndexChanged;
            this.MainForm.UndoButtonClick += this.MainForm_UndoButtonClick;

            this.ViewModel.IsOpenedChanged += delegate
            {
                this.IsOpenedChanged();
            };
            this.ViewModel.IsUndoableChanged += delegate
            {
                this.IsUndoableChanged();
            };
            this.ViewModel.Project.Updated += (sender, e) =>
            {
                Project project = (Project)sender;
                if (e.Property == project.GetProperty(_ => _.GameTitle))
                {
                    this.GameTitleChanged();
                }
            };
            this.ViewModel.EditorState.Updated += (sender, e) =>
            {
                EditorState editorState = (EditorState)sender;
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
            this.IsOpenedChanged();
            this.MapTreeViewPresenter = new MapTreeViewPresenter(this.MainForm.MapTreeView, this.ViewModel);
            this.MapEditorPresenter = new MapEditorPresenter(this.MainForm.MapEditor, this.ViewModel);
            this.TileSetPalettePresenter = new TileSetPalettePresenter(this.MainForm.TileSetPalette, this.ViewModel);
        }
        private MapTreeViewPresenter MapTreeViewPresenter;
        private MapEditorPresenter MapEditorPresenter;
        private TileSetPalettePresenter TileSetPalettePresenter;

        private IMainForm MainForm;

        private ViewModel ViewModel;

        private void MainForm_CloseButtonClick(object sender, EventArgs e)
        {
            Debug.Assert(this.ViewModel.IsOpened);
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
                    return;
                }
            }
            this.ViewModel.Close();
            Debug.Assert(!this.ViewModel.IsOpened);
            Debug.Assert(!this.ViewModel.IsDirty);
        }

        private void MainForm_DrawingModeSwitcherClick(object sender, DrawingModeSwitcherClickEventArgs e)
        {
            this.ViewModel.EditorState.DrawingMode = e.DrawingMode;
        }

        private void MainForm_LayerModeSwitcherClick(object sender, LayerModeSwitcherClickEventArgs e)
        {
            this.ViewModel.EditorState.LayerMode = e.LayerMode;
        }

        private void MainForm_NewButtonClick(object sender, EventArgs e)
        {
            Debug.Assert(!this.ViewModel.IsOpened);
            using (var dialog = this.MainForm.CreateNewProjectDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string directoryPath = Path.Combine(dialog.BasePath, dialog.DirectoryName);
                    this.ViewModel.New(directoryPath, dialog.GameTitle);
                    Debug.Assert(this.ViewModel.IsOpened);
                    Debug.Assert(!this.ViewModel.IsDirty);
                }
            }
        }

        private void MainForm_OpenButtonClick(object sender, EventArgs e)
        {
            Debug.Assert(!this.ViewModel.IsOpened);
            if (this.MainForm.OpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.ViewModel.Open(this.MainForm.OpenFileDialog.FileName);
                Debug.Assert(this.ViewModel.IsOpened);
                // Debug.Assert(!this.ViewModel.IsDirty);
            }
        }

        private void MainForm_PassageButtonClick(object sender, EventArgs e)
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

        private void MainForm_SaveButtonClick(object sender, EventArgs e)
        {
            Debug.Assert(this.ViewModel.IsOpened);
            if (this.ViewModel.IsDirty)
            {
                this.ViewModel.Save();
            }
            Debug.Assert(this.ViewModel.IsOpened);
            Debug.Assert(!this.ViewModel.IsDirty);
        }

        private void MainForm_ScaleModeSwitcherClick(object sender, ScaleModeSwitcherClickEventArgs e)
        {
            this.ViewModel.EditorState.ScaleMode = e.ScaleMode;
        }

        private void MainForm_TileSetSelectorSelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = this.MainForm.TileSetSelectorSelectedIndex;
            if (selectedIndex != -1)
            {
                Map map = this.ViewModel.EditorState.Map;
                if (map != null)
                {
                    int mapId = map.Id;
                    int tileSetId = int.Parse(this.MainForm.TileSetSelectorSelectedItem);
                    this.ViewModel.EditorState.SetSelectedTileSetId(mapId, tileSetId);
                }
            }
        }

        private void MainForm_UndoButtonClick(object sender, EventArgs e)
        {
            Debug.Assert(this.ViewModel.IsOpened);
            Debug.Assert(this.ViewModel.IsUndoable);
            Debug.Assert(this.ViewModel.EditorState.Map != null);
            this.ViewModel.Undo();
            Debug.Assert(this.ViewModel.IsOpened);
        }

        public void Run()
        {
            this.MainForm.Run();
        }

        private void IsOpenedChanged()
        {
            bool isOpened = this.ViewModel.IsOpened;
            Debug.Assert((isOpened == true && !this.ViewModel.IsDirty) || isOpened == false);

            if (isOpened)
            {
                this.MainForm.SetTileSetSelectorItems(this.ViewModel.TileSetCollection.Items.Select(t => t.Id.ToString()));
            }
            else
            {
                this.MainForm.SetTileSetSelectorItems(Enumerable.Empty<string>());
            }

            this.MainForm.IsMapTreeViewEnabled = isOpened;
            this.MainForm.IsNewButtonEnabled = !isOpened;
            this.MainForm.IsOpenButtonEnabled = !isOpened;
            this.MainForm.IsCloseButtonEnabled = isOpened;
            this.MainForm.IsSaveButtonEnabled = isOpened;
            this.MainForm.IsUndoButtonEnabled = isOpened;
            foreach (LayerMode layerMode in Enum.GetValues(typeof(LayerMode)))
            {
                this.MainForm.SetLayerModeSwitcherEnabled(layerMode, isOpened);
            }
            foreach (DrawingMode drawingMode in Enum.GetValues(typeof(DrawingMode)))
            {
                this.MainForm.SetDrawingModeSwitcherEnabled(drawingMode, isOpened);
            }
            foreach (ScaleMode scaleMode in Enum.GetValues(typeof(ScaleMode)))
            {
                this.MainForm.SetScaleModeSwitcherEnabled(scaleMode, isOpened);
            }
            this.MainForm.IsTileSetPaletteEnabled = isOpened;
            this.MainForm.IsTileSetSelectorEnabled = isOpened;
            this.MainForm.IsPassageButtonEnabled = isOpened;

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
            this.MainForm.IsMapEditorEnabled = isOpened;
        }

        private void IsUndoableChanged()
        {
            this.MainForm.IsUndoButtonEnabled = this.ViewModel.IsOpened && this.ViewModel.IsUndoable;
        }

        private void GameTitleChanged()
        {
            if (this.ViewModel.IsOpened)
            {
                this.MainForm.Text = this.ViewModel.Project.GameTitle + " - Shrimp";
            }
            else
            {
                this.MainForm.Text = "Shrimp";
            }
        }

        private void MapIdChanged()
        {
            this.AdjustTileSetsToolStripComboBox();
            this.MainForm.IsTileSetSelectorEnabled = (this.ViewModel.EditorState.Map != null);
            this.MainForm.IsPassageButtonEnabled = (this.ViewModel.EditorState.Map != null);
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
                var items = this.MainForm.GetTileSetSelectorItems().ToArray();
                for (int i = 0; i < items.Length; i++)
                {
                    if (int.Parse((string)items[i]) == tileSetId)
                    {
                        index = i;
                        break;
                    }
                }
                this.MainForm.TileSetSelectorSelectedIndex = index;
            }
        }

        private void LayerModeChanged()
        {
            if (this.ViewModel.IsOpened)
            {
                foreach (LayerMode layerMode in Enum.GetValues(typeof(LayerMode)))
                {
                    bool isChecked = (layerMode == this.ViewModel.EditorState.LayerMode);
                    this.MainForm.SetLayerModeSwitcherChecked(layerMode, isChecked);
                }
            }
            else
            {
                foreach (LayerMode layerMode in Enum.GetValues(typeof(LayerMode)))
                {
                    this.MainForm.SetLayerModeSwitcherChecked(layerMode, false);
                }
            }
        }

        private void DrawingModeChanged()
        {
            if (this.ViewModel.IsOpened)
            {
                foreach (DrawingMode drawingMode in Enum.GetValues(typeof(DrawingMode)))
                {
                    bool isChecked = (drawingMode == this.ViewModel.EditorState.DrawingMode);
                    this.MainForm.SetDrawingModeSwitcherChecked(drawingMode, isChecked);
                }
            }
            else
            {
                foreach (DrawingMode drawingMode in Enum.GetValues(typeof(DrawingMode)))
                {
                    this.MainForm.SetDrawingModeSwitcherChecked(drawingMode, false);
                }
            }
        }

        private void ScaleModeChanged()
        {
            if (this.ViewModel.IsOpened)
            {
                foreach (ScaleMode scaleMode in Enum.GetValues(typeof(ScaleMode)))
                {
                    bool isChecked = (scaleMode == this.ViewModel.EditorState.ScaleMode);
                    this.MainForm.SetScaleModeSwitcherChecked(scaleMode, isChecked);
                }
            }
            else
            {
                foreach (ScaleMode scaleMode in Enum.GetValues(typeof(ScaleMode)))
                {
                    this.MainForm.SetScaleModeSwitcherChecked(scaleMode, false);
                }
            }
        }

        private void TileSetModeChanged()
        {
            if (this.ViewModel.IsOpened)
            {
                this.MainForm.IsPassageButtonChecked =
                    (this.ViewModel.EditorState.TileSetMode == TileSetMode.Passage);
            }
            else
            {
                this.MainForm.IsPassageButtonChecked = false;
            }
        }

    }
}
