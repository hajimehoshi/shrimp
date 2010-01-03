using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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

            this.MapTreeView.EditMenuItemClick += delegate
            {
                if (this.MapTreeView.HasSelectedNode)
                {
                    int id = this.MapTreeView.SelectedNodeId;
                    string name = this.ViewModel.MapCollection.GetName(id);
                    Map map = this.ViewModel.MapCollection.GetMap(id);
                    using (var dialog = this.MapTreeView.CreateMapDialog(id, name, map))
                    {
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            map.Name = dialog.MapName;
                            map.Width = dialog.MapWidth;
                            map.Height = dialog.MapHeight;
                        }
                    }
                }
            };
            this.MapTreeView.InsertMenuItemClick += delegate
            {
                if (this.MapTreeView.HasSelectedNode)
                {
                    int selectedNodeId = this.MapTreeView.SelectedNodeId;
                    Debug.Assert(this.ViewModel.MapCollection.GetRoot(selectedNodeId) ==
                        this.ViewModel.MapCollection.ProjectNodeId);
                    int newId = Util.GetNewId(this.ViewModel.MapCollection.NodeIds);
                    using (var dialog = this.MapTreeView.CreateMapDialog(newId, "", null))
                    {
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            newId = this.ViewModel.MapCollection.Add(selectedNodeId);
                            Map map = this.ViewModel.MapCollection.GetMap(newId);
                            map.Name = dialog.MapName;
                            map.Width = dialog.MapWidth;
                            map.Height = dialog.MapHeight;
                        }
                    }
                }
            };
            this.MapTreeView.DeleteMenuItemClick += delegate
            {
                if (this.MapTreeView.HasSelectedNode)
                {
                    int selectedNodeId = this.MapTreeView.SelectedNodeId;
                    int rootId = this.ViewModel.MapCollection.GetRoot(selectedNodeId);
                    if (!this.ViewModel.MapCollection.Roots.Contains(selectedNodeId))
                    {
                        if (rootId == this.ViewModel.MapCollection.ProjectNodeId)
                        {
                            this.ViewModel.MapCollection.Move(selectedNodeId, this.ViewModel.MapCollection.TrashNodeId);
                        }
                        else if (rootId == this.ViewModel.MapCollection.TrashNodeId)
                        {
                            DialogResult result = MessageBox.Show("Really?", "",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question,
                                MessageBoxDefaultButton.Button2);
                            if (result == DialogResult.Yes)
                            {
                                this.ViewModel.MapCollection.Remove(selectedNodeId);
                            }
                        }
                    }
                }
            };
        }

        private IMapTreeView MapTreeView;
        private ViewModel ViewModel;
    }
}
