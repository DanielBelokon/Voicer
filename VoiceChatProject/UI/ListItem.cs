using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Voicer.UI
{
    public partial class ListItem : UserControl
    {
        private Color currentColor;

        public Color defaultColor;

        public short channelID;
        public short userID;

        private string text;
        public int id;

        public string LabelText
        {
            get
            {
                return text;
            }
        }

        public ListItem(Color color)
        {
            InitializeComponent();
            text = "";
            defaultColor = color;
            currentColor = color;
            channelID = 0;
            userID = 0;
            BackColor = defaultColor;
        }

        public void SetText(string text)
        {
            this.text = text;
        }

        private void ListItem_MouseLeave(object sender, EventArgs e)
        {
            BackColor = currentColor;
        }

        private void ListItem_MouseEnter(object sender, EventArgs e)
        {
            BackColor = Color.AliceBlue;
        }

        public void SelectItem()
        {
            BackColor = Color.LightBlue;
            currentColor = Color.LightBlue;
        }

        public void DeSelect()
        {
            currentColor = defaultColor;
            BackColor = currentColor;
        }

        private void ListItem_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawString(text, DefaultFont, Brushes.Black, new Point(2, 8));
        }
    }
}
