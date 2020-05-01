using Database;
using DataProtection;
using RDPFileReader;
using System;
using System.IO;
using System.Windows.Forms;

namespace MultiRemoteDesktopClient
{
    public partial class ImportWindow : Form
    {
        private OpenFileDialog ofd = null;

        public ImportWindow()
        {
            InitializeComponent();
            InitializeControls();
            InitializeControlEvents();
        }

        public void InitializeControlEvents()
        {
            Shown += new EventHandler(ImportWindow_Shown);
            btnStart.Click += new EventHandler(BtnStart_Click);
            btnBrowse.Click += new EventHandler(BtnBrowse_Click);
        }

        public void InitializeControls()
        {
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            ofd = new OpenFileDialog();
            ofd.Filter = "RDP File|*.rdp";
            ofd.Multiselect = true;
            ofd.Title = "Import RDP File";
            ofd.ShowDialog();

            foreach (string thisFile in ofd.FileNames)
            {
                System.Diagnostics.Debug.WriteLine("reading " + thisFile);

                #region Read RDP File

                RDPFile rdpfile;
                {
                    try
                    {
                        rdpfile = new RDPFile();
                        rdpfile.Read(thisFile);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occured while reading '" + Path.GetFileName(thisFile) + "' and it will be skipped.\r\n\r\nError Message: " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        System.Diagnostics.Debug.WriteLine(ex.Message + "\r\n" + ex.StackTrace);

                        continue;
                    }
                }

                #endregion

                ServerDetails sd = new ServerDetails();
                //TODO: Find out what this group id 1 is
                //sd.GroupID = 1;
                sd.ServerName = Path.GetFileNameWithoutExtension(thisFile);
                sd.Server = rdpfile.FullAddress ?? sd.ServerName;
                sd.Username = rdpfile.Username;

                #region Try decrypting the password from RDP file

                try
                {
                    System.Diagnostics.Debug.WriteLine("reading password " + thisFile);

                    string password = rdpfile.Password;
                    sd.Password = Password.Empty;
                    if (!string.IsNullOrEmpty(password))
                    {
                        // based on http://www.remkoweijnen.nl/blog/2008/03/02/how-rdp-passwords-are-encrypted-2/
                        // he saids, MSTSC just add a ZERO number at the end of the hashed password.
                        // so let's just removed THAT!
                        password = password.Substring(0, password.Length - 1);
                        // and decrypt it!
                        password = DataProtectionForRDPWrapper.Decrypt(password);
                        sd.Password = new Password(password, false);
                    }

                    System.Diagnostics.Debug.WriteLine("reading password done");
                }
                catch (Exception Ex)
                {
                    sd.Password = Password.Empty;
                    if (Ex.Message == "Problem converting Hex to Bytes")
                    {
                        MessageBox.Show("This RDP File '" + Path.GetFileNameWithoutExtension(thisFile) + "' contains a secured password which is currently unsported by this application.\r\nThe importing can still continue but without the password.\r\nYou can edit the password later by selecting a server in 'All Listed Servers' and click 'Edit Settings' button on the toolbar", Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                    else if (Ex.Message.Contains("Exception decrypting"))
                    {
                        MessageBox.Show("Failed to decrypt the password from '" + Path.GetFileNameWithoutExtension(thisFile) + "'", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("An unknown error occured while decrypting the password from '" + Path.GetFileNameWithoutExtension(thisFile) + "'", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                #endregion

                sd.Description = "Imported from " + thisFile;
                sd.ColorDepth = (int)rdpfile.SessionBPP;
                sd.DesktopWidth = rdpfile.DesktopWidth;
                sd.DesktopHeight = rdpfile.DesktopHeight;
                sd.Fullscreen = false;

                ListViewItem thisItem = new ListViewItem(Path.GetFileNameWithoutExtension(thisFile));
                thisItem.SubItems.Add("OK");
                thisItem.SubItems.Add(thisFile);
                thisItem.Tag = sd;
                thisItem.ImageIndex = 0;

                lvRDPFiles.Items.Add(thisItem);
            }

            foreach (ColumnHeader ch in lvRDPFiles.Columns)
            {
                ch.Width = -1;
            }
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem thisItem in lvRDPFiles.Items)
            {
                thisItem.SubItems[1].Text = "Importing...";

                ServerDetails sd = (ServerDetails)thisItem.Tag;

                var saved = GlobalHelper.dbServers.Save(sd);
                thisItem.SubItems[1].Text = saved ? "Done!" : "Not saved";
            }

            foreach (ColumnHeader ch in lvRDPFiles.Columns)
            {
                ch.Width = -1;
            }
        }

        private void ImportWindow_Shown(object sender, EventArgs e)
        {
        }
    }
}