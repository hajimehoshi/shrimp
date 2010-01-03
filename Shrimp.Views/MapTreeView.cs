﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Shrimp.Views.Properties;
using Shrimp.IViews;
using Shrimp.Models;

namespace Shrimp.Views
{
    internal class MapTreeView : TreeView, IMapTreeView
    {
        public event EventHandler<MapTreeViewEventArgs> AfterMapNodeExpand;
        protected virtual void OnAfterMapNodeExpand(MapTreeViewEventArgs e)
        {
            if (this.AfterMapNodeExpand != null)
            {
                this.AfterMapNodeExpand(this, e);
            }
        }

        public event EventHandler<MapTreeViewEventArgs> AfterMapNodeCollapse;
        protected virtual void OnAfterMapNodeCollapse(MapTreeViewEventArgs e)
        {
            if (this.AfterMapNodeCollapse != null)
            {
                this.AfterMapNodeCollapse(this, e);
            }
        }

        public event EventHandler<MapTreeViewEventArgs> AfterMapNodeSelect;
        protected virtual void OnAfterMapNodeSelect(MapTreeViewEventArgs e)
        {
            if (this.AfterMapNodeSelect != null)
            {
                this.AfterMapNodeSelect(this, e);
            }
        }

        public event EventHandler DeleteMenuItemClick;
        protected virtual void OnDeleteMenuItemClick(EventArgs e)
        {
            if (this.DeleteMenuItemClick != null)
            {
                this.DeleteMenuItemClick(this, e);
            }
        }

        public event EventHandler EditMenuItemClick;
        protected virtual void OnEditMenuItemClick(EventArgs e)
        {
            if (this.EditMenuItemClick != null)
            {
                this.EditMenuItemClick(this, e);
            }
        }

        public event EventHandler InsertMenuItemClick;
        protected virtual void OnInsertMenuItemClick(EventArgs e)
        {
            if (this.InsertMenuItemClick != null)
            {
                this.InsertMenuItemClick(this, e);
            }
        }

        public void AddNode(int parentId, int id, string text)
        {
            TreeNode node = new TreeNode(text);
            node.ImageKey = "Document";
            node.Tag = id;
            this.GetTreeNode(parentId).Nodes.Add(node);
        }

        public void AddNodeToRoot(int id, string text)
        {
            TreeNode node = new TreeNode(text);
            node.ImageKey = "Document";
            node.Tag = id;
            this.Nodes.Add(node);
        }

        public void ClearNodes()
        {
            this.Nodes.Clear();
        }

        public bool ContainsNode(int id)
        {
            // TODO: Tune up later
            return this.AllNodes.Any(n => (int)n.Tag == id);
        }

        public IMapDialog CreateMapDialog(int id, string name, Map map)
        {
            return new MapDialog(id, name, map);
        }

        public void ExpandNode(int id)
        {
            this.GetTreeNode(id).Expand();
        }

        public bool HasSelectedNode
        {
            get { return this.SelectedNode != null; }
        }

        public void RemoveNode(int id)
        {
            this.GetTreeNode(id).Remove();
        }

        public int SelectedNodeId
        {
            get
            {
                Debug.Assert(this.SelectedNode != null);
                return (int)this.SelectedNode.Tag;
            }
            set
            {
                Debug.Assert(this.ContainsNode(value));
                this.SelectedNode = this.GetTreeNode(value);
            }
        }

        public void SetNodeImageKey(int id, string imageKey)
        {
            this.GetTreeNode(id).ImageKey = imageKey;
        }

        public void SetNodeText(int id, string text)
        {
            this.GetTreeNode(id).Text = text;
        }

        private TreeNode GetTreeNode(int id)
        {
            // TODO: Tune up later
            return this.AllNodes.First(n => (int)n.Tag == id);
        }

        public MapTreeView(ViewModel viewModel)
            : base()
        {
            this.InitializeComponent();
            this.ItemHeight = (int)(this.Font.Height * 1.8);
            this.DoubleBuffered = true;
            this.contextMenuStrip.Opening += (s, e) =>
            {
                Point location = new Point
                {
                    X = this.contextMenuStrip.Left,
                    Y = this.contextMenuStrip.Top,
                };
                TreeNode treeNode = this.GetNodeAt(this.PointToClient(location));
                if (treeNode != null)
                {
                    this.SelectedNode = treeNode;
                }
                if (this.SelectedNode != null)
                {
                    this.contextMenuStrip.Enabled = true;
                    int id = (int)this.SelectedNode.Tag;
                    int rootId = this.MapCollection.GetRoot(id);
                    this.EditToolStripMenuItem.Enabled =
                        !this.MapCollection.Roots.Contains(id);
                    this.InsertToolStripMenuItem.Enabled =
                        (rootId == this.MapCollection.ProjectNodeId);
                    this.DeleteToolStripMenuItem.Enabled =
                        !this.MapCollection.Roots.Contains(id);
                }
                else
                {
                    this.contextMenuStrip.Enabled = false;
                }
            };
            this.EditToolStripMenuItem.Click += delegate
            {
                this.OnEditMenuItemClick(EventArgs.Empty);
            };
            this.InsertToolStripMenuItem.Click += delegate
            {
                this.OnInsertMenuItemClick(EventArgs.Empty);
            };
            this.DeleteToolStripMenuItem.Click += delegate
            {
                this.OnDeleteMenuItemClick(EventArgs.Empty);
            };
            this.ViewModel = viewModel;
        }

        private ViewModel ViewModel;

        private IEnumerable<TreeNode> AllNodes
        {
            get { return this.Traverse(this.Nodes); }
        }

        private IEnumerable<TreeNode> Traverse(TreeNodeCollection nodeCollection)
        {
            foreach (TreeNode node in nodeCollection)
            {
                yield return node;
                foreach (TreeNode node2 in this.Traverse(node.Nodes))
                {
                    yield return node2;
                }
            }
        }

        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem InsertToolStripMenuItem;
        private ToolStripMenuItem DeleteToolStripMenuItem;
        private ToolStripMenuItem EditToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;

        private MapCollection MapCollection
        {
            get
            {
                if (this.ViewModel != null)
                {
                    return this.ViewModel.MapCollection;
                }
                else
                {
                    return null;
                }
            }
        }

        protected override void OnAfterExpand(TreeViewEventArgs e)
        {
            base.OnAfterExpand(e);
            this.OnAfterMapNodeExpand(new MapTreeViewEventArgs((int)e.Node.Tag));
            this.Invalidate();
        }

        protected override void OnAfterCollapse(TreeViewEventArgs e)
        {
            base.OnAfterCollapse(e);
            this.OnAfterMapNodeCollapse(new MapTreeViewEventArgs((int)e.Node.Tag));
            this.Invalidate();
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            base.OnAfterSelect(e);
            this.OnAfterMapNodeSelect(new MapTreeViewEventArgs((int)e.Node.Tag));
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Space)
            {
                this.EditToolStripMenuItem.PerformClick();
            }
        }

        protected override void OnDrawNode(DrawTreeNodeEventArgs e)
        {
            base.OnDrawNode(e);
            if (!this.Enabled)
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
                g.FillRectangle(new SolidBrush(this.BackColor), bounds);
            }
            if (0 < node.Level && 0 < node.GetNodeCount(false))
            {
                Image toggleImage = node.IsExpanded ? Resources.ToggleSmallCollapse : Resources.ToggleSmall;
                g.DrawImage(toggleImage,
                    bounds.X + this.Indent * (node.Level - 1) + 1,
                    bounds.Y + 2);
            }
            Image image;
            switch (node.ImageKey)
            {
            case "Home":
                image = Resources.Home;
                break;
            case "Bin":
                image = Resources.Bin;
                break;
            default:
                image = Resources.Card;
                break;
            }
            g.DrawImage(image, bounds.X + this.Indent * node.Level + 1, bounds.Y + 2);
            g.DrawString(node.Text, this.Font,
                isSelected ? SystemBrushes.HighlightText : new SolidBrush(this.ForeColor),
                bounds.X + this.Indent * node.Level + 16 + 2,
                bounds.Y + (this.ItemHeight - this.Font.Height) / 2);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.EditToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.InsertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.EditToolStripMenuItem,
            this.toolStripSeparator1,
            this.InsertToolStripMenuItem,
            this.DeleteToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.contextMenuStrip.ShowImageMargin = false;
            this.contextMenuStrip.Size = new System.Drawing.Size(101, 76);
            // 
            // EditToolStripMenuItem
            // 
            this.EditToolStripMenuItem.Name = "EditToolStripMenuItem";
            this.EditToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.EditToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.EditToolStripMenuItem.Text = "Edit";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(97, 6);
            // 
            // InsertToolStripMenuItem
            // 
            this.InsertToolStripMenuItem.Name = "InsertToolStripMenuItem";
            this.InsertToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.InsertToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Insert;
            this.InsertToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.InsertToolStripMenuItem.Text = "Insert";
            // 
            // DeleteToolStripMenuItem
            // 
            this.DeleteToolStripMenuItem.Name = "DeleteToolStripMenuItem";
            this.DeleteToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.DeleteToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.DeleteToolStripMenuItem.Text = "Delete";
            // 
            // MapTreeView
            // 
            this.ContextMenuStrip = this.contextMenuStrip;
            this.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
            this.FullRowSelect = true;
            this.HideSelection = false;
            this.ImageKey = "PageWhite";
            this.LineColor = System.Drawing.Color.Black;
            this.ShowLines = false;
            this.ShowRootLines = false;
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.ComponentModel.IContainer components;
    }
}
