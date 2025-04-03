using Unicomer.Cosacs.Model;
using System.Web;

namespace Unicomer.Cosacs.Business.Interfaces
{
    public interface ITransaction
    {
        JResponse GetUserAccounts(string CustId);
        JResponse CreateRFAccount(AnswerModel creditAnsModel);
        JResponse GetCreditAppQuestions(string CustId);
        JResponse GetContractPDF(GetContract objgetContarct);
        JResponse UploadContractDocuments(UploadDocument UploadContractDocument, string accountNumber, bool isThirdParty = false);
        JResponse GetCustomerCreditSummary(string CustId);
        JResponse GetUserTransactions(UserTransactionInputModel objUserTransactionInputModel);
        JResponse getDocumentStatus();

        JResponse TPCreateRFAccount(string custId, TPTransactionConfirm tPTransactionConfirm);
        JResponse UpdateTransaction(UpdateTransactionQueryString modelUpdateTransactionQueryString, UpdateTransactionBody modelUpdateTransactionBody);
        //JResponse UploadSignatureContractDocuments(UploadDocument UploadContractDocument, string accountNumber,string CustId); 
        JResponse getEmailContract();
        JResponse UpdateContractNotificationStatus(ContractNotificationStatus modelUpdateTransactionBody);
    }
}
