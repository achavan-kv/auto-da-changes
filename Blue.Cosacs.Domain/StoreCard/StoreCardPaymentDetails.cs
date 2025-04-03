using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public partial class StorecardPaymentDetails : Entity
    {
        //public DateTime StatementDate 
        //{
        //    //get { return DatePaymentDue.Value.AddDays(-28); } //todo add the country parameter...
        //}

        public decimal CardLimit
        {
            get;
            set;
        }
        public decimal CardAvailable
        {
            get;
            set;
        }
        //Country[CountryParameterNames.IncInsinServAgrPrint]
    }
}
