using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Blue.Cosacs.Shared;

namespace Blue.Cosacs
{
    public class SCAgreement
    {
        public List<View_StoreCardWithPayments> PaymentDetails { get; set; }
        public Customer Customer { get; set; }
        public CustAddress CustAddress {get; set;}
        public List<CustTel> CustTel { get; set; }
        public view_StoreCardGetProposal Proposal { get; set; }
        public List<Code> Code { get; set; }
        public DateTime DateActive { get; set; } 
        public List<CountryMaintenance> CountryMaintenance { get; set; }
        public View_employment employment { get; set; }


        public string DecimalPlace
        {
            get { return CountryMaintenance.Find(c => c.CodeName == "decimalplaces").Value; }
        }

        public string CountryCode 
        {
            get { return CountryMaintenance.Find(c => c.CodeName == "countrycode").Value; }
        }

        public string CountryName
        {
            get { return CountryMaintenance.Find(c => c.CodeName == "countryname").Value; }
        }
          public double? InterestRate
        {
            get { return PaymentDetails[0].InterestRate; }
        }

   

        public int NoCards 
        {
            get { return PaymentDetails.Count; }
        }

        private string acountStatus;
        public string AccountStatus 
        {
            get { return StoreCardAccountStatus_Lookup.FromString(acountStatus).Description; }
            set { acountStatus = value ; } 
        }

        public string MartialStatus
        {
            get
            {
                var desciption = Code.Find(c => c.code == Customer.maritalstat && c.category == "MS1");
                return desciption == null ? String.Empty : desciption.codedescript;
            }
        }

        public string ResStatus
        {
            get
            {
                var desciption = Code.Find(c => c.code == CustAddress.resstatus && c.category == "RS1");
                return desciption == null ? String.Empty : desciption.codedescript;
            }
        }

        public string OccupationDesc
        {
            get
            {
                var desciption = Code.Find(c => c.code == employment.worktype && c.category == "WT1");
                return desciption == null ? String.Empty : desciption.codedescript;
            }
        }

        public CustTel HomeTel
        {
            get {return CustTel.Find( c => c.tellocn.Trim() == "H") ;}
        }
        public CustTel WorkTel
        {
            get { return CustTel.Find(c => c.tellocn.Trim() == "W"); }
        }
        public CustTel Mobile
        {
            get { return CustTel.Find(c => c.tellocn.Trim() == "M"); }
        }
       
    }
}
