IF EXISTS (SELECT * FROM sysobjects WHERE TYPE = 'P' AND NAME = 'WTR_WarrantyReturnViewSP')
DROP PROCEDURE WTR_WarrantyReturnViewSP
GO

/****** Object:  StoredProcedure [dbo].[WTR_WarrantyReturnViewSP]    Script Date: 10/30/2018 9:43:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ========================================================================
-- Version:		<001> 
-- ========================================================================
CREATE PROCEDURE  [dbo].[WTR_WarrantyReturnViewSP]
	-- Add the parameters for the stored procedure here
As
BEGIN
	Truncate table  [Financial].[WarrantyReturnView_Temp]
   DROP INDEX [NonClusteredIndex-20180910-124356] ON [Financial].[WarrantyReturnView_Temp]

Insert into [Financial].[WarrantyReturnView_Temp]
SELECT W.Id, 
        W.ContractNumber, 
        W.DeliveredOn, 
        W.AccountType, 
        CASE 
            WHEN W.Department = 'PCW' 
            THEN 'PCE' 
            ELSE W.Department 
        END AS Department,
        W.SalePrice, 
        W.CostPrice, 
        W.BranchNo, 
        W.WarrantyNo, 
        W.WarrantyLength, 
        W.MessageId, 
		R.ElapsedMonths,
		R.PercentageReturn,
		(
            (CASE WHEN R.BranchType   = B.BranchType THEN 1 ELSE 0 END) 
            |
			(CASE WHEN R.BranchNumber = W.BranchNo   THEN 2 ELSE 0 END) 
            |
			(CASE WHEN R.WarrantyId   = WW.Id		 THEN 4 ELSE 0 END)
       ) AS [Level],
       ww.TaxRate,
       isnull(wsf.WarrantyLength,0) as FreeWarrantyLength
	from 
        Financial.WarrantyMessage W
        INNER JOIN
            Warranty.WarrantySale wse WITH (NOLOCK) on wse.WarrantyContractNo = W.ContractNumber
	    INNER JOIN Financial.BranchView B WITH (NOLOCK)
	        ON W.BranchNo = B.BranchNo
	    INNER JOIN Warranty.Warranty WW WITH (NOLOCK)
	        ON W.WarrantyNo = WW.Number
        LEFT JOIN warranty.WarrantySale wsf WITH (NOLOCK) 
           -- ON ws.WarrantyContractNo = w.ContractNumber + 'M'
            ON wsf.CustomerAccount = wse.CustomerAccount
            and wsf.ItemId = wse.ItemId
            and wsf.StockLocation = wse.StockLocation
            and wsf.WarrantyGroupId = wse.WarrantyGroupId
            --and wsf.WarrantyType = 'F'
	    INNER JOIN Warranty.WarrantyReturn R WITH (NOLOCK)
	        ON  (R.BranchType IS NULL     OR R.BranchType     = B.BranchType)
	        AND (R.BranchNumber IS NULL   OR R.BranchNumber   = W.BranchNo)
	        AND (R.WarrantyId IS NULL     OR R.WarrantyId     = WW.Id)
	        AND (R.WarrantyLength IS NULL OR R.WarrantyLength = W.WarrantyLength)
	        AND (R.Level_1 IS NULL OR r.Level_1 = CASE WHEN W.Department = 'PCW' THEN 'PCE' ELSE w.Department END)
            AND (R.FreeWarrantyLength IS NULL OR r.FreeWarrantyLength = ISNULL(wsf.WarrantyLength, 12))

			where w.DeliveredOn >DATEADD(MONTH, -24, GETDATE())   --Added by Nilesh May/05/2018
			and wsf.WarrantyType = 'F'

			CREATE NONCLUSTERED INDEX [NonClusteredIndex-20180910-124356] ON [Financial].[WarrantyReturnView_Temp]
(
	[ContractNumber] ASC,
	[Id] ASC,
	[Level] ASC,
	[DeliveredOn] ASC,
	[ElapsedMonths] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)


END

