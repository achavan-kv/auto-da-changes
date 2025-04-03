using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Warehouse.Utils
{
    public class FasciaHelper
    {
        public string Name { get; set; }
        public string Code { get; set; }

        private FasciaHelper(string name, string code)
        {
            this.Name = name;
            this.Code = code;
        }

        public static readonly FasciaHelper CourtsStore = new FasciaHelper("Courts Store", "C");
        public static readonly FasciaHelper NonCourtsStore = new FasciaHelper("Non Courts Store", "N");

        public override int GetHashCode()
        {
            return string.Format("Fascia {0}-{1}", this.Code, this.Name).GetHashCode();
        }

        public static FasciaHelper FromString(string code)
        {
            return AsEnumerable().FirstOrDefault(p => string.Compare(p.Code, code, true) == 0);
        }

        public static IEnumerable<FasciaHelper> AsEnumerable()
        {
            yield return CourtsStore;
            yield return NonCourtsStore;
        }
    }
}
