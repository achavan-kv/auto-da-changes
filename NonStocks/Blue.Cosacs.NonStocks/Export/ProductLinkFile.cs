namespace Blue.Cosacs.NonStocks.Export
{
    public class ProductLinkFile : FileValidation
    {
        // Product Group
        private string _productGroup;
        public string ProductGroup
        {
            get { return _productGroup; }
            set
            {
                ValidateStringLength(value, "product group", 5);
                _productGroup = value;
            }
        }

        // Category
        private string _category;
        public string Category
        {
            get { return _category; }
            set
            {
                ValidateStringLength(value, "category", 5);
                _category = value;
            }
        }

        // Class
        private string _class;
        public string Class
        {
            get { return _class; }
            set
            {
                ValidateStringLength(value, "class", 5);
                _class = value;
            }
        }

        // Sub Class
        private string _subClass;
        public string SubClass
        {
            get { return _subClass; }
            set
            {
                ValidateStringLength(value, "sub class", 5);
                _subClass = value;
            }
        }

        // Associated Item Id
        private string _associatedItemId;
        public string AssociatedItemId
        {
            get { return _associatedItemId; }
            set
            {
                ValidateStringLength(value, "associated item id", 18);
                _associatedItemId = value;
            }
        }
    }
}
