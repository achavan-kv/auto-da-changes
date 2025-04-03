using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Blue.Cosacs.Shared.Collections
{
     public static class SortableBindingListFactory
    {
        public static SortableBindingList<T> Create<T>(IList<T> items)
        {
            return new SortableBindingList<T>(items);
        }    
    }

    public class SortableBindingList<T> : BindingList<T>
    {
        public SortableBindingList() { }

        public SortableBindingList(IList<T> items) : base(new List<T>(items))
        { 
        }

        #region Sorting

        private bool _isSorted;

        protected override bool SupportsSortingCore
        {
            get { return true; }
        }

        private ListSortDirection _SortDirectionCore;

        protected override ListSortDirection SortDirectionCore
        {
            get { return _SortDirectionCore; }
        }

        private PropertyDescriptor _SortPropertyCore;
        protected override PropertyDescriptor SortPropertyCore
        {
            get { return _SortPropertyCore; }
        }

        protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
        {
            _SortPropertyCore = property;
            _SortDirectionCore = direction;

            // Get list to sort
            var items = this.Items as List<T>;

            // Apply and set the sort, if items to sort
            if (items != null)
            {
                var pc = new PropertyComparer<T>(property, direction);
                items.Sort(pc);
                _isSorted = true;
            }
            else
            {
                _isSorted = false;
            }

            // Let bound controls know they should refresh their views
            this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override bool IsSortedCore
        {
            get { return _isSorted; }
        }

        protected override void RemoveSortCore()
        {
            _isSorted = false;
            _SortDirectionCore = ListSortDirection.Ascending;
            _SortPropertyCore = null;
        }

        #endregion
    }
}
