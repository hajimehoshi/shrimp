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
            this.MainForm.NewButtonClicked += (s, e) =>
            {
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
            };
        }

        private IMainForm MainForm;

        // TODO: Remove ViewModel
        private ViewModel ViewModel;

        public void Dispose()
        {
        }
    }
}
