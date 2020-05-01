using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace MultiRemoteDesktopClient
{
    partial class RemoteDesktopClient
    {
        public void InitializeEvent_MainToolbars()
        {
            #region import / export buttons
            {
                toolbar_ImportRDP.Click += new EventHandler(ImportExportRDPFile_Button_Click);
                toolbar_ExportRDP.Click += new EventHandler(ImportExportRDPFile_Button_Click);
                m_File_ImportRDP.Click += new EventHandler(ImportExportRDPFile_Button_Click);
                m_File_ExportRDP.Click += new EventHandler(ImportExportRDPFile_Button_Click);
            }
            #endregion

            #region icon views
            {
                m_View_SLIV_Details.Click += new EventHandler(IconViews);
                m_View_SLIV_Tile.Click += new EventHandler(IconViews);
                m_View_SLIV_Tree.Click += new EventHandler(IconViews);
                toolbar_SLIV_Details.Click += new EventHandler(IconViews);
                toolbar_SLIV_Tile.Click += new EventHandler(IconViews);
                toolbar_SLIV_Tree.Click += new EventHandler(IconViews);
            }
            #endregion

            m_Edit_DeleteClient.Click += new EventHandler(btnDelete_Click);
            toolbar_DisconnectAll.Click += new EventHandler(btnDCAll_Click);
            toolbar_DeleteClient.Click += new EventHandler(btnDelete_Click);
            toolbar_ConnectAll.Click += new EventHandler(toolbar_ConnectAll_Click);

            m_Tools_Configuration.Click += new EventHandler(toolbar_Configuration_Click);
            toolbar_Configuration.Click += new EventHandler(toolbar_Configuration_Click);

            m_Edit_ManageGroups.Click += new EventHandler(toolbar_ManageGroups_Click);
            toolbar_ManageGroups.Click += new EventHandler(toolbar_ManageGroups_Click);

            m_File_Lock.Click += new EventHandler(toobar_Lock_Click);
            toobar_Lock.Click += new EventHandler(toobar_Lock_Click);
        }

        void toobar_Lock_Click(object sender, EventArgs e)
        {
            AskPassword(this);
        }

        void toolbar_ManageGroups_Click(object sender, EventArgs e)
        {
            GroupManagerWindow gmw = new GroupManagerWindow();
            gmw.ShowDialog(this);

            GetServerLists();
        }

        void toolbar_Configuration_Click(object sender, EventArgs e)
        {
            ConfigurationWindow f = new ConfigurationWindow();
            f.ShowDialog();

            if (GlobalHelper.appSettings.Settings.Password == string.Empty)
            {
                toobar_Lock.Enabled = false;
            }
            else
            {
                toobar_Lock.Enabled = true;
            }

            // apply some global settings after reconfiguring
            GlobalHelper.infoWin.EnableInformationWindow = !GlobalHelper.appSettings.Settings.HideInformationPopupWindow;
        }

        void toolbar_ConnectAll_Click(object sender, EventArgs e)
        {
            GroupConnectAll();
        }

        void IconViews(object sender, EventArgs e)
        {
            ServerListViews slView = (ServerListViews)((ToolStripMenuItem)sender).Tag;

            splitter.Size = new Size(3, panelServerLists.Height);
            splitter.Location = new Point(panelServerLists.Width - splitter.Width, 0);

            if (slView != ServerListViews.Tree)
            {
                lvServerLists.View = (View)((ToolStripMenuItem)sender).Tag;

                // move the control
                lvServerLists.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                lvServerLists.Location = new Point(0, lblServerListsPanelTitle.Top + lblServerListsPanelTitle.Height);
                lvServerLists.Size = new Size(panelServerLists.Width - splitter.Width, panelServerLists.Height - lvServerLists.Top);
                lvServerLists.BringToFront();
            }
            else if (slView == ServerListViews.Tree)
            {
                tlvServerLists.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                tlvServerLists.Location = new Point(0, lblServerListsPanelTitle.Top + lblServerListsPanelTitle.Height);
                tlvServerLists.Size = new Size(panelServerLists.Width-splitter.Width, panelServerLists.Height - tlvServerLists.Top);
                tlvServerLists.BringToFront();
            }

            splitter.BringToFront();

            m_View_SLIV.Image = ((ToolStripMenuItem)sender).Image;
            toolbar_SLIV.Image = ((ToolStripMenuItem)sender).Image;

            FixListViewColumn();
        }

        void ImportExportRDPFile_Button_Click(object sender, EventArgs e)
        {
            if (sender == toolbar_ImportRDP || sender == m_File_ImportRDP)
            {
                ImportWindow iw = new ImportWindow();
                iw.ShowDialog();

                GetServerLists();
            }
            else if (sender == toolbar_ExportRDP || sender == m_File_ExportRDP)
            {
                ExportWindow ew = new ExportWindow(ref lvServerLists);
                ew.ShowDialog();
            }
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            ServerSettingsWindow ssw = new ServerSettingsWindow();
            ssw.ShowDialog();

            GetServerLists();
        }

        void btnDelete_Click(object sender, EventArgs e)
        {
            Database.ServerDetails sd = (Database.ServerDetails)lvServerLists.Items[_selIndex].Tag;

            DialogResult dr = MessageBox.Show("Are you sure you want to delete this server " + sd.ServerName + " from the server list", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                GlobalHelper.dbServers.DeleteByID(sd.Id);

                GetServerLists();
            }
        }

        void btnDCAll_Click(object sender, EventArgs e)
        {
            DisconnectAll();
        }

        private void OpenSettingsWindow(object sender, EventArgs e)
        {
            Database.ServerDetails sd = (Database.ServerDetails)lvServerLists.Items[_selIndex].Tag;
            
            ServerSettingsWindow ssw = new ServerSettingsWindow(sd);
            ssw.ApplySettings += new ApplySettings(ssw_ApplySettings);
            ssw.GetClientWindowSize += new GetClientWindowSize(ssw_GetClientWindowSize);
            ssw.ShowDialog();

            GetServerLists();
        }

        #region settings window event (check private void OpenSettingsWindow(object sender, EventArgs e) event)

        Rectangle ssw_GetClientWindowSize()
        {
            Database.ServerDetails sd = (Database.ServerDetails)lvServerLists.Items[_selIndex].Tag;
            RdpClientWindow rdpClientWin = GetClientWindowByTitleParams(sd.Username, sd.ServerName, sd.Server);

            if (rdpClientWin != null)
            {
                return rdpClientWin.rdpClient.RectangleToScreen(rdpClientWin.rdpClient.ClientRectangle);
            }
            else
            {
                MessageBox.Show("The relative RDP Client Window for this server does not exists.", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Rectangle r = new Rectangle(0, 0, sd.DesktopWidth, sd.DesktopHeight);
                return r;
            }
        }

        void ssw_ApplySettings(object sender, Database.ServerDetails sd)
        {
            RdpClientWindow rdpClientWin = GetClientWindowByTitleParams(sd.Username, sd.ServerName, sd.Server);

            if (rdpClientWin != null)
            {
                rdpClientWin.CurrentServer = sd;
                rdpClientWin.Reconnect(true, false, false);
            }
             
        }

        #endregion
    }
}
