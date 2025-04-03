using System;
using System.Xml;
using Blue.Cosacs.Service.Solr;

namespace Blue.Cosacs.Service.Subscribers
{
    public class RequestSubmit : Hub.Client.Subscriber
    {
        public override void Sink(int id, XmlReader message)
        {

            var b = Deserialize<Cosacs.Messages.Service.RequestSubmit>(message);

            var saleInfo = new Warranty.Repositories.WarrantySaleRepository().GetLineItemIdentifier(b.Account, b.ItemId);

            var merchandisingRepo = new Stock.Repositories.ProductRepository();
            var item = merchandisingRepo.GetProductRelationsByItemNumber(b.ItemNumber);

            using (var scope = Context.Write())
            {
                scope.Context.Request.Add(new Request
                {
                    Id = b.Id,
                    Account = b.Account,
                    Branch = b.Branch,
                    Type = "II",
                    State = "New", //To be updated. 
                    CreatedOn = Convert.ToDateTime(DateTime.Now.ToShortTimeString()),
                    CreatedBy = b.CreatedBy,
                    CreatedById = b.CreatedById,      // CreatedById shouldn't be a string!!!
                    CustomerId = b.CustomerId,
                    CustomerTitle = b.Title,
                    CustomerFirstName = b.FirstName,
                    CustomerLastName = b.LastName,
                    CustomerAddressLine1 = b.AddressLine1,
                    CustomerAddressLine2 = b.AddressLine2,
                    CustomerAddressLine3 = b.AddressLine3,
                    CustomerNotes = b.Notes,
                    CustomerPostcode = b.PostCode,
                    ItemId = Convert.ToString(b.ItemId),
                    ItemNumber = b.ItemNumber,
                    ItemAmount = b.ItemValue,
                    ItemSoldBy = b.ItemSoldBy,
                    Item = b.Product,
                    ItemSupplier=b.Supplier,
                    LastUpdatedUser = b.CreatedById,
                    LastUpdatedUserName = b.CreatedBy,
                    LastUpdatedOn = Convert.ToDateTime(DateTime.Now.ToLongTimeString()),
                    ItemStockLocation = saleInfo.Count > 0 ? saleInfo[0].StockLocation : b.StockLocation, 
                    ItemDeliveredOn = saleInfo.Count > 0 ? saleInfo[0].ItemDeliveredOn : null,
                    ProductLevel_1 = item.Department,
                    ProductLevel_2 = item.Category,
                    ProductLevel_3 = item.Class,
                });

                if (b.HomePhone != "")
                {
                    scope.Context.RequestContact.Add(new RequestContact
                    {
                        RequestId = b.Id,
                        Value = b.HomePhone,
                        Type = "HomePhone"
                    });
                }

                if (b.WorkPhone != "")
                {
                    scope.Context.RequestContact.Add(new RequestContact
                    {
                        RequestId = b.Id,
                        Value = b.WorkPhone,
                        Type = "WorkPhone"
                    });
                }

                if (b.MobilePhone != "")
                {
                    scope.Context.RequestContact.Add(new RequestContact
                    {
                        RequestId = b.Id,
                        Value = b.MobilePhone,
                        Type = "MobilePhone"
                    });
                }

                if (b.Email != "")
                {
                    scope.Context.RequestContact.Add(new RequestContact
                    {
                        RequestId = b.Id,
                        Value = b.Email,
                        Type = "Email"
                    });
                }

                scope.Context.SaveChanges();
                scope.Complete();
            }

            SolrIndex.IndexRequest(new[] { b.Id });
        }
    }
}
