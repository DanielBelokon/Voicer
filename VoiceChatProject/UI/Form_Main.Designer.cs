namespace VoicerClient.UI
{
    partial class Voicer_Main
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.MenuStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.connectToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.splitContainer_Chat = new System.Windows.Forms.SplitContainer();
            this.splitContainer_Users = new System.Windows.Forms.SplitContainer();
            this.label1 = new System.Windows.Forms.Label();
            this.SendChatButton = new System.Windows.Forms.Button();
            this.InputBox_BackPanel = new System.Windows.Forms.Panel();
            this.chatbox_Input = new System.Windows.Forms.RichTextBox();
            this.ChatTab = new System.Windows.Forms.TabControl();
            this.ChatTab_Server = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chatbox_Server = new System.Windows.Forms.RichTextBox();
            this.ChatTab_Channel = new System.Windows.Forms.TabPage();
            this.ChatBox_BackPanel = new System.Windows.Forms.Panel();
            this.chatbox_Output = new System.Windows.Forms.RichTextBox();
            this.ChatArea_BackPanel = new System.Windows.Forms.Panel();
            this.ClientListControl = new VoicerClient.UI.UserList();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Chat)).BeginInit();
            this.splitContainer_Chat.Panel1.SuspendLayout();
            this.splitContainer_Chat.Panel2.SuspendLayout();
            this.splitContainer_Chat.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Users)).BeginInit();
            this.splitContainer_Users.Panel1.SuspendLayout();
            this.splitContainer_Users.Panel2.SuspendLayout();
            this.splitContainer_Users.SuspendLayout();
            this.InputBox_BackPanel.SuspendLayout();
            this.ChatTab.SuspendLayout();
            this.ChatTab_Server.SuspendLayout();
            this.panel1.SuspendLayout();
            this.ChatTab_Channel.SuspendLayout();
            this.ChatBox_BackPanel.SuspendLayout();
            this.ChatArea_BackPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuStrip,
            this.settingsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1122, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "MenuStrip";
            // 
            // MenuStrip
            // 
            this.MenuStrip.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectToToolStripMenuItem,
            this.disconnectToolStripMenuItem});
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Size = new System.Drawing.Size(64, 20);
            this.MenuStrip.Text = "Connect";
            // 
            // connectToToolStripMenuItem
            // 
            this.connectToToolStripMenuItem.Name = "connectToToolStripMenuItem";
            this.connectToToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.connectToToolStripMenuItem.Text = "Connect to...";
            this.connectToToolStripMenuItem.Click += new System.EventHandler(this.connectToToolStripMenuItem_Click);
            // 
            // disconnectToolStripMenuItem
            // 
            this.disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
            this.disconnectToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.disconnectToolStripMenuItem.Text = "Disconnect";
            this.disconnectToolStripMenuItem.Click += new System.EventHandler(this.disconnectToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.preferencesToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.preferencesToolStripMenuItem.Text = "Preferences";
            this.preferencesToolStripMenuItem.Click += new System.EventHandler(this.preferencesToolStripMenuItem_Click);
            // 
            // StatusLabel
            // 
            this.StatusLabel.AutoSize = true;
            this.StatusLabel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatusLabel.Location = new System.Drawing.Point(12, 619);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(0, 14);
            this.StatusLabel.TabIndex = 5;
            // 
            // splitContainer_Chat
            // 
            this.splitContainer_Chat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer_Chat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_Chat.Location = new System.Drawing.Point(0, 0);
            this.splitContainer_Chat.Name = "splitContainer_Chat";
            this.splitContainer_Chat.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer_Chat.Panel1
            // 
            this.splitContainer_Chat.Panel1.Controls.Add(this.splitContainer_Users);
            // 
            // splitContainer_Chat.Panel2
            // 
            this.splitContainer_Chat.Panel2.Controls.Add(this.SendChatButton);
            this.splitContainer_Chat.Panel2.Controls.Add(this.InputBox_BackPanel);
            this.splitContainer_Chat.Panel2.Controls.Add(this.ChatTab);
            this.splitContainer_Chat.Size = new System.Drawing.Size(1122, 611);
            this.splitContainer_Chat.SplitterDistance = 426;
            this.splitContainer_Chat.TabIndex = 6;
            this.splitContainer_Chat.TabStop = false;
            // 
            // splitContainer_Users
            // 
            this.splitContainer_Users.BackColor = System.Drawing.Color.GhostWhite;
            this.splitContainer_Users.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer_Users.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_Users.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer_Users.Location = new System.Drawing.Point(0, 0);
            this.splitContainer_Users.Name = "splitContainer_Users";
            // 
            // splitContainer_Users.Panel1
            // 
            this.splitContainer_Users.Panel1.Controls.Add(this.ClientListControl);
            // 
            // splitContainer_Users.Panel2
            // 
            this.splitContainer_Users.Panel2.Controls.Add(this.label1);
            this.splitContainer_Users.Size = new System.Drawing.Size(1122, 426);
            this.splitContainer_Users.SplitterDistance = 356;
            this.splitContainer_Users.TabIndex = 0;
            this.splitContainer_Users.TabStop = false;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(74, 391);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(16, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "...";
            // 
            // SendChatButton
            // 
            this.SendChatButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SendChatButton.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.SendChatButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SendChatButton.Location = new System.Drawing.Point(1069, 131);
            this.SendChatButton.Name = "SendChatButton";
            this.SendChatButton.Size = new System.Drawing.Size(40, 40);
            this.SendChatButton.TabIndex = 2;
            this.SendChatButton.Text = "button1";
            this.SendChatButton.UseVisualStyleBackColor = false;
            this.SendChatButton.Click += new System.EventHandler(this.SendChatButton_Click);
            // 
            // InputBox_BackPanel
            // 
            this.InputBox_BackPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InputBox_BackPanel.BackColor = System.Drawing.SystemColors.ControlDark;
            this.InputBox_BackPanel.Controls.Add(this.chatbox_Input);
            this.InputBox_BackPanel.Location = new System.Drawing.Point(11, 131);
            this.InputBox_BackPanel.Name = "InputBox_BackPanel";
            this.InputBox_BackPanel.Size = new System.Drawing.Size(1052, 42);
            this.InputBox_BackPanel.TabIndex = 4;
            // 
            // chatbox_Input
            // 
            this.chatbox_Input.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chatbox_Input.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.chatbox_Input.Font = new System.Drawing.Font("Arial Rounded MT Bold", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chatbox_Input.Location = new System.Drawing.Point(1, 1);
            this.chatbox_Input.MaxLength = 1024;
            this.chatbox_Input.Name = "chatbox_Input";
            this.chatbox_Input.Size = new System.Drawing.Size(1050, 40);
            this.chatbox_Input.TabIndex = 1;
            this.chatbox_Input.Text = "";
            this.chatbox_Input.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chatbox_Input_KeyDown);
            // 
            // ChatTab
            // 
            this.ChatTab.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.ChatTab.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ChatTab.Controls.Add(this.ChatTab_Server);
            this.ChatTab.Controls.Add(this.ChatTab_Channel);
            this.ChatTab.HotTrack = true;
            this.ChatTab.Location = new System.Drawing.Point(3, 3);
            this.ChatTab.Name = "ChatTab";
            this.ChatTab.SelectedIndex = 0;
            this.ChatTab.Size = new System.Drawing.Size(1115, 122);
            this.ChatTab.TabIndex = 6;
            // 
            // ChatTab_Server
            // 
            this.ChatTab_Server.BackColor = System.Drawing.Color.GhostWhite;
            this.ChatTab_Server.Controls.Add(this.panel1);
            this.ChatTab_Server.Location = new System.Drawing.Point(4, 4);
            this.ChatTab_Server.Name = "ChatTab_Server";
            this.ChatTab_Server.Padding = new System.Windows.Forms.Padding(3);
            this.ChatTab_Server.Size = new System.Drawing.Size(1107, 96);
            this.ChatTab_Server.TabIndex = 0;
            this.ChatTab_Server.Text = "Server";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Controls.Add(this.chatbox_Server);
            this.panel1.Location = new System.Drawing.Point(5, 6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1094, 83);
            this.panel1.TabIndex = 1;
            // 
            // chatbox_Server
            // 
            this.chatbox_Server.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.chatbox_Server.Location = new System.Drawing.Point(1, 1);
            this.chatbox_Server.Name = "chatbox_Server";
            this.chatbox_Server.Size = new System.Drawing.Size(1092, 81);
            this.chatbox_Server.TabIndex = 0;
            this.chatbox_Server.Text = "";
            // 
            // ChatTab_Channel
            // 
            this.ChatTab_Channel.BackColor = System.Drawing.Color.GhostWhite;
            this.ChatTab_Channel.Controls.Add(this.ChatBox_BackPanel);
            this.ChatTab_Channel.Location = new System.Drawing.Point(4, 4);
            this.ChatTab_Channel.Name = "ChatTab_Channel";
            this.ChatTab_Channel.Padding = new System.Windows.Forms.Padding(3);
            this.ChatTab_Channel.Size = new System.Drawing.Size(1107, 96);
            this.ChatTab_Channel.TabIndex = 1;
            this.ChatTab_Channel.Text = "Channel";
            // 
            // ChatBox_BackPanel
            // 
            this.ChatBox_BackPanel.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ChatBox_BackPanel.Controls.Add(this.chatbox_Output);
            this.ChatBox_BackPanel.Location = new System.Drawing.Point(5, 6);
            this.ChatBox_BackPanel.Name = "ChatBox_BackPanel";
            this.ChatBox_BackPanel.Size = new System.Drawing.Size(1095, 83);
            this.ChatBox_BackPanel.TabIndex = 5;
            // 
            // chatbox_Output
            // 
            this.chatbox_Output.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chatbox_Output.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.chatbox_Output.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.chatbox_Output.Font = new System.Drawing.Font("Arial Rounded MT Bold", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chatbox_Output.Location = new System.Drawing.Point(1, 1);
            this.chatbox_Output.Name = "chatbox_Output";
            this.chatbox_Output.ReadOnly = true;
            this.chatbox_Output.Size = new System.Drawing.Size(1093, 81);
            this.chatbox_Output.TabIndex = 0;
            this.chatbox_Output.TabStop = false;
            this.chatbox_Output.Text = "";
            this.chatbox_Output.TextChanged += new System.EventHandler(this.chatbox_Output_TextChanged);
            // 
            // ChatArea_BackPanel
            // 
            this.ChatArea_BackPanel.Controls.Add(this.splitContainer_Chat);
            this.ChatArea_BackPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChatArea_BackPanel.Location = new System.Drawing.Point(0, 24);
            this.ChatArea_BackPanel.Name = "ChatArea_BackPanel";
            this.ChatArea_BackPanel.Size = new System.Drawing.Size(1122, 611);
            this.ChatArea_BackPanel.TabIndex = 7;
            // 
            // ClientListControl
            // 
            this.ClientListControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ClientListControl.AutoScroll = true;
            this.ClientListControl.AutoScrollMinSize = new System.Drawing.Size(100, 100);
            this.ClientListControl.Location = new System.Drawing.Point(3, 3);
            this.ClientListControl.Name = "ClientListControl";
            this.ClientListControl.Size = new System.Drawing.Size(348, 418);
            this.ClientListControl.TabIndex = 0;
            // 
            // Voicer_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.GhostWhite;
            this.ClientSize = new System.Drawing.Size(1122, 635);
            this.Controls.Add(this.ChatArea_BackPanel);
            this.Controls.Add(this.StatusLabel);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Voicer_Main";
            this.Text = "Voicer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Voicer_Main_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer_Chat.Panel1.ResumeLayout(false);
            this.splitContainer_Chat.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Chat)).EndInit();
            this.splitContainer_Chat.ResumeLayout(false);
            this.splitContainer_Users.Panel1.ResumeLayout(false);
            this.splitContainer_Users.Panel2.ResumeLayout(false);
            this.splitContainer_Users.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Users)).EndInit();
            this.splitContainer_Users.ResumeLayout(false);
            this.InputBox_BackPanel.ResumeLayout(false);
            this.ChatTab.ResumeLayout(false);
            this.ChatTab_Server.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ChatTab_Channel.ResumeLayout(false);
            this.ChatBox_BackPanel.ResumeLayout(false);
            this.ChatArea_BackPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem MenuStrip;
        private System.Windows.Forms.ToolStripMenuItem connectToToolStripMenuItem;
        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.SplitContainer splitContainer_Chat;
        private System.Windows.Forms.SplitContainer splitContainer_Users;
        private System.Windows.Forms.Panel ChatArea_BackPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox chatbox_Input;
        private System.Windows.Forms.Panel InputBox_BackPanel;
        private System.Windows.Forms.Panel ChatBox_BackPanel;
        private System.Windows.Forms.Button SendChatButton;
        private System.Windows.Forms.RichTextBox chatbox_Output;
        private UI.UserList ClientListControl;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disconnectToolStripMenuItem;
        private System.Windows.Forms.TabPage ChatTab_Server;
        private System.Windows.Forms.TabPage ChatTab_Channel;
        private System.Windows.Forms.TabControl ChatTab;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RichTextBox chatbox_Server;
    }
}

