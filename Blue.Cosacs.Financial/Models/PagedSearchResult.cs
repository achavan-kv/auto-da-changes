namespace Blue.Cosacs.Financial.Models
{
    using System.Collections.Generic;

    public class PagedSearchResult<T>
    {
        public int Count { get; set; }
        public List<T> Page { get; set; }
    }
}
