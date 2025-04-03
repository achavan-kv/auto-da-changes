using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Threading;

namespace Blue.Cosacs.Shared.Collections
{
    public class ThreadedBindingList
    {
        public static ThreadedBindingList<T> Create<T>(IList<T> list)
        {
            return new ThreadedBindingList<T>(list);
        }
    }

    public class ThreadedBindingList<T> : BindingList<T>
    {
        public ThreadedBindingList() { }

        public ThreadedBindingList(IList<T> list) : base(list) { }

        private SynchronizationContext ctx = SynchronizationContext.Current;

        public ThreadedBindingList<T> Add(IEnumerable<T> items)
        {
            foreach (var item in items)
                Add(item);
            return this;
        }

        protected override void OnAddingNew(AddingNewEventArgs e)
        {

            if (ctx == null)
            {
                BaseAddingNew(e);
            }
            else
            {
                ctx.Send(delegate
                {
                    BaseAddingNew(e);
                }, null);
            }
        }
        void BaseAddingNew(AddingNewEventArgs e)
        {
            base.OnAddingNew(e);
        }
        protected override void OnListChanged(ListChangedEventArgs e)
        {
            // SynchronizationContext ctx = SynchronizationContext.Current;
            if (ctx == null)
            {
                BaseListChanged(e);
            }
            else
            {
                ctx.Send(delegate
                {
                    BaseListChanged(e);
                }, null);
            }
        }
        void BaseListChanged(ListChangedEventArgs e)
        {
            base.OnListChanged(e);
        }
    }
}
