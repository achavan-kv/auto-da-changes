using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Unicomer.Cosacs.Model
{
    public class DeleteScheduleRecord
    {
        public string ServiceCode { get; set; }
        public string Code { get; set; }
        public string IsInsertRecord { get; set; }
        public string IsEODRecords { get; set; }
        public string Message { get; set; }

    }
}