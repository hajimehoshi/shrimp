using System;
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
    public class MainFormPresenter : IDisposable
    {
        public MainFormPresenter(IMainForm mainForm, ViewModel viewModel)
        {
            this.MainForm = mainForm;
            this.ViewModel = viewModel;
            this.MainForm.CloseButtonClicked += this.MainForm_CloseButtonClicked;
            this.MainForm.DrawingModeSwitcherClicked += this.MainForm_DrawingModeSwitcherClicked;
            this.MainForm.LayerModeSwitcherClicked += this.MainForm_LayerModeSwitcherClicked;
            this.MainForm.NewButtonClicked += this.MainForm_NewButtonClicked;
            this.MainForm.OpenButtonClicked += this.MainForm_OpenButtonClicked;
            this.MainForm.PassageButtonClicked += this.MainForm_PassageButtonClicked;
            this.MainForm.SaveButtonClicked += this.MainForm_SaveButtonClicked;
            this.MainForm.ScaleModeSwitcherClicked += this.MainForm_ScaleModeSwitcherClicked;
            this.MainForm.UndoButtonClicked += this.MainForm_UndoButtonClicked;

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
        }

        private void MapIdChanged()
        {
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

        private IMainForm MainForm;

        private ViewModel ViewModel;

        private void MainForm_CloseButtonClicked(object sender, EventArgs e)
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

        private void MainForm_DrawingModeSwitcherClicked(object sender, DrawingModeSwitcherClickedEventArgs e)
        {
            this.ViewModel.EditorState.DrawingMode = e.DrawingMode;
        }

        private void MainForm_LayerModeSwitcherClicked(object sender, LayerModeSwitcherClickedEventArgs e)
        {
            this.ViewModel.EditorState.LayerMode = e.LayerMode;
        }

        private void MainForm_NewButtonClicked(object sender, EventArgs e)
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

        private void MainForm_OpenButtonClicked(object sender, EventArgs e)
        {
            Debug.Assert(!this.ViewModel.IsOpened);
            if (this.MainForm.OpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.ViewModel.Open(this.MainForm.OpenFileDialog.FileName);
                Debug.Assert(this.ViewModel.IsOpened);
                // Debug.Assert(!this.ViewModel.IsDirty);
            }
        }

        private void MainForm_PassageButtonClicked(object sender, EventArgs e)
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

        private void MainForm_SaveButtonClicked(object sender, EventArgs e)
        {
            Debug.Assert(this.ViewModel.IsOpened);
            if (this.ViewModel.IsDirty)
            {
                this.ViewModel.Save();
            }
            Debug.Assert(this.ViewModel.IsOpened);
            Debug.Assert(!this.ViewModel.IsDirty);
        }

        private void MainForm_ScaleModeSwitcherClicked(object sender, ScaleModeSwitcherClickedEventArgs e)
        {
            this.ViewModel.EditorState.ScaleMode = e.ScaleMode;
        }

        private void MainForm_UndoButtonClicked(object sender, EventArgs e)
        {
            Debug.Assert(this.ViewModel.IsOpened);
            // Known Bug: this assertion fails
            Debug.Assert(this.ViewModel.IsUndoable);
            Debug.Assert(this.ViewModel.EditorState.Map != null);
            this.ViewModel.Undo();
            Debug.Assert(this.ViewModel.IsOpened);
        }

        public void Dispose()
        {
            // Do Nothing
        }
    }
}
