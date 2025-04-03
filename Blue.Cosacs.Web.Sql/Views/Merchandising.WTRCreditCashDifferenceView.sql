IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[Merchandising].[WTRCreditCashDifferenceView]'))
	DROP VIEW [Merchandising].[WTRCreditCashDifferenceView]

/****** Object:  View [Merchandising].[WTRCreditCashDifferenceView]    Script Date: 11/22/2018 6:14:34 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
-- ========================================================================
-- Version:		<003> 
-- ========================================================================
CREATE VIEW [Merchandising].[WTRCreditCashDifferenceView]
AS
  
  WITH ProductIds
  AS
  (
  SELECT DISTINCT p.Id
                         FROM Merchandising.Product p
                         INNER JOIN Merchandising.ProductStockLevel psl 
                         ON p.Id = psl.ProductId
                         INNER JOIN Merchandising.ProductStatus ps
                         ON p.[Status] = ps.Id
                         WHERE ps.Name = 'Deleted'
                             AND psl.StockOnHand = 0
                             AND psl.StockOnOrder = 0
                             AND psl.StockAvailable = 0
  )
  ,

  ExportData(DateKey, TransactionDate, Fascia, LocationId, LocationName, SaleType, Division, DepartmentCode, Department, ClassCode, Class, Price, GrossProfit)
    AS
    (
        SELECT CONVERT(INT, CONVERT(VARCHAR(8), DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), d.datetrans), 112)) AS DateKey
               , CAST(DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), d.datetrans) AS DATE) AS TransactionDate
               , l.Fascia
               , l.SalesId AS LocationId
               , l.Name AS LocationName
               , 'Credit/Cash Difference' AS SaleType
               , ISNULL(division.Tag, '(Unknown Division)') AS Division
               , ISNULL(Department.Code, '0') AS DepartmentCode
               , ISNULL(Department.Tag, '(Unknown Department)') AS Department
               , ISNULL(Class.Code, '0') AS ClassCode
               , ISNULL(Class.Tag, '(Unknown Class)') AS Class
			     ,CONVERT(DECIMAL(20,10),
			   ROUND(SUM(ISNULL(
			    (CASE 
				 WHEN c.agrmttaxtype = 'E' AND c.taxtype = 'E'  OR  ag.TaxFree = 1
					--THEN CASE WHEN  d.delorcoll !='R' THEN (((d.transvalue/quantity)-i.unitpricecash)*quantity)
					THEN CASE WHEN  d.delorcoll !='R' THEN (((d.transvalue/quantity)-spa.CashPrice)*quantity)
					ELSE 0.00
					END
				WHEN c.agrmttaxtype = 'I' AND c.taxtype = 'E' 
					--THEN CASE WHEN  d.delorcoll !='R' THEN (((((d.transvalue*100)/(100+ isnull(i.taxrate,0)))/quantity)-i.unitpricecash)*quantity)
					THEN CASE WHEN  d.delorcoll !='R' THEN (((((d.transvalue*100)/(100+ isnull(i.taxrate,0)))/quantity)-spa.CashPrice)*quantity)
					ELSE 0.00 END
				ELSE 
					--CASE WHEN  d.delorcoll !='R' THEN ( ((((d.transvalue*100)/(100+ isnull(i.taxrate,0)))/quantity) - ((i.unitpricecash * 100)/(isnull(i.taxrate,0) + 100)))*quantity )
					CASE WHEN  d.delorcoll !='R' THEN ( ((((d.transvalue*100)/(100+ isnull(i.taxrate,0)))/quantity) - ((spa.CashPrice * 100)/(isnull(i.taxrate,0) + 100)))*quantity )
					ELSE 0.00 END
				END ),0.00)
				),2)) AS Price
				 ,CONVERT(DECIMAL(20,10),
			   ROUND(SUM(ISNULL(
			    (CASE 
				 WHEN c.agrmttaxtype = 'E' AND c.taxtype = 'E'  OR  ag.TaxFree = 1
					--THEN CASE WHEN  d.delorcoll !='R' THEN (((d.transvalue/quantity)-i.unitpricecash)*quantity)
					THEN CASE WHEN  d.delorcoll !='R' THEN (((d.transvalue/quantity)-spa.CashPrice)*quantity)
					ELSE 0.00
					END
				WHEN c.agrmttaxtype = 'I' AND c.taxtype = 'E' 
					--THEN CASE WHEN  d.delorcoll !='R' THEN (((((d.transvalue*100)/(100+ isnull(i.taxrate,0)))/quantity)-i.unitpricecash)*quantity)
					THEN CASE WHEN  d.delorcoll !='R' THEN (((((d.transvalue*100)/(100+ isnull(i.taxrate,0)))/quantity)-spa.CashPrice)*quantity)
					ELSE 0.00 END
				ELSE 
					--CASE WHEN  d.delorcoll !='R' THEN ( ((((d.transvalue*100)/(100+ isnull(i.taxrate,0)))/quantity) - ((i.unitpricecash * 100)/(isnull(i.taxrate,0) + 100)))*quantity )
					CASE WHEN  d.delorcoll !='R' THEN ( ((((d.transvalue*100)/(100+ isnull(i.taxrate,0)))/quantity) - ((spa.CashPrice * 100)/(isnull(i.taxrate,0) + 100)))*quantity )
					ELSE 0.00 END
				END ),0.00)
				),2)) AS GrossProfit 
       FROM country AS c  with(Nolock)
	   CROSS JOIN delivery d with(Nolock)
	   INNER JOIN Merchandising.Dates AS dates 
	        ON CONVERT(INT, CONVERT(VARCHAR(8), DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), d.datetrans), 112)) = dates.DateKey 
	    INNER JOIN branch AS b  
		ON b.branchno = LEFT(d.acctno, 3) 
		AND  D.acctno LIKE '___0%'
		INNER JOIN Merchandising.Location l 
		ON l.SalesId = b.branchno
		 INNER JOIN agreement ag 
		 on d.acctno = ag.acctno
        and d.agrmtno = ag.agrmtno
		INNER JOIN stockitem i         
		ON i.ItemID  = d.ItemID  
		And l.salesid = i.stocklocn 
		AND i.itemtype = 'S' 
		INNER JOIN Merchandising.Product AS p 
		ON p.SKU = i.sku
        LEFT OUTER JOIN Merchandising.ProductHierarchyView AS division 
	        ON division.ProductId = p.Id 
	        AND division.[Level] = 'Division' 
        LEFT OUTER JOIN  Merchandising.ProductHierarchyView AS Department 
	        ON Department.ProductId = p.Id 
	        AND Department.[Level] = 'Department'
        LEFT OUTER JOIN MERchandising.ProductHierarchyView as Class
	        ON Class.ProductId = p.Id
	        AND Class.[Level] = 'Class' 
		LEFT JOIN StockPriceAudit spa 
        ON spa.ID = d.ItemID 
        and spa.BranchNo = LEFT(d.acctno, 3) 
        and spa.DateChange = (select Top 1 datechange
                                from StockPriceAudit spa2
                                where spa2.ID = spa.ID
                                and spa2.BranchNo = spa.BranchNo
                                and spa2.DateChange <= d.datetrans 
				Order by datechange desc)
	WHERE d.datetrans > DATEADD(month, -24, GETDATE()) 				
        AND p.[Status] != (SELECT Id FROM Merchandising.ProductStatus where Name = 'Non Active')
        AND p.Id NOT IN (SELECT * from ProductIds)      
	AND ISNULL(d.quantity,0) != 0 		
			--AND i.unitpricecash != i.unitpricehp  ---Commented on 09May2019 by Charudatt
	AND spa.CreditPrice!=spa.CashPrice   ---Added on 09May2019 by Charudatt
        GROUP BY CONVERT(INT, CONVERT(VARCHAR(8), DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), d.datetrans), 112)), 
                 CAST(DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), d.datetrans) AS DATE), 
                 l.Fascia, 
                 l.SalesId, 
                 l.Name, 
                 ISNULL(division.Tag, '(Unknown Division)'), 
                 ISNULL(Department.Code, '0'), 
                 ISNULL(Department.Tag, '(Unknown Department)'), 
                 ISNULL(Class.Code, '0'), 
                 ISNULL(Class.Tag, '(Unknown Class)')
   )


    SELECT ROW_NUMBER() OVER(ORDER BY DateKey) AS Id,
           data.*
    FROM (
            SELECT * FROM ExportData
            UNION ALL --Fascia
            SELECT DateKey, 
                   TransactionDate, 
                   Fascia, 
                   f.FasciaId AS LocationId, 
                   Fascia AS LocationName, 
                   SaleType AS SaleType, 
                   Division AS Division, 
                   DepartmentCode AS DepartmentCode,
                   Department AS Department, 
                   ClassCode AS ClassCode,
                   Class AS Class,
                   SUM(Price) AS Price, 
                   SUM(GrossProfit) AS GrossProfit
            FROM ExportData e
            INNER JOIN [Merchandising].[Merchandising.WTRFasciaView] f on e.Fascia = f.FasciaName
            GROUP BY DateKey, 
                     TransactionDate, 
                     Fascia, 
                     f.FasciaId,
                     SaleType,
                     Division,
                     DepartmentCode,
                     Department,
                     ClassCode,
                     Class
            UNION ALL --Country
            SELECT DateKey, 
                   TransactionDate, 
                   'Company' AS Fascia, 
                   0 AS LocationId, 
                   'Company' AS LocationName, 
                   SaleType AS SaleType, 
                   Division AS Division, 
                   DepartmentCode AS DepartmentCode,
                   Department AS Department, 
                   ClassCode AS ClassCode,
                   Class AS Class,
                   SUM(Price) AS Price, 
                   SUM(GrossProfit) AS GrossProfit
            FROM ExportData
            GROUP BY DateKey, 
                     TransactionDate,
                     SaleType,
                     Division,
                     DepartmentCode,
                     Department,
                     ClassCode,
                     Class
        ) AS data



GO