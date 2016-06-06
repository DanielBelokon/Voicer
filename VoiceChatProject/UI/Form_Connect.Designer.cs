namespace VoicerClient.UI
{
    partial class Form_Connect
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
            this.Textbox_ServerIp = new System.Windows.Forms.TextBox();
            this.Textbox_Nickname = new System.Windows.Forms.TextBox();
            this.Button_Connect = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.infolabel_NickName = new System.Windows.Forms.Label();
            this.infolabel_ServerAdress = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Textbox_ServerIp
            // 
            this.Textbox_ServerIp.Location = new System.Drawing.Point(125, 10);
            this.Textbox_ServerIp.Name = "Textbox_ServerIp";
            this.Textbox_ServerIp.Size = new System.Drawing.Size(151, 20);
            this.Textbox_ServerIp.TabIndex = 0;
            this.Textbox_ServerIp.Text = "127.0.0.1";
            // 
            // Textbox_Nickname
            // 
            this.Textbox_Nickname.Location = new System.Drawing.Point(172, 36);
            this.Textbox_Nickname.Name = "Textbox_Nickname";
            this.Textbox_Nickname.Size = new System.Drawing.Size(104, 20);
            this.Textbox_Nickname.TabIndex = 1;
            this.Textbox_Nickname.Text = "Danny";
            // 
            // Button_Connect
            // 
            this.Button_Connect.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Button_Connect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_Connect.Location = new System.Drawing.Point(172, 62);
            this.Button_Connect.Name = "Button_Connect";
            this.Button_Connect.Size = new System.Drawing.Size(104, 23);
            this.Button_Connect.TabIndex = 2;
            this.Button_Connect.Text = "Connect";
            this.Button_Connect.UseVisualStyleBackColor = true;
            this.Button_Connect.Click += new System.EventHandler(this.Button_Connect_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.infolabel_NickName);
            this.panel1.Controls.Add(this.infolabel_ServerAdress);
            this.panel1.Controls.Add(this.Textbox_Nickname);
            this.panel1.Controls.Add(this.Button_Connect);
            this.panel1.Controls.Add(this.Textbox_ServerIp);
            this.panel1.Location = new System.Drawing.Point(4, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(288, 212);
            this.panel1.TabIndex = 3;
            // 
            // infolabel_NickName
            // 
            this.infolabel_NickName.AutoSize = true;
            this.infolabel_NickName.Location = new System.Drawing.Point(27, 39);
            this.infolabel_NickName.Name = "infolabel_NickName";
            this.infolabel_NickName.Size = new System.Drawing.Size(58, 13);
            this.infolabel_NickName.TabIndex = 4;
            this.infolabel_NickName.Text = "Nickname:";
            // 
            // infolabel_ServerAdress
            // 
            this.infolabel_ServerAdress.AutoSize = true;
            this.infolabel_ServerAdress.Location = new System.Drawing.Point(9, 13);
            this.infolabel_ServerAdress.Name = "infolabel_ServerAdress";
            this.infolabel_ServerAdress.Size = new System.Drawing.Size(76, 13);
            this.infolabel_ServerAdress.TabIndex = 3;
            this.infolabel_ServerAdress.Text = "Server Adress:";
            // 
            // Form_Connect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.GhostWhite;
            this.ClientSize = new System.Drawing.Size(304, 236);
            this.Controls.Add(this.panel1);
            this.Name = "Form_Connect";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Connect";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_Connect_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox Textbox_ServerIp;
        private System.Windows.Forms.TextBox Textbox_Nickname;
        private System.Windows.Forms.Button Button_Connect;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label infolabel_NickName;
        private System.Windows.Forms.Label infolabel_ServerAdress;
    }
}