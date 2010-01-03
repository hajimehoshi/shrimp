using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shrimp.IViews
{
    public interface IMapTreeView
    {
        event EventHandler DeleteMenuItemClicked;
        event EventHandler EditMenuItemClicked;
        event EventHandler InsertMenuItemClicked;

        bool HasSelectedNode { get; }
    }
}
