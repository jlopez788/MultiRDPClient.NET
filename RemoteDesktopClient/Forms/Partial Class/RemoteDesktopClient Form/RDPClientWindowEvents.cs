using System;
using System.Windows.Forms;

namespace MultiRemoteDesktopClient
{
    partial class RemoteDesktopClient
    {
        private void ClientWin_LoginComplete(object sender, EventArgs e, int ListIndex)
        {
            lvServerLists.Items[ListIndex].ImageIndex = 0;
            //tabMDIChild.SelectedTab.ImageIndex = 0;
        }

        private void ClientWin_OnFormActivated(object sender, EventArgs e, int ListIndex, IntPtr Handle)
        {
            //foreach (Crownwood.Magic.Controls.TabPage tabMDI in tabMDIChild.TabPages)
            //{
            //    if ((IntPtr)tabMDI.Tag == Handle)
            //    {
            //        tabMDI.Selected = true;
            //        break;
            //    }
            //}
        }

        private void ClientWin_OnFormClosing(object sender, FormClosingEventArgs e, int ListIndex, IntPtr Handle)
        {
            lvServerLists.Items[ListIndex].ImageIndex = 1;
            //foreach (Crownwood.Magic.Controls.TabPage tabMDI in tabMDIChild.TabPages)
            //{
            //    if ((IntPtr)tabMDI.Tag == Handle)
            //    {
            //        tabMDIChild.TabPages.Remove(tabMDI);
            //        break;
            //    }
            //}

            GlobalHelper.MDIChildrens = MdiChildren;
        }

        private void ClientWin_OnFormShown(object sender, EventArgs e, int ListIndex, IntPtr Handle)
        {
            RdpClientWindow rcw = (RdpClientWindow)sender;

            Crownwood.Magic.Controls.TabPage tabMDI = new Crownwood.Magic.Controls.TabPage(); //(rcw.Text, rcw.toolStrip1);
            tabMDI.Title = lvServerLists.Items[ListIndex].Text;
            tabMDI.ImageIndex = 0;
            tabMDI.Tag = rcw.Handle;
            rcw = null;
            tabMDI.Selected = true;

            //tabMDIChild.TabPages.Add(tabMDI);
            GlobalHelper.MDIChildrens = MdiChildren;
        }

        private void ClientWin_ServerSettingsChanged(object sender, Database.ServerDetails sd, int ListIndex)
        {
            ListViewItem item = lvServerLists.Items[ListIndex];

            if (item != null)
            {
                item.Text = sd.ServerName;
                item.SubItems[1].Text = sd.Server;
                item.SubItems[2].Text = sd.Description;
                item.Tag = sd;
            }
        }
    }
}