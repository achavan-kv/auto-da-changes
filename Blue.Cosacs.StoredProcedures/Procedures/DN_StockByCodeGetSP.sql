SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_StockByCodeGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_StockByCodeGetSP]
GO


CREATE PROCEDURE 	dbo.DN_StockByCodeGetSP
-- **********************************************************************
-- Title: DN_StockByCodeGetSP.sql
-- Developer: ?
-- Date: ?
-- Purpose: Called from the Stock Enquiry By Product screen to return 
--			a product and the branches the product is available at.

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 13/05/11	 IP  RI - #3626 - Search either based on the SKU or IUPC or ItemNo (Courts Code)
-- 27/07/11  IP  RI - Return VendorStyle and VendorLongStyle. Concatonate ColourName and ColourCode
-- 28/09/12  IP  #10393 - LW74980 - Added StockInfo.VendorEan to fulltext search. Users require to search by barcode.
-- **********************************************************************
			@prodCode varchar(18),
			@return int OUTPUT
AS

	SET 	@return = 0			--initialise return code
	
	SELECT	stocklocn as 'Location',
			stockactual as 'Actual Stock',
			stockfactavailable as 'Available Stock',	
			stockdamage as 'Damage Stock',
			stockonorder as 'Stock On Order',
			RepossessedItem AS 'Repossessed',
			deleted,			-- 69647 Add a deleted column
			stocklastplanneddate as 'Stock Delivery Date',
			IUPC,
			itemno AS 'Courts Code',
			ItemID,             -- RI CR1212
			SKU,
			VendorEAN AS 'Vendor EAN',
			ISNULL(Brand,'') as Brand,		--RM O6/10/11 #8391
			ISNULL(VendorLongStyle,'') as Model,		--IP - 27/07/11 - RI
			ISNULL(VendorStyle,'') as VendorStyle,				--IP - 27/07/11 - RI
			CASE 
				WHEN isnull(ColourName,'') != '' OR isnull(ColourCode,'') !='' THEN isnull(ColourName,'') + ' : ' +  isnull(ColourCode,'')
				ELSE '' 
			END as Colour	
					
	FROM	stockitem
	WHERE	(IUPC = @prodCode OR SKU = @prodCode OR itemNo = @prodCode OR vendorEan = @prodCode) --IP - 28/09/12 - #10393 - LW74980 Added VendorEan -- ItemNo column contains Courts Code
	AND		CONVERT(VARCHAR, category) NOT IN (SELECT code FROM dbo.code WHERE category = 'WAR')
		
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

