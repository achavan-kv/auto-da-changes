using System;

namespace STL.PL

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