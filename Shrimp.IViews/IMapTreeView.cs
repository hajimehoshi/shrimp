using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using Shrimp.Models;

namespace Shrimp.IViews
{
    public enum MapTreeViewImage
    {
        Plus,
        Minus,
        Map,
        Home,
        Bin,
    }

    public interface IMapTreeView
    {
        event EventHandler<MapTreeViewEventArgs> AfterMapNodeExpand;
        event EventHandler<MapTreeViewEventArgs> AfterMapNodeCollapse;
        event EventHandler<MapTreeViewEventArgs> AfterMapNodeSelect;
        event EventHandler ContextMenuOpening;
        event EventHandler DeleteMenuItemClick;
        event DrawTreeNodeEventHandler DrawNode;
        event EventHandler EditMenuItemClick;
        event EventHandler InsertMenuItemClick;

        void AddNode(int parentId, int id, string text);
        void AddNodeToRoot(int id, string text);
        Color BackColor { get; }
        void ClearNodes();
        bool ContainsNode(int id);
        IMapDialog CreateMapDialog(int id, string name, Map map);
        bool Enabled { get; set; }
        void ExpandNode(int id);
        Font Font { get; }
        Color ForeColor { get; }
        Image GetImage(MapTreeViewImage image);
        bool HasSelectedNode { get; }
        int Indent { get; }
        bool IsContextMenuEnabled { get; set; }
        bool IsDeleteMenuItemEnabled { get; set; }
        bool IsEditMenuItemEnabled { get; set; }
        bool IsInsertMenuItemEnabled { get; set; }
        int ItemHeight { get; }
        void RemoveNode(int id);
        int SelectedNodeId { get; set; }
        void SetNodeImageKey(int id, string imageKey);
        void SetNodeText(int id, string text);
    }

    public class MapTreeViewEventArgs : EventArgs
    {
        public MapTreeViewEventArgs(int id)
        {
            this.Id = id;
        }

        public int Id { get; private set; }
    }
}
