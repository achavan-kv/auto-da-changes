using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    //Generic DropDownItem<T1,T2> or KeyValuePair<TKey, TValue> or Tuple<T1, T2> could have been used if it's not webservices and 3.5+ 
    public class DropDownItem
    {
        public int ValueMember { get; set; }
        public string DisplayMember { get; set; }
    }
}
