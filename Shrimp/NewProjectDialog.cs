using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Shrimp
{
    internal partial class NewProjectDialog : Form
    {
        public NewProjectDialog()
        {
            this.InitializeComponent();
            string myDocumentPath =
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //string defaultPath = Path.Combine(myDocumentPath, "ShrimpProjects");
            string defaultPath = myDocumentPath;
            this.BasePathTextBox.Text = defaultPath;
            this.ValidateValues();
        }

        public string DirectoryName
        {
            get
            {
                return this.DirectoryNameTextBox.Text;
            }
        }

        public string GameTitle
        {
            get
            {
                return this.GameTitleTextBox.Text;
            }
        }

        public string BasePath
        {
            get
            {
                return this.BasePathTextBox.Text;
            }
        }

        private void FolderPathButton_Click(object sender, EventArgs e)
        {
            this.FolderBrowserDialog.SelectedPath = this.BasePathTextBox.Text;
            if (this.FolderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                this.BasePathTextBox.Text = this.FolderBrowserDialog.SelectedPath;
            }
        }

        private void DirectoryNameTextBox_TextChanged(object sender, EventArgs e)
        {
            this.ValidateValues();
        }

        private void GameTitleTextBox_TextChanged(object sender, EventArgs e)
        {
            this.ValidateValues();
        }

        private void BasePathTextBox_TextChanged(object sender, EventArgs e)
        {
            this.ValidateValues();
        }

        private void ValidateValues()
        {
            this.ErrorProvider.Clear();
            bool isValid = true;
            if (!(new Regex(@"^[a-zA-Z0-9_-]+$")).IsMatch(this.DirectoryName))
            {
                this.ErrorProvider.SetError(this.DirectoryNameLabel, "Invalid folder name");
                isValid = false;
            }
            else if (Directory.Exists(Path.Combine(this.BasePath, this.DirectoryName)))
            {
                this.ErrorProvider.SetError(this.DirectoryNameLabel, "Already exists");
                isValid = false;
            }
            if (this.GameTitle == "")
            {
                this.ErrorProvider.SetError(this.GameTitleLabel, "Input game title");
                isValid = false;
            }
            if (!Directory.Exists(this.BasePath))
            {
                this.ErrorProvider.SetError(this.BasePathLabel, "Directory not found");
                isValid = false;
            }
            this.OKButton.Enabled = isValid;
        }
    }
}
