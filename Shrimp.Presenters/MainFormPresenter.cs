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
            this.MainForm.NewButtonClicked += this.MainForm_NewButtonClicked;
            this.MainForm.OpenButtonClicked += this.MainForm_OpenButtonClicked;
            this.MainForm.CloseButtonClicked += this.MainForm_CloseButtonClicked;
            this.MainForm.SaveButtonClicked += this.MainForm_SaveButtonClicked;
            this.MainForm.UndoButtonClicked += this.MainForm_UndoButtonClicked;
            this.MainForm.LayerModeSwitcherClicked += this.MainForm_LayerModeSwitcherClicked;
            this.MainForm.DrawingModeSwitcherClicked += this.MainForm_DrawingModeSwitcherClicked;
            this.MainForm.ScaleModeSwitcherClicked += this.MainForm_ScaleModeSwitcherClicked;
        }

        private IMainForm MainForm;

        // TODO: Remove ViewModel
        private ViewModel ViewModel;

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

        private void MainForm_UndoButtonClicked(object sender, EventArgs e)
        {
            Debug.Assert(this.ViewModel.IsOpened);
            Debug.Assert(this.ViewModel.IsUndoable);
            Debug.Assert(this.ViewModel.EditorState.Map != null);
            this.ViewModel.Undo();
            Debug.Assert(this.ViewModel.IsOpened);
        }

        private void MainForm_LayerModeSwitcherClicked(object sender, LayerModeSwitcherClickedEventArgs e)
        {
            this.ViewModel.EditorState.LayerMode = e.LayerMode;
        }

        private void MainForm_DrawingModeSwitcherClicked(object sender, DrawingModeSwitcherClickedEventArgs e)
        {
            this.ViewModel.EditorState.DrawingMode = e.DrawingMode;
        }
        
        private void MainForm_ScaleModeSwitcherClicked(object sender, ScaleModeSwitcherClickedEventArgs e)
        {
            this.ViewModel.EditorState.ScaleMode = e.ScaleMode;
        }

        public void Dispose()
        {
        }
    }
}
