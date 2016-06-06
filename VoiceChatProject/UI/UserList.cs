using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Voicer.ServerObjects;

namespace Voicer.UI
{
    public partial class UserList : UserControl
    {
        protected Server server;

        public int selectedID;

        private List<ListItem> itemControlList;

        public event EventHandler ListItemClicked;

        public UserList()
        {
            itemControlList = new List<ListItem>();
            InitializeComponent();
            selectedID = -1;
            DoubleBuffered = true;
        }

        private void UserList_Load(object sender, EventArgs e)
        {
        }

        public void SetServer(Server server)
        {
            this.server = server;
            server.UserlistUpdate += OnUserlistUpdate;
            CreateItems();
        }

        public void OnUserlistUpdate(object sender, EventArgs e)
        {
            CreateItems();
        }

        private void CreateItems()
        {

            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(CreateItems));
                return;
            }
            int verScrollValue = VerticalScroll.Value;
            AutoScroll = false;
            SuspendLayout();
            ClearUsers();
            itemControlList = new List<ListItem>();
            selectedID = -1;
            int i = 0;

            try
            {
                foreach (Channel channel in server.Channels.ToList())
                {
                    AddItem(channel.Name, i, 0, Color.Gainsboro).channelID = channel.ID;
                    i++;

                    foreach (User curUser in channel.Users)
                    {
                        AddItem(curUser.Name, i, 1, Color.GhostWhite).userID = curUser.ID;
                        i++;
                    }
                }
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("An error occured while creating channels...");
            }
            finally
            {
                ResumeLayout();
                VerticalScroll.Value = verScrollValue;
                AutoScroll = true;
            }
        }

        private ListItem AddItem(string text, int itemId, int hierarchy, Color defColor)
        {
            ListItem newItem = new ListItem(defColor);
            newItem.SetText(text);
            newItem.id = itemId;
            newItem.Location = new Point(hierarchy * 25, itemId * (newItem.Size.Height + 4));
            newItem.Anchor = (AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top);
            newItem.Click += new EventHandler(OnListItemClicked);
            itemControlList.Add(newItem);
            Controls.Add(newItem);

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

        public void UserSwapChannel()
        {

        }

        internal void ClearUsers()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(ClearUsers));
                return;
            }
            foreach (ListItem curItem in itemControlList)
            {
                Controls.Remove(curItem);
                curItem.Dispose();
            }
        }
    }
}
