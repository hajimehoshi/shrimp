namespace Shrimp
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.MainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.LeftSplitContainer = new System.Windows.Forms.SplitContainer();
            this.TileSetPaletteToolStrip = new System.Windows.Forms.ToolStrip();
            this.TileSetsToolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.PassageToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.TileSetPalette = new Shrimp.TileSetPalette();
            this.MapTreeView = new Shrimp.MapTreeView();
            this.ToolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.ToolStrip = new System.Windows.Forms.ToolStrip();
            this.NewToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.OpenToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.CloseToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.SaveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.UndoToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.Layer1ToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.Layer2ToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.EventToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.PenToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.Scale1ToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.Scale2ToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.Scale4ToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.Scale8ToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.MainSplitContainer.Panel1.SuspendLayout();
            this.MainSplitContainer.SuspendLayout();
            this.LeftSplitContainer.Panel1.SuspendLayout();
            this.LeftSplitContainer.Panel2.SuspendLayout();
            this.LeftSplitContainer.SuspendLayout();
            this.TileSetPaletteToolStrip.SuspendLayout();
            this.ToolStripContainer.ContentPanel.SuspendLayout();
            this.ToolStripContainer.TopToolStripPanel.SuspendLayout();
            this.ToolStripContainer.SuspendLayout();
            this.ToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainSplitContainer
            // 
            this.MainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.MainSplitContainer.IsSplitterFixed = true;
            this.MainSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.MainSplitContainer.Name = "MainSplitContainer";
            // 
            // MainSplitContainer.Panel1
            // 
            this.MainSplitContainer.Panel1.Controls.Add(this.LeftSplitContainer);
            this.MainSplitContainer.Size = new System.Drawing.Size(792, 525);
            this.MainSplitContainer.SplitterDistance = 300;
            this.MainSplitContainer.SplitterWidth = 3;
            this.MainSplitContainer.TabIndex = 0;
            // 
            // LeftSplitContainer
            // 
            this.LeftSplitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LeftSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LeftSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.LeftSplitContainer.Name = "LeftSplitContainer";
            this.LeftSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // LeftSplitContainer.Panel1
            // 
            this.LeftSplitContainer.Panel1.Controls.Add(this.TileSetPaletteToolStrip);
            this.LeftSplitContainer.Panel1.Controls.Add(this.TileSetPalette);
            // 
            // LeftSplitContainer.Panel2
            // 
            this.LeftSplitContainer.Panel2.Controls.Add(this.MapTreeView);
            this.LeftSplitContainer.Size = new System.Drawing.Size(300, 525);
            this.LeftSplitContainer.SplitterDistance = 290;
            this.LeftSplitContainer.SplitterWidth = 3;
            this.LeftSplitContainer.TabIndex = 0;
            // 
            // TileSetPaletteToolStrip
            // 
            this.TileSetPaletteToolStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.TileSetPaletteToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.TileSetPaletteToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TileSetsToolStripComboBox,
            this.PassageToolStripButton});
            this.TileSetPaletteToolStrip.Location = new System.Drawing.Point(0, 260);
            this.TileSetPaletteToolStrip.Name = "TileSetPaletteToolStrip";
            this.TileSetPaletteToolStrip.Size = new System.Drawing.Size(296, 26);
            this.TileSetPaletteToolStrip.TabIndex = 1;
            // 
            // TileSetsToolStripComboBox
            // 
            this.TileSetsToolStripComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TileSetsToolStripComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.TileSetsToolStripComboBox.Name = "TileSetsToolStripComboBox";
            this.TileSetsToolStripComboBox.Size = new System.Drawing.Size(121, 26);
            this.TileSetsToolStripComboBox.SelectedIndexChanged += new System.EventHandler(this.TileSetsToolStripComboBox_SelectedIndexChanged);
            // 
            // PassageToolStripButton
            // 
            this.PassageToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.PassageToolStripButton.Image = global::Shrimp.Properties.Resources.TrafficLight;
            this.PassageToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.PassageToolStripButton.Name = "PassageToolStripButton";
            this.PassageToolStripButton.Size = new System.Drawing.Size(23, 23);
            this.PassageToolStripButton.Text = "Passage";
            this.PassageToolStripButton.Click += new System.EventHandler(this.PassageToolStripButton_Click);
            // 
            // TileSetPalette
            // 
            this.TileSetPalette.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.TileSetPalette.AutoScroll = true;
            this.TileSetPalette.Location = new System.Drawing.Point(0, 0);
            this.TileSetPalette.Name = "TileSetPalette";
            this.TileSetPalette.Size = new System.Drawing.Size(100, 105);
            this.TileSetPalette.TabIndex = 0;
            this.TileSetPalette.ViewModel = null;
            // 
            // MapTreeView
            // 
            this.MapTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.MapTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MapTreeView.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
            this.MapTreeView.FullRowSelect = true;
            this.MapTreeView.HideSelection = false;
            this.MapTreeView.ImageKey = "PageWhite";
            this.MapTreeView.ItemHeight = 21;
            this.MapTreeView.Location = new System.Drawing.Point(0, 0);
            this.MapTreeView.Name = "MapTreeView";
            this.MapTreeView.ShowLines = false;
            this.MapTreeView.ShowRootLines = false;
            this.MapTreeView.Size = new System.Drawing.Size(296, 228);
            this.MapTreeView.TabIndex = 0;
            this.MapTreeView.ViewModel = null;
            // 
            // ToolStripContainer
            // 
            // 
            // ToolStripContainer.BottomToolStripPanel
            // 
            this.ToolStripContainer.BottomToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            // 
            // ToolStripContainer.ContentPanel
            // 
            this.ToolStripContainer.ContentPanel.Controls.Add(this.MainSplitContainer);
            this.ToolStripContainer.ContentPanel.Size = new System.Drawing.Size(792, 525);
            this.ToolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ToolStripContainer.Location = new System.Drawing.Point(0, 0);
            this.ToolStripContainer.Name = "ToolStripContainer";
            this.ToolStripContainer.Size = new System.Drawing.Size(792, 566);
            this.ToolStripContainer.TabIndex = 1;
            this.ToolStripContainer.Text = "toolStripContainer1";
            // 
            // ToolStripContainer.TopToolStripPanel
            // 
            this.ToolStripContainer.TopToolStripPanel.Controls.Add(this.ToolStrip);
            this.ToolStripContainer.TopToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            // 
            // ToolStrip
            // 
            this.ToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewToolStripButton,
            this.OpenToolStripButton,
            this.CloseToolStripButton,
            this.SaveToolStripButton,
            this.toolStripSeparator1,
            this.UndoToolStripButton,
            this.toolStripSeparator4,
            this.Layer1ToolStripButton,
            this.Layer2ToolStripButton,
            this.EventToolStripButton,
            this.toolStripSeparator2,
            this.PenToolStripButton,
            this.toolStripSeparator3,
            this.Scale1ToolStripButton,
            this.Scale2ToolStripButton,
            this.Scale4ToolStripButton,
            this.Scale8ToolStripButton});
            this.ToolStrip.Location = new System.Drawing.Point(0, 0);
            this.ToolStrip.Name = "ToolStrip";
            this.ToolStrip.Size = new System.Drawing.Size(792, 41);
            this.ToolStrip.Stretch = true;
            this.ToolStrip.TabIndex = 0;
            // 
            // NewToolStripButton
            // 
            this.NewToolStripButton.Image = global::Shrimp.Properties.Resources.Document;
            this.NewToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.NewToolStripButton.Name = "NewToolStripButton";
            this.NewToolStripButton.Size = new System.Drawing.Size(38, 38);
            this.NewToolStripButton.Text = "New";
            this.NewToolStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.NewToolStripButton.Click += new System.EventHandler(this.NewToolStripButton_Click);
            // 
            // OpenToolStripButton
            // 
            this.OpenToolStripButton.Image = global::Shrimp.Properties.Resources.FolderOpen;
            this.OpenToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.OpenToolStripButton.Name = "OpenToolStripButton";
            this.OpenToolStripButton.Size = new System.Drawing.Size(42, 38);
            this.OpenToolStripButton.Text = "Open";
            this.OpenToolStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.OpenToolStripButton.Click += new System.EventHandler(this.OpenToolStripButton_Click);
            // 
            // CloseToolStripButton
            // 
            this.CloseToolStripButton.Image = global::Shrimp.Properties.Resources.Folder;
            this.CloseToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.CloseToolStripButton.Name = "CloseToolStripButton";
            this.CloseToolStripButton.Size = new System.Drawing.Size(43, 38);
            this.CloseToolStripButton.Text = "Close";
            this.CloseToolStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.CloseToolStripButton.Click += new System.EventHandler(this.CloseToolStripButton_Click);
            // 
            // SaveToolStripButton
            // 
            this.SaveToolStripButton.Image = global::Shrimp.Properties.Resources.Disk;
            this.SaveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SaveToolStripButton.Name = "SaveToolStripButton";
            this.SaveToolStripButton.Size = new System.Drawing.Size(41, 38);
            this.SaveToolStripButton.Text = "Save";
            this.SaveToolStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.SaveToolStripButton.Click += new System.EventHandler(this.SaveToolStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 41);
            // 
            // UndoToolStripButton
            // 
            this.UndoToolStripButton.Image = global::Shrimp.Properties.Resources.ArrowReturn180Left;
            this.UndoToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.UndoToolStripButton.Name = "UndoToolStripButton";
            this.UndoToolStripButton.Size = new System.Drawing.Size(42, 38);
            this.UndoToolStripButton.Text = "Undo";
            this.UndoToolStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.UndoToolStripButton.Click += new System.EventHandler(this.UndoToolStripButton_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 41);
            // 
            // Layer1ToolStripButton
            // 
            this.Layer1ToolStripButton.Image = global::Shrimp.Properties.Resources.Map;
            this.Layer1ToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Layer1ToolStripButton.Name = "Layer1ToolStripButton";
            this.Layer1ToolStripButton.Size = new System.Drawing.Size(56, 38);
            this.Layer1ToolStripButton.Text = "Layer 1";
            this.Layer1ToolStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // Layer2ToolStripButton
            // 
            this.Layer2ToolStripButton.Image = global::Shrimp.Properties.Resources.Map;
            this.Layer2ToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Layer2ToolStripButton.Name = "Layer2ToolStripButton";
            this.Layer2ToolStripButton.Size = new System.Drawing.Size(56, 38);
            this.Layer2ToolStripButton.Text = "Layer 2";
            this.Layer2ToolStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // EventToolStripButton
            // 
            this.EventToolStripButton.Image = global::Shrimp.Properties.Resources.Balloons;
            this.EventToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.EventToolStripButton.Name = "EventToolStripButton";
            this.EventToolStripButton.Size = new System.Drawing.Size(45, 38);
            this.EventToolStripButton.Text = "Event";
            this.EventToolStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 41);
            // 
            // PenToolStripButton
            // 
            this.PenToolStripButton.Image = global::Shrimp.Properties.Resources.Pencil;
            this.PenToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.PenToolStripButton.Name = "PenToolStripButton";
            this.PenToolStripButton.Size = new System.Drawing.Size(33, 38);
            this.PenToolStripButton.Text = "Pen";
            this.PenToolStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 41);
            // 
            // Scale1ToolStripButton
            // 
            this.Scale1ToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.Scale1ToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("Scale1ToolStripButton.Image")));
            this.Scale1ToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Scale1ToolStripButton.Name = "Scale1ToolStripButton";
            this.Scale1ToolStripButton.Size = new System.Drawing.Size(31, 38);
            this.Scale1ToolStripButton.Text = "1/1";
            // 
            // Scale2ToolStripButton
            // 
            this.Scale2ToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.Scale2ToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("Scale2ToolStripButton.Image")));
            this.Scale2ToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Scale2ToolStripButton.Name = "Scale2ToolStripButton";
            this.Scale2ToolStripButton.Size = new System.Drawing.Size(31, 38);
            this.Scale2ToolStripButton.Text = "1/2";
            // 
            // Scale4ToolStripButton
            // 
            this.Scale4ToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.Scale4ToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("Scale4ToolStripButton.Image")));
            this.Scale4ToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Scale4ToolStripButton.Name = "Scale4ToolStripButton";
            this.Scale4ToolStripButton.Size = new System.Drawing.Size(31, 38);
            this.Scale4ToolStripButton.Text = "1/4";
            // 
            // Scale8ToolStripButton
            // 
            this.Scale8ToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.Scale8ToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("Scale8ToolStripButton.Image")));
            this.Scale8ToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Scale8ToolStripButton.Name = "Scale8ToolStripButton";
            this.Scale8ToolStripButton.Size = new System.Drawing.Size(31, 38);
            this.Scale8ToolStripButton.Text = "1/8";
            // 
            // OpenFileDialog
            // 
            this.OpenFileDialog.Filter = "Project File (*.json)|*.json|All Files (*.*)|*.*";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(792, 566);
            this.Controls.Add(this.ToolStripContainer);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Shrimp";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.MainSplitContainer.Panel1.ResumeLayout(false);
            this.MainSplitContainer.ResumeLayout(false);
            this.LeftSplitContainer.Panel1.ResumeLayout(false);
            this.LeftSplitContainer.Panel1.PerformLayout();
            this.LeftSplitContainer.Panel2.ResumeLayout(false);
            this.LeftSplitContainer.ResumeLayout(false);
            this.TileSetPaletteToolStrip.ResumeLayout(false);
            this.TileSetPaletteToolStrip.PerformLayout();
            this.ToolStripContainer.ContentPanel.ResumeLayout(false);
            this.ToolStripContainer.TopToolStripPanel.ResumeLayout(false);
            this.ToolStripContainer.TopToolStripPanel.PerformLayout();
            this.ToolStripContainer.ResumeLayout(false);
            this.ToolStripContainer.PerformLayout();
            this.ToolStrip.ResumeLayout(false);
            this.ToolStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer MainSplitContainer;
        private System.Windows.Forms.ToolStripContainer ToolStripContainer;
        private System.Windows.Forms.ToolStrip ToolStrip;
        private System.Windows.Forms.ToolStripButton NewToolStripButton;
        private System.Windows.Forms.SplitContainer LeftSplitContainer;
        private System.Windows.Forms.ToolStripButton OpenToolStripButton;
        private System.Windows.Forms.OpenFileDialog OpenFileDialog;
        private System.Windows.Forms.ToolStripButton CloseToolStripButton;
        private TileSetPalette TileSetPalette;
        private System.Windows.Forms.ToolStrip TileSetPaletteToolStrip;
        private System.Windows.Forms.ToolStripButton SaveToolStripButton;
        private MapTreeView MapTreeView;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton PenToolStripButton;
        private System.Windows.Forms.ToolStripComboBox TileSetsToolStripComboBox;
        private System.Windows.Forms.ToolStripButton Layer1ToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton Layer2ToolStripButton;
        private System.Windows.Forms.ToolStripButton EventToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton UndoToolStripButton;
        private System.Windows.Forms.ToolStripButton Scale1ToolStripButton;
        private System.Windows.Forms.ToolStripButton Scale2ToolStripButton;
        private System.Windows.Forms.ToolStripButton Scale4ToolStripButton;
        private System.Windows.Forms.ToolStripButton Scale8ToolStripButton;
        private System.Windows.Forms.ToolStripButton PassageToolStripButton;
    }
}

