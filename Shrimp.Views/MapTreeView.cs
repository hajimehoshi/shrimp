using System;
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

        public event EventHandler ContextMenuOpening;
        protected virtual void OnContextMenuOpening(EventArgs e)
        {
            if (this.ContextMenuOpening != null)
            {
                this.ContextMenuOpening(this, e);
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
            this.IdToTreeNode.Add(id, node);
        }

        public void AddNodeToRoot(int id, string text)
        {
            TreeNode node = new TreeNode(text);
            node.ImageKey = "Document";
            node.Tag = id;
            this.Nodes.Add(node);
            this.IdToTreeNode.Add(id, node);
        }

        public void ClearNodes()
        {
            this.IdToTreeNode.Clear();
            this.Nodes.Clear();
        }

        public bool ContainsNode(int id)
        {
            return this.IdToTreeNode.ContainsKey(id);
        }

        public IMapDialog CreateMapDialog(int id, string name, Map map)
        {
            return new MapDialog(id, name, map);
        }

        public void ExpandNode(int id)
        {
            this.GetTreeNode(id).Expand();
        }

        public Image GetImage(MapTreeViewImage image)
        {
            switch (image)
            {
            case MapTreeViewImage.Plus:
                return Resources.ToggleSmall;
            case MapTreeViewImage.Minus:
                return Resources.ToggleSmallCollapse;
            case MapTreeViewImage.Map:
                return Resources.Card;
            case MapTreeViewImage.Home:
                return Resources.Home;
            case MapTreeViewImage.Bin:
                return Resources.Bin;
            default:
                Debug.Fail("Invalid MapTreeViewImage");
                return null;
            }
        }

        public bool HasSelectedNode
        {
            get { return this.SelectedNode != null; }
        }

        public bool IsContextMenuEnabled
        {
            get { return this.contextMenuStrip.Enabled; }
            set { this.contextMenuStrip.Enabled = value; }
        }

        public bool IsDeleteMenuItemEnabled
        {
            get { return this.DeleteToolStripMenuItem.Enabled; }
            set { this.DeleteToolStripMenuItem.Enabled = value; }
        }

        public bool IsEditMenuItemEnabled
        {
            get { return this.EditToolStripMenuItem.Enabled; }
            set { this.EditToolStripMenuItem.Enabled = value; }
        }

        public bool IsInsertMenuItemEnabled
        {
            get { return this.InsertToolStripMenuItem.Enabled; }
            set { this.InsertToolStripMenuItem.Enabled = value; }
        }

        public void RemoveNode(int id)
        {
            this.IdToTreeNode.Remove(id);
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
            return this.IdToTreeNode[id];
        }

        public MapTreeView()
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
                this.OnContextMenuOpening(EventArgs.Empty);
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
        }

        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem InsertToolStripMenuItem;
        private ToolStripMenuItem DeleteToolStripMenuItem;
        private ToolStripMenuItem EditToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private Dictionary<int, TreeNode> IdToTreeNode = new Dictionary<int, TreeNode>();

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
            this.contextMenuStrip.Size = new System.Drawing.Size(142, 76);
            // 
            // EditToolStripMenuItem
            // 
            this.EditToolStripMenuItem.Name = "EditToolStripMenuItem";
            this.EditToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.EditToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.EditToolStripMenuItem.Text = "Edit";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(138, 6);
            // 
            // InsertToolStripMenuItem
            // 
            this.InsertToolStripMenuItem.Name = "InsertToolStripMenuItem";
            this.InsertToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.InsertToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Insert;
            this.InsertToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.InsertToolStripMenuItem.Text = "Insert";
            // 
            // DeleteToolStripMenuItem
            // 
            this.DeleteToolStripMenuItem.Name = "DeleteToolStripMenuItem";
            this.DeleteToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.DeleteToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
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
