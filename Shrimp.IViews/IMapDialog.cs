using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Shrimp.IViews
{
    public interface IMapDialog : IDisposable
    {
        DialogResult ShowDialog();
        string MapName { get; }
        int MapWidth { get; }
        int MapHeight { get; }
    }
}
