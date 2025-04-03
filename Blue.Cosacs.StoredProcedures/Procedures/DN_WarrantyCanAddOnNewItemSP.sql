SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_WarrantyCanAddOnNewItemSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_WarrantyCanAddOnNewItemSP]
GO


CREATE PROCEDURE 	dbo.DN_WarrantyCanAddOnNewItemSP
-- =============================================
-- Author:		?
-- Create date: ?
-- Description:	
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 03/08/11  IP  RI - System Integration changes
-- =============================================
            @itemId int,
            @stocklocn smallint,
            @price money,
			@return int OUTPUT
AS

	SET 	@return = 0			--initialise return code
	
   	DECLARE	
			--@checkitemno  varchar(8),
			@checkitemno  varchar(18),										--IP - 03/08/11 - RI
			@category smallint,
            @refcode varchar(4)
            
    SELECT 	@category = category,
            @refcode = LEFT(refcode,2)
   	FROM 	stockitem
   	WHERE 	itemId = @itemId 
   	AND     stocklocn = @stocklocn
     	
	SET @refcode = @refcode + '%'
    
    /*if this item is the warranty itself then it cannot have a warranty added to it*/
    --IF (@category = 12 or @category = 82)
     IF (@category in (select distinct code from code where category = 'WAR')
        or (select ItemType from StockInfo si where si.Id = @itemId) = 'N') --IP - 29/10/09 - CoSACS Improvement - Remove hardcoded discounts & warranties --IP - 29/10/09 - CoSACS Improvement - Remove hardcoded discounts & warranties
	    SET	@return = 1

    /*check whether warranty available for this item and price*/
 --  	IF(@return = 0)
 --   BEGIN
 --		--SELECT	@checkitemno = S.itemno 
 --		SELECT	@checkitemno = S.iupc									 --IP - 03/08/11 - RI 
	--    FROM	warrantyband W, stockitem S, stockitem S1
	--    WHERE	S.ItemID = @itemId
	--    AND		S.stocklocn = @stocklocn
	--    AND		w.refcode like @refcode
	--    AND 	S1.ItemID = W.ItemID
	--    AND		S1.stocklocn = @stocklocn
	--    AND		@price BETWEEN W.minprice AND W.maxprice 
    	
 --   	IF(@@rowcount = 0)
 --      		SET	@return = 3
	--END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO