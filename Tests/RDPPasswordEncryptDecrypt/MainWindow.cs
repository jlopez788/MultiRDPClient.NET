using DataProtection;
using System;
using System.Windows.Forms;

namespace RDPPasswordEncryptDecrypt
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnEncrypt_Click(object sender, EventArgs e)
        {
            txHash.Text = DataProtectionForRDPWrapper.Encrypt(txPassword.Text);
        }

        private void BtnDecrypt_Click(object sender, EventArgs e)
        {
            txPassword.Text = DataProtectionForRDPWrapper.Decrypt(txHash.Text);
        }
    }
}