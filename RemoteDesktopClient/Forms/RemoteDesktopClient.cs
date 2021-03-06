﻿using Database;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MultiRemoteDesktopClient
{
    public partial class RemoteDesktopClient : Form
    {
        private class RdpContext
        {
            public ServerDetails Server { get; set; }
            public RdpState State { get; set; }

            public RdpContext(ServerDetails server, RdpState state)
            {
                Server = server;
                State = state;
            }

            public override bool Equals(object obj) => obj is RdpContext rdp && (Server?.Equals(rdp?.Server) ?? false);

            public override int GetHashCode() => Server?.GetHashCode() ?? 0;
        }

        private FormWindowState _lastWindowState;
        private Controls.TreeListViewControlHooks tlvch;
        private HashSet<RdpContext> Contexts = new HashSet<RdpContext>(10);

        public RemoteDesktopClient()
        {
            Initialize();

            Welcome();

            var size = Screen.PrimaryScreen.Bounds.Size;
            Size = new Size(Convert.ToInt32(size.Width * 0.9), Convert.ToInt32(size.Height * 0.8));
            if (!AskPassword(this))
            {
                toobar_Lock.Enabled = false;
            }
        }

        public void DoArguments(string[] args)
        {
            /*
             * List of valid arguments:
             *
             * /sname <server name>
             * - connect to existing server by Server Name
             *
             * /gname <group name>
             * - Connect to multiple server inside a group by providing a Group Name
            */
            var cmdArgs = new CommandLine.Utility.Arguments(args);
            if (cmdArgs["sname"] != null)
            {
                ConnectByServerName(cmdArgs["sname"]);
            }

            if (cmdArgs["gname"] != null)
            {
                GroupConnectAll(cmdArgs["gname"]);
            }
        }

        public void Initialize()
        {
            InitializeComponent();
            InitializeControl();
            InitializeControlEvents();
        }

        public void InitializeControl()
        {
            //dont show the Pin button for a while
            btnPinServerLists.Visible = false;
            // set initial panel dock style
            panelServerLists.Tag = DockStyle.None;
            // simulate clicking to Pin Button
            btnPinServerLists_Click(btnPinServerLists, null);

            #region views

            {
                m_View_SLIV_Details.Tag = ServerListViews.Details;
                m_View_SLIV_Tile.Tag = ServerListViews.Tile;
                m_View_SLIV_Tree.Tag = ServerListViews.Tree;
                toolbar_SLIV_Details.Tag = ServerListViews.Details;
                toolbar_SLIV_Tile.Tag = ServerListViews.Tile;
                toolbar_SLIV_Tree.Tag = ServerListViews.Tree;
            }

            #endregion

            #region Informatin Window

            GlobalHelper.infoWin.EnableInformationWindow = !GlobalHelper.appSettings.Settings.HideInformationPopupWindow;
            GlobalHelper.infoWin.AddControl(new object[] {
                    lvServerLists,
                    tlvServerLists,
                    toolbar_EditSettings
                });

            #endregion

            #region listview server list control hooks

            lvServerLists.AddControlForEmptyListItem(new object[] {
                    toolbar_DeleteClient,
                    toolbar_EditSettings,
                    toolbar_ConnectAll,
                    m_Edit_DeleteClient,
                    m_File_EditSettings
                });

            lvServerLists.AddControlForItemSelection(new object[] {
                    toolbar_DeleteClient,
                    toolbar_EditSettings,
                    toolbar_ConnectAll,
                    m_Edit_DeleteClient,
                    m_File_EditSettings,
                    lvServerListsContextMenu_DeleteClient,
                    lvServerListsContextMenu_EditClientSettings,
                    lvServerListsContextMenu_ConnectAll
                });

            #endregion

            #region tree listview control hooks

            tlvch = new Controls.TreeListViewControlHooks(ref tlvServerLists);

            tlvch.AddControlForEmptyListItem(new object[] {
                    toolbar_DeleteClient,
                    toolbar_EditSettings,
                    toolbar_ConnectAll,
                    m_Edit_DeleteClient,
                    m_File_EditSettings
                });

            tlvch.AddControlForItemSelection(new object[] {
                    toolbar_DeleteClient,
                    toolbar_EditSettings,
                    toolbar_ConnectAll,
                    m_Edit_DeleteClient,
                    m_File_EditSettings,
                    lvServerListsContextMenu_DeleteClient,
                    lvServerListsContextMenu_EditClientSettings,
                    lvServerListsContextMenu_ConnectAll
                });

            #endregion

            #region treelistview columns

            // TreeListView's Design time support is so buggy and usually deletes the columns
            tlvServerLists.Columns.AddRange(new CommonTools.TreeListColumn[] {
                new CommonTools.TreeListColumn("server_name", "Server Name", 50),
                new CommonTools.TreeListColumn("server", "Server ", 50),
                new CommonTools.TreeListColumn("descr", "Description", 50)
            });
            tlvServerLists.Columns["server_name"].AutoSize = true;
            tlvServerLists.Columns["server_name"].AutoSizeRatio = 100;
            tlvServerLists.Columns["server"].AutoSize = true;
            tlvServerLists.Columns["server"].AutoSizeRatio = 50;

            #endregion

            _lastPanelWidth = panelServerLists.Width;

            // change server list view
            IconViews(toolbar_SLIV_Tree, null);

            // show thumbnail form;
            //RDThumbnailsWindow rdtnwin = new RDThumbnailsWindow();
            //rdtnwin.Show();
        }

        public void InitializeControlEvents()
        {
            Shown += new EventHandler(RemoteDesktopClient_Shown);
            FormClosing += new FormClosingEventHandler(RemoteDesktopClient_FormClosing);
            SizeChanged += new EventHandler(RemoteDesktopClient_SizeChanged);
            m_Help_About.Click += new EventHandler(AboutToolStripMenuItem_Click);

            #region splitter

            {
                splitter.MouseDown += new MouseEventHandler(Splitter_MouseDown);
                splitter.MouseMove += new MouseEventHandler(Splitter_MouseMove);
                splitter.MouseUp += new MouseEventHandler(Splitter_MouseUp);
            }

            #endregion

            #region main toolbar events

            InitializeEvent_MainToolbars();

            #endregion

            #region server lists events

            InitializeServerListEvents();

            #endregion

            #region mdi tabs

            //tabMDIChild.SelectionChanged += new EventHandler(TabMDIChild_SelectionChanged);
            //tabMDIChild.ClosePressed += new EventHandler(TabMDIChild_ClosePressed);

            #endregion

            #region system tray

            systray.DoubleClick += new EventHandler(Systray_DoubleClick);

            #endregion
        }

        #region default events

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutWindow aw = new AboutWindow();
            aw.ShowDialog();
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Ncm_OnServer_Clicked(object sender, EventArgs e, Database.ServerDetails server_details)
        {
            ListViewItem thisItem = lvServerLists.FindItemWithText(server_details.ServerName, false, 0);
            if (thisItem != null)
            {
                _selIndex = thisItem.Index;
                Connect();
            }
        }

        private void RemoteDesktopClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Contexts.Count == 0)
                return;

            DialogResult dr = MessageBox.Show("Are you sure you want to exit", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void RemoteDesktopClient_Shown(object sender, EventArgs e)
        {
            GetServerLists();

            DoArguments(Environment.GetCommandLineArgs());

            NotificationContextMenu ncm = new NotificationContextMenu();
            ncm.OnDisconnect_Clicked += new DelegateDisconnectEvent(btnDCAll_Click);
            ncm.OnConfiguration_Clicked += new DelegateConfigurationEvent(toolbar_Configuration_Click);
            ncm.OnLock_Clicked += new DelegateLockEvent(toobar_Lock_Click);
            ncm.OnServer_Clicked += new DelegateServerEvent(Ncm_OnServer_Clicked);

            systray.ContextMenuStrip = ncm;
        }

        #region show/hide toolbar and stats events

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        #endregion

        #region client window layout

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void LayoutMdi_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mItem = (ToolStripMenuItem)sender;

            LayoutMdi((MdiLayout)int.Parse(mItem.Tag.ToString()));
        }

        #endregion

        #region mdi tabs

        private void TabMDIChild_ClosePressed(object sender, EventArgs e)
        {
            ActiveMdiChild?.Close();
        }

        private void TabMDIChild_SelectionChanged(object sender, EventArgs e)
        {
            //if (tabMDIChild.SelectedTab == null)
            //    return;

            //foreach (RdpClientWindow f in MdiChildren)
            //{
            //    if ((IntPtr)tabMDIChild.SelectedTab.Tag == f.Handle)
            //    {
            //        f.Activate();
            //        f.rdpClient.Focus();
            //        break;
            //    }
            //}
        }

        #endregion

        #region splitter stuff

        // we have to programmatically move the splitter inside the server lists panel
        // because we have a floating panel which was the server lists panel
        private int splitX = 0;

        private void Splitter_MouseDown(object sender, MouseEventArgs e)
        {
            splitX = e.X;
            splitter.BackColor = Color.FromKnownColor(KnownColor.ActiveCaption);
        }

        private void Splitter_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                splitter.Left += e.X - splitX;
                panelServerLists.Width = splitter.Left + splitter.Width;
            }
        }

        private void Splitter_MouseUp(object sender, MouseEventArgs e)
        {
            splitter.BackColor = Color.FromKnownColor(KnownColor.Control);
        }

        #endregion

        private void RemoteDesktopClient_SizeChanged(object sender, EventArgs e)
        {
            // we only want the Maximize and Normal state of this window
            if (WindowState != FormWindowState.Minimized)
            {
                _lastWindowState = WindowState;
            }

            if (WindowState == FormWindowState.Minimized)
            {
                if (GlobalHelper.appSettings.Settings.HideWhenMinimized)
                {
                    Visible = false;
                }
            }
        }

        private void Systray_DoubleClick(object sender, EventArgs e)
        {
            Visible = true;
            WindowState = _lastWindowState;
            Activate();
            BringToFront();
        }

        #endregion
    }
}