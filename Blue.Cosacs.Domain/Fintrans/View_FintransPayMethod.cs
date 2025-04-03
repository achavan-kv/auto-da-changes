using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public partial class View_FintransPayMethod
    {
        private decimal? returnValue = 0;
        public decimal? ReturnValue
        {
            get 
            {
                return returnValue.HasValue ? returnValue.Value : 0;
            }
            set
            {
                if (returnValue != value)
                {
                    returnValue = value;
                    NotifyPropertyChanged("ReturnValue");
                }
            }
        }
    }
}
