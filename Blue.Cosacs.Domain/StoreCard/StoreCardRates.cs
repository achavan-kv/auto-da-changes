using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Blue.Cosacs.Shared
{
     // Adding Extra properties to generated class from Database Table AA... 
    public partial class  StoreCardRate : INotifyPropertyChanged //, INotifyPropertyChanged
    {
      //  public BindingList<StoreCardRateDetails> details = null;
        public StoreCardRate()
        {
            RateDetails = new List<StoreCardRateDetails>();
        }

        private decimal rate;
        public virtual decimal Rate
        {
            get { return rate; }
            set
            {
                rate = value;
                //NotifyPropertyChanged("Name");
            }
        }
        public virtual List<StoreCardRateDetails> RateDetails { get; set; }

        public virtual bool Modified { get; set; }

     
        //public void AddRateDetailstoRate(int index)
        //{
        //    details[index].ParentID = this.Id;
        //    List<StoreCardRateDetails> detailslist = new List<StoreCardRateDetails>(details);

        // //  this.RateDetails = detailslist.ToArray();


        //}
    }


}
//        private string name;
//        public virtual string Name
//        {
//            get { return name; }
//            set
//            {
//                name = value;
//                //NotifyPropertyChanged("Name");
//            }
//        }






//        private bool rateFixed;
//        public virtual bool RateFixed
//        {
//            get { return rateFixed; }
//            set
//            {
//                rateFixed = value;
//         //       NotifyPropertyChanged("RateFixed");
//            }
//        }
        
//        public virtual StoreCardRate Add(StoreCardRateDetails details)
//        {
//            RateDetails.Add(details);
//            return details.Rate = this;
//        }

//        //public virtual event PropertyChangedEventHandler PropertyChanged;

//        //private void NotifyPropertyChanged(string name)
//        //{
//        //    if (PropertyChanged != null)
//        //        PropertyChanged(this, new PropertyChangedEventArgs(name));
//        //}
        
//    }
//}
