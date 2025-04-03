using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Blue.Cosacs.StockCountApp
{
    public static class ControlExtensions
    { 
        public static void UIThread(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.BeginInvoke(action);
                return;
            }
            action.Invoke();
        }
      
        public static void UIThreadInvoke(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(action);
                return;
            }
            action.Invoke();
        } 
    }
}
