using Unicomer.Cosacs.Model;
using System.Web;

namespace Unicomer.Cosacs.Business.Interfaces
{
    public interface ITransaction
    {
        dynamic BillGeneration(BillGenerationHeader BGH, bool isUpdate);
        dynamic CreatePO(PurchaseOrderModel PO, bool isUpdate);
        dynamic CreateAccount(CreateAccount objJSON);
        dynamic GetCreditAvailability(string CustId);
        dynamic GetGRN(string GRNNo);
        dynamic DeliveryAuthorization(string AcctNo, string DocType);
        dynamic CreateVendorReturn(VendorReturnModel VR);
        dynamic CreateCommissions(Commissions objJSON, bool isUpdate);
        dynamic SyncDataUpdate(string ServiceCode, string Code, bool IsInsertRecord, bool IsEODRecords, string Message, string orderid, string ID);
        dynamic GetPaymentsOrderList(string AcctNo, string ID);
        dynamic GetCancelPaymentsOrderList(string AcctNo);
        dynamic CreateDeliveryConfirmation(string AcctNo, string Id);
        dynamic CreateCustomerReturn(CustomerReturnModel CR);
        dynamic GetVendorReturn(string VendorReturnID);
    }
}
