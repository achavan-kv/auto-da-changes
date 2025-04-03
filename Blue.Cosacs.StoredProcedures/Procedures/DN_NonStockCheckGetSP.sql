SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_NonStockCheckGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_NonStockCheckGetSP]
GO

CREATE PROCEDURE 	dbo.DN_NonStockCheckGetSP
-- =============================================
-- Author:		?
-- Create date: ?
-- Description:	

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 02/08/11  IP  RI - System Integration changes
-- 10/08/11  IP  RI - #4464 - Check that the item is a free gift by checking the category of the item against code category 'FGC'
-- ================================================
			@acctno varchar(12),
			@nonstockexists smallint OUTPUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	SET		@nonstockexists = 0											--IP - 02/08/11 - RI

	--DECLARE @itemno varchar(8)
	DECLARE @itemno varchar(18)											--IP - 02/08/11 - RI
	DECLARE @value money

	--DECLARE 	discounts_cursor CURSOR FOR 
	--SELECT 	d.itemno, sum(transvalue) 
	--FROM 		delivery d, lineitem l 
	--WHERE 	d.acctno = @acctno
	--AND 		d.acctno = l.acctno 
	--AND 		d.itemno = l.itemno 
	--AND 		d.delorcoll in ('D', 'C')
	--AND 		l.price = 0.0 
	--GROUP BY 	d.itemno
	
	--IP - 02/08/11 - RI - Replaces the above code
	
	DECLARE 	discounts_cursor CURSOR FOR 
	SELECT 	isnull(si.iupc,'') as itemno, sum(transvalue) 
	FROM 		delivery d inner join lineitem l on d.acctno = l.acctno 
	AND 		d.ItemID = l.ItemID
	inner join stockinfo si on si.ID = d.ItemID 
	WHERE 	d.acctno = @acctno
	AND 		d.delorcoll in ('D', 'C')
	AND 		l.price = 0.0 
	GROUP BY 	si.iupc

	OPEN discounts_cursor

	FETCH NEXT FROM discounts_cursor 
	INTO @itemno, @value

	WHILE @@FETCH_STATUS = 0 AND @nonstockexists = 0
	BEGIN
		IF ABS(@value) >= 0.001
		BEGIN
			SET @nonstockexists = 1;
		END
	
		FETCH NEXT FROM discounts_cursor 
		INTO @itemno, @value
	END
	CLOSE discounts_cursor
   	DEALLOCATE discounts_cursor


	IF @nonstockexists = 0
	BEGIN
		DECLARE gifts_cursor CURSOR FOR 
		--SELECT itemno, sum(quantity) 
		SELECT isnull(si.iupc,'') as itemno, sum(quantity) 
		FROM delivery 
		INNER JOIN StockInfo si on delivery.ItemID = si.ID		--IP - 02/08/11 - RI
		WHERE acctno = @acctno
		AND transvalue = 0.00 
		--AND itemno like 'GF%'
		--AND isnull(si.iupc,'') like 'GF%'						--IP - 02/08/11 - RI
		AND si.category in (select code from code where category = 'FGC')	--IP - 10/08/11 - RI - Check that the category of the item is that of a free gift
		--GROUP BY itemno
		GROUP BY si.iupc										--IP - 02/08/11 - RI
	
		OPEN gifts_cursor
	
		FETCH NEXT FROM gifts_cursor 
		INTO @itemno, @value
	
		WHILE @@FETCH_STATUS = 0 AND @nonstockexists = 0
		BEGIN
			IF ABS(@value) >= 0.001
			BEGIN
				SET @nonstockexists = 1;
			END
		
			FETCH NEXT FROM gifts_cursor 
			INTO @itemno, @value
		END
		CLOSE gifts_cursor
		DEALLOCATE gifts_cursor
	END

	SET @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

