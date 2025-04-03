SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_StockGetByLocationSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_StockGetByLocationSP]
GO

CREATE PROCEDURE 	dbo.DN_StockGetByLocationSP
			@stocklocn smallint,
			@branchCode smallint,
			@showDeleted int,
			@showAvailable int,
			@prodDesc NVARCHAR(80),
			@limit INT,
			@return int OUTPUT
			
/***********************************************************************************************************
--
-- Project      : CoSACS .NET
-- File Name    : DN_StockGetByLocationSP.SQL
-- File Type    : MSSQL Server Stored Procedure Script
-- Description  : Retrieve stockitems based on location
--				
-- Author       : ?
-- Date         : ?
--
--  
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 05/08/10  IP  UAT(25) UAT5.1.9.0 - Return the itemno as Uppercase
-- 21/04/11  jec CR1212 - return Colour and Style and item Identity 
-- 19/05/11  IP  CR1212 - IUPC was being returned as null. Added isnull check.
-- 31/05/11  NM  CR1212 (RI) - Fulltext Search Support
-- 27/07/11  IP  RI - Return VendorStyle and VendorLongStyle. Concatonate ColourName and ColourCode
-- 06/09/11  IP  RI - #8104 - UAT40 - Populate the Delivery Date from the PurchaseOrderOutstanding table
-- 28/09/12  IP  #10393 - LW74980 - Added StockInfo.VendorEan to fulltext search. Users require to search by barcode.
************************************************************************************************************/

AS

	SET @return = 0			--initialise return code

	SELECT TOP (@limit)
			UPPER(ISNULL(IUPC,'IUPC Missing')) AS 'Product Code', --IP - 05/08/10 - UAT(25) UAT5.1.9.0		--IP - 19/05/11 - CR1212 - IUPC was being returned as null
			SI.itemno AS 'Courts Code',
			ProdStatus AS 'D',
			ItemDescr1 AS 'Product Description 1',
			ItemDescr2 AS 'Product Description 2',
			CASE 
				WHEN isnull(ColourName,'') != '' OR isnull(ColourCode,'') !='' THEN isnull(ColourName,'') + ' : ' +  isnull(ColourCode,'')
				ELSE '' 
			END as Colour,
			--CASE 
			--	WHEN ISNULL(VendorLongStyle,'') = '' THEN ISNULL(VendorStyle,'') 
			--	ELSE VendorLongStyle 
			--END AS Style,
			ISNULL(Brand,'') as Brand,		--IP - 27/07/11 - RI
			ISNULL(VendorLongStyle,'') as Model,		--IP - 27/07/11 - RI
			ISNULL(VendorStyle,'') as VendorStyle,				--IP - 27/07/11 - RI
			SQ.Stock AS 'Actual Stock',
			SQ.QtyAvailable AS 'Available Stock',
			SQ.StockDamage AS 'Damage Stock',
			SQ.StockOnOrder AS 'Stock On Order',
			SQ.Deleted,			-- 69647 Add a deleted column 
			min(po.expectedreceiptdate) AS 'Delivery Date',		--IP - 06/09/11 - RI - #8104 - UAT40
			--CONVERT(DATETIME, NULL) AS 'Delivery Date',			
			SupplierCode AS 'Supplier Code',
			SP.CashPrice AS 'Price',
			SI.ID AS 'ItemID',		-- CR1212
			SI.SKU,
			SI.VendorEAN AS 'Vendor EAN'			
	INTO	#stock				
	FROM	dbo.StockInfo SI
	INNER JOIN FREETEXTTABLE(dbo.StockInfo, (itemdescr1, itemdescr2, itemPOSDescr, itemno, brand, VendorLongStyle,VendorEAN), @prodDesc) K ON SI.ID = K.[Key]		--IP - 28/09/12 - #10393 - LW74980 - Added VendorEAN
	INNER JOIN StockPrice SP ON SI.ID = SP.ID AND SP.BranchNo = @stocklocn
	INNER JOIN StockQuantity SQ ON  SI.ID = SQ.ID AND SQ.StockLocn = @stocklocn
	LEFT JOIN PurchaseOrderOutstanding PO on SI.ID = PO.ItemID AND po.stocklocn = SQ.Stocklocn					--IP - 06/09/11 - RI - #8104 - UAT40
	WHERE CONVERT(VARCHAR,SI.Category) NOT IN (SELECT code FROM dbo.code WHERE category = 'WAR') AND 
	      (SP.DateActivated IS NULL OR SP.DateActivated < GETDATE()) AND	
	      (@showDeleted = 1 OR SQ.Deleted != 'Y') AND
		  (@showAvailable = 0 OR SQ.QtyAvailable > 0)
	GROUP BY IUPC, SI.itemno, ProdStatus, ItemDescr1, ItemDescr2, ColourName, ColourCode, Brand, VendorLongStyle, VendorStyle, SQ.Stock, SQ.QtyAvailable,	--IP - 06/09/11 - RI - #8104 - UAT40
	SQ.StockDamage, SQ.StockOnOrder, SQ.Deleted, SupplierCode, SP.CashPrice,SI.ID, SI.SKU, SI.VendorEAN, K.RANK
	ORDER BY K.RANK DESC

		
	DECLARE @pricefromlocn varchar(10)            
    select @pricefromlocn = value from CountryMaintenance where CodeName =  'pricefromlocn' 
    
	IF(@pricefromlocn = 'false')
	BEGIN
		update #stock
		set price = unitpricecash
		from stockitem
		where stockitem.itemno = #stock.[product code] and stocklocn = @branchCode
	END
	

	SELECT * FROM #stock

	SET	ROWCOUNT 0

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

