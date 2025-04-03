If Exists ( Select *  From  dbo.sysobjects   Where  id = object_id('[dbo].[uspWarrantyHitRate_Summary]')    and OBJECTPROPERTY(id, 'IsProcedure') = 1)
	Drop Procedure [dbo].[uspWarrantyHitRate_Summary] 
GO  
---Exec dbo.uspWarrantyHitRate_Summary '20200120','20200121',1 
--Exec dbo.uspWarrantyHitRate_Detail '20200120','20200121',1 
-- ========================================================================
-- Author:<Zensar,Charudatt Patil>
-- Create date: <01/07/2019>
-- Description:	<Calculate Warranty Hit Rate Peremage for Sales Persons>
-- Version:<008>
-- Changes Done By:<Zensar,Santosh Kanade>
---Changes:- Match the Report with WHR Deatils With Warranty sales with Salesperson (22 Oct-2019) 
-- ========================================================================
CREATE PROCEDURE dbo.uspWarrantyHitRate_Summary
 @DateFrom   Date,
 @DateTo     Date,
 @IncludeReprocessedCancelled Bit
AS    
   BEGIN TRY
   DECLARE
		@BranchNumber SMALLINT = NULL,
		@StoreType CHAR(1) = NULL,
		@SalesPersonId INT = NULL
	DECLARE  @path varchar(1000) = 'd:\users\default'
	DECLARE @statement sqltext
	DECLARE @startDate Datetime,@filename varchar(1000)
	SET @startDate = GetDate()
	set @statement =db_name()
	SELECT @filename = @path+ '\WHRSummary' +  replace(convert(varchar(10), @startDate, 120), '-', '') +'.csv' 
	SET NOCOUNT on		
	 		
	IF OBJECT_ID('Reports..WHRSummary','U') IS NOT NULL 
		DROP TABLE WHRSummary
	IF OBJECT_ID('cosacs..WHRSummary','U') IS NOT NULL 
		DROP TABLE WHRSummary
  	--------------------------------------------------------------------------------------------------------
	PRINT 'Get all Warranty Sales\Coolection Accounts'
	SELECT DISTINCT d.acctno As AccountNumber,saleperson.id As Saleperson, saleperson.Fullname AS SalespersonName,(d.agrmtno) As AgreementNumber,Fascia,d.Itemno
		,CAST(htv.LevelId AS VARCHAR) + ' - ' + htv.Name AS [Division],CAST(htdp.LevelId AS VARCHAR) + ' - ' + htdp.Name AS [Department],CAST(htc.LevelId AS VARCHAR) + ' - ' + htc.Name AS [Class]
	INTO  #AllWarrantyTransTest
	FROM Delivery d WITH(NOLOCK)
		INNER JOIN Stockinfo s WITH(NOLOCK)	ON d.ItemID = s.Id
		INNER JOIN Merchandising.Product p WITH(NOLOCK)	ON d.ItemNo=p.SKU
		INNER JOIN [Merchandising].[ProductHierarchy] phv WITH(NOLOCK) ON p.ID = phv.ProductId AND phv.HierarchyLevelId =1
		INNER JOIN [Merchandising].[HierarchyTag] htv WITH(NOLOCK)	ON phv.HierarchyTagId = htv.id	AND htv.Name NOT IN ('OPTICAL','REPOSSESSED GOODS','DV FREE GIFT','SPARE PARTS')
		INNER JOIN [Merchandising].[ProductHierarchy] phdp WITH(NOLOCK)	ON p.ID = phdp.ProductId AND phdp.HierarchyLevelId =2
		INNER JOIN [Merchandising].[HierarchyTag] htdp WITH(NOLOCK)	ON phdp.HierarchyTagId = htdp.id AND htdp.Name NOT IN ('FLOOR COVERINGS')
		INNER JOIN [Merchandising].[ProductHierarchy] phc WITH(NOLOCK)ON p.ID = phc.ProductId AND phc.HierarchyLevelId =3
		INNER JOIN [Merchandising].[HierarchyTag] htc WITH(NOLOCK)	ON phc.HierarchyTagId = htc.id	AND htc.Name NOT IN ('AUDIO ACCESSORIES')
		INNER JOIN agreement agr WITH(NOLOCK)	ON d.acctno = agr.acctno AND d.agrmtno = agr.agrmtno
		LEFT JOIN [Admin].[User] saleperson WITH(NOLOCK) ON saleperson.id = agr.empeenosale
		INNER JOIN acct a WITH(NOLOCK)  ON d.acctno = a.acctno
		INNER JOIN Warranty.WarrantySale ws WITH(NOLOCK) ON d.acctno = ws.CustomerAccount AND d.agrmtno = ws.AgreementNumber AND ISNULL(ws.WarrantyType, '') != 'F' 
		INNER JOIN Merchandising.Location l  WITH(NOLOCK)	ON d.BranchNo = l.SalesId
	WHERE ((d.delorcoll = 'D') OR (d.delorcoll = 'C' ) OR (d.delorcoll = 'R' ) ) AND ISNULL(a.accttype, '') != '' AND CONVERT(DATE, d.datetrans)BETWEEN @DateFrom AND @DateTo
		 AND d.Itemno NOT IN  (SELECT SKU FROM Nonstocks.NonStock  WITH(NOLOCK))
---------------------------------------------------------------------------------------------------------
	SELECT AccountNumber,Saleperson,SalespersonName,AgreementNumber,Fascia,
		Itemno INTO #AllWarrantyTrans FROM #AllWarrantyTransTest 
	EXCEPT 
	SELECT w.AccountNumber,w.Saleperson,w.SalespersonName,w.AgreementNumber,w.Fascia, w.Itemno 
	FROM #AllWarrantyTransTest w
		INNER JOIN hierarchyexclusion h ON w.Division = h.Division  AND w.Department = h.Department  AND w.Class = h.Class 

   PRINT 'Get all Sold Warranties'
	SELECT  Sum(d.quantity) As NoofWarrantySold, Sum(d.transvalue) As SoldWarrantyValue,aws.Saleperson As Saleperson , aws.SalespersonName, aws.Fascia
	INTO #SoldWarranty
	FROM Delivery d WITH(NOLOCK)
		INNER JOIN Stockinfo si WITH(NOLOCK)ON d.ItemID = si.Id
		INNER JOIN #AllWarrantyTrans aws On d.acctno = aws.AccountNumber AND d.agrmtno = aws.AgreementNumber
		INNER JOIN Warranty.WarrantySale s ON d.acctno = s.CustomerAccount	AND d.agrmtno = s.AgreementNumber AND d.contractno = s.WarrantyContractNo 	AND ISNULL(s.WarrantyType, '') != 'F' /*No point on free @Warranties*/
	WHERE d.delorcoll = 'D' AND contractno != '' AND si.itemtype != 's'
			AND CONVERT(DATE, d.datetrans) BETWEEN @DateFrom AND @DateTo AND  d.itemno Not Like '%M'
	GROUP BY Saleperson,SalespersonName,Fascia
	---------------------------------------------------------------------------------------------------------
	PRINT 'Get all Warrantable Items on the Warranty Sales Account'
	SELECT 	SUM(d.quantity) As WarrantableQty, Sum(d.transvalue) As WarrantableQtyValue,aws.Saleperson As Saleperson ,	aws.SalespersonName,aws.Fascia
	INTO #SoldWarrantable
	FROM Delivery d WITH(NOLOCK)
		INNER JOIN Stockinfo s WITH(NOLOCK)	ON d.ItemID = s.Id
		INNER JOIN Merchandising.Product p WITH(NOLOCK)	ON d.ItemNo=p.SKU
		INNER JOIN [Merchandising].[ProductHierarchy] phv WITH(NOLOCK)	ON p.ID = phv.ProductId AND phv.HierarchyLevelId =1
		INNER JOIN [Merchandising].[HierarchyTag] htv WITH(NOLOCK)	ON phv.HierarchyTagId = htv.id 	AND htv.Name NOT IN ('OPTICAL','REPOSSESSED GOODS','DV FREE GIFT','SPARE PARTS')
		INNER JOIN [Merchandising].[ProductHierarchy] phdp WITH(NOLOCK)	ON p.ID = phdp.ProductId AND phdp.HierarchyLevelId =2
		INNER JOIN [Merchandising].[HierarchyTag] htdp WITH(NOLOCK)	ON phdp.HierarchyTagId = htdp.id AND htdp.Name NOT IN ('FLOOR COVERINGS')
		INNER JOIN [Merchandising].[ProductHierarchy] phc WITH(NOLOCK)	ON p.ID = phc.ProductId AND phc.HierarchyLevelId =3
		INNER JOIN [Merchandising].[HierarchyTag] htc WITH(NOLOCK) 	ON phc.HierarchyTagId = htc.id	AND htc.Name NOT IN ('AUDIO ACCESSORIES')
		INNER JOIN #AllWarrantyTrans aws On d.acctno = aws.AccountNumber AND d.agrmtno = aws.AgreementNumber
		INNER JOIN acct a WITH(NOLOCK)      ON d.acctno = a.acctno
	WHERE d.delorcoll = 'D'  AND s.itemtype = 'S' AND s.Warrantable =1 	AND ISNULL(a.accttype, '') != '' AND CONVERT(DATE, d.datetrans) BETWEEN @DateFrom AND @DateTo
		AND d.quantity >0 AND d.Itemno NOT IN  (SELECT SKU FROM Nonstocks.NonStock  WITH(NOLOCK))
	GROUP BY Saleperson,SalespersonName,Fascia
	--------------------------------------------------------------------------------------------------------
    PRINT 'Get all Returned Warranties'
	SELECT 	Sum(d.quantity) As NoofReturnWarranty, Sum(d.transvalue) As ReturnWarrantableValue,aws.Saleperson ,aws.SalespersonName,aws.Fascia
	INTO #ReturnedWarranty
	FROM Delivery d WITH(NOLOCK) INNER JOIN StockInfo si WITH(NOLOCK)	ON d.ItemID = si.Id
		INNER JOIN #AllWarrantyTrans aws	On d.acctno = aws.AccountNumber	AND d.agrmtno = aws.AgreementNumber
		INNER JOIN Warranty.WarrantySale s 	ON d.acctno = s.CustomerAccount AND d.agrmtno = s.AgreementNumber AND d.contractno = s.WarrantyContractNo AND ISNULL(s.WarrantyType, '') != 'F' /*No point on free @Warranties*/
	WHERE (d.delorcoll = 'C' AND contractno != '')	AND si.itemtype != 's' 	AND 1 = @IncludeReprocessedCancelled AND CONVERT(DATE, d.datetrans) BETWEEN @DateFrom AND @DateTo
	GROUP BY Saleperson,SalespersonName,Fascia
	--------------------------------------------------------------------------------------------------------
	SELECT Distinct d.ParentItemNo, d.acctno As AccountNumber,aws.Saleperson As Saleperson ,aws.SalespersonName,(d.agrmtno) As AgreementNumber,aws.Fascia
	INTO #ReturnWarrantyParent
	FROM Delivery d WITH(NOLOCK) 
		INNER JOIN StockInfo si WITH(NOLOCK)	ON d.ItemID = si.Id
		INNER JOIN #AllWarrantyTrans aws On d.acctno = aws.AccountNumber AND d.agrmtno = aws.AgreementNumber
		INNER JOIN Warranty.WarrantySale s WITH(NOLOCK)	ON d.acctno = s.CustomerAccount AND d.agrmtno = s.AgreementNumber 	AND d.contractno = s.WarrantyContractNo	AND ISNULL(s.WarrantyType, '') != 'F' /*No point on free @Warranties*/
	WHERE ((d.delorcoll = 'C' AND contractno != '')) AND si.itemtype != 's' AND 1 = @IncludeReprocessedCancelled AND CONVERT(DATE, d.datetrans) BETWEEN @DateFrom AND @DateTo
	--------------------------------------------------------------------------------------------------------
	SELECT DISTINCT d.Acctno,d.Itemno,	(d.quantity) As ReturnWarrantableQty,(d.transvalue) As ReturnWarrantableQtyValue,aws.Saleperson As Saleperson ,
		   aws.SalespersonName,aws.Fascia,CONVERT(DATE, d.datetrans) As datetrans
	INTO #RetrunedWarrantableWithinDate
	FROM  Delivery d WITH(NOLOCK) INNER JOIN Stockinfo s WITH(NOLOCK)
		ON d.ItemID = s.Id    	
		INNER JOIN #AllWarrantyTrans aws  On d.acctno = aws.AccountNumber AND d.agrmtno = aws.AgreementNumber AND d.Itemno= aws.Itemno
		INNER JOIN #ReturnWarrantyParent aws1 On d.acctno = aws1.AccountNumber AND d.agrmtno = aws1.AgreementNumber AND d.ItemNo= aws1.ParentItemNo
		INNER JOIN acct a WITH(NOLOCK)  ON d.acctno = a.acctno
	WHERE d.delorcoll = 'c'  AND s.itemtype = 'S' AND contractno = ''  AND s.Warrantable =1  AND ISNULL(a.accttype, '') != '' AND d.quantity <0 
		AND CONVERT(DATE, d.datetrans) BETWEEN @DateFrom AND @DateTo
	--------------------------------------------------------------------------------------------------------
	SELECT DISTINCT d.Acctno,d.Itemno, (d.quantity) As ReturnWarrantableQty,(d.transvalue) As ReturnWarrantableQtyValue,aws.Saleperson As Saleperson ,
		   aws.SalespersonName,aws.Fascia,CONVERT(DATE, d.datetrans) As datetrans
	INTO #RetrunedWarrantableBeforeDate
	FROM  Delivery d WITH(NOLOCK) INNER JOIN Stockinfo s WITH(NOLOCK) ON d.ItemID = s.Id
		INNER JOIN #AllWarrantyTrans aws On d.acctno = aws.AccountNumber AND d.agrmtno = aws.AgreementNumber AND d.Itemno= aws.Itemno 
		INNER JOIN #ReturnWarrantyParent aws1 On d.acctno = aws1.AccountNumber AND d.agrmtno = aws1.AgreementNumber AND d.ItemNo= aws1.ParentItemNo
		INNER JOIN acct a WITH(NOLOCK) ON d.acctno = a.acctno
	WHERE d.delorcoll = 'D'  AND s.itemtype = 'S' AND contractno = ''  AND s.Warrantable =1 AND ISNULL(a.accttype, '') != '' AND d.quantity >0
		  AND CONVERT(DATE, d.datetrans) < @DateFrom AND Not Exists (SELECT 1 FROM #RetrunedWarrantableWithinDate r WHERE d.Acctno =r.Acctno AND d.ItemNo=r.Itemno )
	--------------------------------------------------------------------------------------------------------
	INSERT INTO #RetrunedWarrantableBeforeDate SELECT * FROM #RetrunedWarrantableWithinDate
	--------------------------------------------------------------------------------------------------------
	INSERT INTO #RetrunedWarrantableBeforeDate 
	SELECT DISTINCT d.Acctno,d.Itemno,d.quantity,d.transvalue, saleperson.id,saleperson.Fullname,Fascia,CONVERT(DATE, d.datetrans) 
	FROM  Delivery d WITH(NOLOCK) 
		 INNER JOIN Stockinfo s WITH(NOLOCK) ON d.ItemID = s.Id
		 INNER JOIN Merchandising.Product p WITH(NOLOCK) ON d.ItemNo=p.SKU
		 INNER JOIN [Merchandising].[ProductHierarchy] phv WITH(NOLOCK) ON p.ID = phv.ProductId AND phv.HierarchyLevelId =1
		 INNER JOIN [Merchandising].[HierarchyTag] htv WITH(NOLOCK) ON phv.HierarchyTagId = htv.id AND htv.Name NOT IN ('OPTICAL','REPOSSESSED GOODS','DV FREE GIFT','SPARE PARTS')
		 INNER JOIN [Merchandising].[ProductHierarchy] phdp WITH(NOLOCK) ON p.ID = phdp.ProductId  AND phdp.HierarchyLevelId =2
		 INNER JOIN [Merchandising].[HierarchyTag] htdp WITH(NOLOCK) 	ON phdp.HierarchyTagId = htdp.id AND htdp.Name NOT IN ('FLOOR COVERINGS')
		 INNER JOIN [Merchandising].[ProductHierarchy] phc WITH(NOLOCK) ON p.ID = phc.ProductId  AND phc.HierarchyLevelId =3
		 INNER JOIN [Merchandising].[HierarchyTag] htc WITH(NOLOCK) ON phc.HierarchyTagId = htc.id AND htc.Name NOT IN ('AUDIO ACCESSORIES')
		 INNER JOIN agreement agr WITH(NOLOCK) ON d.acctno = agr.acctno AND d.agrmtno = agr.agrmtno
		 LEFT JOIN [Admin].[User] saleperson WITH(NOLOCK) ON saleperson.id = agr.empeenosale
		 INNER JOIN acct a WITH(NOLOCK) ON d.acctno = a.acctno
		 INNER JOIN Warranty.WarrantySale ws WITH(NOLOCK)  ON d.acctno = ws.CustomerAccount AND d.agrmtno = ws.AgreementNumber AND d.contractno = ws.WarrantyContractNo AND ISNULL(ws.WarrantyType, '') != 'F' 
		 INNER JOIN Merchandising.Location l  WITH(NOLOCK) ON d.BranchNo = l.SalesId
	WHERE d.delorcoll = 'C'  AND s.itemtype = 'S' AND contractno = '' AND s.Warrantable =1 AND ISNULL(a.accttype, '') != ''
		AND d.quantity >0 AND CONVERT(DATE, d.datetrans) < @DateFrom AND Not Exists (SELECT 1 FROM #RetrunedWarrantableBeforeDate r WHERE d.Acctno =r.Acctno AND d.ItemNo=r.Itemno )
	---------------------------------------------------------------------------------------------------------
	PRINT 'Get all warrantable items on the retrurned warranty account'
	SELECT abs(SUM(ReturnWarrantableQty)) As ReturnWarrantableQty,abs(Sum(ReturnWarrantableQtyValue)) As ReturnWarrantableQtyValue,Saleperson,SalespersonName,Fascia
	INTO #RetrunedWarrantable 
	FROM #RetrunedWarrantableBeforeDate
	GROUP BY Saleperson,SalespersonName,Fascia
	---------------------------------------------------------------------------------------------------------
	SELECT DISTINCT Saleperson,SalespersonName,Fascia
	INTO #SalesPerson FROM #AllWarrantyTrans
	-------------------------------------------------------------------------------------------------------------
	SELECT aw.Saleperson,aw.SalespersonName,aw.Fascia, SUM(ISNULL(sw.WarrantableQty,0) - ISNULL(rw.ReturnWarrantableQty,0)) As WarrantableQty ,
		SUM(ISNULL(sw.WarrantableQtyValue,0) - ISNULL(rw.ReturnWarrantableQtyValue,0)) As WarrantableQtyValue ,
		SUM(ISNULL(t.NoofWarrantySold,0)) As NoofWarrantySold,SUM(ISNULL(t.SoldWarrantyValue,0)) As SoldWarrantyValue ,SUM(ISNULL(tr.NoofReturnWarranty,0)) As NoofReturnWarranty ,
		SUM(ISNULL(tr.ReturnWarrantableValue,0)) As ReturnWarrantableValue,SUM((ISNULL(t.NoofWarrantySold,0) + ISNULL(tr.NoofReturnWarranty,0)) ) As NetWarrantySold,
		SUM((ISNULL(t.SoldWarrantyValue,0) + ISNULL(tr.ReturnWarrantableValue,0))) As NetWarrantyValue,
		SUM(ISNULL(t.NoofWarrantySold,0)) + SUM(ISNULL(tr.NoofReturnWarranty,0)) As 'Col1',SUM(ISNULL(sw.WarrantableQty,0) - ISNULL(rw.ReturnWarrantableQty,0)) As 'Col2' 
	INTO #WHRSummary
	FROM  #SalesPerson aw
		LEFT JOIN #SoldWarrantable sw ON aw.Saleperson = sw.Saleperson AND aw.SalespersonName = sw.SalespersonName AND aw.Fascia=sw.Fascia
		LEFT Join #RetrunedWarrantable rw ON aw.Saleperson = rw.Saleperson AND aw.SalespersonName = rw.SalespersonName AND aw.Fascia=rw.Fascia
		LEFT JOIN #SoldWarranty t ON aw.Saleperson = t.Saleperson AND aw.SalespersonName = t.SalespersonName AND aw.Fascia=t.Fascia
		LEFT JOIN #ReturnedWarranty tr ON aw.Saleperson = tr.Saleperson AND aw.SalespersonName = tr.SalespersonName AND aw.Fascia=tr.Fascia
	GROUP BY aw.Saleperson,aw.SalespersonName,aw.Fascia
	-------------------------------------------------------------------------------------------------------------
	UPDATE #WHRSummary SET Col2 = 1 WHERE Col2 = 0
	--------------------------------------------------------------------------------------------------------
	print 'Test'
	SELECT Saleperson,SalespersonName,Fascia,
		WarrantableQty ,
		WarrantableQtyValue ,
		NoofWarrantySold ,
		SoldWarrantyValue ,
		NoofReturnWarranty ,
		ReturnWarrantableValue,
		NetWarrantySold,
		NetWarrantyValue,
		(CONVERT(DECIMAL(18,2), (Col1 ) / (Col2)  * 100) )	AS [Hit_Rate_Percent]
	INTO WHRSummary
	FROM #WHRSummary
	--------------------------------------------------------------------------------------------------------
	Print 'End ' + Convert(Varchar(25),Getdate(),120) 
	DECLARE @columnHeader VARCHAR(8000)
    
	SELECT @columnHeader = COALESCE(@columnHeader+',' ,'')+ ''''+column_name +'''' FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'WHRSummary'
	
	DECLARE @ColumnList VARCHAR(8000)
    --------------------------------------------------------------------------------------------------------
	SELECT @ColumnList = COALESCE(@ColumnList+',' ,'')+ 'CAST('+column_name +' AS VARCHAR)' FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'WHRSummary'
	--------------------------------------------------------------------------------------------------------
	SET @statement =  'bcp " SELECT '+ @columnHeader +' UNION ALL SELECT ' + @ColumnList +  ' FROM ' + @statement  + '.dbo.' + 'WHRSummary" queryout ' +  @filename + ' -t, -c -q -T ';	
    print @statement
    --------------------------------------------------------------------------------------------------------
	EXECUTE Master.dbo.xp_cmdshell @statement	
	--------------------------------------------------------------------------------------------------------
	Select * from dbo.WHRSummary
	----------------------------------------------------------------------------------------------------------
	DROP TABLE #RetrunedWarrantable,#RetrunedWarrantableBeforeDate,WHRSummary,#ReturnWarrantyParent,#SalesPerson,#WHRSummary,#SoldWarranty,#ReturnedWarranty,#SoldWarrantable,#AllWarrantyTrans,#AllWarrantyTransTest;
	 
END TRY
BEGIN CATCH
	DECLARE @err_msg VARCHAR(MAX)                                
	SELECT @err_msg =                               
			   'Procedure ' + CONVERT(VARCHAR(50),ERROR_PROCEDURE()) +                                
			   ', Error ' + CONVERT(VARCHAR(50), ERROR_NUMBER()) +                                
			   ', Severity ' + CONVERT(VARCHAR(5), ERROR_SEVERITY()) +                                
			   ', State ' + CONVERT(VARCHAR(5), ERROR_STATE()) +                                 
			   ', Line ' + CONVERT(VARCHAR(5), ERROR_LINE()) +                                 
			   ', ErrorMessage ' +  CONVERT(VARCHAR(8000), ERROR_MESSAGE()) 
	RAISERROR (@err_msg, 16, 1);      
END CATCH;
GO  

 