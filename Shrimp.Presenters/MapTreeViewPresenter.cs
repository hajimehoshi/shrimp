﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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

            this.MapTreeView.AfterMapNodeCollapse += (sender, e) =>
            {
                this.ViewModel.MapCollection.CollapseNode(e.Id);
            };
            this.MapTreeView.AfterMapNodeExpand += (sender, e) =>
            {
                this.ViewModel.MapCollection.ExpandNode(e.Id);
            };
            this.MapTreeView.AfterMapNodeSelect += (sender, e) =>
            {
                Debug.Assert(this.MapTreeView.SelectedNodeId == e.Id);
                this.ViewModel.EditorState.MapId = e.Id;
            };
            this.MapTreeView.ContextMenuOpening += delegate
            {
                if (this.MapTreeView.HasSelectedNode)
                {
                    this.MapTreeView.IsContextMenuEnabled = true;
                    int id = this.MapTreeView.SelectedNodeId;
                    int rootId = this.ViewModel.MapCollection.GetRoot(id);
                    this.MapTreeView.IsEditMenuItemEnabled = !this.ViewModel.MapCollection.Roots.Contains(id);
                    this.MapTreeView.IsInsertMenuItemEnabled = (rootId == this.ViewModel.MapCollection.ProjectNodeId);
                    this.MapTreeView.IsDeleteMenuItemEnabled = !this.ViewModel.MapCollection.Roots.Contains(id);
                }
                else
                {
                    this.MapTreeView.IsContextMenuEnabled = false;
                }
            };
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
            this.MapTreeView.DrawNode += (sender, e) =>
            {
                if (!this.MapTreeView.Enabled)
                {
                    return;
                }
                Graphics g = e.Graphics;
                TreeNode node = e.Node;
                int id = (int)node.Tag;
                Rectangle bounds = e.Bounds;
                bool isSelected = (e.State & TreeNodeStates.Selected) != 0;
                if (isSelected)
                {
                    g.FillRectangle(SystemBrushes.Highlight, bounds);
                    if ((e.State & TreeNodeStates.Focused) != 0)
                    {
                        ControlPaint.DrawFocusRectangle(g, bounds,
                            SystemColors.HighlightText, SystemColors.Highlight);
                    }
                }
                else
                {
                    g.FillRectangle(new SolidBrush(this.MapTreeView.BackColor), bounds);
                }
                if (0 < node.Level && 0 < node.GetNodeCount(false))
                {
                    Image toggleImage = node.IsExpanded ?
                        this.MapTreeView.GetImage(MapTreeViewImage.Minus) :
                        this.MapTreeView.GetImage(MapTreeViewImage.Plus);
                    g.DrawImage(toggleImage,
                        bounds.X + this.MapTreeView.Indent * (node.Level - 1) + 1,
                        bounds.Y + 2);
                }
                Image image;
                switch (node.ImageKey)
                {
                case "Home":
                    image = this.MapTreeView.GetImage(MapTreeViewImage.Home);
                    break;
                case "Bin":
                    image = this.MapTreeView.GetImage(MapTreeViewImage.Bin);
                    break;
                default:
                    image = this.MapTreeView.GetImage(MapTreeViewImage.Map);
                    break;
                }
                g.DrawImage(image, bounds.X + this.MapTreeView.Indent * node.Level + 1, bounds.Y + 2);
                g.DrawString(node.Text, this.MapTreeView.Font,
                    isSelected ? SystemBrushes.HighlightText : new SolidBrush(this.MapTreeView.ForeColor),
                    bounds.X + this.MapTreeView.Indent * node.Level + 16 + 2,
                    bounds.Y + (this.MapTreeView.ItemHeight - this.MapTreeView.Font.Height) / 2);
            };

            this.ViewModel.IsOpenedChanged += delegate
            {
                this.Initialize();
            };
            this.ViewModel.MapCollection.NodeAdded += (sender, e) =>
            {
                int id = e.NodeId;
                int parentId = this.ViewModel.MapCollection.GetParent(id);
                this.MapTreeView.AddNode(parentId, id, this.ViewModel.MapCollection.GetName(id));
                Map map;
                if (this.ViewModel.MapCollection.TryGetMap(id, out map))
                {
                    map.Updated += Map_Updated;
                }
                this.MapTreeView.ExpandNode(parentId);
                this.MapTreeView.SelectedNodeId = id;
            };
            this.ViewModel.MapCollection.NodeRemoved += (sender, e) =>
            {
                int id = e.NodeId;
                Map map = this.ViewModel.MapCollection.GetMap(id);
                map.Updated -= Map_Updated;
                this.MapTreeView.RemoveNode(id);
            };
            this.ViewModel.MapCollection.NodeMoved += (sender, e) =>
            {
                int id = e.NodeId;
                int newParentId = this.ViewModel.MapCollection.GetParent(id);
                this.MapTreeView.RemoveNode(id);
                this.MapTreeView.AddNode(newParentId, id, this.ViewModel.MapCollection.GetName(id));
                this.MapTreeView.ExpandNode(newParentId);
            };

            this.Initialize();
        }

        private void Initialize()
        {
            this.MapTreeView.ClearNodes();
            if (this.ViewModel.MapCollection != null)
            {
                foreach (int id in this.ViewModel.MapCollection.Roots)
                {
                    this.MapTreeView.AddNodeToRoot(id, this.ViewModel.MapCollection.GetName(id));
                    foreach (int childId in this.ViewModel.MapCollection.GetChildren(id))
                    {
                        this.AddNodeRecursively(id, childId);
                    }
                    if (this.ViewModel.MapCollection.IsExpanded(id))
                    {
                        this.MapTreeView.ExpandNode(id);
                    }
                }
            }
            this.MapTreeView.SetNodeImageKey(this.ViewModel.MapCollection.ProjectNodeId, "Home");
            this.MapTreeView.SetNodeImageKey(this.ViewModel.MapCollection.TrashNodeId, "Bin");
            int selectedId = this.ViewModel.EditorState.MapId;
            if (this.MapTreeView.ContainsNode(selectedId))
            {
                this.MapTreeView.SelectedNodeId = selectedId;
            }
        }

        private void AddNodeRecursively(int parentId, int id)
        {
            this.MapTreeView.AddNode(parentId, id, this.ViewModel.MapCollection.GetName(id));
            Map map;
            if (this.ViewModel.MapCollection.TryGetMap(id, out map))
            {
                map.Updated += this.Map_Updated;
            }
            foreach (int childId in this.ViewModel.MapCollection.GetChildren(id))
            {
                this.AddNodeRecursively(id, childId);
            }
            if (this.ViewModel.MapCollection.IsExpanded(id))
            {
                this.MapTreeView.ExpandNode(id);
            }
        }

        private void Map_Updated(object sender, UpdatedEventArgs e)
        {
            Map map = (Map)sender;
            if (e.Property == map.GetProperty(_ => _.Name))
            {
                int id = map.Id;
                this.MapTreeView.SetNodeText(id, map.Name);
            }
        }

        private IMapTreeView MapTreeView;
        private ViewModel ViewModel;
    }
}
