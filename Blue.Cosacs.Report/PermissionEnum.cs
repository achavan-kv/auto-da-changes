
namespace Blue.Cosacs.Report
{
    public enum ReportPermissionEnum
    {
        /*warranties*/
        WarrantySales = 2002,
        WarrantiesDueRenewal = 2003,
        WarrantyReturns = 2004,
        SecondEffortSolicitationCandidates = 2005,
        HitRate = 2006,
        WarrantyClaims = 2007,
        ProductsWithoutWarranties = 2008,

        /*services*/
        WeeklySummary = 2001,
        ServiceRqResolution = 2010,
        TechnicianCancellations = 2011,
        CustomerFeedbackHappy = 2012,
        SpareParts = 2013,
        InstallationHitRate = 2014,
        ServiceRequestFinancial = 2015,
        ServiceFailures = 2016,
        ResolutionTimesProductCategory = 2017,
        ReplacementData = 2018,
        ServiceMonthlyClaims = 2019,
        OutstandingSRsProdCat = 2020,
        ServiceIncomeAnalysis = 2021,
        ServiceClaims = 2022,
        DeliveryPerformanceSummary = 2023,
        WarrantyTransactions = 2024,
        
        /*merchandising*/
        Trading = 2040,
        TopSku = 2041,
        StockValuation = 2042,
        StockMovement = 2043,
        AllocatedStock = 2044,
        SalesComparison = 2045,
        FinancialQuery = 2046,
        Promotions = 2047,
        StockReceived = 2049,
        NegativeStock = 2050,
        WarehouseOversupply = 2051,
        SummaryUpdateControlReport = 2052,
        BuyerSalesHistory = 2053,
        WeeklyTrading = 2054
    }
}
