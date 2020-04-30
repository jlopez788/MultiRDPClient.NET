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

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            txHash.Text = DataProtectionForRDPWrapper.Encrypt(txPassword.Text);
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            txPassword.Text = DataProtectionForRDPWrapper.Decrypt(txHash.Text);
        }
    }
}