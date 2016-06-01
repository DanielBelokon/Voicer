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
    public partial class UserList : UserControl
    {
        protected List<Channel> channelList;

        public int selectedID;

        private List<ListItem> itemControlList;

        public event EventHandler ListItemClicked;

        public UserList()
        {
            channelList = null;
            itemControlList = new List<ListItem>();
            InitializeComponent();
            selectedID = -1;
            DoubleBuffered = true;
        }

        private void UserList_Load(object sender, EventArgs e)
        {
        }

        public void SetUsers(List<Channel> userList)
        {
            if (channelList != userList)
            {
                channelList = userList;
                CreateItems();
            }
        }

        private void CreateItems()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(CreateItems));
                return;
            }
            this.SuspendLayout();
            ClearUsers();
            itemControlList = new List<ListItem>();
            selectedID = -1;
            int i = 0;
            
            foreach (Channel channel in channelList.ToList())
            {
                AddItem(channel.name, i, 0, Color.Gainsboro).channel = channel;
                i++;

                foreach (User curUser in channel.users)
                {
                    AddItem(curUser.nickname, i, 1, Color.GhostWhite).user = curUser;
                    i++;
                }
            }
            this.ResumeLayout();
        }

        private ListItem AddItem(string text, int itemId, int hierarchy, Color defColor)
        {
            ListItem newItem = new ListItem(defColor);
            newItem.SetText(text);
            newItem.id = itemId;
            newItem.Location = new Point(hierarchy * 25, itemId * (newItem.Size.Height + 4));
            newItem.Anchor = (AnchorStyles.Left | AnchorStyles.Right);
            newItem.Click += new EventHandler(OnListItemClicked);
            itemControlList.Add(newItem);
            this.Controls.Add(newItem);

            return newItem;
        }

        private void OnListItemClicked(object sender, EventArgs e)
        {
            ListItem clicked = (ListItem)sender;

            foreach (ListItem item in itemControlList)
            {
                if (selectedID == item.id)
                {
                    item.DeSelect();
                }
            }

            selectedID = clicked.id;
            clicked.SelectItem();

            // Raise the click event so other classes can interact with it
            if (ListItemClicked != null)
                ListItemClicked(clicked, EventArgs.Empty); 
        }

        internal void ClearUsers()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(ClearUsers));
                return;
            }
            foreach (ListItem curItem in itemControlList)
            {
                this.Controls.Remove(curItem);
                curItem.Dispose();
            }
        }
    }
}
