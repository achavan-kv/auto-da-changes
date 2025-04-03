/***********************************************************************************************************

-- CR#	Fascia Omni changes for Online Product Export Job
--									1)Available Stock calculation current rule IS fascia = Courts, need this changed to make the rule = Omni
--									2) Pricing will be FROM fascia = Omni
--	All the existing ecommerce functionalities should  work for the Curacavo islands.  Some examples include:
--	1) Menu name Online Product Maintenance to be loaded WITH all the relevant product catalogue data.
--	1a) Column name 'Available Stock' to be populated WITH correct data
--	1b) Column names Cash Price, Cash Promotional Price etc to be loaded WITH correct data.
--	2) EOD process to produce correct ecom file WITH correct ISO country letter for the island etc.

************************************************************************************************************/
IF  EXISTS (SELECT 1 FROM sysobjects WHERE name = 'OnlineProductExportSP' AND type in ('P', 'PC'))
DROP PROCEDURE [dbo].[OnlineProductExportSP]

GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[OnlineProductExportSP]
/***********************************************************************************************************
--
-- Project      : CoSACS .NET
-- File Name    : OnlineProductExportSP.SQL
-- File Type    : MSSQL Server Stored Procedure Script
-- Description  : Export Online Products
--				
-- Author       : ?
-- Date         : ?
-- Version 	    : 003
--  
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 26/02/14  IP  #17519 - Ecommerce file does not include the new warranties FROM warranty module
-- 28/02/14  IP  #17519 - When calculating Promotion prices for warranty, use either Retail Price OR Percentage Discount 
--						  FROM Warranty.WarrantyPromotion.
-- 20/05/14  jec #18564 - Prices missing for warranties
-- 10/06/20		Amit Vernekar   #6975935 - Include warranties based on Country Maintenance parameter 'Include Warranties In Online Product Export'
-- 18/06/2020	Sachin Wandhare	CR#	Fascia Omni changes for Online Product Export Job
--									1)Available Stock calculation current rule IS fascia = Courts, need this changed to make the rule = Omni
--									2) Pricing will be FROM fascia = Omni
-- 07/01/2020	Ritesh Joge	CR#	Fascia Courts changes for Online Product Export Job
--									1) Available Stock calculation for uncheck stock qty for DC only current rule is fascia = Courts, 
--									   VirtualWarehouse = false, Active = true
--									2) Pricing will be FROM fascia = Courts
-- 25/10/2020  Snehalata Tilekar    Added 'And' condition on promotion CTE table to get data as per stock location as well
************************************************************************************************************/
	
AS
BEGIN

	SET NOCOUNT ON
	DECLARE @stocklocn SMALLINT
			,@scoringband CHAR(1)
			,@oltermstype VARCHAR(3)
			,@oltermslength SMALLINT
			,@taxtype CHAR(1)
			,@taxrate FLOAT
			,@fascia VARCHAR(100)

	SELECT  @stocklocn= value FROM CountryMaintenance WHERE codename='OnlineDistCentre'
	--SELECT  @scoringband= value FROM CountryMaintenance WHERE codename='OnlineSBandIntRate'
	--SELECT  @oltermstype= value FROM CountryMaintenance WHERE codename='OnlineTermsType'
	--SELECT  @oltermslength= value FROM CountryMaintenance WHERE codename='OnlineTermsLength'
	SELECT  @taxtype= value FROM CountryMaintenance WHERE codename='taxtype'
	SELECT  @taxrate= value FROM CountryMaintenance WHERE codename='taxrate'
			
	;WITH promotion AS
		( SELECT p.fromdate, p.todate, p.unitprice, p.itemid, p.StockLocn
			FROM promoprice p
			WHERE p.fromdate = (SELECT MAX(p2.fromdate) FROM promoprice p2 WHERE p.itemid=p2.itemid AND p.hporcash=p2.hporcash AND p.hporcash='C' AND p.stocklocn = p2.stocklocn)
					AND GETDATE() BETWEEN p.fromdate AND p.todate
		), promotionHP AS
		( SELECT p.fromdate, p.todate, p.unitprice, p.itemid, p.StockLocn
			FROM promoprice p
			WHERE p.fromdate = (SELECT MAX(p2.fromdate) FROM promoprice p2 WHERE p.itemid=p2.itemid AND p.hporcash=p2.hporcash AND p.hporcash='H' AND p.stocklocn = p2.stocklocn)
					AND GETDATE() BETWEEN p.fromdate AND p.todate
		)

	SELECT  ISOCountryCode
			,UPPER(ISNULL(IUPC,'IUPC Missing')) AS 'Product Code' 
			,REPLACE(ItemDescr1,',',' ') AS 'Desc 01'
			,REPLACE(ItemDescr2,',',' ')  AS 'Desc 02'			
			,CAST(SP.CashPrice AS DECIMAL (9,2)) AS 'CashPriceInclTax'
			--CAST(0 AS DECIMAL(9,2)) AS 'Weekly Price',
			,CASE WHEN p.unitprice IS NOT NULL THEN CAST(CASE WHEN @taxtype='E' THEN p.unitprice * (1 + @taxrate/100) ELSE p.unitprice END AS DECIMAL(9,2)) ELSE NULL END AS 'PromoCashPriceInclTax'
			,p.fromdate AS 'PromoStartDate'
			,p.todate AS 'PromoEndDate'
			,CAST(NULL AS int) AS 'StockQty'
			,Case WHEN Itemtype ='S' THEN '3' ELSE '6' END AS 'Product Type'
			,Case WHEN Itemtype ='S' THEN '1' ELSE '0' END AS 'MagMgStock'			
			,si.Id AS ItemId
			,sp.Refcode
			,CAST(pHP.unitprice AS DECIMAL(9,2)) AS 'HPPromoPrice'
			,ISNULL(si.OnlineDConly,0) AS [DC Only]
			--,si.category,si.class
	
	INTO	#stock				
	FROM	StockInfo SI INNER JOIN StockPrice SP ON SI.ID = SP.ID AND SP.BranchNo = @stocklocn
					INNER JOIN StockQuantity SQ ON  SI.ID = SQ.ID AND SQ.StockLocn = @stocklocn
					LEFT OUTER JOIN promotion p ON si.id=p.itemid AND p.StockLocn = @stocklocn
					LEFT OUTER JOIN promotionHP pHP ON si.id=pHP.itemid  AND pHP.StockLocn = @stocklocn,
					Country c		
	WHERE CONVERT(VARCHAR,SI.Category) NOT IN (SELECT code FROM dbo.code WHERE category in ('WAR')) AND		-- removed 'PCDIS'
	      (SP.DateActivated IS NULL OR SP.DateActivated < GETDATE())
			AND si.prodstatus NOT IN ('D','R')		
			--AND (si.category=@category OR @category=0)
			AND ISNULL(OnlineAvailable,0)=1 					
			AND SQ.Deleted != 'Y'
			--AND not exists(SELECT * FROM kitproduct k WHERE k.ItemID=si.id)		-- not a kit
			AND si.category NOT IN (15,85)				-- not second hand/repossessed			
	GROUP BY ISOCountryCode,IUPC, SI.itemtype,  ItemDescr1, ItemDescr2, SQ.Stock, SQ.QtyAvailable,	SQ.Deleted, SP.CashPrice,p.fromdate,p.todate,si.Id, p.unitprice,sp.Refcode,pHP.unitprice,OnlineDConly
			--	,si.category,si.class



	--#17519 - Select warranties linked to items		
	INSERT INTO #stock 
	SELECT DISTINCT ISOCountryCode
		   ,w.Number AS 'Product Code'
		   ,REPLACE(w.[Description],',',' ') AS 'Desc 01'
		   ,'' AS  'Desc 02'
		   ,CAST(0 AS DECIMAL (9,2)) AS 'CashPriceInclTax'
		   ,NULL  AS 'PromoCashPriceInclTax'
		   ,NULL AS 'PromoStartDate'
		   ,NULL AS 'PromoEndDate'
		   ,CAST(NULL AS int) AS 'StockQty'
		   ,'2' AS 'Product Type'
		   ,'0' AS 'MagMgStock'
		   ,w.Id AS ItemId 
		   ,ISNULL(lp.Refcode, '') AS 'refcode'
		   ,CAST(0 AS DECIMAL(9,2))
		   ,0
	FROM  #stock s, stockinfo si,code dep,
	warranty.LinkProduct lp INNER JOIN warranty.Link l ON l.Id = lp.LinkId
	INNER JOIN warranty.LinkWarranty lw ON l.Id = lw.LinkId
	INNER JOIN warranty.warranty w ON w.Id = lw.WarrantyId
	WHERE s.ItemId = si.Id 
	AND si.category = dep.code
	AND dep.category in ('PCE', 'PCF', 'PCO', 'PCW')
	AND CONVERT(VARCHAR,SI.Category) NOT IN (SELECT code FROM dbo.code WHERE category in ('WAR'))
	AND (
			(lp.ItemNumber = s.[Product Code] OR (lp.ItemNumber IS NULL OR lp.ItemNumber = 'ALL')) AND
			(lp.Level_1 = dep.category OR lp.Level_1 IS NULL) AND
			(lp.Level_2 = si.category OR lp.Level_2 IS NULL) AND
			(lp.Level_3 = si.Class OR lp.Level_3 IS NULL) AND
			(lp.RefCode = s.Refcode OR lp.RefCode IS NULL)
		)
	AND w.Deleted = 0 
	--AND w.Free = 0
	AND w.typecode!='F'
 
	-- UPDATE Stock available

	SET @fascia = CASE WHEN EXISTS(SELECT 1 FROM Country WHERE CountryCode = 'Q') THEN 'OMNI' ELSE 'Courts' END
	
	;WITH alllocations AS
	( SELECT itemid,SUM(sq.qtyAvailable) AS AvailStock
		FROM #stock s INNER JOIN stockquantity sq ON s.itemid=sq.id
						INNER JOIN branch b ON sq.stocklocn=b.branchno  
						INNER JOIN Merchandising.Location L ON L.SalesId = b.BranchNo 
			WHERE L.Fascia = @fascia AND L.Active = 1 AND L.VirtualWarehouse = 0
		GROUP BY itemid	
	) 
	
	UPDATE #stock
		SET [StockQty]= CASE WHEN s.[DC Only]=0 THEN a.AvailStock ELSE sq.qtyAvailable END
	FROM #stock s INNER JOIN alllocations a ON s.itemid=a.ItemId
			INNER JOIN stockquantity sq ON s.itemid=sq.id AND sq.stocklocn=@stocklocn
	WHERE s.[Product Type]='3'
	 
	
	UPDATE #stock SET		
		--SET [Weekly Price]= ((CASE WHEN @taxtype='E' THEN CASE WHEN HPPromoPrice IS NULL THEN sp.CreditPrice ELSE HPPromoPrice END * (1 + @taxrate/100)
		--						ELSE CASE WHEN HPPromoPrice IS NULL THEN sp.CreditPrice ELSE HPPromoPrice END END 
		--				-CASE WHEN CASE WHEN HPPromoPrice IS NULL THEN sp.CreditPrice ELSE HPPromoPrice END<tt.DefaultDeposit THEN 0 ELSE tt.DefaultDeposit END) * 
		--						(1 + (tt.ServPcent/100 * @oltermslength/12)) )/(@oltermslength/12 * 52),
			[CashPriceInclTax]= CASE WHEN @taxtype='E' THEN sp.CashPrice * (1 + @taxrate/100) ELSE sp.CashPrice END
			
	FROM stockprice sp 
	INNER JOIN #stock s ON sp.ID=s.ItemId	
	WHERE sp.branchno = @stocklocn
	AND s.[Product Type]!='2'				--#17519
	--,



	--		termstypeallbands tt
	--WHERE tt.TermsType=@oltermstype AND tt.Band=@scoringband

	-- #14487 Special Tax rates
	;WITH promotion AS
		( SELECT p.unitprice,p.itemid,p.StockLocn 
			FROM promoprice p
			WHERE p.fromdate = (SELECT MAX(p2.fromdate) FROM promoprice p2 WHERE p.itemid=p2.itemid AND p.hporcash=p2.hporcash AND p.hporcash='C' AND p.stocklocn=p2.stocklocn)
					AND GETDATE() BETWEEN p.fromdate AND p.todate
		)	
	UPDATE #stock 
		SET	[CashPriceInclTax]= CASE WHEN @taxtype='E' THEN sp.CashPrice * (1 + CAST(c.codedescript AS FLOAT)/100) ELSE sp.CashPrice END,
			PromoCashPriceInclTax = CASE WHEN p.unitprice IS NOT NULL THEN CASE WHEN @taxtype='E' THEN p.unitprice * (1 + CAST(c.codedescript AS FLOAT)/100) ELSE p.unitprice END ELSE NULL END 
	FROM #stock s INNER JOIN StockInfo si ON s.ItemId=si.id 
				  INNER JOIN stockprice sp ON s.ItemId=sp.id
				  INNER JOIN code c ON si.iupc=c.code AND c.category='TXR'
				  LEFT OUTER JOIN promotion p ON si.id=p.itemid AND p.StockLocn = @stocklocn 
				  WHERE sp.branchno = @stocklocn
				  AND s.[Product Type]!='2'			--#17519


	--#17519 - Update CashPriceInclTax for warranties
	;WITH WarrantyRetailPrice AS 
	(
		SELECT wp.WarrantyId, wp.BranchNumber, MAX(wp.RetailPrice) AS RetailPrice, w.TaxRate
		FROM warranty.WarrantyPrice wp
		INNER JOIN warranty.warranty w ON wp.WarrantyId = w.Id
		INNER JOIN #stock s ON s.ItemId = wp.WarrantyId 
		WHERE wp.EffectiveDate <= GETDATE()
		GROUP BY  wp.WarrantyId, wp.BranchNumber, w.TaxRate
	)

	UPDATE #stock
	SET [CashPriceInclTax]= CASE WHEN @taxtype='E' THEN wp.RetailPrice * (1 + wp.TaxRate/100) ELSE wp.RetailPrice END
	FROM #stock s INNER JOIN WarrantyRetailPrice wp ON s.ItemId = wp.WarrantyId
	WHERE (wp.BranchNumber = @stocklocn OR wp.BranchNumber IS NULL)				-- #18564
	AND s.[Product Type]='2'

	--#17519 - Update PromoCashPriceInclTax for warranties
	;WITH WarrantyPromotion AS
	(
		SELECT wpp.WarrantyId, wpp.BranchNumber, wpp.RetailPrice, wpp.PercentageDiscount, wpp.StartDate, wpp.EndDate, w.TaxRate
		FROM warranty.WarrantyPromotion wpp INNER JOIN #stock s ON wpp.WarrantyId = s.ItemId
		INNER JOIN Warranty.Warranty w ON wpp.WarrantyId = w.Id
		WHERE wpp.StartDate = (SELECT MAX(wpp2.StartDate) FROM warranty.WarrantyPromotion wpp2 WHERE wpp2.WarrantyId = wpp.WarrantyId)
		AND GETDATE() BETWEEN wpp.StartDate AND wpp.EndDate
	)

	UPDATE #stock
	SET PromoCashPriceInclTax = CASE WHEN @taxtype='E' AND ISNULL(wpp.RetailPrice,0) != 0 THEN wpp.RetailPrice * (1 + wpp.TaxRate/100)
							         WHEN @taxtype='E'AND ISNULL(wpp.PercentageDiscount,0) != 0 THEN (s.CashPriceInclTax - (s.CashPriceInclTax * (wpp.PercentageDiscount/100)))  --#17519
									 ELSE wpp.RetailPrice END,
		PromoStartDate = wpp.StartDate,
		PromoEndDate = wpp.EndDate
	FROM #stock s INNER JOIN WarrantyPromotion wpp ON s.ItemId = wpp.WarrantyId
	AND (wpp.BranchNumber = @stocklocn OR wpp.BranchNumber IS NULL)				-- #18564
	AND s.[Product Type]='2'

	UPDATE #stock
			SET [PromoCashPriceInclTax]= NULL,[PromoStartDate]=NULL,[PromoEndDate]=NULL
	WHERE [PromoEndDate]<GETDATE()

	
	ALTER TABLE #stock DROP COLUMN Itemid
	ALTER TABLE #stock DROP COLUMN RefCode
	ALTER TABLE #stock DROP COLUMN HPPromoPrice
	ALTER TABLE #stock DROP COLUMN [Product Type]
	ALTER TABLE #stock DROP COLUMN [DC Only]
	--ALTER TABLE #stock DROP COLUMN category
	--ALTER TABLE #stock DROP COLUMN class

	DECLARE @IncludeWarranties BIT = 1

	IF  EXISTS (SELECT * FROM [CountryMaintenance] WITH(NOLOCK) WHERE CodeName='IncWarrantyInOnlineProdExport')
	BEGIN
		SELECT @IncludeWarranties = CAST(ISNULL(value, 1) AS BIT) from CountryMaintenance where CodeName='IncWarrantyInOnlineProdExport'	
	END


	-- When Include warranty is True then we need to select all the  records from #stock.
	IF @IncludeWarranties = 1
	BEGIN

		SELECT DISTINCT [ISOCountryCode],[Product Code],[Desc 01],[Desc 02],[CashPriceInclTax],[PromoCashPriceInclTax],[PromoStartDate],[PromoEndDate],[StockQty],
		[MagMgStock] 
		FROM		#stock 
		ORDER BY	[Product Code]

	END 
	ELSE 
	BEGIN
		
		SELECT DISTINCT [ISOCountryCode],[Product Code],[Desc 01],[Desc 02],[CashPriceInclTax],[PromoCashPriceInclTax],[PromoStartDate],[PromoEndDate],[StockQty],
		[MagMgStock]
		FROM		#stock 
		WHERE		[Product Code]  NOT IN (SELECT Number FROM [Warranty].[Warranty])
		ORDER BY	[Product Code]

	END


	SET	ROWCOUNT 0	
	DROP TABLE #stock

END
GO