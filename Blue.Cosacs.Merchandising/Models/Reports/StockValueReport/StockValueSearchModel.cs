namespace Blue.Cosacs.Merchandising.Models
{
    using System;
    using System.Collections.Generic;

    public class StockValueSearchModel
    {
        private string location;
        private string fascia;
        private string divisionName;
        private string departmentName;
        private string className;

        public DateTime PeriodEndDate { get; set; }
        public bool IsGrouped { get; set; }
        public List<int> ColIndices { get; set; }
        public string Location
        {
            get
            {
                return location;
            }
            set
            {
                location = value.Trim();
            }
        }
        public string Fascia
        {
            get
            {
                return fascia;
            }
            set
            {
                fascia = value.Trim();
            }
        }
        public string DivisionName
        {
            get
            {
                return divisionName;
            }
            set
            {
                divisionName = string.IsNullOrEmpty(value) ? string.Empty : value.Trim();
            }
        }
        public string DepartmentName
        {
            get
            {
                return departmentName;
            }
            set
            {
                departmentName = string.IsNullOrEmpty(value) ? string.Empty : value.Trim();
            }
        }
        public string ClassName
        {
            get
            {
                return className;
            }
            set
            {
                className = string.IsNullOrEmpty(value) ? string.Empty : value.Trim();
            }
        }
    }
}