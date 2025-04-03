using System;
using System.Collections.Generic;
using System.Text;

namespace BBSL.Libraries.Printing
{
    public class ServiceItem
    {
        private string _serialno = "";
        public string serialno
        {
            get { return _serialno; }
            set { _serialno = value; }
        }

        private string _modelno = "";
        public string modelno
        {
            get { return _modelno; }
            set { _modelno = value; }
        }
    }
}

