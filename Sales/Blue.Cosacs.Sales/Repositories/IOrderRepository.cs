using Blue.Cosacs.Sales.Models;
using System.Collections.Generic;

namespace Blue.Cosacs.Sales.Repositories
{
    public interface IOrderRepository
    {
        OrderDto Get(string id);
        OrderSaveReturn Save(OrderDto salesOrder, int userId);
        ReceiptReprintResponse SearchOrdersForRePrint(int branchNo, System.DateTime dateFrom, System.DateTime dateTo, string invoiceNoMin, string invoiceNoMax, int start, int rows);
        OrderExtendedDto GetOrderForRePrint(string invoiceNo, string currentUser, string receiptType);
        IList<OrderExtendedDto> GetOrdersForRePrintAll(int branchNo, System.DateTime dateFrom, System.DateTime dateTo, int invoiceNoMin, int invoiceNoMax, string currentUser);
        void IndexNewCustomer(string firstName, string lastName, int userId);
    }
}