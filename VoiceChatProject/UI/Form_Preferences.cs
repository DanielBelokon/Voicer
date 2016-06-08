using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VoicerClient.Properties;
using VoicerClient.Sound;

namespace VoicerClient.UI
{
    public partial class Form_Preferences : Form
    {
        private bool waitingForKeypress = false;
        private Voicer_Main mainForm;

        public Form_Preferences(Voicer_Main mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            UpdateValues();
        }

        private void UpdateValues()
        {
            Pref_MicThreshhold.Value = (int)(Settings.Default.Audio_Threshhold * 10000);
            Pref_PTTButton.Text = ((Keys)Settings.Default.Audio_PTTKey).ToString();
        }

        private void Pref_MicThreshhold_Scroll(object sender, EventArgs e)
        {
            Settings.Default.Audio_Threshhold = (double)Pref_MicThreshhold.Value / 10000;
            Audio.SetThreashhold((double)Pref_MicThreshhold.Value / 10000);
        }

        private void Button_SaveSettings_Click(object sender, EventArgs e)
        {
            Settings.Default.Save();
            this.Close();
        }

        private void Button_ResetSettings_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("All your settings will be reset, are you sure you want to proceed?", "Reset Conformation", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                Settings.Default.Reset();
                UpdateValues();
            }
        }

        private void Pref_PTTButton_Click(object sender, EventArgs e)
        {
            Pref_PTTButton.BackColor = Color.AliceBlue;
            waitingForKeypress = true;
            this.KeyPreview = true;
        }

        private void Form_Preferences_KeyDown(object sender, KeyEventArgs e)
        {
            if (waitingForKeypress && e.KeyCode != Keys.Escape)
            {
                Pref_PTTButton.Text = e.KeyCode.ToString();
                Settings.Default.Audio_PTTKey = (int)e.KeyCode;
            }

            waitingForKeypress = false;
            this.KeyPreview = false;
            Pref_PTTButton.BackColor = Color.GhostWhite;
        }

        private void radioButtons_Changed(object sender, EventArgs e)
        {
            if (radioButton_ptt.Checked)
            {
                Pref_PTTButton.Show();
                Settings.Default.Enable_PTT = true;
                mainForm.EnablePTT();
            }

            else if (radioButton_Detection.Checked)
            {
                Pref_PTTButton.Hide();
                Settings.Default.Enable_PTT = false;
                mainForm.DisablePTT();
            }
            
        }

        private void tab_Audio_Click(object sender, EventArgs e)
        {

        }
    }
}
