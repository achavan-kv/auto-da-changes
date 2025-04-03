using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace STL.PL
{
    /// <summary>
    /// This is extended to make sure the scrollbars enabled whenever they are visible
    /// http://connect.microsoft.com/VisualStudio/feedback/details/113022/datagridview-scrollbar-sometimes-visible-but-not-enabled
    /// </summary>
    public class DataGridViewExtended : DataGridView
    {
        public DataGridViewExtended() : base(){ }

        //HACK to make sure scrollbar is enabled
        protected override void OnVisibleChanged(EventArgs e)
        {
            VerticalScrollBar.Enabled = VerticalScrollBar.Visible && this.Enabled;
            HorizontalScrollBar.Enabled = HorizontalScrollBar.Visible && this.Enabled;
            base.OnVisibleChanged(e);
        }
    }
}
