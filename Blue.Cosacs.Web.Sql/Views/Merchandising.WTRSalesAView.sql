IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[Merchandising].[WTRSalesAView]'))
	DROP VIEW [Merchandising].[WTRSalesAView]

/****** Object:  View [Merchandising].[WTRSalesAView]    Script Date: 11/22/2018 6:11:42 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
-- ========================================================================
-- Version:		<002>
-- ========================================================================
CREATE VIEW [Merchandising].[WTRSalesAView]
AS

  WITH ProductIds
  AS
  (
  SELECT DISTINCT p.Id
                         FROM Merchandising.Product p with(Nolock)
                         INNER JOIN Merchandising.ProductStockLevel psl with(Nolock)
                         ON p.Id = psl.ProductId
                         INNER JOIN Merchandising.ProductStatus ps with(Nolock)
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
SELECT   CONVERT(INT, CONVERT(VARCHAR(8), DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), d.datetrans), 112)) AS DateKey,
        CAST(DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()),  d.datetrans) AS DATE) AS TransactionDate,
        l.Fascia,
        f.FasciaId AS LocationId,
        l.Fascia AS LocationName,
        'Product Sales' AS SaleType,
        ISNULL(Division.Tag, '(Unknown Division)') AS Division,
        ISNULL(Department.Code, '0') AS DepartmentCode,
        ISNULL(Department.Tag, '(Unknown Department)') AS Department,
        ISNULL(Class.Code, '0') AS ClassCode,
        ISNULL(Class.Tag, '(Unknown Class)') AS Class
       ,CONVERT(DECIMAL(20,10),
				ROUND(SUM(ISNULL(
				 (CASE 
					WHEN c.agrmttaxtype = 'E' AND c.taxtype = 'E' OR ag.TaxFree = 1
					--THEN CASE WHEN (ROUND(i.unitpricecash, 2)) > 0  AND d.acctno LIKE '___0%'  AND d.delorcoll !='R' THEN (ROUND(i.unitpricecash, 2)  * d.quantity)
					THEN CASE WHEN (ROUND(ISNULL(pp.unitprice,spa.CashPrice), 2)) > 0  AND d.acctno LIKE '___0%'  AND d.delorcoll !='R' THEN (ROUND(ISNULL(pp.unitprice,spa.CashPrice), 2)  * d.quantity)
							ELSE d .transvalue
							END
					 WHEN c.agrmttaxtype = 'I' AND c.taxtype = 'E' 
					 --THEN CASE WHEN (ROUND(i.unitpricecash, 2)) > 0 AND d.acctno LIKE '___0%'  AND d.delorcoll !='R' THEN (ROUND(i.unitpricecash, 2)  * d.quantity)
					  THEN CASE WHEN (ROUND(ISNULL(pp.unitprice,spa.CashPrice), 2)) > 0 AND d.acctno LIKE '___0%'  AND d.delorcoll !='R' THEN (ROUND(ISNULL(pp.unitprice,spa.CashPrice), 2)  * d.quantity)
						ELSE (d .transvalue * 100) / ( isnull(i.taxrate,0) + 100)  END
					ELSE 
						--CASE WHEN (ROUND(i.unitpricecash, 2)) > 0 AND d.acctno LIKE '___0%'  AND d.delorcoll !='R' THEN ( (i.unitpricecash * 100) / ( isnull(i.taxrate,0) + 100)  * d.quantity)
						CASE WHEN (ROUND(ISNULL(pp.unitprice,spa.CashPrice), 2)) > 0 AND d.acctno LIKE '___0%'  AND d.delorcoll !='R' THEN ( (ISNULL(pp.unitprice,spa.CashPrice) * 100) / ( isnull(i.taxrate,0) + 100)  * d.quantity)
						ELSE (d .transvalue * 100) / ( isnull(i.taxrate,0) + 100)  END
					END				
					),0.00)),2 )
				) AS Price 
				 ,CONVERT(DECIMAL(20,10),
				 ROUND(SUM(ISNULL(
				(CASE 
	            WHEN c.agrmttaxtype = 'E' AND c.taxtype = 'E' OR  ag.TaxFree = 1
	            THEN 
					CASE WHEN (ROUND(ISNULL(pp.unitprice,spa.CashPrice), 2)) > 0  AND d.acctno LIKE '___0%'  AND d.delorcoll !='R'
					THEN
					(ROUND(ISNULL(pp.unitprice,spa.CashPrice), 2) - ISNULL(cp.AverageWeightedCost, 0))  - 
					(ISNULL(cp.AverageWeightedCost, 0) * COALESCE(Class.FirstYearWarrantyProvision, Department.FirstYearWarrantyProvision, Division.FirstYearWarrantyProvision, 0))
					ELSE
					((d .transvalue/d.quantity) - ISNULL(cp.AverageWeightedCost, 0))  - 
					(ISNULL(cp.AverageWeightedCost, 0) * COALESCE(Class.FirstYearWarrantyProvision, Department.FirstYearWarrantyProvision, Division.FirstYearWarrantyProvision, 0))
					END
	           WHEN c.agrmttaxtype = 'I' AND c.taxtype = 'E' 
			   THEN
					CASE WHEN (ROUND(ISNULL(pp.unitprice,spa.CashPrice), 2)) > 0 AND d.acctno LIKE '___0%'  AND d.delorcoll !='R'
					THEN
					(ROUND(ISNULL(pp.unitprice,spa.CashPrice), 2) - ISNULL(cp.AverageWeightedCost, 0))  - 
					(ISNULL(cp.AverageWeightedCost, 0) *COALESCE(Class.FirstYearWarrantyProvision, Department.FirstYearWarrantyProvision, Division.FirstYearWarrantyProvision, 0))
					ELSE 
					((d .transvalue * 100) / (isnull(i.taxrate,0) + 100) / d.quantity) - ISNULL(cp.AverageWeightedCost, 0) - 
					(ISNULL(cp.AverageWeightedCost, 0) *COALESCE(Class.FirstYearWarrantyProvision, Department.FirstYearWarrantyProvision, Division.FirstYearWarrantyProvision, 0))
					END
			  ELSE 
					CASE WHEN (ROUND(ISNULL(pp.unitprice,spa.CashPrice), 2)) > 0 AND d.acctno LIKE '___0%'  AND d.delorcoll !='R' 
					THEN
					((ISNULL(pp.unitprice,spa.CashPrice) * 100) / (isnull(i.taxrate,0) + 100) - ISNULL(cp.AverageWeightedCost, 0))  - 
					(ISNULL(cp.AverageWeightedCost, 0) *COALESCE(Class.FirstYearWarrantyProvision, Department.FirstYearWarrantyProvision, Division.FirstYearWarrantyProvision, 0))
					ELSE
					((d .transvalue * 100) / ( isnull(i.taxrate,0) + 100) / d.quantity) - ISNULL(cp.AverageWeightedCost, 0)  - 
					(ISNULL(cp.AverageWeightedCost, 0) *COALESCE(Class.FirstYearWarrantyProvision, Department.FirstYearWarrantyProvision, Division.FirstYearWarrantyProvision, 0))
					END
			    END
				) * d.quantity, 0.00 )),2)
				)AS GrossProfit
		FROM country AS c with(Nolock)
		CROSS JOIN delivery d with(Nolock) 
	    INNER JOIN Merchandising.Dates AS dt with(Nolock)
        ON CONVERT(INT, CONVERT(VARCHAR(8), DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), d.datetrans), 112)) = dt.DateKey
		INNER JOIN branch AS b with(Nolock) 
		ON b.branchno = LEFT(d.acctno, 3) 
		INNER JOIN acct a with(Nolock)
		ON d.acctno = a.acctno
		INNER JOIN Merchandising.Location l with(Nolock)
		ON l.SalesId = b.branchno
		 INNER JOIN agreement ag with(Nolock)
		 on d.acctno = ag.acctno
        and d.agrmtno = ag.agrmtno
		INNER JOIN stockitem i   with(Nolock)      
		ON i.ItemID  = d.ItemID  
		And l.salesid = i.stocklocn 
		AND i.itemtype= 'S'
		INNER JOIN [Merchandising].[Merchandising.WTRFasciaView] f 
		on l.Fascia = f.FasciaName
		INNER JOIN Merchandising.Product AS p with(Nolock)
		ON p.SKU = i.sku		
		INNER JOIN merchandising.costprice cp with(Nolock)
			on cp.productId = p.id 
			and cp.Id = (SELECT top 1 id               
						 FROM merchandising.costprice cp2 with(Nolock)
						 WHERE cp2.productid = cp.productid 
                         AND cp2.AverageWeightedCostUpdated <= d.datetrans
						 order by cp2.AverageWeightedCostUpdated desc )    
      LEFT JOIN Merchandising.ProductHierarchyView AS Division with(Nolock) 
        ON Division.ProductId = p.Id
        AND Division.[Level] = 'Division'
      LEFT JOIN Merchandising.ProductHierarchyView AS Department with(Nolock)
        ON Department.ProductId = p.Id
        AND Department.[Level] = 'Department'
      LEFT JOIN Merchandising.ProductHierarchyView as Class with(Nolock)
        ON Class.ProductId = p.Id
        AND Class.[Level] = 'Class'
      LEFT JOIN StockPriceAudit spa with(Nolock)
        ON spa.ID = d.ItemID 
        and spa.BranchNo = LEFT(d.acctno, 3) 
	    and spa.DateChange = (select   Max(datechange)
                                from StockPriceAudit spa2 with(Nolock)   where spa2.ID = spa.ID and spa2.BranchNo = spa.BranchNo  
		and spa2.DateChange <= (select  top 1 (d2.datetrans)  from delivery d2 with(Nolock)  where d2.acctno = d.acctno
                                    and d2.ItemID = d.ItemID   and d2.stocklocn = d.stocklocn  and d2.contractno = d.contractno
                                    and d2.ParentItemID = d.ParentItemID   and d2.delorcoll = 'D'  order by datetrans desc
									) )
	 LEFT JOIN promoprice pp with(Nolock)
         ON d.ItemID = pp.ItemID 
            and pp.stocklocn = LEFT(d.acctno, 3)                              
            and ((select top 1 (select mAX( D2.DateChange)  from LineItemaudit d2 with(Nolock) where d.acctno = d2.acctno and d.itemno = d2.itemno 
				and d.agrmtno=d2.agrmtno AND  d2.stocklocn = d.stocklocn AND   D2.valueafter>0))  >= pp.fromdate                                                          
            and (select top 1 (select mAX( D2.DateChange)  from LineItemaudit d2 with(Nolock) where d.acctno = d2.acctno and d.itemno = d2.itemno 
				and d.agrmtno=d2.agrmtno AND  d2.stocklocn = d.stocklocn AND   D2.valueafter>0 )) <= pp.todate)
            and pp.hporcash = 
                case
                    when a.accttype = 'C' then 'C'
					else 'H'
                end

      WHERE p.[Status] IN (SELECT Id FROM Merchandising.ProductStatus with(Nolock) where ID > 1 ) --Name = 'Non Active')   
		AND   d.datetrans > DATEADD(month, -24, GETDATE())		
       AND p.Id NOT IN (Select * from ProductIDs)
			AND ISNULL(d.quantity,0) != 0    
		  GROUP BY
        CONVERT(INT, CONVERT(VARCHAR(8), DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), d.datetrans), 112)),
        CAST(DATEADD(mi, DATEDIFF(mi, GETUTCDATE(), GETDATE()), d.datetrans) AS DATE),
        l.Fascia,
        f.FasciaId,
        ISNULL(Division.Tag, '(Unknown Division)'),
        ISNULL(Department.Code, '0'),
        ISNULL(Department.Tag, '(Unknown Department)'),
        ISNULL(Class.Code, '0'),
        ISNULL(Class.Tag, '(Unknown Class)')
    )
    
    SELECT ROW_NUMBER() OVER(ORDER BY DateKey) AS Id, 
           data.*
    FROM (
            SELECT * FROM ExportData
            UNION ALL
            SELECT DateKey, 
                   TransactionDate, 
                   'Company' AS Fascia, 
                   0 AS LocationId, 
                   'Company' AS LocationName,
                   SaleType, 
                   Division, 
                   DepartmentCode, 
                   Department, 
                   ClassCode, 
                   Class, 
                   SUM(ISNULL(Price,0.00)) AS Price, 
                   SUM(ISNULL(GrossProfit,0.00)) AS GrossProfit
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