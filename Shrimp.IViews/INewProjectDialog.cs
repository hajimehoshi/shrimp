using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Shrimp.IViews
{
    public interface INewProjectDialog : IDisposable
    {
        DialogResult ShowDialog();
        string DirectoryName { get; }
        string GameTitle { get; }
        string BasePath { get; }
    }
}
