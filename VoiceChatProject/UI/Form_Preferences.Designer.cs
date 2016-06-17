namespace VoicerClient.UI
{
    partial class Form_Preferences
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
            this.Pref_MicThreshhold = new System.Windows.Forms.TrackBar();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tab_General = new System.Windows.Forms.TabPage();
            this.tab_Audio = new System.Windows.Forms.TabPage();
            this.radioButton_Detection = new System.Windows.Forms.RadioButton();
            this.radioButton_ptt = new System.Windows.Forms.RadioButton();
            this.Pref_PTTButton = new System.Windows.Forms.Label();
            this.Button_SaveSettings = new System.Windows.Forms.Button();
            this.Button_ResetSettings = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Pref_MicThreshhold)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tab_Audio.SuspendLayout();
            this.SuspendLayout();
            // 
            // Pref_MicThreshhold
            // 
            this.Pref_MicThreshhold.BackColor = System.Drawing.Color.GhostWhite;
            this.Pref_MicThreshhold.Location = new System.Drawing.Point(6, 7);
            this.Pref_MicThreshhold.Maximum = 100;
            this.Pref_MicThreshhold.Name = "Pref_MicThreshhold";
            this.Pref_MicThreshhold.Size = new System.Drawing.Size(242, 45);
            this.Pref_MicThreshhold.TabIndex = 0;
            this.Pref_MicThreshhold.Value = 5;
            this.Pref_MicThreshhold.Scroll += new System.EventHandler(this.Pref_MicThreshhold_Scroll);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tab_General);
            this.tabControl1.Controls.Add(this.tab_Audio);
            this.tabControl1.HotTrack = true;
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(408, 135);
            this.tabControl1.TabIndex = 2;
            // 
            // tab_General
            // 
            this.tab_General.BackColor = System.Drawing.Color.GhostWhite;
            this.tab_General.Location = new System.Drawing.Point(4, 22);
            this.tab_General.Name = "tab_General";
            this.tab_General.Padding = new System.Windows.Forms.Padding(3);
            this.tab_General.Size = new System.Drawing.Size(400, 109);
            this.tab_General.TabIndex = 0;
            this.tab_General.Text = "General";
            // 
            // tab_Audio
            // 
            this.tab_Audio.BackColor = System.Drawing.Color.GhostWhite;
            this.tab_Audio.Controls.Add(this.radioButton_Detection);
            this.tab_Audio.Controls.Add(this.radioButton_ptt);
            this.tab_Audio.Controls.Add(this.Pref_PTTButton);
            this.tab_Audio.Controls.Add(this.Pref_MicThreshhold);
            this.tab_Audio.Location = new System.Drawing.Point(4, 22);
            this.tab_Audio.Name = "tab_Audio";
            this.tab_Audio.Padding = new System.Windows.Forms.Padding(3);
            this.tab_Audio.Size = new System.Drawing.Size(400, 109);
            this.tab_Audio.TabIndex = 1;
            this.tab_Audio.Text = "Audio";
            this.tab_Audio.Click += new System.EventHandler(this.tab_Audio_Click);
            // 
            // radioButton_Detection
            // 
            this.radioButton_Detection.AutoSize = true;
            this.radioButton_Detection.Location = new System.Drawing.Point(14, 81);
            this.radioButton_Detection.Name = "radioButton_Detection";
            this.radioButton_Detection.Size = new System.Drawing.Size(101, 17);
            this.radioButton_Detection.TabIndex = 3;
            this.radioButton_Detection.Text = "Voice Detection";
            this.radioButton_Detection.UseVisualStyleBackColor = true;
            this.radioButton_Detection.CheckedChanged += new System.EventHandler(this.radioButtons_Changed);
            // 
            // radioButton_ptt
            // 
            this.radioButton_ptt.AutoSize = true;
            this.radioButton_ptt.Checked = true;
            this.radioButton_ptt.Location = new System.Drawing.Point(14, 58);
            this.radioButton_ptt.Name = "radioButton_ptt";
            this.radioButton_ptt.Size = new System.Drawing.Size(89, 17);
            this.radioButton_ptt.TabIndex = 2;
            this.radioButton_ptt.TabStop = true;
            this.radioButton_ptt.Text = "Push-To-Talk";
            this.radioButton_ptt.UseVisualStyleBackColor = true;
            this.radioButton_ptt.CheckedChanged += new System.EventHandler(this.radioButtons_Changed);
            // 
            // Pref_PTTButton
            // 
            this.Pref_PTTButton.BackColor = System.Drawing.Color.GhostWhite;
            this.Pref_PTTButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Pref_PTTButton.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Pref_PTTButton.Location = new System.Drawing.Point(254, 7);
            this.Pref_PTTButton.Name = "Pref_PTTButton";
            this.Pref_PTTButton.Size = new System.Drawing.Size(133, 33);
            this.Pref_PTTButton.TabIndex = 1;
            this.Pref_PTTButton.Text = "X";
            this.Pref_PTTButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Pref_PTTButton.Click += new System.EventHandler(this.Pref_PTTButton_Click);
            // 
            // Button_SaveSettings
            // 
            this.Button_SaveSettings.Location = new System.Drawing.Point(344, 153);
            this.Button_SaveSettings.Name = "Button_SaveSettings";
            this.Button_SaveSettings.Size = new System.Drawing.Size(72, 23);
            this.Button_SaveSettings.TabIndex = 3;
            this.Button_SaveSettings.Text = "Save";
            this.Button_SaveSettings.UseVisualStyleBackColor = true;
            this.Button_SaveSettings.Click += new System.EventHandler(this.Button_SaveSettings_Click);
            // 
            // Button_ResetSettings
            // 
            this.Button_ResetSettings.Location = new System.Drawing.Point(16, 153);
            this.Button_ResetSettings.Name = "Button_ResetSettings";
            this.Button_ResetSettings.Size = new System.Drawing.Size(102, 23);
            this.Button_ResetSettings.TabIndex = 4;
            this.Button_ResetSettings.Text = "Reset Settings";
            this.Button_ResetSettings.UseVisualStyleBackColor = true;
            this.Button_ResetSettings.Click += new System.EventHandler(this.Button_ResetSettings_Click);
            // 
            // Form_Preferences
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.GhostWhite;
            this.ClientSize = new System.Drawing.Size(433, 187);
            this.Controls.Add(this.Button_ResetSettings);
            this.Controls.Add(this.Button_SaveSettings);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form_Preferences";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Preferences";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_Preferences_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.Pref_MicThreshhold)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tab_Audio.ResumeLayout(false);
            this.tab_Audio.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TrackBar Pref_MicThreshhold;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tab_General;
        private System.Windows.Forms.TabPage tab_Audio;
        private System.Windows.Forms.Button Button_SaveSettings;
        private System.Windows.Forms.Button Button_ResetSettings;
        private System.Windows.Forms.Label Pref_PTTButton;
        private System.Windows.Forms.RadioButton radioButton_Detection;
        private System.Windows.Forms.RadioButton radioButton_ptt;
    }
}