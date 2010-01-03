using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shrimp.IViews;
using Shrimp.Models;

namespace Shrimp.Presenters
{
    internal class MapTreeViewPresenter
    {
        public MapTreeViewPresenter(IMapTreeView mapTreeView, ViewModel viewModel)
        {
            this.MapTreeView = mapTreeView;
            this.ViewModel = viewModel;
        }

        private IMapTreeView MapTreeView;
        private ViewModel ViewModel;
    }
}
