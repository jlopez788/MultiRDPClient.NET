using AxMSTSCLib;
using Database;
using System;
using System.Drawing;
using System.Windows.Forms;
using Win32APIs;

namespace MultiRemoteDesktopClient
{
    public partial class RdpClientWindow : Form
    {
        public event EventHandler<StateChangeEventArgs> StateChange;
        public event LoginComplete LoginComplete;
        public event OnFormActivated OnFormActivated;
        public event OnFormShown OnFormShown;
        public event OnFormClosing OnRdpFormClosing;
        public event ServerSettingsChanged ServerSettingsChanged;
        public const int WM_LEAVING_FULLSCREEN = 0x4ff;
        private bool _isFitToWindow = false;
        private PopupMDIContainer popupmdi = null;
        public ServerDetails CurrentServer { get; set; }
        public int ListIndex { get; set; }
        public RdpState State { get; private set; }

        public RdpClientWindow(ServerDetails sd, Form parent)
        {
            InitializeComponent();
            InitializeControl(sd);
            InitializeControlEvents();

            MdiParent = parent;
            Visible = true;
        }

        public void InitializeControl(ServerDetails sd)
        {
            GlobalHelper.infoWin.AddControl(new object[] {
                btnFitToScreen
            });

            CurrentServer = sd;

            rdpClient.Server = sd.Server;
            rdpClient.UserName = sd.Username;
            //rdpClient.Domain = sd.dom
            rdpClient.AdvancedSettings2.ClearTextPassword = sd.Password;
            rdpClient.ColorDepth = sd.ColorDepth;
            rdpClient.DesktopWidth = sd.DesktopWidth;
            rdpClient.DesktopHeight = sd.DesktopHeight;
            rdpClient.FullScreen = sd.Fullscreen;

            // this fixes the rdp control locking issue
            // when lossing its focus
            //rdpClient.AdvancedSettings3.ContainerHandledFullScreen = -1;
            //rdpClient.AdvancedSettings3.DisplayConnectionBar = true;
            //rdpClient.FullScreen = true;
            //rdpClient.AdvancedSettings3.SmartSizing = true;
            //rdpClient.AdvancedSettings3.PerformanceFlags = 0x00000100;

            //rdpClient.AdvancedSettings2.allowBackgroundInput = -1;
            rdpClient.AdvancedSettings2.AcceleratorPassthrough = -1;
            rdpClient.AdvancedSettings2.Compress = -1;
            rdpClient.AdvancedSettings2.BitmapPersistence = -1;
            rdpClient.AdvancedSettings2.BitmapPeristence = -1;
            //rdpClient.AdvancedSettings2.BitmapCacheSize = 512;
            rdpClient.AdvancedSettings2.CachePersistenceActive = -1;

            // custom port
            if (sd.Port != 0)
            {
                rdpClient.AdvancedSettings2.RDPPort = sd.Port;
            }

            btnConnect.Enabled = false;

            panel1.Visible = false;
            tmrSC.Enabled = false;
        }

        public void InitializeControlEvents()
        {
            Shown += new EventHandler(RdpClientWindow_Shown);
            FormClosing += new FormClosingEventHandler(RdpClientWindow_FormClosing);

            btnDisconnect.Click += new EventHandler(ToolbarButtons_Click);
            btnConnect.Click += new EventHandler(ToolbarButtons_Click);
            btnReconnect.Click += new EventHandler(ToolbarButtons_Click);
            btnSettings.Click += new EventHandler(ToolbarButtons_Click);
            btnFullscreen.Click += new EventHandler(ToolbarButtons_Click);
            m_FTS_FitToScreen.Click += new EventHandler(ToolbarButtons_Click);
            m_FTS_Strech.Click += new EventHandler(ToolbarButtons_Click);
            btnPopout_in.Click += new EventHandler(BtnPopout_in_Click);

            rdpClient.OnConnecting += new EventHandler(RdpClient_OnConnecting);
            rdpClient.OnConnected += new EventHandler(RdpClient_OnConnected);
            rdpClient.OnLoginComplete += new EventHandler(RdpClient_OnLoginComplete);
            rdpClient.OnDisconnected += new AxMSTSCLib.IMsTscAxEvents_OnDisconnectedEventHandler(RdpClient_OnDisconnected);

            btnSndKey_TaskManager.Click += new EventHandler(SendKeys_Button_Click);

            tmrSC.Tick += new EventHandler(TmrSC_Tick);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x21)  // mouse click
            {
                rdpClient.Focus();
            }
            else if (m.Msg == WM_LEAVING_FULLSCREEN)
            {
            }

            base.WndProc(ref m);
        }

        private void BtnPopout_in_Click(object sender, EventArgs e)
        {
            // we just can't move our entire form
            // into different window because of the ActiveX error
            // crying out about the Windowless control.

            if (int.Parse(btnPopout_in.Tag.ToString()) == 0)
            {
                popupmdi = new PopupMDIContainer();
                popupmdi.Show();
                popupmdi.PopIn(ref rdpPanelBase, this, CurrentServer.ServerName);

                btnPopout_in.Image = Properties.Resources.pop_in_16;
                btnPopout_in.Tag = 1;
            }
            else if (int.Parse(btnPopout_in.Tag.ToString()) == 1)
            {
                popupmdi.PopOut(ref rdpPanelBase, this);

                btnPopout_in.Image = Properties.Resources.pop_out_16;
                btnPopout_in.Tag = 0;
            }
        }

        private void ChangeState(RdpState state, int? reason = null)
        {
            State = state;
            btnConnect.Enabled = state == RdpState.Disconnected;
            btnDisconnect.Enabled = state == RdpState.Connected;

            StateChange?.Invoke(this, new StateChangeEventArgs(CurrentServer, state, ListIndex, reason));
        }

        #region EVENT: Send Keys

        private void SendKeys_Button_Click(object sender, EventArgs e)
        {
            rdpClient.Focus();

            if (sender == btnSndKey_TaskManager)
            {
                //SendKeys.Send("(^%)");
                SendKeys.Send("(^%{END})");
            }

            //rdpClient.AdvancedSettings2.HotKeyCtrlAltDel;
        }

        #endregion

        #region EVENT: RDP Client

        private void RdpClient_OnConnected(object sender, EventArgs e)
        {
            Status("Connected to " + CurrentServer.Server);

            ChangeState(RdpState.Connected);

            System.Diagnostics.Debug.WriteLine("OnConnected " + rdpClient.Connected);
        }

        private void RdpClient_OnConnecting(object sender, EventArgs e)
        {
            Status("Connecting to " + CurrentServer.Server);

            ChangeState(RdpState.Connecting);

            System.Diagnostics.Debug.WriteLine("OnConnecting " + rdpClient.Connected);
        }

        private void RdpClient_OnDisconnected(object sender, IMsTscAxEvents_OnDisconnectedEvent e)
        {
            Status("Disconnected from " + CurrentServer.Server);

            ChangeState(RdpState.Disconnected, e.discReason);

            System.Diagnostics.Debug.WriteLine("OnDisconnected " + rdpClient.Connected);
        }

        private void RdpClient_OnLoginComplete(object sender, EventArgs e)
        {
            Status("Loged in using " + CurrentServer.Username + " user account");

            { // check connection status on output
                System.Diagnostics.Debug.WriteLine("OnLoginComplete " + rdpClient.Connected);
            }

            LoginComplete?.Invoke(this, e, ListIndex);
        }

        #endregion

        #region EVENT: server settings window

        private void Ssw_ApplySettings(object sender, ServerDetails sd)
        {
            CurrentServer = sd;

            MessageBox.Show("This will restart your connection", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

            Reconnect(true, false, false);
        }

        private Rectangle Ssw_GetClientWindowSize()
        {
            return rdpClient.RectangleToScreen(rdpClient.ClientRectangle);
        }

        #endregion

        #region EVENT: other form controls

        private void RdpClientWindow_Activated(object sender, EventArgs e)
        {
            rdpClient.Focus();

            OnFormActivated?.Invoke(this, e, ListIndex, Handle);
        }

        private void RdpClientWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (State == RdpState.Disconnected)
                return;

            DialogResult dr = MessageBox.Show("Are you sure you want to close this window?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (dr == DialogResult.Yes)
            {
                Disconnect();
                rdpClient.Dispose();

                OnRdpFormClosing?.Invoke(this, e, ListIndex, Handle);

                Dispose();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void RdpClientWindow_Shown(object sender, EventArgs e)
        {
            OnFormShown?.Invoke(this, e, ListIndex, Handle);

            // stretch RD view
            ToolbarButtons_Click(m_FTS_Strech, null);
        }

        private void TmrSC_Tick(object sender, EventArgs e)
        {
            pictureBox1.BackgroundImage = GetCurrentScreen();
        }

        private void ToolbarButtons_Click(object sender, EventArgs e)
        {
            if (sender == btnDisconnect)
            {
                Disconnect();
            }
            else if (sender == btnConnect)
            {
                Connect();
            }
            else if (sender == btnReconnect)
            {
                Reconnect(false, _isFitToWindow, false);
            }
            else if (sender == btnSettings)
            {
                ServerSettingsWindow ssw = new ServerSettingsWindow(CurrentServer);

                ssw.ApplySettings += new ApplySettings(Ssw_ApplySettings);
                ssw.GetClientWindowSize += new GetClientWindowSize(Ssw_GetClientWindowSize);
                ssw.ShowDialog();

                CurrentServer = ssw.CurrentServerSettings();

                ServerSettingsChanged?.Invoke(sender, CurrentServer, ListIndex);
            }
            else if (sender == btnFullscreen)
            {
                DialogResult dr = MessageBox.Show("You are about to enter in Fullscreen mode.\r\nBy default, the remote desktop resolution will be the same as what you see on the window.\r\n\r\nWould you like to resize it automatically based on your screen resolution though it will be permanent as soon as you leave in Fullscreen.\r\n\r\nNote: This will reconnect.", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dr == DialogResult.Yes)
                {
                    Reconnect(false, false, true);
                }
                else
                {
                    rdpClient.FullScreen = true;
                }
            }
            else if (sender == m_FTS_FitToScreen)
            {
                DialogResult dr = MessageBox.Show("This will resize the server resolution based on this current client window size, though it will not affect you current settings.\r\n\r\nDo you want to continue?", Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

                if (dr == DialogResult.OK)
                {
                    Reconnect(true, true, false);
                }
            }
            else if (sender == m_FTS_Strech)
            {
                if (int.Parse(m_FTS_Strech.Tag.ToString()) == 0)
                {
                    rdpClient.AdvancedSettings3.SmartSizing = true;
                    m_FTS_Strech.Text = "Don't Stretch";
                    m_FTS_Strech.Tag = 1;
                }
                else
                {
                    rdpClient.AdvancedSettings3.SmartSizing = false;
                    m_FTS_Strech.Text = "Stretch";
                    m_FTS_Strech.Tag = 0;
                }
            }
        }

        #endregion

        #region METHOD: s

        public void Connect()
        {
            Status("Starting ...");
            rdpClient.Connect();
        }

        public void Disconnect()
        {
            Status("Disconnecting ...");
            rdpClient.DisconnectedText = "Disconnected";

            if (rdpClient.Connected != 0)
            {
                rdpClient.Disconnect();
            }
        }

        public Image GetCurrentScreen()
        {
            return APIs.ControlToImage.GetControlScreenshot(panel2);
        }

        public void Reconnect(bool hasChanges, bool isFitToWindow, bool isFullscreen)
        {
            Disconnect();

            Status("Waiting for the server to properly disconnect ...");

            // wait for the server to properly disconnect
            while (rdpClient.Connected != 0)
            {
                System.Threading.Thread.Sleep(1000);
                Application.DoEvents();
            }

            Status("Reconnecting ...");

            if (hasChanges)
            {
                rdpClient.Server = CurrentServer.Server;
                rdpClient.UserName = CurrentServer.Username;
                rdpClient.AdvancedSettings2.ClearTextPassword = CurrentServer.Password;
                rdpClient.ColorDepth = CurrentServer.ColorDepth;

                _isFitToWindow = isFitToWindow;

                if (isFitToWindow)
                {
                    rdpClient.DesktopWidth = rdpClient.Width;
                    rdpClient.DesktopHeight = rdpClient.Height;
                }
                else
                {
                    rdpClient.DesktopWidth = CurrentServer.DesktopWidth;
                    rdpClient.DesktopHeight = CurrentServer.DesktopHeight;
                }

                rdpClient.FullScreen = CurrentServer.Fullscreen;
            }

            if (isFullscreen)
            {
                rdpClient.DesktopWidth = Screen.PrimaryScreen.Bounds.Width;
                rdpClient.DesktopHeight = Screen.PrimaryScreen.Bounds.Height;

                rdpClient.FullScreen = true;
            }

            Connect();
        }

        private void Status(string stat)
        {
            lblStatus.Text = stat;
        }

        #endregion
    }

    public class StateChangeEventArgs : EventArgs
    {
        public ServerDetails Server { get; }
        public RdpState State { get; }
        public int ListIndex { get; }
        public int? Reason { get; }

        public StateChangeEventArgs(ServerDetails server, RdpState rdpState, int index, int? reason = null)
        {
            Server = server;
            State = rdpState;
            ListIndex = index;
            Reason = reason;
        }
    }

    public delegate void LoginComplete(object sender, EventArgs e, int ListIndex);

    public delegate void OnFormActivated(object sender, EventArgs e, int ListIndex, IntPtr Handle);

    public delegate void OnFormClosing(object sender, FormClosingEventArgs e, int ListIndex, IntPtr Handle);

    public delegate void OnFormShown(object sender, EventArgs e, int ListIndex, IntPtr Handle);

    public delegate void ServerSettingsChanged(object sender, ServerDetails sd, int ListIndex);
}