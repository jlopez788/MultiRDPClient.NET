/*
Author: Jayzon Ragasa | aka: Nullstring
Application Developer - Anomalist Designs LLC

 * --
 * TextboxRequiredWrapper 1.0
 * --

*/

using System;
using System.Windows.Forms;

namespace TextboxRequiredWrappers
{
    public class TextboxRequiredWrapper
    {
        private Control[] _assocCtl;
        private Control[] _textbox;
        private string reqFieldMessage = "This field is required";

        /// <summary>
        /// Add common controls for validation.
        /// </summary>
        /// <param name="textbox"></param>
        public void AddRange(Control[] textbox)
        {
            _textbox = textbox;
            foreach (Control ctrl in _textbox)
            {
                if (ctrl.Text == string.Empty)
                {
                    ctrl.Text = reqFieldMessage;
                    ctrl.Font = new System.Drawing.Font(ctrl.Font.FontFamily, ctrl.Font.Size, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                }

                ctrl.LostFocus += new EventHandler(Ctrl_LostFocus);
                ctrl.GotFocus += new EventHandler(Ctrl_GotFocus);
                ctrl.TextChanged += new EventHandler(Ctrl_TextChanged);
            }
        }

        public bool IsAllFieldSet()
        {
            bool ret = true;

            if (_textbox == null)
            {
                return true;
            }

            foreach (Control ctrl in _textbox)
            {
                if (ctrl.Text == reqFieldMessage)
                {
                    ret = false;
                    break;
                }
            }

            return ret;
        }

        private void Ctrl_GotFocus(object sender, EventArgs e)
        {
            Control ctrl = (Control)sender;

            if (ctrl.Text == reqFieldMessage)
            {
                ctrl.Text = string.Empty;
            }
        }

        private void Ctrl_LostFocus(object sender, EventArgs e)
        {
            Control ctrl = (Control)sender;

            if (ctrl.Text == string.Empty)
            {
                ctrl.Font = new System.Drawing.Font(ctrl.Font.FontFamily, ctrl.Font.Size, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                ctrl.Text = reqFieldMessage;
            }
        }

        private void Ctrl_TextChanged(object sender, EventArgs e)
        {
            Control ctrl = (Control)sender;

            System.Diagnostics.Debug.WriteLine(">'" + ctrl.Text + "'");

            if (ctrl.Text != reqFieldMessage)
            {
                ctrl.Font = new System.Drawing.Font(ctrl.Font.FontFamily, ctrl.Font.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

                if (_assocCtl != null)
                {
                    foreach (Control c in _assocCtl)
                    {
                        c.Enabled = true;
                    }
                }
            }
            else
            {
                if (_assocCtl != null)
                {
                    foreach (Control c in _assocCtl)
                    {
                        c.Enabled = false;
                    }
                }
            }
        }
    }
}