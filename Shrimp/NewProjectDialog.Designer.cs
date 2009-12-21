namespace Shrimp
{
    partial class NewProjectDialog
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
            this.DirectoryNameTextBox = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.OKButton = new System.Windows.Forms.Button();
            this.GameTitleTextBox = new System.Windows.Forms.TextBox();
            this.BasePathButton = new System.Windows.Forms.Button();
            this.BasePathTextBox = new System.Windows.Forms.TextBox();
            this.FolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.ErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.DirectoryNameLabel = new System.Windows.Forms.Label();
            this.GameTitleLabel = new System.Windows.Forms.Label();
            this.BasePathLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ErrorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // DirectoryNameTextBox
            // 
            this.DirectoryNameTextBox.Location = new System.Drawing.Point(8, 24);
            this.DirectoryNameTextBox.Name = "DirectoryNameTextBox";
            this.DirectoryNameTextBox.Size = new System.Drawing.Size(160, 19);
            this.DirectoryNameTextBox.TabIndex = 0;
            this.DirectoryNameTextBox.Text = "samancing-roga";
            this.DirectoryNameTextBox.TextChanged += new System.EventHandler(this.DirectoryNameTextBox_TextChanged);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(272, 120);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(72, 24);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // OKButton
            // 
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(192, 120);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(72, 24);
            this.OKButton.TabIndex = 2;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            // 
            // GameTitleTextBox
            // 
            this.GameTitleTextBox.Location = new System.Drawing.Point(184, 24);
            this.GameTitleTextBox.Name = "GameTitleTextBox";
            this.GameTitleTextBox.Size = new System.Drawing.Size(160, 19);
            this.GameTitleTextBox.TabIndex = 0;
            this.GameTitleTextBox.Text = "Samancing Roga";
            this.GameTitleTextBox.TextChanged += new System.EventHandler(this.GameTitleTextBox_TextChanged);
            // 
            // BasePathButton
            // 
            this.BasePathButton.Location = new System.Drawing.Point(320, 72);
            this.BasePathButton.Name = "BasePathButton";
            this.BasePathButton.Size = new System.Drawing.Size(24, 20);
            this.BasePathButton.TabIndex = 1;
            this.BasePathButton.Text = "...";
            this.BasePathButton.UseVisualStyleBackColor = true;
            this.BasePathButton.Click += new System.EventHandler(this.FolderPathButton_Click);
            // 
            // BasePathTextBox
            // 
            this.BasePathTextBox.Location = new System.Drawing.Point(8, 72);
            this.BasePathTextBox.Name = "BasePathTextBox";
            this.BasePathTextBox.Size = new System.Drawing.Size(312, 19);
            this.BasePathTextBox.TabIndex = 0;
            this.BasePathTextBox.TextChanged += new System.EventHandler(this.BasePathTextBox_TextChanged);
            // 
            // ErrorProvider
            // 
            this.ErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.ErrorProvider.ContainerControl = this;
            // 
            // DirectoryNameLabel
            // 
            this.DirectoryNameLabel.AutoSize = true;
            this.DirectoryNameLabel.Location = new System.Drawing.Point(8, 8);
            this.DirectoryNameLabel.Name = "DirectoryNameLabel";
            this.DirectoryNameLabel.Size = new System.Drawing.Size(72, 12);
            this.DirectoryNameLabel.TabIndex = 3;
            this.DirectoryNameLabel.Text = "Folder Name:";
            // 
            // GameTitleLabel
            // 
            this.GameTitleLabel.AutoSize = true;
            this.GameTitleLabel.Location = new System.Drawing.Point(184, 8);
            this.GameTitleLabel.Name = "GameTitleLabel";
            this.GameTitleLabel.Size = new System.Drawing.Size(63, 12);
            this.GameTitleLabel.TabIndex = 4;
            this.GameTitleLabel.Text = "Game Title:";
            // 
            // BasePathLabel
            // 
            this.BasePathLabel.AutoSize = true;
            this.BasePathLabel.Location = new System.Drawing.Point(8, 56);
            this.BasePathLabel.Name = "BasePathLabel";
            this.BasePathLabel.Size = new System.Drawing.Size(30, 12);
            this.BasePathLabel.TabIndex = 5;
            this.BasePathLabel.Text = "Path:";
            // 
            // NewProjectDialog
            // 
            this.AcceptButton = this.OKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(510, 248);
            this.Controls.Add(this.BasePathLabel);
            this.Controls.Add(this.GameTitleLabel);
            this.Controls.Add(this.DirectoryNameLabel);
            this.Controls.Add(this.DirectoryNameTextBox);
            this.Controls.Add(this.GameTitleTextBox);
            this.Controls.Add(this.BasePathButton);
            this.Controls.Add(this.BasePathTextBox);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewProjectDialog";
            this.Padding = new System.Windows.Forms.Padding(0, 0, 5, 5);
            this.Text = "New Project";
            ((System.ComponentModel.ISupportInitialize)(this.ErrorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox DirectoryNameTextBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.TextBox GameTitleTextBox;
        private System.Windows.Forms.Button BasePathButton;
        private System.Windows.Forms.TextBox BasePathTextBox;
        private System.Windows.Forms.FolderBrowserDialog FolderBrowserDialog;
        private System.Windows.Forms.ErrorProvider ErrorProvider;
        private System.Windows.Forms.Label BasePathLabel;
        private System.Windows.Forms.Label GameTitleLabel;
        private System.Windows.Forms.Label DirectoryNameLabel;
    }
}