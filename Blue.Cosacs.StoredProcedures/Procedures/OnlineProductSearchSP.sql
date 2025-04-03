/***********************************************************************************************************

-- CR#	Fascia Omni changes for Online Product Export Job
--									1)Available Stock calculation current rule is fascia = Courts, need this changed to make the rule = Omni
--									2) Pricing will be FROM fascia = Omni
--	All the existing ecommerce functionalities should  work for the Curacavo islands.  Some examples include:
--	1) Menu name Online Product Maintenance to be loaded WITH all the relevant product catalogue data.
--	1a) Column name 'Available Stock' to be populated WITH correct data
--	1b) Column names Cash Price, Cash Promotional Price etc to be loaded WITH correct data.
--	2) EOD process to produce correct ecom file WITH correct ISO country letter for the island etc.

************************************************************************************************************/
IF  EXISTS (SELECT 1 FROM sysobjects WHERE name = 'OnlineProductSearchSP' AND type IN ('P', 'PC'))
DROP PROCEDURE [dbo].[OnlineProductSearchSP]

GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE 	[dbo].[OnlineProductSearchSP]
/***********************************************************************************************************
--
-- Project      : CoSACS .NET
-- File Name    : OnlineProductSearchSP.SQL
-- File Type    : MSSQL Server Stored Procedure Script
-- Description  : Retrieve Online Products
--				
-- Author       : John Croft
-- Date         : 01 July 2013
-- Version 	    : 003
--  
-- Change Control
-- --------------
-- Date			By					Description
-- ----			--					-----------
--18 June 2020	Sachin Wandhare		CR# Fascia Omni changes for Product Maintenance
--									1)Available Stock calculation current rule is fascia = Courts, need this changed to make the rule = Omni
--									2) Pricing will be FROM fascia = Omni
--
--30 June 2020	Ritesh Joge     	CR# Fascia Courts changes for Product Maintenance
--									1) Available Stock calculation for uncheck stock qty for DC only current rule is fascia = Courts, 
--									   VirtualWarehouse = false, Active = true
--									2) Pricing will be FROM fascia = Courts
--
--25 Nov 2020  Snehalata Tilekar    Added 'And' condition on promotion CTE table to get data as per stock location as well
************************************************************************************************************/
			 @location VARCHAR(3)
			,@category SMALLINT
			,@online CHAR(1)
			,@dateAdded DATETIME
			,@dateRemoved DATETIME
			,@prodDesc NVARCHAR(80)
			,@limit INT
			,@return INT OUTPUT
AS

SET NOCOUNT ON
	-- table created as used IN "if" stmt
	CREATE TABLE [dbo].[#stock](
	 [Online] [BIT] NULL
	,[Product Code] [varchar](18) NULL
	,[Product Type] [varchar](9) NOT NULL
	,[Product Description 1] [varchar](32) NOT NULL
	,[Product Description 2] [varchar](40) NOT NULL
	,[Cash Price] [DECIMAL](11, 2) NULL
	--[Weekly Price] [DECIMAL](9, 2) NULL	
	,[DC Only] [BIT] NULL
	,[Available Stock] [INT] NULL
	,[Cash Promotional Price] [DECIMAL](11, 2) NULL
	,[Date Promo Starts] [datetime] NULL
	,[Date Promo Ends] [datetime] NULL
	,[Date Added] [datetime] NULL
	,[Date Removed] [datetime] NULL
	,[ItemId] [INT] NOT NULL
	,[HPPromoPrice] [DECIMAL](11, 2) NULL
	,[StkDC] [INT] NULL
	,[StkAll] [INT] NULL
) ON [PRIMARY] 

	SET @dateadded = CAST(@dateadded AS DATE)
	SET @dateRemoved = CAST(@dateRemoved AS DATE)

	DECLARE @stocklocn SMALLINT
			,@scoringband CHAR(1)
			,@oltermstype VARCHAR(3)
			,@oltermslength SMALLINT
			,@taxtype CHAR(1)
			,@taxrate FLOAT
			,@facia VARCHAR(100)

	SELECT @stocklocn= value FROM CountryMaintenance WHERE codename='OnlineDistCentre'
	--SELECT  @scoringband= value FROM CountryMaintenance WHERE codename='OnlineSBandIntRate'
	--SELECT  @oltermstype= value FROM CountryMaintenance WHERE codename='OnlineTermsType'
	--SELECT  @oltermslength= value FROM CountryMaintenance WHERE codename='OnlineTermsLength'
	SELECT  @taxtype= value FROM CountryMaintenance WHERE codename='taxtype'
	SELECT  @taxrate= value FROM CountryMaintenance WHERE codename='taxrate'
	
	SET @return = 0			--initialise return code
	
IF REPLACE(@prodDesc,'"','')!=''
	-- searching ON description etc - ie using FREETEXTSEARCH
BEGIN
	;WITH promotion AS
		( SELECT p.itemid, p.fromdate, p.todate, p.unitprice, p.StockLocn
			FROM promoprice p
			WHERE p.fromdate = (SELECT MAX(p2.fromdate) FROM promoprice p2 WHERE p.itemid=p2.itemid AND p.hporcash=p2.hporcash AND p.hporcash='C' AND p.stocklocn = p2.stocklocn)
					AND GETDATE() BETWEEN p.fromdate AND p.todate
		), promotionHP AS
		( SELECT p.itemid, p.fromdate, p.todate, p.unitprice, p.StockLocn
			FROM promoprice p
			WHERE p.fromdate = (SELECT MAX(p2.fromdate) FROM promoprice p2 WHERE p.itemid=p2.itemid AND p.hporcash=p2.hporcash AND p.hporcash='H' AND p.stocklocn = p2.stocklocn)
					AND GETDATE() BETWEEN p.fromdate AND p.todate
		)
		
	INSERT INTO #stock
	SELECT TOP (@limit)
			CAST(ISNULL(OnlineAvailable,0) AS BIT) AS [Online]
			,UPPER(ISNULL(IUPC,'IUPC Missing')) AS 'Product Code'			
			,CASE WHEN Itemtype ='S' THEN 'Stock' ELSE 'Non-Stock' END AS 'Product Type'
			,ItemDescr1 AS 'Product Description 1'
			,ItemDescr2 AS 'Product Description 2'			
			,CAST(SP.CashPrice AS DECIMAL (11,2)) AS 'Cash Price'			
			,CAST(ISNULL(OnlineDConly,0) AS BIT) AS [DC Only]
			,CAST(NULL AS INT) AS 'Available Stock'
			--CAST(0 AS DECIMAL(9,2)) AS 'Weekly Price',
			--CASE WHEN p.unitprice IS NOT NULL THEN CAST(CASE WHEN @taxtype='E' THEN p.unitprice * (1 + @taxrate/100) ELSE p.unitprice END AS DECIMAL(11,2)) ELSE NULL END AS 'Cash Promotional Price',
			,CASE WHEN p.unitprice IS NOT NULL THEN CAST(p.unitprice AS DECIMAL(11,2)) ELSE NULL END AS 'Cash Promotional Price'
			,p.fromdate AS 'Date Promo Starts'
			,p.todate AS 'Date Promo Ends'
			,si.OnlineDateAdded AS 'Date Added'
			,si.OnlineDateRemoved AS 'Date Removed'
			,si.Id AS ItemId
			,CAST(pHP.unitprice AS DECIMAL(11,2)) AS 'HPPromoPrice'
			,CAST(NULL AS INT)
			,CAST(NULL AS INT)	
	FROM	dbo.StockInfo SI
	INNER JOIN FREETEXTTABLE(dbo.StockInfo, (itemdescr1, itemdescr2, itemPOSDescr, itemno, brand, VendorLongStyle,VendorEAN), @prodDesc) K ON SI.ID = K.[Key]		
	INNER JOIN StockPrice SP ON SI.ID = SP.ID AND SP.BranchNo = @stocklocn
	INNER JOIN StockQuantity SQ ON  SI.ID = SQ.ID AND SQ.StockLocn = @stocklocn
	LEFT OUTER JOIN promotion p ON si.id=p.itemid AND p.StockLocn = @stocklocn
	LEFT OUTER JOIN promotionHP pHP ON si.id=pHP.itemid  AND pHP.StockLocn = @stocklocn		
	WHERE CONVERT(VARCHAR,SI.Category) NOT IN (SELECT code FROM dbo.code WHERE category IN ('WAR')) AND			-- removed 'PCDIS'
	      (SP.DateActivated IS NULL OR SP.DateActivated < GETDATE())
			AND si.prodstatus NOT IN ('D','R')		
			AND (si.category=@category OR @category=0)
			AND ((ISNULL(OnlineAvailable,0)=0 AND @online='N') 
					OR (ISNULL(OnlineAvailable,0)=1 AND @online='Y')
					OR @online='A')
			AND (si.OnlineDateAdded>=@dateAdded OR @dateAdded='1900-01-01')
			AND (si.OnlineDateRemoved>=@dateRemoved OR @dateRemoved='1900-01-01')
			AND SQ.Deleted != 'Y'
			AND NOT EXISTS(SELECT 1 FROM kitproduct k WHERE k.ItemID=si.id)		-- not a kit
			AND si.category NOT IN (15,85)				-- not second hand/repossessed
			AND ((ISNULL(si.OnlineDConly,0) = 0 AND @location='A')    -- All Branches
				OR (ISNULL(si.OnlineDConly,0) = 1 AND @location='DC')    -- Distribution centre
				OR (@location='B'))									-- Either
			
	GROUP BY IUPC, SI.itemtype,  ItemDescr1, ItemDescr2, SQ.Stock, SQ.QtyAvailable,	OnlineAvailable, OnlineDconly,
	SQ.Deleted, SP.CashPrice,p.fromdate,p.todate,si.Id, p.unitprice,pHP.unitprice,OnlineDateAdded,OnlineDateRemoved, K.[RANK]
	--ORDER BY K.RANK DESC
END
ELSE
	-- not searching ON description etc - ie not using FREETEXTSEARCH
BEGIN		
	;WITH promotion AS
		( SELECT p.itemid, p.fromdate, p.todate, p.unitprice, p.StockLocn
			FROM promoprice p
			WHERE p.fromdate = (SELECT MAX(p2.fromdate) FROM promoprice p2 WHERE p.itemid=p2.itemid AND p.hporcash=p2.hporcash AND p.hporcash='C' AND p.stocklocn = p2.stocklocn)
					AND GETDATE() BETWEEN p.fromdate AND p.todate
		), promotionHP AS
		( SELECT p.itemid, p.fromdate, p.todate, p.unitprice, p.StockLocn
			FROM promoprice p
			WHERE p.fromdate = (SELECT MAX(p2.fromdate) FROM promoprice p2 WHERE p.itemid=p2.itemid AND p.hporcash=p2.hporcash AND p.hporcash='H' AND p.stocklocn = p2.stocklocn)
					AND GETDATE() BETWEEN p.fromdate AND p.todate
		)
		
	INSERT INTO #stock
	SELECT TOP (@limit)
			 CAST(ISNULL(OnlineAvailable,0) AS BIT) AS [Online]
			,UPPER(ISNULL(IUPC,'IUPC Missing')) AS 'Product Code'			
			,Case WHEN Itemtype ='S' THEN 'Stock' ELSE 'Non-Stock' END AS 'Product Type'
			,ItemDescr1 AS 'Product Description 1'
			,ItemDescr2 AS 'Product Description 2'		
			,CAST(SP.CashPrice AS DECIMAL (11,2)) AS 'Cash Price'			
			,CAST(ISNULL(OnlineDConly,0) AS BIT) AS [DC Only]
			,CAST(NULL AS INT) AS 'Available Stock'
			--CAST(0 AS DECIMAL(9,2)) AS 'Weekly Price',
			--CASE WHEN p.unitprice IS NOT NULL THEN CAST(CASE WHEN @taxtype='E' THEN p.unitprice * (1 + @taxrate/100) ELSE p.unitprice END AS DECIMAL(11,2)) ELSE NULL END AS 'Cash Promotional Price',
			,CASE WHEN p.unitprice IS NOT NULL THEN CAST(p.unitprice AS DECIMAL(11,2)) ELSE NULL END AS 'Cash Promotional Price'
			,p.fromdate AS 'Date Promo Starts'
			,p.todate AS 'Date Promo Ends'
			,si.OnlineDateAdded AS 'Date Added'
			,si.OnlineDateRemoved AS 'Date Removed'
			,si.Id AS ItemId
			,CAST(pHP.unitprice AS DECIMAL(11,2)) AS 'HPPromoPrice'
			,CAST(NULL AS INT)
			,CAST(NULL AS INT)
	FROM	dbo.StockInfo SI
	INNER JOIN StockPrice SP ON SI.ID = SP.ID AND SP.BranchNo = @stocklocn
	INNER JOIN StockQuantity SQ ON  SI.ID = SQ.ID AND SQ.StockLocn = @stocklocn
	LEFT OUTER JOIN promotion p ON si.id=p.itemid AND p.StockLocn = @stocklocn
	LEFT OUTER JOIN promotionHP pHP ON si.id=pHP.itemid  AND pHP.StockLocn = @stocklocn		
	WHERE CONVERT(VARCHAR,SI.Category) NOT IN (SELECT code FROM dbo.code WHERE category IN ('WAR')) AND			-- removed 'PCDIS'
	      (SP.DateActivated IS NULL OR SP.DateActivated < GETDATE())
			AND si.prodstatus NOT IN ('D','R')		
			AND (si.category=@category OR @category=0)
			AND ((ISNULL(OnlineAvailable,0)=0 AND @online='N') 
					OR (ISNULL(OnlineAvailable,0)=1 AND @online='Y')
					OR @online='A')
			AND (si.OnlineDateAdded>=@dateAdded OR @dateAdded='1900-01-01')
			AND (si.OnlineDateRemoved>=@dateRemoved OR @dateRemoved='1900-01-01')
			AND SQ.Deleted != 'Y'
			AND NOT EXISTS(SELECT 1 FROM kitproduct k WHERE k.ItemID=si.id)		-- not a kit
			AND si.category NOT IN (15,85)				-- not second hand/repossessed
			AND ((ISNULL(si.OnlineDConly,0) = 0 AND @location='A')    -- All Branches
				OR (ISNULL(si.OnlineDConly,0) = 1 AND @location='DC')    -- Distribution centre
				OR (@location='B'))									-- Either
			
	GROUP BY IUPC, SI.itemtype,  ItemDescr1, ItemDescr2, SQ.Stock, SQ.QtyAvailable,	OnlineAvailable, OnlineDconly,
	SQ.Deleted, SP.CashPrice,p.fromdate,p.todate,si.Id, p.unitprice,pHP.unitprice,OnlineDateAdded,OnlineDateRemoved

END

	-- UPDATE Stock available	

	SET @facia = CASE WHEN EXISTS(SELECT 1 FROM Country WHERE CountryCode = 'Q') THEN 'OMNI' ELSE 'Courts' END
	
	;WITH alllocations AS
	( SELECT itemid,SUM(sq.qtyAvailable) AS AvailStock
		FROM #stock s INNER JOIN stockquantity sq ON s.itemid=sq.id
						INNER JOIN branch b ON sq.stocklocn=b.branchno  
						INNER JOIN Merchandising.Location L ON L.SalesId = b.BranchNo 
			WHERE  L.Fascia = @facia AND L.Active = 1 AND L.VirtualWarehouse = 0
		GROUP BY itemid	
	) 
	
	UPDATE #stock
		SET [Available Stock]=CASE WHEN s.[DC Only]=0 THEN a.AvailStock ELSE sq.qtyAvailable END,
			[StkDC]=sq.qtyAvailable,
			[StkAll]= a.AvailStock 
	FROM #stock s INNER JOIN alllocations a ON s.itemid=a.ItemId
			INNER JOIN stockquantity sq ON s.itemid=sq.id AND sq.stocklocn=@stocklocn
	WHERE s.[Product Type]='Stock'	
	
	---- #14407 CashPrice now tax inclusive/exclusive
	--UPDATE #stock SET		
	--	--SET [Weekly Price]= ((CASE WHEN @taxtype='E' THEN CASE WHEN HPPromoPrice is NULL THEN sp.CreditPrice ELSE HPPromoPrice END * (1 + @taxrate/100)
	--	--						ELSE CASE WHEN HPPromoPrice is NULL THEN sp.CreditPrice ELSE HPPromoPrice END END 
	--	--				-CASE WHEN CASE WHEN HPPromoPrice is NULL THEN sp.CreditPrice ELSE HPPromoPrice END<tt.DefaultDeposit THEN 0 ELSE tt.DefaultDeposit END) * 
	--	--						(1 + (tt.ServPcent/100 * @oltermslength/12)) )/(@oltermslength/12 * 52),
	--		[Cash Price]= CASE WHEN @taxtype='E' THEN sp.CashPrice * (1 + @taxrate/100) ELSE sp.CashPrice END
			
	--FROM stockprice sp INNER JOIN #stock s ON sp.ID=s.ItemId,
	--		termstypeallbands tt
	--WHERE tt.TermsType=@oltermstype AND tt.Band=@scoringband

	-- #14487 Special Tax rates	- not applicable 
	--UPDATE #stock 
	--	SET	[Cash Price]= CASE WHEN @taxtype='E' THEN s.[Cash Price] * (1 + CAST(c.codedescript AS float)/100) ELSE s.[Cash Price] END
	--FROM #stock s INNER JOIN StockInfo si ON s.ItemId=si.id 
	--			  INNER JOIN code c ON si.iupc=c.code AND c.category='TXR'

	UPDATE #stock
			SET [Cash Promotional Price]= NULL,[Date Promo Starts]=NULL,[Date Promo Ends]=NULL
	WHERE [Date Promo Ends]<GETDATE()

	ALTER TABLE #stock DROP COLUMN HPPromoPrice
	ALTER TABLE #stock DROP COLUMN [Product Type]
	
	SELECT [Online],[Product Code], [Product Description 1],[Product Description 2],
	[Cash Price],[DC Only],[Available Stock],[Cash Promotional Price],[Date Promo Starts],[Date Promo Ends],
	[Date Added],[Date Removed],[ItemId], [StkDC],[StkAll] 
	FROM #stock

	SET	ROWCOUNT 0

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
