using System;
using System.Windows.Forms;

namespace MultiRemoteDesktopClient
{
    public delegate void DelegateDisconnectEvent(object sender, EventArgs e);

    public delegate void DelegateConfigurationEvent(object sender, EventArgs e);

    public delegate void DelegateLockEvent(object sender, EventArgs e);

    public delegate void DelegateServerEvent(object sender, EventArgs e, Database.ServerDetails server_details);

    public class NotificationContextMenu : ContextMenuStrip
    {
        public event DelegateDisconnectEvent OnDisconnect_Clicked;
        public event DelegateConfigurationEvent OnConfiguration_Clicked;
        public event DelegateLockEvent OnLock_Clicked;
        public event DelegateServerEvent OnServer_Clicked;

        public NotificationContextMenu()
        {
            AddMenuItems();
        }

        private void AddMenuItems()
        {
            Items.Clear();

            GlobalHelper.dbGroups.GetGroupsWithServerCount();
            var groups = GlobalHelper.dbGroups.Items;
            var servers = GlobalHelper.dbServers.Items;

            ToolStripMenuItem[] menuItemGroups = new ToolStripMenuItem[groups.Count];

            int cnt = 0;
            foreach (Database.GroupDetails gd in groups)
            {
                //if (gd.ServerCount == 0) { continue; }

                ToolStripMenuItem[] menuItemServers = new ToolStripMenuItem[gd.ServerCount];
                int scnt = 0;
                foreach (Database.ServerDetails sd in servers)
                {
                    if (gd.Id == sd.GroupID)
                    {
                        System.Diagnostics.Debug.WriteLine(sd.ServerName);
                        menuItemServers[scnt] = new ToolStripMenuItem(sd.ServerName, Properties.Resources.mstscax_dll_I345e_0409_16, new EventHandler(Servers_Clicked));
                        menuItemServers[scnt].Tag = sd;
                        menuItemServers[scnt].Name = "menuItem" + sd.ServerName.Replace(" ", "_");
                        scnt++;
                    }
                }

                menuItemGroups[cnt] = new ToolStripMenuItem(gd.GroupName, Properties.Resources.manage_groups_16, menuItemServers);
                cnt++;
            }

            Items.AddRange(new ToolStripItem[] {
                new ToolStripMenuItem("Servers", null, menuItemGroups),
                new ToolStripMenuItem("Disconnect All", Properties.Resources.disconnect_all_16, new EventHandler(DisconnectAll_Clicked)),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Configuration", Properties.Resources.filemgmt_dll_I00ec_0409_16, new EventHandler(Configuration_Clicked)),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Lock", Properties.Resources.LogOff_16, new EventHandler(Lock_Clicked)),
                new ToolStripMenuItem("Exit", Properties.Resources.Shutdown_16, new EventHandler(Exit_Clicked)),
            });
        }

        public void UpdateMenuItems()
        {
            AddMenuItems();
        }

        private void DisconnectAll_Clicked(object sender, EventArgs e)
        {
            OnDisconnect_Clicked?.Invoke(this, e);
        }

        private void Servers_Clicked(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            OnServer_Clicked?.Invoke(this, e, (Database.ServerDetails)menuItem.Tag);
        }

        private void Configuration_Clicked(object sender, EventArgs e)
        {
            OnConfiguration_Clicked?.Invoke(this, e);
        }

        private void Lock_Clicked(object sender, EventArgs e)
        {
            OnLock_Clicked?.Invoke(this, e);
        }

        private void Exit_Clicked(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}