using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Warranty.Repositories
{
    public class WarrantyPromotionValidationException : System.Exception
    {
        internal WarrantyPromotionValidationException(string errorMessage) : base(errorMessage)
        {

        }

        public override string Message
        {
            get
            {
                return base.Message;
            }
        }
    }
}
