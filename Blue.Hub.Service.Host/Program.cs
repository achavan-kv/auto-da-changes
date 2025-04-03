using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Blue.Hub.Service.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            var proc = Process.Start("Blue.Hub.Service.exe");
            proc.WaitForExit();
        }
    }
}
