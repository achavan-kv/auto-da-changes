using System;
using System.Collections.Generic;
using System.Xml.Serialization;

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

    [XmlIgnore]
    public List<SimpleStockCountProductViewModel> Products { get; set; }
}