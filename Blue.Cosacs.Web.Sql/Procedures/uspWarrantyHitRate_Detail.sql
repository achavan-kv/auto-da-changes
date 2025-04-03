/****** Object:  StoredProcedure [dbo].[uspWarrantyHitRate_Detail]    Script Date: 10/3/2019 2:04:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF (OBJECT_ID('dbo.uspWarrantyHitRate_Detail') IS NOT NULL)
	DROP PROCEDURE dbo.uspWarrantyHitRate_Detail
GO
--exec uspWarrantyHitRate_Detail '20191023','20200131',1
-- ========================================================================
-- Author     :	<Zensar>
-- Create date: <01/07/2019>
-- Description:	<Calculate Warranty Hit Rate Peremage for Sales Persons>
-- Version    :	<008> 
-- ========================================================================
CREATE PROCEDURE [dbo].[uspWarrantyHitRate_Detail] 
@DateFrom   Date ,
@DateTo     Date,
@IncludeReprocessedCancelled Bit 
AS  
   BEGIN TRY

	DECLARE
		@BranchNumber                SmallInt = NULL,
		@StoreType                   Char(1) = NULL,
		@SalesPersonId               Int = NULL
	
	SET NOCOUNT ON
		DECLARE  @path varchar(1000) = 'd:\users\default'
		DECLARE @statement sqltext
		DECLARE @startDate Datetime,@filename varchar(1000)

	SET @startDate = GetDate()
	Set @statement =db_name()
	----------------------------------------------------------------------------------------------------------
	SELECT @filename = @path+ '\WHRDetail' +  replace(convert(varchar(10), @startDate, 120), '-', '') +'.csv' 
	----------------------------------------------------------------------------------------------------------
	IF OBJECT_ID('cosacs..WHRDetail','U') IS NOT NULL 
		DROP TABLE WHRDetail
	--------------------------------------------------------------------------------------------------------
	PRINT 'Get all Warranty Sales\Collection Accounts'
	SELECT DISTINCT d.acctno AS AccountNumber, saleperson.id AS Saleperson, saleperson.Fullname AS SalespersonName,(d.agrmtno) AS AgreementNumber,Fascia,d.Itemno
		INTO #AllWarrantyTrans
	FROM Delivery d WITH(NOLOCK)
		INNER JOIN Merchandising.Product p WITH(NOLOCK) ON d.ItemNo=p.SKU
		INNER JOIN [Merchandising].[ProductHierarchy] phv WITH(NOLOCK) ON p.ID = phv.ProductId AND phv.HierarchyLevelId =1
		INNER JOIN [Merchandising].[HierarchyTag] htv WITH(NOLOCK) ON phv.HierarchyTagId = htv.id AND htv.Name NOT IN ('OPTICAL','REPOSSESSED GOODS','DV FREE GIFT','SPARE PARTS')
		INNER JOIN [Merchandising].[ProductHierarchy] phdp WITH(NOLOCK) ON p.ID = phdp.ProductId AND phdp.HierarchyLevelId =2
		INNER JOIN [Merchandising].[HierarchyTag] htdp WITH(NOLOCK)  ON phdp.HierarchyTagId = htdp.id AND htdp.Name NOT IN ('FLOOR COVERINGS')
		INNER JOIN [Merchandising].[ProductHierarchy] phc WITH(NOLOCK) ON p.ID = phc.ProductId AND phc.HierarchyLevelId =3
		INNER JOIN [Merchandising].[HierarchyTag] htc WITH(NOLOCK) ON phc.HierarchyTagId = htc.id AND htc.Name NOT IN ('AUDIO ACCESSORIES')
		INNER JOIN agreement agr WITH(NOLOCK) ON d.acctno = agr.acctno AND d.agrmtno = agr.agrmtno
		LEFT JOIN [Admin].[User] saleperson WITH(NOLOCK) ON saleperson.id = agr.empeenosale
		INNER JOIN acct a WITH(NOLOCK) ON d.acctno = a.acctno
	 	INNER JOIN Merchandising.Location l  WITH(NOLOCK) ON d.BranchNo = l.SalesId
		INNER JOIN Stockinfo si WITH(NOLOCK) ON d.itemno=si.itemno
	WHERE ((d.delorcoll = 'D') OR (d.delorcoll = 'C' )  OR (d.delorcoll = 'R' )) AND ISNULL(a.accttype, '') != '' 
		 AND CONVERT(DATE, d.datetrans)BETWEEN @DateFrom AND @DateTo AND d.Itemno NOT IN  (SELECT SKU FROM Nonstocks.NonStock  WITH(NOLOCK))
	---------------------------------------------------------------------------------------------------------
	PRINT 'Get all Sold Warranties'
	SELECT DISTINCT d.ItemId, d.itemno, d.acctno AS AccountNumber,s.StockLocation,s.WarrantyContractNo,d.ParentItemID, d.agrmtno AS AgreementNumber, 
		CONVERT(DECIMAL(18, 2), s.WarrantySalePrice) AS WarrantySalePrice, CONVERT(DECIMAL(18, 2),	s.WarrantyCostPrice) AS WarrantyCostPrice,s.WarrantyId,d.datetrans,aws.Saleperson
	INTO #Warranties
	FROM Delivery d WITH(NOLOCK)
		INNER JOIN Stockinfo si WITH(NOLOCK) ON d.ItemID = si.Id
		INNER JOIN #AllWarrantyTrans aws On d.acctno = aws.AccountNumber AND d.agrmtno = aws.AgreementNumber
		INNER JOIN Warranty.WarrantySale s WITH(NOLOCK) ON d.acctno = s.CustomerAccount AND d.agrmtno = s.AgreementNumber AND d.contractno = s.WarrantyContractNo
			AND ISNULL(s.WarrantyType, '') != 'F' /*No point on free @Warranties*/
	WHERE d.delorcoll = 'D' AND contractno != '' AND si.itemtype != 's' AND CONVERT(DATE, d.datetrans) BETWEEN @DateFrom AND @DateTo AND  d.itemno Not Like '%M'
	Print 'Step 1'
	-----------------------------------------------------------------------------------------------------------
	PRINT 'Calculate Sold Warranty Prices'
	SELECT COUNT(*) AS TotalRcordCount,d.AccountNumber, d.ParentItemID, d.AgreementNumber,MAX(d.ItemId) AS ItemId,SUM(WarrantyCostPrice) AS WarrantyCostPrice,
		   SUM(WarrantySalePrice) AS WarrantySalePrice
	INTO #WarrantyPrices
	FROM #Warranties d  
		GROUP BY d.AccountNumber, d.ParentItemID, d.AgreementNumber
	----------------------------------------------------------------------------------------------------------
	PRINT 'Calculate Sold Warranty Comission'
	SELECT AcctNo, sc.ItemId, SUM(CommissionAmount) AS CommissionAmount
	INTO #WarrantyComission
	FROM SalesCommission sc WITH(NOLOCK)
		INNER JOIN #Warranties w ON w.WarrantyContractNo = sc.ContractNo
	GROUP BY AcctNo, sc.ItemId, StockLocn 
	----------------------------------------------------------------------------------------------------------
	SELECT DISTINCT  delorcoll, d.acctno, contractno,CONVERT(DATE, d.datetrans) AS datetrans,d.ItemID,d.Quantity,  d.agrmtno, d.branchno,d.ItemNo,d.ParentItemNo,
		 CONVERT(DECIMAL(18, 2), d.transvalue) AS Value,si.itemtype,si.Warrantable
		,CAST(htv.LevelId AS VARCHAR) + ' - ' + htv.Name AS [Division],CAST(htdp.LevelId AS VARCHAR) + ' - ' + htdp.Name AS [Department]
		,CAST(htc.LevelId AS VARCHAR) + ' - ' + htc.Name AS [Class],aws.Saleperson	AS [Sales_Person_Id],aws.SalespersonName  AS [Sold_By]
		,CASE WHEN b.StoreType = 'C' THEN 'COURTS' ELSE 'NON COURTS (Tropigas/Lucky Dollar)' END AS [Store_Type]
		,CASE WHEN a.accttype = 'S' AND ca.acctno IS NOT NULL THEN 'POS' WHEN a.accttype IN ('B', 'M', 'O', 'R') THEN 'Credit'
			  WHEN a.accttype = 'C' THEN 'Cash' WHEN a.accttype = 'G' THEN 'POS'  ELSE 'POS'END AS [Account_Type]
	INTO #d 
	FROM Delivery d WITH(NOLOCK)
		INNER JOIN StockInfo si WITH(NOLOCK) ON d.ItemID = si.Id 
		INNER JOIN Merchandising.Product p WITH(NOLOCK) ON d.ItemNo=p.SKU
		INNER JOIN [Merchandising].[ProductHierarchy] phv WITH(NOLOCK) ON p.ID = phv.ProductId  AND phv.HierarchyLevelId =1
		INNER JOIN [Merchandising].[HierarchyTag] htv WITH(NOLOCK) ON phv.HierarchyTagId = htv.id AND htv.Name NOT IN ('OPTICAL','REPOSSESSED GOODS','DV FREE GIFT','SPARE PARTS')
		INNER JOIN [Merchandising].[ProductHierarchy] phdp WITH(NOLOCK) ON p.ID = phdp.ProductId  AND phdp.HierarchyLevelId =2
		INNER JOIN [Merchandising].[HierarchyTag] htdp WITH(NOLOCK) ON phdp.HierarchyTagId = htdp.id AND htdp.Name NOT IN ('FLOOR COVERINGS')
		INNER JOIN [Merchandising].[ProductHierarchy] phc WITH(NOLOCK) ON p.ID = phc.ProductId  AND phc.HierarchyLevelId =3
		INNER JOIN [Merchandising].[HierarchyTag] htc WITH(NOLOCK) ON phc.HierarchyTagId = htc.id AND htc.Name NOT IN ('AUDIO ACCESSORIES')
		INNER JOIN branch b WITH(NOLOCK) ON d.branchno = b.branchno
		INNER JOIN acct a WITH(NOLOCK) ON d.acctno = a.acctno
		LEFT JOIN custacct ca WITH(NOLOCK) ON d.acctno = ca.acctno AND ca.custid = 'PAID & TAKEN'
		INNER JOIN #AllWarrantyTrans aws On d.acctno = aws.AccountNumber AND d.agrmtno = aws.AgreementNumber
	WHERE CONVERT(DATE, d.datetrans) BETWEEN @DateFrom AND @DateTo AND si.itemtype='S' And delorcoll='D' AND contractno = ''  AND ISNULL(a.accttype, '') != ''
		AND d.Itemno NOT IN  (SELECT SKU FROM Nonstocks.NonStock  WITH(NOLOCK))
	----------------------------------------------------------------------------------------------------------
	SELECT DISTINCT d.ItemID,d.acctno As AccountNumber,d.quantity As WarrantableQty,CONVERT(DECIMAL(18, 2), d.transvalue) AS WarrantableQtyValue,
		   aws.Saleperson AS Saleperson,aws.SalespersonName AS [Sold_By],d.agrmtno AS AgreementNumber,d.itemno
	INTO #Warrantable
	FROM Delivery d WITH(NOLOCK)
		INNER JOIN StockInfo si WITH(NOLOCK) ON d.ItemID = si.Id
		INNER JOIN Merchandising.Product p WITH(NOLOCK) ON d.ItemNo=p.SKU
		INNER JOIN [Merchandising].[ProductHierarchy] phv WITH(NOLOCK)	ON p.ID = phv.ProductId AND phv.HierarchyLevelId =1
		INNER JOIN [Merchandising].[HierarchyTag] htv WITH(NOLOCK)	ON phv.HierarchyTagId = htv.id AND htv.Name NOT IN ('OPTICAL','REPOSSESSED GOODS','DV FREE GIFT','SPARE PARTS')
		INNER JOIN [Merchandising].[ProductHierarchy] phdp WITH(NOLOCK)	ON p.ID = phdp.ProductId AND phdp.HierarchyLevelId =2
		INNER JOIN [Merchandising].[HierarchyTag] htdp WITH(NOLOCK)	ON phdp.HierarchyTagId = htdp.id AND htdp.Name NOT IN ('FLOOR COVERINGS')
		INNER JOIN [Merchandising].[ProductHierarchy] phc WITH(NOLOCK) ON p.ID = phc.ProductId AND phc.HierarchyLevelId =3
		INNER JOIN [Merchandising].[HierarchyTag] htc WITH(NOLOCK)	ON phc.HierarchyTagId = htc.id 	AND htc.Name NOT IN ('AUDIO ACCESSORIES')
		INNER JOIN #AllWarrantyTrans aws On d.acctno = aws.AccountNumber	AND d.agrmtno = aws.AgreementNumber
		INNER JOIN acct a WITH(NOLOCK)  ON d.acctno = a.acctno
	WHERE d.delorcoll = 'D'  AND si.itemtype = 'S' AND si.Warrantable =1 AND ISNULL(a.accttype, '') != '' AND CONVERT(DATE, d.datetrans) BETWEEN @DateFrom AND @DateTo
		AND d.quantity >0 AND d.Itemno NOT IN  (SELECT SKU FROM Nonstocks.NonStock  WITH(NOLOCK))
	----------------------------------------------------------------------------------------------------------
	Print 'Start ' + Convert(Varchar(25),Getdate(),120) 
	SELECT DISTINCT	d.acctno AS AccountNumber, d.ParentItemID,(d.agrmtno) AS AgreementNumber,Sum( CONVERT(DECIMAL(18, 2), s.WarrantySalePrice)) AS ReturnWarrantySalePrice, 
		Sum(CONVERT(DECIMAL(18, 2), s.WarrantyCostPrice)) AS ReturnWarrantyCostPrice,Sum(d.quantity) As ReturnWarrantyQty
	INTO #ReturnedWarranties
	FROM Delivery d WITH(NOLOCK)
		INNER JOIN StockInfo si WITH(NOLOCK)ON d.ItemID = si.Id
		INNER JOIN #AllWarrantyTrans aws On d.acctno = aws.AccountNumber AND d.agrmtno = aws.AgreementNumber
		INNER JOIN Warranty.WarrantySale s  WITH(NOLOCK) ON d.acctno = s.CustomerAccount AND d.agrmtno = s.AgreementNumber	AND d.contractno = s.WarrantyContractNo
			AND ISNULL(s.WarrantyType, '') != 'F' /*No point on free @Warranties*/
	WHERE ((d.delorcoll = 'C' AND contractno != '')) AND si.itemtype != 's' AND 1 = @IncludeReprocessedCancelled AND CONVERT(DATE, d.datetrans) BETWEEN @DateFrom AND @DateTo
	GROUP BY d.acctno, d.ParentItemID, d.agrmtno
	----------------------------------------------------------------------------------------------------------
	Print 'End ' + Convert(Varchar(25),Getdate(),120) 
	SELECT DISTINCT	d.delorcoll, d.itemno, d.acctno AS AccountNumber,  d.branchno as branchno,d.ParentItemID,(d.agrmtno) AS AgreementNumber,CONVERT(DECIMAL(18, 2), s.WarrantySalePrice) AS ReturnWarrantySalePrice, 
		CONVERT(DECIMAL(18, 2), s.WarrantyCostPrice) AS ReturnWarrantyCostPrice,s.WarrantyId,d.datetrans,si.category,d.quantity As ReturnWarrantyQty,aws.Saleperson
	INTO #RepossedWarranties
	FROM Delivery d WITH(NOLOCK)
		INNER JOIN StockInfo si WITH(NOLOCK) ON d.ItemID = si.Id
		INNER JOIN #AllWarrantyTrans aws	On d.acctno = aws.AccountNumber	AND d.agrmtno = aws.AgreementNumber
		INNER JOIN Warranty.WarrantySale s  WITH(NOLOCK) ON d.acctno = s.CustomerAccount AND d.agrmtno = s.AgreementNumber AND d.contractno = s.WarrantyContractNo
			AND ISNULL(s.WarrantyType, '') != 'F' /*No point on free @Warranties*/
	WHERE ((d.delorcoll = 'R' AND contractno != '')) AND si.itemtype != 's' AND 1 = @IncludeReprocessedCancelled AND CONVERT(DATE, d.datetrans) BETWEEN @DateFrom AND @DateTo
	-----------------------------------------------------------------------------------------------------------
	SELECT Distinct	d.ItemId AS ItemId,	d.acctno AS AccountNumber, d.branchno as branchno,Sum(d.quantity) AS Quantity,CONVERT(DECIMAL(18, 2), Sum(d.transvalue)) AS Value,
			(d.agrmtno) As AgreementNumber
	INTO #ReturnedProducts
	FROM Delivery d WITH(NOLOCK)
		INNER JOIN StockInfo si WITH(NOLOCK) ON d.ItemID = si.Id
		INNER JOIN #AllWarrantyTrans aws On d.acctno = aws.AccountNumber AND d.agrmtno = aws.AgreementNumber
		INNER JOIN #ReturnedWarranties rw ON d.acctno = rw.AccountNumber AND d.agrmtno = rw.AgreementNumber	AND d.ItemId=rw.ParentItemID
	WHERE d.delorcoll = 'C' AND si.itemtype = 's'	AND 1 = @IncludeReprocessedCancelled AND CONVERT(DATE, d.datetrans) BETWEEN @DateFrom AND @DateTo
	Group By d.ItemId,d.acctno,d.branchno,d.agrmtno
	----------------------------------------------------------------------------------------------------------
	;With PotentialSale(AccountNumber, AgreementNumber, ItemId, WarrantyPotentialSalePrice) AS
	(
		SELECT CustomerAccount, AgreementNumber, ItemId, CONVERT(DECIMAL(18, 2), MAX(p.WarrantyRetailPrice)) AS WarrantyPotentialSalePrice
		FROM Warranty.WarrantyPotentialSale p
		WHERE  	ISNULL(p.WarrantyType, '') != 'F'AND p.SoldOn BETWEEN @DateFrom AND @DateTo 
		GROUP BY CustomerAccount, AgreementNumber, ItemId
	)
	SELECT Distinct	d.branchno AS [Branch_Number],d.acctno AS  AccountNumber,d.[Account_Type],d.[Division],	d.[Department],	d.[Class],d.[Sales_Person_Id],d.[Sold_By],
		d.[Store_Type],	d.ItemNo,d.ParentItemNo,Isnull(d.Quantity,0)+ (ISNULL(rp.Quantity, 0)) AS [No_Product_Sales],
        Isnull(d.Value,0) + (ISNULL(rp.Value, 0)) AS [Value_of_Product_Sales],ISNULL(w.TotalRcordCount, 0) + ISNULL(r.ReturnWarrantyQty,0) AS [No_Warranty_Sales],
		Isnull(w.WarrantyCostPrice,0) - (ISNULL(r.ReturnWarrantyCostPrice, 0)) AS [Cost_of_warranty_sales],
        Isnull(w.WarrantySalePrice,0) - (ISNULL(r.ReturnWarrantySalePrice, 0)) AS [Value_of_warranty_sales],
		(ISNULL(tw.WarrantableQty, 0)) AS [Number_of_warrantable_sales],(ISNULL(tw.WarrantableQtyValue, 0)) AS [Value_of_warrantable_sales],						
		Isnull(PS.WarrantyPotentialSalePrice,0) AS [Potential_value_of_warranty_sales],	ISNULL(r.ReturnWarrantyQty,0) AS [Number_of_warranties_cancelled],
		ISNULL(r.ReturnWarrantySalePrice,0) AS [Value_of_warranties_cancelled],	ISNULL(rw.ReturnWarrantyQty,0) 	AS [Number_of_warranties_repossessed],
		ISNULL(rw.ReturnWarrantySalePrice,0) AS [Value_of_warranties_repossessed],(CONVERT(DECIMAL(18,2), sc.CommissionAmount)) AS [Commission_paid_to_sales_staff]
	INTO WHRDetailTest
	FROM #d d
	LEFT JOIN #ReturnedProducts rp	ON d.acctno = rp.AccountNumber	AND d.agrmtno = rp.AgreementNumber	AND d.ItemId = rp.ItemId AND d.branchno = rp.branchno 
	LEFT JOIN #WarrantyPrices w 	ON d.acctno = w.AccountNumber	AND d.ItemId = w.ParentItemID AND d.agrmtno = w.AgreementNumber
	LEFT JOIN #ReturnedWarranties r	ON d.acctno = r.AccountNumber	AND d.agrmtno = r.AgreementNumber	
	LEFT JOIN #RepossedWarranties rw ON d.acctno = rw.AccountNumber	AND d.branchno = rw.branchno AND d.agrmtno = rw.AgreementNumber	
	LEFT JOIN #WarrantyComission sc ON w.AccountNumber = sc.AcctNo
	LEFT JOIN PotentialSale ps 	ON ps.AccountNumber  = d.acctno	AND ps.AgreementNumber = d.agrmtno
	LEFT JOIN #Warrantable tw ON d.acctno = tw.AccountNumber AND d.[Sales_Person_Id]=tw.Saleperson AND d.agrmtno=tw.AgreementNumber
	WHERE ISNULL(d.[Store_Type], '') = CASE	WHEN @StoreType IS NULL THEN ISNULL(d.[Store_Type], '')	ELSE @StoreType	  END
      AND ISNULL(d.[Sales_Person_Id], 0) = CASE	WHEN @SalesPersonId IS NULL THEN ISNULL(d.[Sales_Person_Id], 0)	ELSE @SalesPersonId	END
	  AND d.branchno = CASE WHEN @BranchNumber IS NULL THEN d.branchno ELSE @BranchNumber END
	ORDER BY  d.branchno, d.[Sales_Person_Id]
	----------------------------------------------------------------------------------------------------------
	SELECT * INTO WHRDetail FROM WHRDetailTest 
	EXCEPT
	SELECT w.* FROM WHRDetailTest w
		INNER JOIN hierarchyexclusion h ON w.Division = h.Division AND w.Department = h.Department AND w.Class = h.Class 
 	---------------------------------------------------------------------------------------------------------
	DECLARE @columnHeader VARCHAR(8000)
    SELECT @columnHeader = COALESCE(@columnHeader+',' ,'')+ ''''+column_name +'''' FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'WHRDetail'
	----------------------------------------------------------------------------------------------------------
    DECLARE @ColumnList VARCHAR(8000)
    SELECT @ColumnList = COALESCE(@ColumnList+',' ,'')+ 'CAST('+column_name +' AS VARCHAR)' FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'WHRDetail'
	----------------------------------------------------------------------------------------------------------

	SET @statement =  'bcp " SELECT '+ @columnHeader +' UNION ALL SELECT ' + @ColumnList + ' FROM ' + @statement + '.dbo.' + 'WHRDetail" queryout ' +  @filename + ' -t, -c -q -T ';	
	 ----------------------------------------------------------------------------------------------------------    
	Print @statement
    EXECUTE Master.dbo.xp_cmdshell @statement	
	----------------------------------------------------------------------------------------------------------
	Select * from WHRDetail 
	----------------------------------------------------------------------------------------------------------
	DROP TABLE #d,#Warranties,#WarrantyPrices,#WarrantyComission,#ReturnedWarranties,#Warrantable,WHRDetailTest,#RepossedWarranties,WHRDetail;
	----------------------------------------------------------------------------------------------------------
	
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
