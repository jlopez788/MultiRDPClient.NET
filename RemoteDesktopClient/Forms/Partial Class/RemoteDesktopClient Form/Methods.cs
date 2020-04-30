using System.Windows.Forms;

namespace MultiRemoteDesktopClient
{
    partial class RemoteDesktopClient
    {
        public void GetServerLists()
        {
            tlvServerLists.Nodes.Clear();
            lvServerLists.Items.Clear();
            lvServerLists.Groups.Clear();

            GetGroups();

            var items = GlobalHelper.dbServers.Items;
            foreach (Database.ServerDetails sd in items)
            {
                // add items to ListView
                ListViewItem item = new ListViewItem(sd.ServerName);
                item.SubItems.Add(sd.Server);
                item.SubItems.Add(sd.Description);
                item.ImageIndex = 1;
                item.Tag = sd;
                item.Group = lvServerLists.Groups["gid" + sd.GroupID.ToString()];

                lvServerLists.Items.Add(item);

                // add items to TreeListView
                object[] o = {
                                 sd.ServerName,
                                 sd.Server,
                                 sd.Description
                             };
                CommonTools.Node n = new CommonTools.Node(o);
                n.Tag = sd;
                var key = $"gid_{sd.GroupID}";
                tlvServerLists.Nodes[key].Nodes.Add(sd.Id.ToString(), n);
                tlvServerLists.Nodes[key].ExpandAll();
            }

            FixListViewColumn();
        }

        public void GetGroups()
        {
            var items = GlobalHelper.dbGroups.Items;
            foreach (Database.GroupDetails gd in items)
            {
                // add groups to ListView
                var key = $"gid_{gd.Id}";
                ListViewGroup lvg = new ListViewGroup(key, gd.GroupName);
                lvServerLists.Groups.Add(lvg);

                // add parent node to TreeListView
                var n = new CommonTools.Node(gd.GroupName);
                n.MakeVisible();
                tlvServerLists.Nodes.Add(key, gd.GroupName);
            }
        }

        public void FixListViewColumn()
        {
            object x = new object();

            lock (x) // force to resize the listview columns
            {
                foreach (ColumnHeader ch in lvServerLists.Columns)
                {
                    ch.Width = -2;
                }
            }
        }

        public RdpClientWindow GetClientWindowByTitleParams(params object[] args)
        {
            RdpClientWindow ret = null;

            string formTitlePattern = "Remote Desktop Client - {0}@{1}[{2}]";
            string formTitle = string.Format(formTitlePattern, args);

            foreach (RdpClientWindow f in MdiChildren)
            {
                if (f.Text == formTitle)
                {
                    ret = f;
                }
            }

            return ret;
        }

        private void DisconnectAll()
        {
            foreach (RdpClientWindow f in MdiChildren)
            {
                f.Disconnect();
            }
        }

        private void ShowMe()
        {
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }
            // get our current "TopMost" value (ours will always be false though)
            bool top = TopMost;
            // make our form jump to the top of everything
            TopMost = true;
            // set it back to whatever it was
            TopMost = top;
        }

        public void SetupServerListPanel(DockStyle dock)
        {
            panelServerLists.Dock = dock;
            panelServerLists.Tag = dock;

            if (dock == DockStyle.None)
            {
                panelServerLists.Top = tabstripLeftPanel.Top;
                panelServerLists.Left = tabstripLeftPanel.Left + tabstripLeftPanel.Width;
                panelServerLists.Height = tabstripLeftPanel.Height;
                panelServerLists.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left;

                panelMDIToolbars.BringToFront();
                panelServerLists.BringToFront();
            }
            else if (dock == DockStyle.Left)
            {
                panelServerLists.Dock = dock;
                panelServerLists.BringToFront();
                panelMDIToolbars.BringToFront();
            }
        }

        public void ConnectByServerName(string server_name)
        {
            ListViewItem item = lvServerLists.FindItemWithText(server_name, false, 0, true);
            if (item != null)
            {
                _selIndex = item.Index;
                Connect();
            }
        }

        public void Connect()
        {
            object x = new object();

            lock (x)
            {
                Database.ServerDetails sd = (Database.ServerDetails)lvServerLists.Items[_selIndex].Tag;

                bool canCreateNewForm = true;
                string formTitlePattern = "Remote Desktop Client - {0}@{1}[{2}]";
                string formTitle = string.Format(formTitlePattern, sd.Username, sd.ServerName, sd.Server);

                foreach (Form f in MdiChildren)
                {
                    if (f.Text == formTitle)
                    {
                        f.Activate();
                        canCreateNewForm = false;
                        break;
                    }
                }

                if (canCreateNewForm)
                {
                    RdpClientWindow clientWin = new RdpClientWindow(sd, this);
                    clientWin.Connected += new Connected(ClientWin_Connected);
                    clientWin.Connecting += new Connecting(ClientWin_Connecting);
                    clientWin.LoginComplete += new LoginComplete(ClientWin_LoginComplete);
                    clientWin.Disconnected += new Disconnected(ClientWin_Disconnected);
                    clientWin.OnFormShown += new OnFormShown(ClientWin_OnFormShown);
                    clientWin.OnFormClosing += new OnFormClosing(ClientWin_OnFormClosing);
                    clientWin.OnFormActivated += new OnFormActivated(ClientWin_OnFormActivated);
                    clientWin.ServerSettingsChanged += new ServerSettingsChanged(ClientWin_ServerSettingsChanged);
                    clientWin.Text = formTitle;
                    clientWin.MdiParent = this;
                    System.Diagnostics.Debug.WriteLine(Handle);
                    clientWin.ListIndex = _selIndex;
                    clientWin.Show();
                    clientWin.BringToFront();
                    clientWin.Connect();
                }
            }
        }

        /// <summary>
        /// Connect all items in a group.
        /// </summary>
        public void GroupConnectAll()
        {
            // hmmm... let's just rely on our ListView

            // check what group are we at
            ListViewGroup thisGroup = lvServerLists.Items[_selIndex].Group;

            // connect all items in the group
            foreach (ListViewItem thisItem in thisGroup.Items)
            {
                _selIndex = thisItem.Index;
                Connect();
            }
        }

        public void GroupConnectAll(string groupname)
        {
            bool foundAGroup = false;

            foreach (ListViewGroup group in lvServerLists.Groups)
            {
                System.Diagnostics.Debug.WriteLine(group.Header + ", " + groupname);
                if (group.Header == groupname)
                {
                    // check if we have items in a group
                    if (group.Items.Count != 0)
                    {
                        // so let's just get the first item
                        _selIndex = group.Items[0].Index;

                        // and connect all items in the group
                        GroupConnectAll();

                        foundAGroup = true;

                        break;
                    }
                }
            }

            if (!foundAGroup)
            {
                MessageBox.Show("No server's found on associated on this group '" + groupname + "'", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public void Welcome()
        {
            DialogResult dr;

            var db = new Database.Groups();
            if (!GlobalHelper.appSettings.IsAppConfigExists())
            {
                dr = MessageBox.Show(
                    "Looks like it's your first time to use Multi Remote Desktop Client .Net!\r\n\r\nThe application created a default password for you called \"pass\".\r\nDo you like to update your password now?",
                    Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question
                );

                if (dr == DialogResult.Yes)
                {
                    // call our toolbar_Configuration event method.
                    toolbar_Configuration_Click(toolbar_Configuration, null);
                }

                // create our new database schema and default datas
                db.ResetDatabase();
            }

            db.Delete(false);
        }

        public bool AskPassword(object sender)
        {
            if (GlobalHelper.appSettings.Settings.Password == string.Empty)
            {
                return false;
            }

            PasswordWindow pw = new PasswordWindow();
            DialogResult dr = pw.ShowDialog(this);

            if (dr == DialogResult.Cancel)
            {
                //lock(this)
                {
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
            }

            return true;
        }
    }
}