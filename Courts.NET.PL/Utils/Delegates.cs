using System;
using System.Collections.Generic;
using System.Text;

namespace STL.PL.Utils
{
    public delegate void Action<in T1, in T2>(T1 arg1, T2 arg2);
    public delegate void Action<in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3);
    public delegate void Action();
}
