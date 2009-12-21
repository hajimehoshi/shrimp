using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Shrimp.Properties;
using Shrimp.Models;

namespace Shrimp
{
    internal class MapTreeView : TreeView
    {
        public MapTreeView()
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
            this.EditToolStripMenuItem.Click += (s, e) =>
            {
                if (this.SelectedNode != null)
                {
                    int id = (int)this.SelectedNode.Tag;
                    string name = this.MapCollection.GetName(id);
                    Map map = this.MapCollection.GetMap(id);
                    using (var dialog = new MapDialog(id, name, map))
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
            this.InsertToolStripMenuItem.Click += (s, e) =>
            {
                if (this.SelectedNode != null)
                {
                    int selectedNodeId = (int)this.SelectedNode.Tag;
                    Debug.Assert(this.MapCollection.GetRoot(selectedNodeId) ==
                        this.MapCollection.ProjectNodeId);
                    int newId = Util.GetNewId(this.MapCollection.NodeIds);
                    using (var dialog = new MapDialog(newId, "", null))
                    {
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            newId = this.MapCollection.Add(selectedNodeId);
                            Map map = this.MapCollection.GetMap(newId);
                            map.Name = dialog.MapName;
                            map.Width = dialog.MapWidth;
                            map.Height = dialog.MapHeight;
                        }
                    }
                }
            };
            this.DeleteToolStripMenuItem.Click += (s, e) =>
            {
                if (this.SelectedNode != null)
                {
                    int selectedNodeId = (int)this.SelectedNode.Tag;
                    int rootId = this.MapCollection.GetRoot(selectedNodeId);
                    if (!this.MapCollection.Roots.Contains(selectedNodeId))
                    {
                        if (rootId == this.MapCollection.ProjectNodeId)
                        {
                            this.MapCollection.Move(selectedNodeId, this.MapCollection.TrashNodeId);
                        }
                        else if (rootId == this.MapCollection.TrashNodeId)
                        {
                            DialogResult result = MessageBox.Show("Really?", "",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question,
                                MessageBoxDefaultButton.Button2);
                            if (result == DialogResult.Yes)
                            {
                                this.MapCollection.Remove(selectedNodeId);
                            }
                        }
                    }
                }
            };

        }

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

        public ViewModel ViewModel
        {
            get { return this.viewModel; }
            set
            {
                if (this.viewModel != value)
                {
                    if (this.viewModel != null)
                    {
                        this.viewModel.IsOpenedChanged -= this.ViewModel_IsOpenedChanged;
                        this.viewModel.MapCollection.NodeAdded -= this.Tree_NodeAdded;
                        this.viewModel.MapCollection.NodeRemoved -= this.Tree_NodeRemoved;
                        this.viewModel.MapCollection.NodeMoved -= this.Tree_NodeMoved;
                    }
                    this.viewModel = value;
                    if (this.viewModel != null)
                    {
                        this.viewModel.IsOpenedChanged += this.ViewModel_IsOpenedChanged;
                        this.viewModel.MapCollection.NodeAdded += this.Tree_NodeAdded;
                        this.viewModel.MapCollection.NodeRemoved += this.Tree_NodeRemoved;
                        this.viewModel.MapCollection.NodeMoved += this.Tree_NodeMoved;
                    }
                    this.Initialize();
                }
            }
        }
        private ViewModel viewModel;

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

        private EditorState EditorState
        {
            get
            {
                if (this.ViewModel != null)
                {
                    return this.ViewModel.EditorState;
                }
                else
                {
                    return null;
                }
            }
        }

        private void ViewModel_IsOpenedChanged(object sender, EventArgs e)
        {
            this.Initialize();
        }

        private void Initialize()
        {
            this.Nodes.Clear();
            if (this.MapCollection != null)
            {
                foreach (int id in this.MapCollection.Roots)
                {
                    this.AddTreeNode(this.Nodes, id);
                }
            }
            this.AllNodes.First(n => (int)n.Tag == this.MapCollection.ProjectNodeId).ImageKey = "Folder";
            this.AllNodes.First(n => (int)n.Tag == this.MapCollection.TrashNodeId).ImageKey = "Bin";
            int selectedId = this.EditorState.MapId;
            TreeNode selectedNode = this.AllNodes.FirstOrDefault(n => (int)n.Tag == selectedId);
            if (selectedNode != null)
            {
                this.SelectedNode = selectedNode;
            }
        }

        private void AddTreeNode(TreeNodeCollection parentNodes, int id)
        {
            TreeNode node = new TreeNode(this.MapCollection.GetName(id));
            node.ImageKey = "Document";
            node.Tag = id;
            Map map;
            if (this.MapCollection.TryGetMap(id, out map))
            {
                map.Updated += Map_Updated;
            }
            parentNodes.Add(node);
            foreach (int childId in this.MapCollection.GetChildren(id))
            {
                this.AddTreeNode(node.Nodes, childId);
            }
            if (this.MapCollection.IsExpanded(id))
            {
                node.Expand();
            }
        }

        private void Tree_NodeAdded(object sender, NodeEventArgs e)
        {
            int id = e.NodeId;
            TreeNode node = new TreeNode(this.MapCollection.GetName(id));
            node.ImageKey = "Document";
            node.Tag = id;
            Map map;
            if (this.MapCollection.TryGetMap(id, out map))
            {
                map.Updated += Map_Updated;
            }
            int parentId = this.MapCollection.GetParent(id);
            TreeNode parentNode = this.AllNodes.First(n => (int)n.Tag == parentId);
            parentNode.Nodes.Add(node);
            parentNode.Expand();
            this.SelectedNode = node;
        }

        private void Tree_NodeRemoved(object sender, NodeEventArgs e)
        {
            int id = e.NodeId;
            Map map = this.MapCollection.GetMap(id);
            map.Updated -= Map_Updated;
            this.AllNodes.First(n => (int)n.Tag == id).Remove();
        }

        private void Tree_NodeMoved(object sender, NodeEventArgs e)
        {
            int id = e.NodeId;
            TreeNode node = this.AllNodes.First(n => (int)n.Tag == id);
            int newParentId = this.MapCollection.GetParent(id);
            TreeNode newParentNode = this.AllNodes.First(n => (int)n.Tag == newParentId);
            node.Remove();
            newParentNode.Nodes.Add(node);
            newParentNode.Expand();
        }

        private void Map_Updated(object sender, UpdatedEventArgs e)
        {
            Map map = (Map)sender;
            if (e.Property == map.GetProperty(_ => _.Name))
            {
                int id = map.Id;
                TreeNode node = this.AllNodes.First(n => (int)n.Tag == id);
                node.Text = map.Name;
            }
        }

        protected override void OnAfterExpand(TreeViewEventArgs e)
        {
            base.OnAfterExpand(e);
            this.MapCollection.ExpandNode((int)e.Node.Tag);
            this.Invalidate();
        }

        protected override void OnAfterCollapse(TreeViewEventArgs e)
        {
            base.OnAfterCollapse(e);
            this.MapCollection.CollapseNode((int)e.Node.Tag);
            this.Invalidate();
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            base.OnAfterSelect(e);
            this.EditorState.MapId = (int)this.SelectedNode.Tag;
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
            Image image = Resources.Card;
            if (id == this.MapCollection.ProjectNodeId)
            {
                image = Resources.Home;
            }
            else if (id == this.MapCollection.TrashNodeId)
            {
                image = Resources.Bin;
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
