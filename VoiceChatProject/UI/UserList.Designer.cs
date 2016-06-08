namespace VoicerClient.UI
{
    partial class UserList
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.itemMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.createToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.subChannelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.childChannelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteChannelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editChannelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.itemMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // itemMenuStrip
            // 
            this.itemMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createToolStripMenuItem,
            this.deleteChannelToolStripMenuItem,
            this.editChannelToolStripMenuItem});
            this.itemMenuStrip.Name = "itemMenuStrip";
            this.itemMenuStrip.Size = new System.Drawing.Size(183, 92);
            // 
            // createToolStripMenuItem
            // 
            this.createToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.subChannelToolStripMenuItem,
            this.childChannelToolStripMenuItem});
            this.createToolStripMenuItem.Name = "createToolStripMenuItem";
            this.createToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.createToolStripMenuItem.Text = "Create New Channel";
            // 
            // subChannelToolStripMenuItem
            // 
            this.subChannelToolStripMenuItem.Name = "subChannelToolStripMenuItem";
            this.subChannelToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.subChannelToolStripMenuItem.Text = "Sub Channel";
            // 
            // childChannelToolStripMenuItem
            // 
            this.childChannelToolStripMenuItem.Name = "childChannelToolStripMenuItem";
            this.childChannelToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.childChannelToolStripMenuItem.Text = "Child Channel";
            // 
            // deleteChannelToolStripMenuItem
            // 
            this.deleteChannelToolStripMenuItem.Name = "deleteChannelToolStripMenuItem";
            this.deleteChannelToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.deleteChannelToolStripMenuItem.Text = "Delete Channel";
            // 
            // editChannelToolStripMenuItem
            // 
            this.editChannelToolStripMenuItem.Name = "editChannelToolStripMenuItem";
            this.editChannelToolStripMenuItem.Size = new System.Drawing.Size(182, 22);
            this.editChannelToolStripMenuItem.Text = "Edit Channel";
            // 
            // UserList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(100, 100);
            this.DoubleBuffered = true;
            this.Name = "UserList";
            this.Size = new System.Drawing.Size(296, 463);
            this.Load += new System.EventHandler(this.UserList_Load);
            this.itemMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        #endregion

        private System.Windows.Forms.ContextMenuStrip itemMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem createToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem subChannelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem childChannelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteChannelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editChannelToolStripMenuItem;
    }
}
