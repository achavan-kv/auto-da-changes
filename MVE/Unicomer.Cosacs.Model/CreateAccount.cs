using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicomer.Cosacs.Model
{
    public class CreateAccount
    {
        public string CustId { get; set; }
        public string AccountType { get; set; }
        public short BranchNo { get; set; }
    }
}
