SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_WarrantyCanAddSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_WarrantyCanAddSP]
GO


CREATE PROCEDURE 	dbo.DN_WarrantyCanAddSP
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
			@acctno varchar(12),
		 	@itemId int,
            @stocklocn smallint,
            @price money,
			@return int OUTPUT
AS

	SET 	@return = 0			--initialise return code
	
   	DECLARE	@status int, 
			@checkacctno varchar(12),
			--@checkitemno  varchar(8),
			@checkitemno  varchar(18),										--IP - 03/08/11 - RI
			@datedel datetime,
           	@datewarranty datetime,
			@category smallint,
            @refcode varchar(4)
    	
    SELECT 	@category = category,
            @refcode = LEFT(refcode,2)
   	FROM 	stockitem
   	WHERE 	itemId = @itemId AND 
			stocklocn = stocklocn -- NM , UAT(5.2) - 697 
     	
	SET @refcode = @refcode + '%'
    
    /*if this item is the warranty itself then it cannot have a warranty added to it*/
    --IF (@category = 12 or @category = 82)
    IF (@category in (select distinct code from code where category = 'WAR')) --IP - 29/10/09 - CoSACS Improvement - Remove hardcoded discounts & warranties
	    SET	@return = 1

	--#16208
    /*check whether warranty available for this item and price*/
 --  	IF(@return = 0)
 --   BEGIN
 --		--SELECT	@checkitemno = S.itemno 
 --		SELECT	@checkitemno = S.iupc									--IP - 03/08/11 - RI 
	--    FROM	warrantyband W, stockitem S, stockitem S1
	--    WHERE	S.itemId = @itemId
	--    AND		S.stocklocn = @stocklocn
	--    AND		w.refcode like @refcode
	--    AND 	S1.itemId = W.itemId
	--    AND		S1.stocklocn = @stocklocn
	--    AND		@price BETWEEN W.minprice AND W.maxprice 
    	
 --   	IF(@@rowcount = 0)
 --      		SET	@return = 3
	--END

	--#16208 #15988 - remove check on stock item being warrantable - no longer required.
	--IF(@return = 0)
 --   BEGIN
 --		SELECT	*
	--    FROM stockitem S
	--    WHERE	S.itemId = @itemId
	--    AND		S.stocklocn = @stocklocn
	--	AND		S.warrantable != 1

 --   	IF(@@rowcount != 0)
 --      		SET	@return = 3
	--END

    /* now check whether item already delivered*/
	IF(@return = 0)
	BEGIN
        --SELECT 	@datedel = MIN(datedel) 
		--IP - 29/07/08 - UAT5.1 - UAT(494)- Was previously taking the original date of when an item was
		--delivered. This prevented a warranty from being added after an identical replacement on the item
		--if the warranty was no longer valid, as the period after delivery that a warranty could be added
		--had passed. 
		
		SELECT 	@datedel = MAX(datedel) 
	    FROM    delivery 
	    WHERE   acctno = @acctno 
	    AND     itemId = @itemId 
	    AND     stocklocn = @stocklocn

       	IF(@datedel is not null)
            SET @return = 4
	END

	IF(@return = 4)
	BEGIN
    	SELECT 	@datewarranty = DATEADD(day, warrantydays, @datedel)
    	FROM 	country
    
	    IF(@datewarranty > getdate())
      	    SET @return = 0
    END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

