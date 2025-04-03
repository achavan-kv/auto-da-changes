using System;
using System.Collections.Generic;
using System.Text;

namespace STL.PL
{
    public class AddressHistory
    {
        public List<Add> AddressList { get; set; }
        private List<Add> addresslist = new List<Add>();

        public void Add(string addtype, string accttype, DateTime datein)
        {
            var add = addresslist.Find(delegate(Add item) { return item.AccType == addtype && item.Addtype == accttype; });
            if (add != null)
                add.Datein = datein;
            else
                addresslist.Add(new Add { AccType = accttype.Trim().ToUpper(), Addtype = addtype.Trim().ToUpper(), Datein = datein });

        }

        public bool AddTest(string addtype, string accttype, DateTime datein)
        {
            //var add = addresslist.Find(delegate(Add item) { return item.AccType == addtype && item.Addtype == accttype; });
            var add = addresslist.Find(delegate(Add item) { return item.AccType == addtype && item.Addtype == accttype && item.Datein == datein;}); //#12141

            if (add == null)
            {
                addresslist.Add(new Add { AccType = accttype.Trim().ToUpper(), Addtype = addtype.Trim().ToUpper(), Datein = datein });
                return false;
            }
            else
            {
                return true;
            }
        }

        public void Clear()
        {
            addresslist.Clear();
        }

    }

    public class Add
    {
        public DateTime Datein { get; set; }
        public string Addtype { get; set; }
        public string AccType { get; set; }
    }
}
