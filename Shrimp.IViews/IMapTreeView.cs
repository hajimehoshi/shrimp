using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shrimp.Models;

namespace Shrimp.IViews
{
    public interface IMapTreeView
    {
        event EventHandler DeleteMenuItemClicked;
        event EventHandler EditMenuItemClicked;
        event EventHandler InsertMenuItemClicked;

        IMapDialog CreateMapDialog(int id, string name, Map map);
        bool HasSelectedNode { get; }
        int SelectedNodeId { get; }
    }
}
