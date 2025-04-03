using System;
using System.Collections.Generic;
using System.Text;

namespace STL.PL
{
    // Used to determine whether Customer Search initiated from Sanction S1
    public static class Spouse
    {
        private static bool _visited = false;
        public static bool Visited
        {
            get { return _visited; }
            set { _visited = value; }
        }
    }
}
