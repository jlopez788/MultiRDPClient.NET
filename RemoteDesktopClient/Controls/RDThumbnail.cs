using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MultiRemoteDesktopClient.Controls
{
    public partial class RDThumbnail : UserControl
    {
        string _title = string.Empty;
        IntPtr _mdichild_parentHandle = IntPtr.Zero;
        Image _tnImage = null;

        public RDThumbnail()
        {
            InitializeComponent();
            panelDrawing.Dock = DockStyle.Fill;
            panelDrawing.BringToFront();
        }

        public string Title
        {
            set
            {
                _title = value;
                lblTitle.Text = _title;
            }
            get { return _title; }
        }

        public Image RDImage
        {
            set
            {
                _tnImage = value;
                panelDrawing.BackgroundImage = _tnImage;
            }
            get { return _tnImage; }
        }

        public IntPtr MDIChild_Handle
        {
            set
            {
                _mdichild_parentHandle = value;
            }
            get { return _mdichild_parentHandle; }
        }
    }
}
