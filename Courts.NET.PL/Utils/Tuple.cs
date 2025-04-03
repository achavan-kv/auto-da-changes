using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace STL.PL.Utils
{
    ///These Tuple<T1,T2, T3, ..> are available in 4.0 as System.Tuple 
    ///They are are immutable and implement the interfaces like IComparable, ITuple, etc
    ///So DotNet's implementation should instead be used if available.
    public class Tuple<T1>
    {
        [DebuggerStepThrough]
        public Tuple(T1 item1)
        {
            Item1 = item1;
        }

        public T1 Item1 { get; set; }
    }

    public class Tuple<T1, T2> : Tuple<T1>
    {
        [DebuggerStepThrough]
        public Tuple(T1 item1, T2 item2)
            : base(item1)
        {
            Item2 = item2;
        }

        public T2 Item2 { get; set; }
    }

    public class Tuple<T1, T2, T3> : Tuple<T1, T2>
    {
        [DebuggerStepThrough]
        public Tuple(T1 item1, T2 item2, T3 item3)
            : base(item1, item2)
        {
            Item3 = item3;
        }

        public T3 Item3 { get; set; }
    }

    public class Tuple<T1, T2, T3, T4> : Tuple<T1, T2, T3>
    {
        [DebuggerStepThrough]
        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4)
            : base(item1, item2, item3)
        {
            Item4 = item4;
        }

        public T4 Item4 { get; set; }
    }

    public static class Tuple
    {
        [DebuggerStepThrough]
        public static Tuple<T1> Create<T1>(T1 t1)
        {
            return new Tuple<T1>(t1);
        }

        [DebuggerStepThrough]
        public static Tuple<T1, T2> Create<T1, T2>(T1 t1, T2 t2)
        {
            return new Tuple<T1, T2>(t1, t2);
        }

        [DebuggerStepThrough]
        public static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
        {
            return new Tuple<T1, T2, T3>(t1, t2, t3);
        }

        [DebuggerStepThrough]
        public static Tuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            return new Tuple<T1, T2, T3, T4>(t1, t2, t3, t4);
        }
    }
}
