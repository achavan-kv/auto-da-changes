SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_StockGetLocationsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_StockGetLocationsSP]
GO

CREATE PROCEDURE 	dbo.DN_StockGetLocationsSP
-- =============================================
-- Author:		????
-- Create date: ??
-- Title:	DN_StockGetLocationsSP
--
--	This procedure will retrieve details Stock locations
-- 
-- Change Control
-----------------
-- 23/06/11 jec #3969 - CR1254 Service use itemId
-- =============================================
	-- Add the parameters for the function here
			--@productCode varchar(18),
			@itemId		INT,				-- RI
			@productCodeOut varchar(18) OUT,
			@deleted char(1) OUT,
			@includeWarranties BIT,  -- LW 71731 - To allow warranties to be added on Legacy CashandGo Return screen --IP - 17/02/10 - CR1072 - LW 71731 - Cash&Go Fixes from 4.3
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	/* first attempt to translate the itemno in case it is a scanned barcode */
	--SELECT	@productCodeOut = itemno
	--FROM		BarCodeItem
	--WHERE	barcodeno = @productCode

	--IF(@@rowcount>0)
	--	SET	@productCode = @productCodeOut

	SELECT	TOP 1
			@deleted = deleted, 
			@productCodeOut = IUPC
	FROM	stockitem
	--WHERE	(itemno = @productCode OR IUPC = @productCode)
	WHERE	ItemId = @itemId			-- RI
	AND		category not in (12, 82)

	SELECT	stocklocn
	FROM		stockitem
	--WHERE	(itemno = @productCode OR IUPC = @productCode)
	--AND		category not in (12, 82)
	WHERE	ItemId = @itemId			-- RI
	AND		(@includeWarranties = 1 OR category not in (12, 82)) --IP - 17/02/10 - CR1072 - LW 71731 - Cash&Go Fixes from 4.3
	AND (@includewarranties = 1 OR NOT EXISTS (SELECT * FROM code WHERE category = 'INST'
	AND CODE.CODE = stockitem.itemno)) -- exclude installations from direct ddint
	and isnull(DateActivated,'1-jan-1900') < getdate()
	--AND		(stock > 0
	--OR		category in (36,37,38,46,47,48,86,87,88))		--allows discounts to have zero stock

	IF(@@rowcount = 0)
		SET	@return = -1

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End