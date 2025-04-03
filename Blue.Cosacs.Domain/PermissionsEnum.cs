using System;
using System.Collections.Generic;
using System.Text;

namespace Blue.Cosacs.Shared
{
    public enum CosacsPermissionEnum
    {
        AllowDutyFreeSales = 1001,
        UnpaidAcctsCurrentUsr = 210,
        UnpaidAcctsCurrentBranch = 211,
        UnpaidAcctsAllBranches = 212,
        CancelAccount = 114,
        CustDetailsViewBankDetails = 222,
        CustDetailsViewFinancialTab = 294,
        CustDetailsViewEmploymentDetails = 223,
        CustDetailsViewResidentialDetails = 296,
        CustDetailsPhotoPrevious = 295,
        NewSalesChangeBranch = 107,
        CustomerSearch = 9,
        PaymentsCustomerSearch = 345,
        CashLoan = 373,
        CashLoanDisbursement = 374,             //#14910
        ChangeOrderBeforeDA = 330,
        CodeMaintenance = 79,
        AuthoriseICViewProposal =362,
        CashierTotalsEmployeePrint = 101,
        CashierTotalsBranchPrint = 100,
        LegacyCashAndGo = 156,
        CashierBranchFloat = 170,
        SerivceFoodLoss = 365 ,
        ServicePerviousRepairTotal = 357,
        ServiceWriteOffCash = 366,
        ServiceReassignPermission = 363,
        ServiceAllocationTab = 287,
        ServiceViewAudit = 280,
        ServiceBatchPrint = 316,
        Bailiff = 381,
        InstantReplacementCashAndGo = 133,            //#17290
        TelephoneActionReviewButton = 343,
        TelephoneActionAllToAllAccounts = 342,
        BailiffReviewAccountSearch = 176,
        BailiffReviewAllocateSingleAccount = 332,
        BailiffReviewViewReferences = 344,
        FailedDeliveriesAllCSR = 395,               // #12230
        TelephoneCaller = 380,                                  //#11243
        CreateEditScoringRules = 57,              // #12093
        ImportScoringRules = 58,              // #12093
        ImportScorebandMatrix = 60,       // #12093
        EditScoringMatrix = 59,              // #12093
        StoreCardBatchPrint = 1204,          //#12385
        ServiceViewBERReplacements = 1100,           //#11989
        DuplicateCustomers = 1005,            //#19422 - CR17976
        SalesCommEnquiryCSR = 9001,
        SalesCommissionBranchEnquiry = 9003,
        CashLoanDisbursementRecordBankTransfer = 8002,
		EditAppSpendFactor = 1212,
		EditDepSpendFactor = 1214        
    }
}
