using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MultiRemoteDesktopClient
{
    public partial class PasswordWindow : Form
    {
        int incPassCount = 0;

        bool isCanceled = true;

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

        void PasswordWindow_Shown(object sender, EventArgs e)
        {
            // check our password
            try
            {
                string pword = GlobalHelper.appSettings.Settings.Password;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Your password has been tampered and it will cause the application to terminate and it will not run until you deleted the configuration file.\r\n\r\nDatabase is safe, please make a backup before deleting the configuration file.\r\n\r\nApplication will now terminate ...", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                isCanceled = true;
                Close();
            }
        }

        void PasswordWindow_FormClosing(object sender, FormClosingEventArgs e)
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

        void btnGo_Click(object sender, EventArgs e)
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

        void btnCancel_Click(object sender, EventArgs e)
        {
            isCanceled = true;
        }

        void btnRenewCAPTCHA_Click(object sender, EventArgs e)
        {
            captcha1.Renew();
        }

        void ResizeWindow(bool showCAPTCHA)
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
