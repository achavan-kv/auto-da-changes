using System;
using System.Collections.Generic;
using System.Text;
namespace STL.PL.StoreCard

    //Transporting random classes via events.
{
    public class GenericEventHandler<T> : EventArgs
    {
        public GenericEventHandler(T results)
        {
            this.Results = results;
        }

        public T Results
        {
            get;
            set;
        }
    }
}
