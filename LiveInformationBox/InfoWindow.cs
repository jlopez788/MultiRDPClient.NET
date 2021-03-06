﻿/*
 * LiveInformationBox 2.0
 * by Jayson Ragasa aka Nullstring
 * Baguio City, Philippines
 * -
 * 
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using Win32APIs;

namespace LiveInformationBox
{
    public partial class InfoWindow : Form
    {
        private const int CS_DROPSHADOW = 0x00020000;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int WS_EX_TOPMOST = 0x00000008; 

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams p = base.CreateParams;
                p.ClassStyle |= CS_DROPSHADOW;
                p.ExStyle = (WS_EX_NOACTIVATE | WS_EX_TOPMOST);
                //p.Param = IntPtr.Zero;
                return p;
            }
        }

        public enum WindowPositions
        {
            TOP_LEFT = 0,
            TOP_RIGHT = 1,
            BOTTOM_LEFT = 2,
            BOTTOM_RIGHT = 3
        };

        const int WinMargin_X = 24;
        const int WinMargin_Y = 24;

        private string _xmlInfoFile = string.Empty;
        private WindowPositions _winpos = WindowPositions.BOTTOM_RIGHT;

        public InfoWindow(string XMLInfoFile, WindowPositions winpos)
        {
            if (File.Exists(XMLInfoFile))
            {
                _xmlInfoFile = XMLInfoFile;
                _winpos = winpos;

                InitializeComponent();
                
                FormClosing += new FormClosingEventHandler(InfoWindow_FormClosing);
                Paint += new PaintEventHandler(InfoWindow_Paint);
                Resize += new EventHandler(InfoWindow_Resize);

                tmr.Interval = 5000;
                tmr.Tick += new EventHandler(tmr_Tick);

                btnClose.Click += new EventHandler(btnClose_Click);
            }
        }

        #region control extra properties

        public bool EnableInformationWindow { set; get; } = false;

        #endregion

        #region local events

        void tmr_Tick(object sender, EventArgs e)
        {
            HideWindow();
        }

        void InfoWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            HideWindow();
        }

        void InfoWindow_Paint(object sender, PaintEventArgs e)
        {
            Brush brGradient = new System.Drawing.Drawing2D.LinearGradientBrush(ClientRectangle, Color.White, Color.FromArgb(228, 228,240), 90, false);
            e.Graphics.FillRectangle(brGradient, ClientRectangle);

            ControlPaint.DrawBorder(e.Graphics, ClientRectangle, Color.FromKnownColor(KnownColor.ActiveBorder), ButtonBorderStyle.Solid);

            lblInfo.BackColor = Color.Transparent;
        }

        void InfoWindow_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }

        void btnClose_Click(object sender, EventArgs e)
        {
            HideWindow();
        }

        #endregion

        #region host control events

        private void c_MouseLeave(object sender, EventArgs e)
        {
            //this.Hide();
        }

        private void c_MouseHover(object sender, EventArgs e)
        {
            Type t = sender.GetType();
            PropertyInfo piName = t.GetProperty("Name");

            if (piName != null)
            {
                string controlName = piName.GetValue(sender, null).ToString();

                XMLInformationReader xmlinforeader = new XMLInformationReader(_xmlInfoFile);
                InformationDetails infoDet = xmlinforeader.Read(controlName);

                if (infoDet != null)
                {
                    lblTitle.Text = infoDet.Title;
                    lblShortDescription.Text = infoDet.ShortDescription;
                    lblInfo.Text = infoDet.Information;

                    ShowWindow();
                }
            }
        }

        #endregion

        #region methods

        public void AddControl(object[] ctr)
        {
            //BindingFlags myBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            foreach (object o in ctr)
            {
                Type t = o.GetType();

                EventInfo eiMouseHover = t.GetEvent("MouseHover");
                if (eiMouseHover != null)
                {
                    eiMouseHover.AddEventHandler(o, new EventHandler(c_MouseHover));
                }

                EventInfo eiMouseLeave = t.GetEvent("MouseLeave");
                if (eiMouseLeave != null)
                {
                    eiMouseLeave.AddEventHandler(o, new EventHandler(c_MouseLeave));
                }
            }
        }

        void HideWindow()
        {
            Hide();
            tmr.Enabled = false;
        }

        void ShowWindow()
        {
            if (!EnableInformationWindow)
            {
                return;
            }

            tmr.Enabled = true;

            #region resize window
            {
                int margin_x = 10;
                int margin_y = 10;

                lblInfo.Location = new Point(margin_x, panel1.Height + margin_y);

                Size s = new Size();
                s.Height = lblInfo.Top + lblInfo.Height + margin_y;

                if (lblInfo.Left + lblInfo.Width >= Width)
                {
                    s.Width = (margin_x + lblInfo.Width) + margin_x;
                }
                else
                {
                    s.Width = ClientSize.Width;
                }

                ClientSize = s;
                
                //this.Show();
                APIs.ShowInactiveTopmost(Handle);
            }
            #endregion

            #region window postition
            {
                switch (_winpos)
                {
                    case WindowPositions.TOP_LEFT:
                        Location = new Point(
                            WinMargin_X,
                            WinMargin_Y
                        );

                        break;
                    case WindowPositions.TOP_RIGHT:
                        Location = new Point(
                            (Screen.PrimaryScreen.WorkingArea.Width - Width) - WinMargin_X,
                            WinMargin_Y
                        );

                        break;
                    case WindowPositions.BOTTOM_LEFT:
                        Location = new Point(
                            WinMargin_X,
                            (Screen.PrimaryScreen.WorkingArea.Height - Height) - WinMargin_Y
                        );

                        break;
                    case WindowPositions.BOTTOM_RIGHT:
                        Location = new Point(
                            (Screen.PrimaryScreen.WorkingArea.Width - Width) - WinMargin_X,
                            (Screen.PrimaryScreen.WorkingArea.Height - Height) - WinMargin_Y
                        );

                        break;
                }
            }
            #endregion
        }

        #endregion
    }
}