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

        public Channel channel;
        public User user;

        private string text;
        public int id;

        public string LabelText
        {
            get
            {
                return this.text;
            }
        }

        public ListItem(Color color)
        {
            InitializeComponent();
            this.text = "";
            TextLabel.Text = "";
            defaultColor = color;
            currentColor = color;
            this.BackColor = defaultColor;
        }

        public void SetText(string text)
        {
            this.text = text;
            TextLabel.Text = text;
        }

        private void ListItem_MouseLeave(object sender, EventArgs e)
        {
            this.BackColor = currentColor;
        }

        private void ListItem_MouseEnter(object sender, EventArgs e)
        {
            this.BackColor = Color.AliceBlue;
        }

        public void SelectItem()
        {
            this.BackColor = Color.LightBlue;
            currentColor = Color.LightBlue;
        }

        public void DeSelect()
        {
            currentColor = defaultColor;
            this.BackColor = currentColor;
        }
    }
}
