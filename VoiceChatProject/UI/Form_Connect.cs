using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VoicerClient.UI
{
    public partial class Form_Connect : Form
    {
        public string serverIp;
        public string nickname;

        private bool invalidInput;

        public Form_Connect()
        {
            InitializeComponent();
            invalidInput = false;
        }

        private void Button_Connect_Click(object sender, EventArgs e)
        {
            serverIp = Textbox_ServerIp.Text;
            nickname = Textbox_Nickname.Text;

            if (serverIp == null || serverIp == "" || nickname == "" || nickname == null)
            {
                MessageBox.Show("You must enter a valid address or nickname!", "Error");

                // Don't close the form if input is invalid
                invalidInput = true;
            }
        }

        private void Form_Connect_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = invalidInput;
            invalidInput = false;
        }
    }
}
