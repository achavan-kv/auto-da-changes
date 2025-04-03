using System;
using System.Collections.Generic;

public class SimpleStockCountViewModel
{
    public SimpleStockCountViewModel()
    {
        Products = new List<SimpleStockCountProductViewModel>();
    }

    public int Id { get; set; }

    public int LocationId { get; set; }

    public string Location { get; set; }

    public string Type { get; set; }

    public DateTime CountDate { get; set; }

    public int? StartedById { get; set; }

    public string StartedBy { get; set; }

    public DateTime? StartedDate { get; set; }

    public List<SimpleStockCountProductViewModel> Products { get; set; }
}