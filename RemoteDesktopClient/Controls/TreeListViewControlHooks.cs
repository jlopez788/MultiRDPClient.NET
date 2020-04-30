using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using CommonTools;

namespace MultiRemoteDesktopClient.Controls
{
    // since the CommonTools.TreeListView are unstable
    // we just have to reference the control instead of extending something to it.
    public class TreeListViewControlHooks
    {
        CommonTools.TreeListView _treelistview;
        Object[] _objCtrl_EmptyListItem;
        Object[] _objCtrl_ItemSelection;

        public TreeListViewControlHooks(ref TreeListView tlv)
        {
            _treelistview = tlv;

            tlv.MouseClick +=new MouseEventHandler(tlv_MouseClick);
        }

        public void AddControlForEmptyListItem(Object[] ctrl)
        {
            _objCtrl_EmptyListItem = ctrl;
            EnableControls(false, _objCtrl_EmptyListItem);
        }

        public void AddControlForItemSelection(Object[] ctrl)
        {
            _objCtrl_ItemSelection = ctrl;
        }

        private void EnableControls(bool enable, Object[] ctrl)
        {
            if (ctrl != null)
            {
                foreach (Object o in ctrl)
                {
                    Type t = o.GetType();
                    System.Reflection.PropertyInfo pi = t.GetProperty("Enabled");

                    if (pi.CanWrite)
                    {
                        pi.SetValue(o, enable, null);
                    }
                }
            }
        }

        public void EnableControls(bool enable)
        {
            EnableControls(enable, _objCtrl_ItemSelection);
        }

        void tlv_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Left)
            {
                Node n = _treelistview.CalcHitNode(new System.Drawing.Point(e.X, e.Y));

                if (n == null)
                {
                    EnableControls(false, _objCtrl_ItemSelection);
                }
                else
                {
                    // comment this line
                    // since we're controling the selection from the outside
                    //EnableControls(true, this._objCtrl_ItemSelection);
                }
            }
        }

        //protected override void OnMouseClick(MouseEventArgs e)
        //{
        //    //base.OnMouseClick(e);
        //}

        //protected override void OnMouseDown(MouseEventArgs e)
        //{
        //    //if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Left)
        //    //{
        //    //    ListViewHitTestInfo lvhi = HitTest(e.X, e.Y);

        //    //    if (lvhi.Item == null)
        //    //    {
        //    //        EnableControls(false, this._objCtrl_ItemSelection);
        //    //    }
        //    //    else
        //    //    {
        //    //        EnableControls(true, this._objCtrl_ItemSelection);
        //    //    }
        //    //}

        //    base.OnMouseDown(e);
        //}
    }
}
