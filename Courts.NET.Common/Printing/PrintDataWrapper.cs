using System;
using System.Collections.Generic;
using System.Text;

namespace BBSL.Libraries.Printing
{
    public class PrintDataWrapper<T>
    {
        public PrintDataWrapper()
        {
        }

        public PrintDataWrapper(T value, bool shouldBePrinted)
        {
            this.Value = value;
            this.ShouldBePrinted = shouldBePrinted;
        }

        public bool ShouldBePrinted
        { get; set; }

        public T Value { get; set; }
    }
}
