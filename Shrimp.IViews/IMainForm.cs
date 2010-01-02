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
        event EventHandler CloseButtonClicked;
        event EventHandler<DrawingModeSwitcherClickedEventArgs> DrawingModeSwitcherClicked;
        event EventHandler<LayerModeSwitcherClickedEventArgs> LayerModeSwitcherClicked;
        event EventHandler NewButtonClicked;
        event EventHandler OpenButtonClicked;
        event EventHandler PassageButtonClicked;
        event EventHandler SaveButtonClicked;
        event EventHandler<ScaleModeSwitcherClickedEventArgs> ScaleModeSwitcherClicked;
        event EventHandler SelectedTileSetChanged;
        event EventHandler TileSetSelectorSelectedIndexChanged;
        event EventHandler UndoButtonClicked;

        INewProjectDialog CreateNewProjectDialog();
        bool GetDrawingModeSwitcherEnabled(DrawingMode drawingMode);
        bool GetLayerModeSwitcherEnabled(LayerMode layerMode);
        bool GetScaleModeSwitcherEnabled(ScaleMode scaleMode);
        void Run();
        void SetDrawingModeSwitcherEnabled(DrawingMode drawingMode, bool isEnabled);
        void SetLayerModeSwitcherEnabled(LayerMode layerMode, bool isEnabled);
        void SetScaleModeSwitcherEnabled(ScaleMode scaleMode, bool isEnabled);
        void SetTileSetSelectorItems(IEnumerable<string> items);

        bool IsCloseButtonEnabled { get; set; }
        bool IsMapTreeViewEnabled { get; set; }
        bool IsNewButtonEnabled { get; set; }
        bool IsOpenButtonEnabled { get; set; }
        bool IsPassageButtonChecked { get; set; }
        bool IsPassageButtonEnabled { get; set; }
        bool IsSaveButtonEnabled { get; set; }
        bool IsTileSetPaletteEnabled { get; set; }
        bool IsTileSetSelectorEnabled { get; set; }
        bool IsUndoButtonEnabled { get; set; }
        OpenFileDialog OpenFileDialog { get; }
        string Text { get; set; }
        int TileSetSelectorSelectedIndex { get; set; }
        string TileSetSelectorSelectedItem { get; }
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
