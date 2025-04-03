using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Blue.Cosacs.StockCountApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>        
        static void Main()
        {
            Application.Run(new StockCountListForm());
        }
    }
}
