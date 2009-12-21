using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Shrimp.Models;

namespace Shrimp
{
    internal partial class MapDialog : Form
    {
        public MapDialog(int id, string name, Map map)
        {
            this.InitializeComponent();
            this.Map = map;
            this.WidthNumericUpDown.Minimum = Map.MinWidth;
            this.WidthNumericUpDown.Maximum = Map.MaxWidth;
            this.HeightNumericUpDown.Minimum = Map.MinHeight;
            this.HeightNumericUpDown.Maximum = Map.MaxHeight;
            this.Text += " (ID:" + id + ")";
            if (this.Map != null)
            {
                this.NameTextBox.Text = name;
                this.WidthNumericUpDown.Value = map.Width;
                this.HeightNumericUpDown.Value = map.Height;
            }
            this.ValidateValues();
        }

        public Map Map { get; private set; }

        public string MapName
        {
            get { return this.NameTextBox.Text; }
        }

        public int MapWidth
        {
            get { return (int)this.WidthNumericUpDown.Value; }
        }

        public int MapHeight
        {
            get { return (int)this.HeightNumericUpDown.Value; }
        }

        private void ValidateValues()
        {
            this.ErrorProvider.Clear();
            bool isValid = true;
            if (this.MapName == "")
            {
                this.ErrorProvider.SetError(this.NameLabel, "Invalid name");
                isValid = false;
            }
            this.OKButton.Enabled = isValid;
        }

        private void NameTextBox_TextChanged(object sender, EventArgs e)
        {
            this.ValidateValues();
        }

        private void WidthNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            this.ValidateValues();
        }

        private void HeightNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            this.ValidateValues();
        }
    }
}
