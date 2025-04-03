using System;

namespace Blue.Cosacs.NonStocks.Export
{
    public class PromotionalPriceFile : FileValidation
    {
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

        // HP Price 1 (format 9(8) V99)
        private decimal _HPPrice1;
        public decimal HPPrice1
        {
            get { return _HPPrice1; }
            set
            {
                ValidateDecimalPriceValue(value, "hp price 1");
                _HPPrice1 = value;
            }
        }

        // HP Date ‘From’ 1 (format ddmmyy)
        private DateTime? _HPDateFrom1;
        public DateTime? HPDateFrom1
        {
            get { return _HPDateFrom1; }
            set
            {
                ValidateDate(value, "hp date ‘from’ 1");
                _HPDateFrom1 = value;
            }
        }

        // HP Date ‘To’ 1 (format ddmmyy)
        private DateTime? _HPDateTo1;
        public DateTime? HPDateTo1
        {
            get { return _HPDateTo1; }
            set
            {
                ValidateDate(value, "hp date ‘to’ 1");
                _HPDateTo1 = value;
            }
        }


        // HP Price 2 (format 9(8) V99)
        private decimal _HPPrice2;
        public decimal HPPrice2
        {
            get { return _HPPrice2; }
            set
            {
                ValidateDecimalPriceValue(value, "hp price 2");
                _HPPrice2 = value;
            }
        }

        // HP Date ‘From’ 2 (format ddmmyy)
        private DateTime? _HPDateFrom2;
        public DateTime? HPDateFrom2
        {
            get { return _HPDateFrom2; }
            set
            {
                ValidateDate(value, "hp date ‘from’ 2");
                _HPDateFrom2 = value;
            }
        }

        // HP Date ‘To’ 2 (format ddmmyy)
        private DateTime? _HPDateTo2;
        public DateTime? HPDateTo2
        {
            get { return _HPDateTo2; }
            set
            {
                ValidateDate(value, "hp date ‘to’ 2");
                _HPDateTo2 = value;
            }
        }


        // HP Price 3 (format 9(8) V99)
        private decimal _HPPrice3;
        public decimal HPPrice3
        {
            get { return _HPPrice3; }
            set
            {
                ValidateDecimalPriceValue(value, "hp price 3");
                _HPPrice3 = value;
            }
        }

        // HP Date ‘From’ 3 (format ddmmyy)
        private DateTime? _HPDateFrom3;
        public DateTime? HPDateFrom3
        {
            get { return _HPDateFrom3; }
            set
            {
                ValidateDate(value, "hp date ‘from’ 3");
                _HPDateFrom3 = value;
            }
        }

        // HP Date ‘To’ 3 (format ddmmyy)
        private DateTime? _HPDateTo3;
        public DateTime? HPDateTo3
        {
            get { return _HPDateTo3; }
            set
            {
                ValidateDate(value, "hp date ‘to’ 3");
                _HPDateTo3 = value;
            }
        }


        // Cash Price 1 (format 9(8) V99)
        private decimal _cashPrice1;
        public decimal CashPrice1
        {
            get { return _cashPrice1; }
            set
            {
                ValidateDecimalPriceValue(value, "cash price 1");
                _cashPrice1 = value;
            }
        }

        // Cash Date ‘From’ 1 (format ddmmyy)
        private DateTime? _cashDateFrom1;
        public DateTime? CashDateFrom1
        {
            get { return _cashDateFrom1; }
            set
            {
                ValidateDate(value, "cash date ‘from’ 1");
                _cashDateFrom1 = value;
            }
        }

        // Cash Date ‘To’ 1 (format ddmmyy)
        private DateTime? _cashDateTo1;
        public DateTime? CashDateTo1
        {
            get { return _cashDateTo1; }
            set
            {
                ValidateDate(value, "cash date ‘to’ 1");
                _cashDateTo1 = value;
            }
        }


        // Cash Price 2 (format 9(8) V99)
        private decimal _cashPrice2;
        public decimal CashPrice2
        {
            get { return _cashPrice2; }
            set
            {
                ValidateDecimalPriceValue(value, "cash price 2");
                _cashPrice2 = value;
            }
        }

        // Cash Date ‘From’ 2 (format ddmmyy)
        private DateTime? _cashDateFrom2;
        public DateTime? CashDateFrom2
        {
            get { return _cashDateFrom2; }
            set
            {
                ValidateDate(value, "cash date ‘from’ 2");
                _cashDateFrom2 = value;
            }
        }

        // Cash Date ‘To’ 2 (format ddmmyy)
        private DateTime? _cashDateTo2;
        public DateTime? CashDateTo2
        {
            get { return _cashDateTo2; }
            set
            {
                ValidateDate(value, "cash date ‘to’ 2");
                _cashDateTo2 = value;
            }
        }


        // Cash Price 3 (format 9(8) V99)
        private decimal _cashPrice3;
        public decimal CashPrice3
        {
            get { return _cashPrice3; }
            set
            {
                ValidateDecimalPriceValue(value, "cash price 3");
                _cashPrice3 = value;
            }
        }

        // Cash Date ‘From’ 3 (format ddmmyy)
        private DateTime? _cashDateFrom3;
        public DateTime? CashDateFrom3
        {
            get { return _cashDateFrom3; }
            set
            {
                ValidateDate(value, "cash date ‘from’ 3");
                _cashDateFrom3 = value;
            }
        }

        // Cash Date ‘To’ 3 (format ddmmyy)
        private DateTime? _cashDateTo3;
        public DateTime? CashDateTo3
        {
            get { return _cashDateTo3; }
            set
            {
                ValidateDate(value, "cash date ‘to’ 3");
                _cashDateTo3 = value;
            }
        }

    }
}

//   1 -   8  Product Code
//         9  Comma
//  10 -  11  Warehouse Number
//        12  Comma
//  13 -  22  HP Price 1 (format 9(8) V99)
//        23  Comma
//  24 -  29  HP Date ‘From’ 1(format ddmmyy)
//        30  Comma
//  31 -  36  HP Date ‘To’ 1(format ddmmyy)
//        37  Comma
//  38 -  47  HP Price 2 (format 9(8) V99)
//        48  Comma
//  49 -  54  HP Date ‘From’ 2(format ddmmyy)
//        55  Comma
//  56 -  61  HP Date ‘To’ 2(format ddmmyy)
//        62  Comma
//  63 -  72  HP Price 3 (format 9(8) V99)
//        73  Comma
//  74 -  79  HP Date ‘From’ 3 (format ddmmyy)
//        80  Comma
//  81 -  86  HP Date ‘To’ 3 (format ddmmyy)
//        87  Comma
//  88 -  97  Cash Price 1 (format 9(8) V99)
//        98  Comma
//  99 - 104  Cash Date ‘From’ 1 (format ddmmyy)
//       105  Comma
// 106 - 111  Cash Date ‘To’ 1 (format ddmmyy)
//       112  Comma
// 113 - 122  Cash Price 2 (format 9(8) V99)
//       123  Comma
// 124 – 129  Cash Date ‘From’ 2 (format ddmmyy)
//       130  Comma
// 131 – 136  Cash Date ‘To’ 2 (format ddmmyy)
//       137  Comma
// 138 – 147  Cash Price 3 (format 9(8) V99)
//       148  Comma
// 149 – 154  Cash Date ‘From’ 3 (format ddmmyy)
//       155  Comma
// 156 - 161  Cash Date ‘To’ 3 (format ddmmyy)
