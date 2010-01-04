﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shrimp.Models;

namespace Shrimp.IViews
{
    public interface IMapTreeView
    {
        event EventHandler<MapTreeViewEventArgs> AfterMapNodeExpand;
        event EventHandler<MapTreeViewEventArgs> AfterMapNodeCollapse;
        event EventHandler<MapTreeViewEventArgs> AfterMapNodeSelect;
        event EventHandler ContextMenuOpening;
        event EventHandler DeleteMenuItemClick;
        event EventHandler EditMenuItemClick;
        event EventHandler InsertMenuItemClick;

        void AddNode(int parentId, int id, string text);
        void AddNodeToRoot(int id, string text);
        void ClearNodes();
        bool ContainsNode(int id);
        IMapDialog CreateMapDialog(int id, string name, Map map);
        void ExpandNode(int id);
        bool HasSelectedNode { get; }
        bool IsContextMenuEnabled { get; set; }
        bool IsDeleteMenuItemEnabled { get; set; }
        bool IsEditMenuItemEnabled { get; set; }
        bool IsInsertMenuItemEnabled { get; set; }
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
