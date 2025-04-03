using System.Collections.Generic;

// TODO: Move this to Glaucous ( See 'JSendResult' in 'Merchandising' project)
namespace Blue.Cosacs.Sales.Models
{
    public class JSendResult<T>
    {
        public string Status { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}
