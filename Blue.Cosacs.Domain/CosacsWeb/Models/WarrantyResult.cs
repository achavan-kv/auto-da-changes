using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using STL.Common.Constants.Tags;
using STL.Common.Constants.Elements;

namespace STL.Common.Services.Model
{
    // Maps to Json format. Yay.
    public class WarrantyResult
    {
        public List<Item> Items { get; set; }
        public WarrantySearchByProduct ProductSearch { get; set; }        
        

        public IEnumerable<WarrantyResultFlat> ToFlat()
        {
            var warranties = new List<WarrantyResultFlat>();
            foreach (var item in Items)
            {
                warranties.Add(new WarrantyResultFlat
                {
                    Code = item.warrantyLink.Number,
                    Description = item.warrantyLink.Description,
                    Length = item.warrantyLink.Length,
                    Location = ProductSearch.Location,
                    Product = ProductSearch.Product,
                    PromotionPrice = item.promotion == null ? (decimal?)null : item.promotion.Price,
                    RetailPrice = item.price == null ? (decimal?)null : item.price.RetailPrice,                 //#16019
                    CostPrice = item.price == null ? (decimal?)null : item.price.CostPrice,                       //#16019
                    TaxRate = item.warrantyLink.TaxRate,
                    WarrantyType = item.warrantyLink.TypeCode      //#17883             //#16019
                });
            }
            return warranties;
        }


        public class Item
        {
            public Price price { get; set; }
            public Promotion promotion { get; set; }
            public WarrantyLink warrantyLink { get; set; }
            public string Status { get; set; }

            public WarrantyItemXml ToItem()
            {
                return new WarrantyItemXml
                {
                    RetailPrice = this.price != null ? this.price.RetailPrice : 0,
                    CostPrice = this.price != null? this.price.CostPrice : 0,                   //#15167
                    Id = this.warrantyLink.Id,
                    WarrantyType = this.warrantyLink.TypeCode,                          //#17883
                    TaxRate = this.warrantyLink.TaxRate,
                    PromotionPrice = this.promotion != null ? this.promotion.Price : (decimal?)null,
                    Length = this.warrantyLink.Length,
                    Code = this.warrantyLink.Number,
                    Description = this.warrantyLink.Description,
                    WarrantyDepartment = this.warrantyLink.WarrantyTags[0].TagName
                };
            }

        }

        public class Price
        {
            public decimal RetailPrice { get; set; }
            public decimal CostPrice { get; set; }
            public decimal TaxInclusivePriceChange { get; set; }
        }

        public class Promotion
        {
            public decimal Price { get; set; }
        }

        public class WarrantyLink
        {
            public int Id { get; set; }
            public int Length { get; set; }
            public string Number { get; set; }
            public decimal TaxRate { get; set; }
            public string TypeCode { get; set; }          //#17883
            public string Description { get; set; }
            public List<WarrantyTags> WarrantyTags { get; set; }
        }

        public class WarrantyTags
        {
            public int TagId { get; set; }
            public int LevelId { get; set; }
            public string TagName { get; set; }

            public const int MaxLevel = 99;
        }
    }

    // Maps to crap xmlnode format. Boooo.
    // Don't change order as display order and other crap cosacs code will break.
    // Hopefully can be deleted soon.
    [Serializable]
    public class WarrantyItemXml
    {
        const string Type = "Warranty";
        const int WarrantyIdStart = 100000;
        private int _id;
        public int Id
        {
            get { return _id + WarrantyIdStart; }
            set { _id = value; }
        }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal CostPrice { get; set; }                      //#15167
        public decimal? PromotionPrice { get; set; }
        public int Length { get; set; }
        public decimal TaxRate { get; set; }
        public string WarrantyType { get; set; }                    //#17883
        public string ContractNumber { get; set; }
        public string BranchForDeliveryNote { get; set; }
        public string DeliveryTime { get; set; }
        public string DeliveryDate { get; set; }
        public int Quantity { get; set; }
        public decimal Value { get; set; }
        public int Location { get; set; }
        public string WarrantyDepartment { get; set; }

        public WarrantyItemXml Clone()
        {
            return new WarrantyItemXml
            {
                RetailPrice = this.RetailPrice,
                CostPrice = this.CostPrice,                     //#15167
                Id = this.Id,
                WarrantyType = this.WarrantyType,               //#17883
                TaxRate = this.TaxRate,
                PromotionPrice = this.PromotionPrice,           // #15166
                Length = this.Length,
                Code = this.Code,
                Description = this.Description,
                WarrantyDepartment = this.WarrantyDepartment
            };
        }



        //So shit, yet I don't care.
        public XmlNode ToXml()
        {
            XmlDocument doc = new XmlDocument();
            //   XmlUtilities xml = new XmlUtilities();
            XmlNode itemNode = this.CreateItemNode(doc);
            itemNode.Attributes[Tags.Quantity].Value = "0";
            itemNode.Attributes[Tags.Value].Value = "0";
            itemNode.Attributes[Tags.DeliveryDate].Value = "";
            itemNode.Attributes[Tags.DeliveryTime].Value = "";
            itemNode.Attributes[Tags.BranchForDeliveryNote].Value = "";
            itemNode.Attributes[Tags.ColourTrim].Value = "";
            itemNode.Attributes[Tags.DeliveredQuantity].Value = "0";
            itemNode.Attributes[Tags.PlannedDeliveryDate].Value = "";
            itemNode.Attributes[Tags.DeliveryAddress].Value = "";
            itemNode.Attributes[Tags.DeliveryArea].Value = "";
            itemNode.Attributes[Tags.DeliveryProcess].Value = "";
            itemNode.Attributes[Tags.QuantityDiff].Value = "Y";
            itemNode.Attributes[Tags.ScheduledQuantity].Value = "0";
            itemNode.Attributes[Tags.TaxAmount].Value = "0";
            itemNode.Attributes[Tags.ContractNumber].Value = "";
            itemNode.Attributes[Tags.ExpectedReturnDate].Value = "";
            itemNode.Attributes[Tags.PurchaseOrder].Value = Convert.ToBoolean(0).ToString();
            itemNode.Attributes[Tags.Assembly].Value = "";
            itemNode.Attributes[Tags.Damaged].Value = "";
            itemNode.Attributes[Tags.PurchaseOrderNumber].Value = "";
            itemNode.Attributes[Tags.ReplacementItem].Value = Convert.ToBoolean(0).ToString();
            itemNode.Attributes[Tags.SalesBrnNo].Value = "";             //IP - 23/05/11 - CR1212 - RI - #3651
            itemNode.Attributes[Tags.Express].Value = "";                              //IP - 07/06/12 - #10229 - Warehouse & Deliveries
            itemNode.Attributes[Tags.Key].Value = this.Id + "|" + this.Location.ToString();
            itemNode.Attributes[Tags.Type].Value = Type;
            itemNode.Attributes[Tags.ItemId].Value = this.Id.ToString();
            itemNode.Attributes[Tags.Code].Value = this.Code;
            itemNode.Attributes[Tags.Description1].Value = this.Description;
            itemNode.Attributes["Length"].Value = this.Length.ToString();
            itemNode.Attributes[Tags.TaxRate].Value = this.TaxRate.ToString();
            itemNode.Attributes["WarrantyType"].Value = this.WarrantyType.ToString();           //#17883
            itemNode.Attributes[Tags.ContractNumber].Value = this.ContractNumber;
            itemNode.Attributes[Tags.BranchForDeliveryNote].Value = this.BranchForDeliveryNote.ToString();
            itemNode.Attributes[Tags.DeliveryTime].Value = this.DeliveryTime;
            itemNode.Attributes[Tags.DeliveryDate].Value = this.DeliveryDate;
            itemNode.Attributes[Tags.Quantity].Value = this.Quantity.ToString();
            itemNode.Attributes[Tags.Value].Value = this.Value.ToString();
            itemNode.Attributes[Tags.UnitPrice].Value = this.Value.ToString();
            itemNode.Attributes[Tags.Location].Value = this.Location.ToString();
            itemNode.Attributes[Tags.DeliveredQuantity].Value = "0";
            itemNode.Attributes[Tags.SPIFFItem].Value = "false";
            itemNode.Attributes[Tags.RepoItem].Value = "false";
            itemNode.Attributes[Tags.ValueControlled].Value = "false";
            itemNode.Attributes[Tags.AvailableStock].Value = "0";                       //#15064
            itemNode.Attributes[Tags.CostPrice].Value = Convert.ToString(this.CostPrice);                 //#15070
            itemNode.Attributes[Tags.HPPrice].Value = Convert.ToString(this.RetailPrice);                 //#16338
            itemNode.Attributes[Tags.CashPrice].Value = Convert.ToString(this.RetailPrice);               //#16338
            itemNode.Attributes[Tags.ProductCategory].Value = this.WarrantyDepartment == "Electrical" ? "PCE" : "PCF";
            itemNode.AppendChild(doc.CreateElement(Elements.RelatedItem));
            return itemNode;
        }

        private XmlNode CreateItemNode(XmlDocument doc)
        {
            XmlNode node = doc.CreateElement(Elements.Item);
            node.Attributes.Append(doc.CreateAttribute(Tags.Key));
            node.Attributes.Append(doc.CreateAttribute(Tags.Type));
            node.Attributes.Append(doc.CreateAttribute(Tags.Code));
            node.Attributes.Append(doc.CreateAttribute(Tags.Location));
            node.Attributes.Append(doc.CreateAttribute(Tags.AvailableStock));
            node.Attributes.Append(doc.CreateAttribute(Tags.DamagedStock));
            node.Attributes.Append(doc.CreateAttribute(Tags.Description1));
            node.Attributes.Append(doc.CreateAttribute(Tags.Description2));
            node.Attributes.Append(doc.CreateAttribute(Tags.SupplierCode));
            node.Attributes.Append(doc.CreateAttribute(Tags.UnitPrice));
            node.Attributes.Append(doc.CreateAttribute(Tags.CashPrice));
            node.Attributes.Append(doc.CreateAttribute(Tags.CostPrice));
            node.Attributes.Append(doc.CreateAttribute(Tags.HPPrice));
            node.Attributes.Append(doc.CreateAttribute(Tags.DutyFreePrice));
            node.Attributes.Append(doc.CreateAttribute(Tags.ValueControlled));
            node.Attributes.Append(doc.CreateAttribute(Tags.Quantity));
            node.Attributes.Append(doc.CreateAttribute(Tags.Value));
            node.Attributes.Append(doc.CreateAttribute(Tags.DeliveryDate));
            node.Attributes.Append(doc.CreateAttribute(Tags.DeliveryTime));
            node.Attributes.Append(doc.CreateAttribute(Tags.BranchForDeliveryNote));
            node.Attributes.Append(doc.CreateAttribute(Tags.ColourTrim));
            node.Attributes.Append(doc.CreateAttribute(Tags.TaxRate));
            node.Attributes.Append(doc.CreateAttribute(Tags.DeliveredQuantity));
            node.Attributes.Append(doc.CreateAttribute(Tags.PlannedDeliveryDate));
            node.Attributes.Append(doc.CreateAttribute(Tags.CanAddWarranty));
            node.Attributes.Append(doc.CreateAttribute(Tags.DeliveryAddress));
            node.Attributes.Append(doc.CreateAttribute(Tags.DeliveryArea));
            node.Attributes.Append(doc.CreateAttribute(Tags.DeliveryProcess));
            node.Attributes.Append(doc.CreateAttribute(Tags.DateDelivered));
            node.Attributes.Append(doc.CreateAttribute(Tags.QuantityDiff));
            node.Attributes.Append(doc.CreateAttribute(Tags.ScheduledQuantity));
            node.Attributes.Append(doc.CreateAttribute(Tags.TaxAmount));
            node.Attributes.Append(doc.CreateAttribute(Tags.ContractNumber));
            node.Attributes.Append(doc.CreateAttribute(Tags.ReturnItemNo));
            node.Attributes.Append(doc.CreateAttribute(Tags.ReturnLocation));
            node.Attributes.Append(doc.CreateAttribute(Tags.FreeGift));
            node.Attributes.Append(doc.CreateAttribute(Tags.ExpectedReturnDate));
            node.Attributes.Append(doc.CreateAttribute(Tags.QtyOnOrder));
            node.Attributes.Append(doc.CreateAttribute(Tags.PurchaseOrder));
            node.Attributes.Append(doc.CreateAttribute(Tags.LeadTime));
            node.Attributes.Append(doc.CreateAttribute(Tags.Assembly));
            node.Attributes.Append(doc.CreateAttribute(Tags.Damaged));
            node.Attributes.Append(doc.CreateAttribute(Tags.ProductCategory));
            node.Attributes.Append(doc.CreateAttribute(Tags.SparePartsCategory)); // Required for selecting a spare part for a Service Request JH 08/11/2007
            node.Attributes.Append(doc.CreateAttribute(Tags.Deleted));
            node.Attributes.Append(doc.CreateAttribute(Tags.PurchaseOrderNumber));
            node.Attributes.Append(doc.CreateAttribute(Tags.ReplacementItem));
            node.Attributes.Append(doc.CreateAttribute(Tags.SPIFFItem));
            node.Attributes.Append(doc.CreateAttribute(Tags.RefCode)); //IP - 28/01/10 - LW 72136
            node.Attributes.Append(doc.CreateAttribute(Tags.ItemRejected)); //IP - 04/06/10 - UAT(262) UAT5.2.1.0 Log
            node.Attributes.Append(doc.CreateAttribute(Tags.Category));
            node.Attributes.Append(doc.CreateAttribute(Tags.ItemId));         //CR1212
            node.Attributes.Append(doc.CreateAttribute(Tags.ColourName));     //CR1212 jec 21/04/11
            node.Attributes.Append(doc.CreateAttribute(Tags.Style));          //CR1212 jec 21/04/11    
            node.Attributes.Append(doc.CreateAttribute(Tags.SalesBrnNo));   //IP - 23/05/11 - CR1212 - RI - #3651
            node.Attributes.Append(doc.CreateAttribute(Tags.RepoItem));     // RI
            node.Attributes.Append(doc.CreateAttribute(Tags.Class));        // 27/07/11 - RI - #4415
            node.Attributes.Append(doc.CreateAttribute(Tags.SubClass));     // 27/07/11 - RI - #4415
            node.Attributes.Append(doc.CreateAttribute(Tags.Brand));        //IP - 19/09/11 - RI - #8218 - CR8201
            node.Attributes.Append(doc.CreateAttribute(Tags.Express));      //IP - 07/06/12 - #10229 - Warehouse & Deliveries
            node.Attributes.Append(doc.CreateAttribute("Length"));
            node.Attributes.Append(doc.CreateAttribute("WarrantyType"));    //#17883

            return node;
        }
    }

    public class WarrantySearchByProduct
    {
        public string Product { get; set; }
        public decimal? PriceVATEx { get; set; }
        public short Location { get; set; }
        public DateTime? Date { get; set; }
    }

    public class WarrantyResultFlat
    {
        public string Product { get; set; }
        public short Location { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? CostPrice { get; set; }             //#15070
        public int Id { get; set; }
        public decimal TaxRate { get; set; }
        public decimal? PromotionPrice { get; set; }
        public int Length { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string WarrantyType { get; set; }            //#17883 // #16019
    }
}
