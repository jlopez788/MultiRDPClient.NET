using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CommonTools
{
	public class FloatComboBox : NameObjectComboBox<float>
	{
	}
	public class NameObjectComboBox<T> : ComboBox
    {
        NameObjectCollection<T> m_items = new NameObjectCollection<T>();
		public new NameObjectCollection<T> Items
		{
			get { return m_items; }
			set
			{
				m_items = value;
				DataSource = m_items;
			}
		}
		public new NameObject<T> SelectedItem
		{
			get { return base.SelectedItem as NameObject<T>; }
			set { base.SelectedItem = value; } 
		}
		public NameObjectComboBox()
		{
			DisplayMember = "Name";
			ValueMember = "Object";
		}
		protected override void OnLeave(EventArgs e)
		{
			if (DataBindings.Count > 0)
				DataBindings[0].WriteValue();
			base.OnLeave(e);
		}
	}
}
