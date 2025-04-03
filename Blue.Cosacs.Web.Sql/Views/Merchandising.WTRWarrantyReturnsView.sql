IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[Merchandising].[WTRWarrantyReturnsView]'))
	DROP VIEW [Merchandising].[WTRWarrantyReturnsView]

/****** Object:  View [Merchandising].[WTRWarrantyReturnsView]    Script Date: 8/24/2018 7:03:42 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
-- ========================================================================
-- Version:		<002>
-- ========================================================================
CREATE VIEW [Merchandising].[WTRWarrantyReturnsView]
AS

    SELECT d.contractno AS WarrantyContractNumber,
           w.Number AS WarrantyNumber,
           CAST(d.datetrans AS DATE) AS ReturnDate,
           d.delorcoll AS CollectOrRepossession,
           CASE 
	        WHEN SUBSTRING(d.acctno, 4, 1) = '5' THEN 'Cash'
	        WHEN SUBSTRING(d.acctno, 4, 1) = '4' THEN 'Cash'
	        ELSE 'Credit'
	      END AS SaleType
    FROM delivery AS d 
    INNER JOIN StockInfo AS i 
	    ON i.Id = d.ItemID 
    INNER JOIN Warranty.Warranty AS w 
	    ON w.Number = COALESCE(i.SKU, i.itemno, i.IUPC)
    WHERE d.delorcoll != 'D'
        AND w.TypeCode = 'E'
	    AND d.datetrans > DATEADD(month, -24, GETDATE())   --Added by Charudatt Aug/24/2018 because of timeout issue

GO