using System;
using System.Linq;
using Blue.Cosacs.Sales.Common;

namespace Blue.Cosacs.Sales.Models
{
    [Serializable]
    public class OrderExtendedDto : OrderDto
    {
        public OrderExtendedDto()
        {
            PrintCopy = "ORIGINAL";
        }
        public string BranchName { get; set; }
        public string BranchAddress1 { get; set; }
        public string BranchAddress2 { get; set; }
        public string BranchAddress3 { get; set; }

        public string CurrentUser { get; set; }
        public string CreatedByName { get; set; }
        public string CurrentDateTime { get; set; }
        public string ReceiptType { get; set; }
        public string LoggedInBranchNo { get; set; }
        public bool ChangeGiven { get; set; }
        public decimal PositiveAmountSum { get; set; }
        public decimal NegativeAmountSum { get; set; }

        public string TaxName { get; set; }
        public string CountryName { get; set; }
        public string CompanyTaxNumber { get; set; }
        public decimal TaxRate { get; set; }

        public string SalesPerson { get; set; }
        public string PrintCopy { get; set; }

        public bool HasWarranties
        {
            get
            {
                var ret = Items.Where(p => p.ParentId == Id && p.ItemTypeId == (int)ItemTypeEnum.Warranty)
                            .ToList().Any();

                return ret;
            }
        }
    }
}
