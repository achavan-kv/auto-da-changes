IF EXISTS (SELECT * FROM sysobjects WHERE TYPE = 'P' AND NAME = 'WTRWarrantyReturnValuesViewSP')
DROP PROCEDURE WTRWarrantyReturnValuesViewSP

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =================================================================================================
-- Version:		<001> 
--				002 - Optimization changes to improve WRT_warrantyReturnValuesView job execution time.  
-- =================================================================================================

CREATE PROCEDURE WTRWarrantyReturnValuesViewSP
As
BEGIN

IF OBJECT_ID('Merchandising.WTRWarrantyReturnValuesView_Temp', 'U') IS NOT NULL
 DROP TABLE Merchandising.WTRWarrantyReturnValuesView_Temp

 select Id,MAX(wm.[Level]) as [Level] Into #temp   FROM Financial.WarrantyReturnView wm   group by  Id ---OPTION (MAXDOP 4) 

 SELECT wr.WarrantyNumber,
           wr.WarrantyContractNumber, 
           wm.Department, 
           wm.BranchNo, 
           wm.SalePrice AS OriginalSalePrice, 
           wm.CostPrice AS OriginalCostPrice, 
           wm.PercentageReturn, 
           CASE 
	           WHEN (SELECT ISNULL(c.ValueBit, 0) FROM Config.Setting c WHERE c.Id = 'TaxInclusive' AND c.Namespace = 'Blue.Cosacs.Merchandising') = 0 
		           THEN wm.SalePrice
	           ELSE (wm.SalePrice * 100)/(100 + wm.TaxRate)
           END AS SalePrice,
           wm.CostPrice AS CostPrice,
           CASE 
	           WHEN (SELECT ISNULL(c.ValueBit, 0) FROM Config.Setting c WHERE c.Id = 'TaxInclusive' AND c.Namespace = 'Blue.Cosacs.Merchandising') = 0 
		           THEN wm.SalePrice - (wm.PercentageReturn * wm.SalePrice / 100)
	           ELSE (wm.SalePrice * (100 - wm.PercentageReturn) / (100 + wm.TaxRate))
           END AS ReturnValue,
		   wm.CostPrice - (wm.PercentageReturn * wm.CostPrice / 100) AS ReturnCost,
           wr.ReturnDate AS ReturnDate,
           wr.SaleType
    INTO Merchandising.WTRWarrantyReturnValuesView_Temp
    FROM Financial.WarrantyReturnView wm
    INNER JOIN Merchandising.WTRWarrantyReturnsView wr
    ON wr.WarrantyContractNumber = wm.ContractNumber
    WHERE wm.[Level] = (SELECT  ([Level]) FROM  #temp wm2 WHERE wm.Id = wm2.Id)
        AND wm.ElapsedMonths = CASE
								    WHEN dbo.FullMonthsDiff(wm.DeliveredOn, wr.ReturnDate) < 1 THEN 1
								    ELSE dbo.FullMonthsDiff(wm.DeliveredOn, wr.ReturnDate)
							    END  
        AND NOT EXISTS (SELECT 'A' FROM IgnoreCRECRF WHERE ContractNo = wm.ContractNumber)


   
END
