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

        event EventHandler<LayerModeSwitcherClickedEventArgs> LayerModeSwitcherClicked;
        event EventHandler<DrawingModeSwitcherClickedEventArgs> DrawingModeSwitcherClicked;
        event EventHandler<ScaleModeSwitcherClickedEventArgs> ScaleModeSwitcherClicked;

        event EventHandler PassageButtonClicked;
        event EventHandler SelectedTileSetChanged;

        INewProjectDialog CreateNewProjectDialog();
        OpenFileDialog OpenFileDialog { get; }
        string Text { get; set; }
        bool UndoButtonEnabled { get; set; }
        void Run();
    }

    public class LayerModeSwitcherClickedEventArgs : EventArgs
    {
        public LayerModeSwitcherClickedEventArgs(LayerMode layerMode)
        {
            this.LayerMode = layerMode;
        }

        public LayerMode LayerMode { get; private set; }
    }

    public class DrawingModeSwitcherClickedEventArgs : EventArgs
    {
        public DrawingModeSwitcherClickedEventArgs(DrawingMode drawingMode)
        {
            this.DrawingMode = drawingMode;
        }

        public DrawingMode DrawingMode { get; private set; }
    }

    public class ScaleModeSwitcherClickedEventArgs : EventArgs
    {
        public ScaleModeSwitcherClickedEventArgs(ScaleMode scaleMode)
        {
            this.ScaleMode = scaleMode;
        }

        public ScaleMode ScaleMode { get; private set; }
    }
}
