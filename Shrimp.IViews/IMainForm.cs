using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shrimp.IViews
{
    public interface IMainForm
    {
        event EventHandler NewButtonClicked;
        event EventHandler OpenButtonClicked;
        event EventHandler CloseButtonClicked;
        event EventHandler SaveButtonClicked;
        event EventHandler UndoButtonClicked;
        event EventHandler PassageButtonClicked;
        event EventHandler SelectedTileSetChanged;

        INewProjectDialog CreateNewProjectDialog();
        void Run();
    }
}
