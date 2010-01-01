using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Shrimp.Models;

namespace Shrimp.IViews
{
    public interface IMainForm
    {
        event EventHandler NewButtonClicked;
        event EventHandler OpenButtonClicked;
        event EventHandler CloseButtonClicked;
        event EventHandler SaveButtonClicked;
        event EventHandler UndoButtonClicked;

        event EventHandler<LayerSwitcherClickedEventArgs> LayerSwitcherClicked;

        event EventHandler PassageButtonClicked;
        event EventHandler SelectedTileSetChanged;

        INewProjectDialog CreateNewProjectDialog();
        OpenFileDialog OpenFileDialog { get; }
        void Run();
    }

    public class LayerSwitcherClickedEventArgs : EventArgs
    {
        public LayerSwitcherClickedEventArgs(LayerMode layerMode)
        {
            this.LayerMode = layerMode;
        }

        public LayerMode LayerMode { get; private set; }
    }
}
