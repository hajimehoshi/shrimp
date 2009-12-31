using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Shrimp.IViews;

namespace Shrimp.Presenters
{
    public class MainFormPresenter
    {
        public MainFormPresenter(IMainForm mainForm)
        {
            this.MainForm = mainForm;
            this.MainForm.NewButtonClicked += (s, e) =>
            {
                using (var dialog = this.MainForm.CreateNewProjectDialog())
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        string directoryPath = Path.Combine(dialog.BasePath, dialog.DirectoryName);
                        /*this.ViewModel.New(directoryPath, dialog.GameTitle);
                        Debug.Assert(this.ViewModel.IsOpened);
                        Debug.Assert(!this.ViewModel.IsDirty);*/
                    }
                }
            };
        }

        private IMainForm MainForm;

    }
}
