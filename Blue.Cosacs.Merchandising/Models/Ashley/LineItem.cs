using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blue.Cosacs.Merchandising.Models.Ashley
{
    public class LineItem
    {
        public short OrigBr { get; set; }
        public string AccountNumber { get; set; }        
        public int AgreementNumber { get; set; }      
        public int BuffNo { get; set; }
        public string ItemNumber { get; set; }
        public int ItemID { get; set; }        
        public string ItemSuppText { get; set; }        
        public double Quantity { get; set; }
        public decimal Value { get; set; }       
        public double DeliveredQuantity { get; set; }
        public double ScheduledQuantity { get; set; }
        public short StockLocation { get; set; }
        public decimal Price { get; set; }        
        public decimal OrderValue { get; set; }
        public DateTime DateRequiredDelivery { get; set; }
        public string TimeRequiredDelivery { get; set; }
        public DateTime DatePlannedDelivery { get; set; }
        public short DeliveryNoteBranch { get; set; }
        public string QuantityDiff { get; set; }
        public string ItemType { get; set; }
        public short HasString { get; set; }
        public string Notes { get; set; }
        public double TaxAmount { get; set; }
        public string ParentItemNumber { get; set; }      
        public int ParentItemID { get; set; }
        public bool RepoItem { get; set; }
        public short ParentStockLocation { get; set; }
        public short IsKit { get; set; }        
        //public DataTable Codes { get; set; }
        public DateTime DateOfLastDelivery { get; set; }       
        public string DeliveryAddress { get; set; }        
        public string DeliveryArea { get; set; }
        public string DeliveryProcess { get; set; }
        //public DataTable ItemDetails { get; set; }        
        public decimal realDiscount { get; set; }        
        public string ContractNo { get; set; } 
        public DateTime ExpectedReturnDate { get; set; }        
        public string ReturnItemNumber { get; set; }
        public int ReturnItemId { get; set; }
        public short ReturnStockLocn { get; set; }
        public string AuditSource { get; set; }
        public string Damaged { get; set; }        
        public string Assembly { get; set; }        
        public bool SPIFFItem { get; set; }        
        public bool IsComponent { get; set; }        
        public string VanNo { get; set; }
        public DateTime DhlInterfaceDate { get; set; }
        public DateTime DhlPickingDate { get; set; }
        public string DhlDNNo { get; set; }        
        public double Taxrate { get; set; }        
        public string ShipQty { get; set; } 
        public bool ItemRejected { get; set; }
       public short? SalesBrnNo { get; set; }
       public string Express { get; set; }
        public int LineItemId { get; set; }        
        public bool ReadyAssist { get; set; }
       public int InvoiceVersion { get; set; }
       
    }
}
/*  Table Fields in LineItem 
 * origbr,
acctno,
agrmtno,
itemno,
itemsupptext,
quantity,
delqty,	
stocklocn,
price,
ordval,
datereqdel,
timereqdel,
dateplandel,
delnotebranch,
qtydiff,
itemtype,
notes,
taxamt,
isKit,
deliveryaddress,
parentitemno,
parentlocation,
contractno,
expectedreturndate,
deliveryprocess,
deliveryarea,
DeliveryPrinted,
assemblyrequired,
damaged,
OrderNo,
Orderlineno,
PrintOrder,
taxrate,
ItemID,
ParentItemID,
SalesBrnNo,
ID,
Express,
WarrantyGroupId
 */
