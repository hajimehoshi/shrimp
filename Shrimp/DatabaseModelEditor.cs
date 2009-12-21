using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Shrimp
{
    public partial class DatabaseModelEditor : UserControl
    {
        public DatabaseModelEditor()
        {
            this.InitializeComponent();
            Font font = this.ModelNameLabel.Font;
            this.ModelNameLabel.Font = new Font(font.FontFamily, font.Size * 1.5f);
        }
    }
}
