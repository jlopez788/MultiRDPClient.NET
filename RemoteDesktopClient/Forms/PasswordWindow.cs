using Database;
using System;
using System.Windows.Forms;

namespace MultiRemoteDesktopClient
{
    public partial class PasswordWindow : Form
    {
        private int incPassCount = 0;

        private bool isCanceled = true;

        public PasswordWindow()
        {
            InitializeComponent();

            ResizeWindow(false);

            FormClosing += new FormClosingEventHandler(PasswordWindow_FormClosing);
            Shown += new EventHandler(PasswordWindow_Shown);

            btnGo.Click += new EventHandler(btnGo_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
            btnRenewCAPTCHA.Click += new EventHandler(btnRenewCAPTCHA_Click);
        }

        private void PasswordWindow_Shown(object sender, EventArgs e)
        {
            Utility.Try(() => _ = GlobalHelper.appSettings.Settings.Password, ex => {
                isCanceled = true;
                MessageBox.Show("Your password has been tampered and it will cause the application to terminate and it will not run until you deleted the configuration file.\r\n\r\nDatabase is safe, please make a backup before deleting the configuration file.\r\n\r\nApplication will now terminate ...", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            });
        }

        private void PasswordWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            // set the default DialogResult value to OK
            // we have to set this because this form's DialogResult
            // is set to Cancel
            DialogResult = DialogResult.OK;

            if (isCanceled)
            {
                DialogResult = DialogResult.Cancel;
            }
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            if (txPassword.Text == GlobalHelper.appSettings.Settings.Password)
            {
                bool ok = true;

                if (groupboxCAPTCHA.Visible)
                {
                    if (txCaptcha.Text != captcha1.RandomText)
                    {
                        ok = false;

                        MessageBox.Show("CAPTCHA Verification.\r\n\r\nDidn't match.", "CAPTCHA Verification", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
                    }
                }

                if (ok)
                {
                    isCanceled = false;
                    Close();
                }
            }
            else
            {
                DialogResult dr = MessageBox.Show("Incorrect password.\r\n\r\nPlease try again or press Cancel button to terminate this application", Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);

                if (dr == DialogResult.Cancel)
                {
                    isCanceled = true;
                    Close();
                }

                incPassCount++;

                if (incPassCount >= 3)
                {
                    ResizeWindow(true);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            isCanceled = true;
        }

        private void btnRenewCAPTCHA_Click(object sender, EventArgs e)
        {
            captcha1.Renew();
        }

        private void ResizeWindow(bool showCAPTCHA)
        {
            if (!showCAPTCHA)
            {
                Height = 190;
            }
            else
            {
                Height = 352;

                // center form to parent
                Top = ((Owner.Height - Height) / 2) + Owner.Top;
            }

            groupboxCAPTCHA.Visible = showCAPTCHA;
        }
    }
}