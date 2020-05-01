using System;
using System.Windows.Forms;

namespace MultiRemoteDesktopClient
{
    public partial class ConfigurationWindow : Form
    {
        public ConfigurationWindow()
        {
            InitializeComponent();
            InitializeControls();
            InitializeControlEvents();
        }

        public void InitializeControls()
        {
            txPassword.Text = GlobalHelper.appSettings.Settings.Password;
            cbHideWhenMinimized.Checked = GlobalHelper.appSettings.Settings.HideWhenMinimized;
            cbNotificationWindow.Checked = GlobalHelper.appSettings.Settings.HideInformationPopupWindow;
        }

        public void InitializeControlEvents()
        {
            btnSave.Click += new EventHandler(DefaultButtons_Click);
            btnClose.Click += new EventHandler(DefaultButtons_Click);

            lblShowPass.Click += new EventHandler(lblShowPass_Click);
        }

        private void lblShowPass_Click(object sender, EventArgs e)
        {
            lblShowPass = (Label)sender;

            if (int.Parse(lblShowPass.Tag.ToString()) == 0)
            {
                lblShowPass.Text = "Hide";
                txPassword.UseSystemPasswordChar = false;
                lblShowPass.Tag = 1;
            }
            else
            {
                lblShowPass.Text = "Show";
                txPassword.UseSystemPasswordChar = true;
                lblShowPass.Tag = 0;
            }
        }

        private void DefaultButtons_Click(object sender, EventArgs e)
        {
            if (sender == btnSave)
            {
                GlobalHelper.appSettings.Settings.Password = txPassword.Text;
                GlobalHelper.appSettings.Settings.HideWhenMinimized = cbHideWhenMinimized.Checked;
                GlobalHelper.appSettings.Settings.HideInformationPopupWindow = cbNotificationWindow.Checked;

                if (GlobalHelper.appSettings.Save())
                {
                    Close();
                }
                else
                {
                    MessageBox.Show("Failed to make changes.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}