using System;

namespace Blue.Cosacs.NonStocks.Export
{
    public class ProductAmendmentFile : FileValidation
    {
        // Warehouse number
        private short _warehouseNumber;
        public short WarehouseNumber
        {
            get { return _warehouseNumber; }
            set
            {
                ValidateInt16Value(value, "warehouse number", 999);
                _warehouseNumber = value;
            }
        }

        // Product Code
        private string _productCode;
        public string ProductCode
        {
            get { return _productCode; }
            set
            {
                ValidateStringLength(value, "product code", 8);
                _productCode = value;
            }
        }

        // Supplier Product Description
        private string _supplierProductDescription;
        public string SupplierProductDescription
        {
            get { return _supplierProductDescription; }
            set
            {
                ValidateStringLength(value, "supplier product description", 18);
                _supplierProductDescription = value;
            }
        }

        // Product Main Description
        private string _productMainDescription;
        public string ProductMainDescription
        {
            get { return _productMainDescription; }
            set
            {
                ValidateStringLength(value, "product main description", 25);
                _productMainDescription = value;
            }
        }

        // Product Extra Description
        private string _productExtraDescription;
        public string ProductExtraDescription
        {
            get { return _productExtraDescription; }
            set
            {
                ValidateStringLength(value, "product extra description", 40);
                _productExtraDescription = value;
            }
        }

        // HP Price (format S9(8)V99))
        private decimal _HPPrice;
        public decimal HPPrice
        {
            get { return _HPPrice; }
            set
            {
                ValidateDecimalPriceValue(value, "hp price");
                _HPPrice = value;
            }
        }

        // Cash Price (format S9(8)V99))
        private decimal _cashPrice;
        public decimal CashPrice
        {
            get { return _cashPrice; }
            set
            {
                ValidateDecimalPriceValue(value, "cash price");
                _cashPrice = value;
            }
        }

        // Product Category
        private string _productCategory;
        public string ProductCategory
        {
            get { return _productCategory; }
            set
            {
                ValidateStringLength(value, "product category", 5);
                _productCategory = value;
            }
        }

        // Supplier AC No
        private string _supplierACNo;
        public string SupplierACNo
        {
            get { return _supplierACNo; }
            set
            {
                ValidateStringLength(value, "supplier ac no", 10);
                _supplierACNo = value;
            }
        }

        // Product Status (e.g. ‘L’ = Lead Time, ‘D’ = Discontinued)
        private char _productStatus;
        public char ProductStatus
        {
            get { return _productStatus; }
            set
            {
                value = value.ToString().ToUpper()[0];
                if (value != ' ' && value != 'L' && value != 'D')
                {
                    throw new ArgumentException("Invalid product status, only 'L' and 'D' are valid values.");
                }

                _productStatus = value;
            }
        }

        // Warrantable Product (‘Y’ or ‘N’)
        public char _warrantableProduct;
        public char WarrantableProduct
        {
            get { return _warrantableProduct; }
            set
            {
                ValidateCharYesNo(value, "warrantable product");
                _warrantableProduct = value;
            }
        }

        // Product Type Type
        public string ProductTypeType
        {
            // ‘02’ = Product w/o stock, ‘03’ = Product with stock, ‘06’ = Non-stock item, ‘07’ = Kit product
            get
            {
                return "06"; // We only return non-stocks
            }
        }

        // Special Price (format S9(8)V99))
        private decimal _specialPrice;
        public decimal SpecialPrice
        {
            get { return _specialPrice; }
            set
            {
                ValidateDecimalPriceValue(value, "special price");
                _specialPrice = value;
            }
        }

        // Warranty Reference
        private string _warrantyReference;
        public string WarrantyReference
        {
            get { return _warrantyReference; }
            set
            {
                ValidateStringLength(value, "warranty reference", 2);
                _warrantyReference = value;
            }
        }

        // Product EAN Code
        private string _productEANCode;
        public string ProductEANCode
        {
            get { return _productEANCode; }
            set
            {
                ValidateStringLength(value, "product ean code", 13);
                _productEANCode = value;
            }
        }

        // Product Lead Time
        private string _productLeadTime;
        public string ProductLeadTime
        {
            get { return _productLeadTime; }
            set
            {
                ValidateStringLength(value, "product lead time", 3);
                _productLeadTime = value;
            }
        }

        // Warranty Renewal (‘Y’ or ‘N’)
        private char _warrantyRenewal;
        public char WarrantyRenewal
        {
            get { return _warrantyRenewal; }
            set
            {
                ValidateCharYesNo(value, "warranty renewal", true);
                _warrantyRenewal = value;
            }
        }

        // Ready to Assemble (‘Y’ or ‘N’)
        public char _readyToAssemble;
        public char ReadyToAssemble
        {
            get { return _readyToAssemble; }
            set
            {
                ValidateCharYesNo(value, "ready to assemble", true);
                _readyToAssemble = value;
            }
        }

        // Deletion indicator (‘Y’ or ‘N’)
        public char _deletionIndicator;
        public char DeletionIndicator
        {
            get { return _deletionIndicator; }
            set
            {
                ValidateCharYesNo(value, "");
                _deletionIndicator = value;
            }
        }

        // Cost Price
        private decimal _costPrice;
        public decimal CostPrice
        {
            get { return _costPrice; }
            set
            {
                ValidateDecimalPriceValue(value, "cost price");
                _costPrice = value;
            }
        }

        // Supplier Name
        private string _supplierName;
        public string SupplierName
        {
            get { return _supplierName; }
            set
            {
                ValidateStringLength(value, "supplier name", 25);
                _supplierName = value;
            }
        }

        //Tax Rate
        private decimal _taxRate;
        public decimal TaxRate
        {
            get { return _taxRate; }
            set
            {
                _taxRate = value;
            }
        }

    }
}

//   1 –   2  Warehouse number
//         3  Comma
//   4 -  11  Product Code
//        12  Comma
//  13 –  30  Supplier Product Description
//        31  Comma
//  32 -  56  Product Main Description
//        57  Comma
//  58 -  97  Product Extra Description
//        98  Comma
//  99 – 109  HP Price (format S9(8)V99))
//       110  Comma
// 111 – 121  Cash Price (format S9(8)V99))
//       122  Comma
// 123 - 124  Product Category
//       125  Comma
// 126 - 135  Supplier AC No
//       136  Comma
//       137  Product Status (e.g. ‘L’ = Lead Time, ‘D’ = Discontinued)
//       138  Comma
//       139  Warrantable Product (‘Y’ or ‘N’)
//       140  Comma
// 141 - 142  Product Type Type (‘02’ = Product w/o stock, ‘03’ = Product with stock, ‘06’ = Non-stock item, ‘07’ = Kit product)
//       143  Comma
// 144 – 154  Special Price (format S9(8)V99))
//       155  Comma
// 156 - 157  Warranty Reference
//       158  Comma
// 159 - 171  Product EAN Code
//       172  Comma
// 173 - 175  Product Lead Time
//       176  Comma
//       177  Warranty Renewal (‘Y’ or ‘N’)
//       178  Comma
//       179  Ready to Assemble (‘Y’ or ‘N’)
//       180  Comma
//       181  Deletion indicator (‘Y’ or ‘N’)
//       182  Comma
// 183 - 193  Cost Price
//       194  Comma
// 195 - 219  Supplier Name
