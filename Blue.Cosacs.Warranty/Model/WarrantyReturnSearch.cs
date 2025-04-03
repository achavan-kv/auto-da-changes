
using Blue.Data;
namespace Blue.Cosacs.Warranty.Model
{
   public class WarrantyReturnSearch : PagedSearch
    {
        public int? Id { get; set; }
        public string SearchCriteria { get; set; }
    }
}
