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
        event EventHandler<ClosingEventArgs> Closing;
    }

    public class ClosingEventArgs : EventArgs
    {
        public ClosingEventArgs()
        {
            this.Cancel = false;
        }

        public bool Cancel { get; set; }
    }
}
