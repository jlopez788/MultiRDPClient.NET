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
    public partial class SplashScreenWindow : Form
    {
        float _opacity = 0.10f;

        public SplashScreenWindow()
        {
            InitializeComponent();
            Opacity = 0.0f;
            Shown += new EventHandler(SplashScreenWindow_Shown);
        }

        void SplashScreenWindow_Shown(object sender, EventArgs e)
        {
            timer2.Interval = 28;
            timer2.Enabled = true;
            timer2.Tick += new EventHandler(timer2_Tick);
        }

        void timer2_Tick(object sender, EventArgs e)
        {
            if (Opacity >= 0 || Opacity <= 1)
            {
                Opacity += _opacity;
            }

            if(Opacity == 1)
            {
                timer1.Enabled = true;
                timer1.Tick += new EventHandler(timer1_Tick);
            }
        }

        void timer1_Tick(object sender, EventArgs e)
        {
            timer2.Enabled = false;
            Close();
        }
    }
}
