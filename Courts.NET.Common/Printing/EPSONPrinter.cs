using System;
using System.Collections.Generic;
using System.Text;
using com.epson.pos.driver;
using System.Threading;

namespace BBSL.Printing
{
    public class EPSONPrinter : IPrinter
    {
        public static readonly Drawer[] Drawers = { Drawer.EPS_BI_DRAWER_1, Drawer.EPS_BI_DRAWER_2 };
        public static readonly Pulse[] Pulses = { Pulse.EPS_BI_PULSE_100, Pulse.EPS_BI_PULSE_200, Pulse.EPS_BI_PULSE_300, Pulse.EPS_BI_PULSE_400, Pulse.EPS_BI_PULSE_500, Pulse.EPS_BI_PULSE_600, Pulse.EPS_BI_PULSE_700, Pulse.EPS_BI_PULSE_800 };

        public event Action<String> ErrorEvent;

        void  OnErrorEvent(String str)
        {
            if (ErrorEvent != null)
                ErrorEvent(str);
        }

        StatusAPI statusAPI;
        string printerName;
        public StatusAPI StatusAPI
        {
            get
            {
                if (statusAPI == null)
                    statusAPI = new StatusAPI(OpenType.TYPE_PRINTER, printerName);

                return statusAPI;
            }
        }

        public EPSONPrinter(string printerName)
        {
            this.printerName = printerName;
        }

        #region IPrinter Members

        public void OpenDrawer()
        {
			try
			{
                int i = 0;
                while (!OpenDrawer(StatusAPI))
                {
                    if (i == 10000)
                        break;
                    i++;
                }
			}
			catch(Exception ex)
			{
                OnErrorEvent("Failed to open drawer.\n\n" + ex.Message);
			}
        }

        bool OpenDrawer(StatusAPI statusAPI)
        {
            foreach(Drawer drawer in Drawers)
                if(statusAPI.OpenDrawer(drawer, Pulse.EPS_BI_PULSE_100) == ErrorCode.SUCCESS)
                    return true;

            return false;
        }

        #endregion
    }
}
