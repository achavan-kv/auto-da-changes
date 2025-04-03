//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.ComponentModel;

//namespace Blue.Cosacs.Shared
//{
//    [Serializable]
//    public partial class StoreCardRateDetails : INotifyPropertyChanged
//    {

//    }
//}

//        public virtual int Id { get; set; }
//        public virtual int ParentId { get; set; }

//        public virtual StoreCardRate Rate { get; set; }

//        private int appscorefrom;
//        public virtual int AppScoreFrom
//        {
//            get { return appscorefrom; }
//            set
//            {
//                appscorefrom = value;
//                NotifyPropertyChanged("AppScoreFrom");
//            }
//        }


//        private int appscoreTo;
//        public virtual int AppScoreTo
//        {
//            get { return appscoreTo; }
//            set
//            {
//                appscoreTo = value;
//                NotifyPropertyChanged("ScoreTo");
//            }
//        }




//        private int behavescorefrom;
//        public virtual int BehaveScoreFrom
//        {
//            get { return behavescorefrom; }
//            set
//            {
//                behavescorefrom = value;
//                NotifyPropertyChanged("BehaveScoreFrom");
//            }
//        }


//        private int behavescoreTo;
//        public virtual int BehaveScoreTo
//        {
//            get { return behavescoreTo; }
//            set
//            {
//                behavescoreTo = value;
//                NotifyPropertyChanged("BehaveScoreTo");
//            }
//        }




//        private decimal purchaseInterestRate;
//        public virtual decimal PurchaseInterestRate
//        {
//            get { return purchaseInterestRate; }
//            set
//            {
//                purchaseInterestRate = value;
//                NotifyPropertyChanged("PurchaseInterestRate");
//            }
//        }


//        //private decimal retailRateVariable;
//        //public virtual decimal RetailRateVariable
//        //{
//        //    get { return retailRateVariable; }
//        //    set
//        //    {
//        //        retailRateVariable = value;
//        //        NotifyPropertyChanged("RetailRateVariable");
//        //    }
//        //}

//        public virtual event PropertyChangedEventHandler PropertyChanged;

//        private void NotifyPropertyChanged(string name)
//        {
//            if (PropertyChanged != null)
//                PropertyChanged(this, new PropertyChangedEventArgs(name));
//        }
//    }
//}
