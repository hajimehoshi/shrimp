using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
        event FormClosingEventHandler FormClosing;
    }
}
