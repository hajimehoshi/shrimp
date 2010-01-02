using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shrimp.IViews;
using Shrimp.Models;

namespace Shrimp.Presenters
{
    internal class MapEditorPresenter
    {
        public MapEditorPresenter(IMapEditor mapEditor, ViewModel viewModel)
        {
            this.MapEditor = mapEditor;
            this.ViewModel = viewModel;
        }

        private IMapEditor MapEditor;
        private ViewModel ViewModel;
    }
}
