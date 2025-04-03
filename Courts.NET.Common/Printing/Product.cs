using System.Collections.Generic;
using System.Text;

namespace BBSL.Libraries.Printing
{
    public class Product
    {
        public string ProductCode
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public decimal TaxAmount
        {
            get;
            set;
        }

        public decimal UnitPrice
        {
            get;
            set;
        }

        public decimal TotalPrice
        {
            get;
            set;
        }

        public int Quantity
        {
            get;
            set;
        }

        public decimal AdditionalTaxAmount     //BCX - ISSUE : This is used for LUX tax for curacao 
        {
            get;
            set;
        }


        public decimal AdditionalTaxPercent     //BCX - ISSUE : This is used for LUX tax for curacao 
        {
            get;
            set;
        }
        public decimal TaxPercent    //BCX - ISSUE : This is used for LUX tax for curacao
        {
            get;
            set;
        }

        public bool IsEmpty
        {
            get
            {
                return Quantity == 0
                    && TotalPrice == 0
                    && UnitPrice == 0
                    && TaxAmount == 0
                    && string.IsNullOrEmpty(Description)
                    && string.IsNullOrEmpty(ProductCode);
            }
        }
    }
}
