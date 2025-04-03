SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DeliveryGetQuantitySP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DeliveryGetQuantitySP]
GO

CREATE PROCEDURE 	dbo.DN_DeliveryGetQuantitySP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_DeliveryGetQuantitySP.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Retrieve the delivered quantity for an item
-- Date         : ??
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 16/05/11  ip  RI Integration changes - CR1212 - #3627 - Changed joins to use ItemID and ParentItemID rather than ItemNo and ParentItemNo
-- ================================================
			@acctNo varchar(12),
			@agreementNo int,
			--@itemNo varchar(8), 
			@itemID int,			--IP - 16/05/11 - CR1212 - #3627
			@location smallint,
			@contractno varchar(10),
			--@ParentItemNo varchar(8),
			@parentItemID int,		--IP - 16/05/11 - CR1212 - #3627
			@delivered float OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	SET	@delivered = 0

	--IP - 27/05/09 - (69962)
	IF(@contractno !='')
	BEGIN
		SELECT	@delivered = isnull(sum(quantity), 0)
		FROM	delivery
		WHERE	acctno = @acctNo
		AND		agrmtno = @agreementNo
		--AND		itemno = @itemNo
		AND		ItemID = @itemID					--IP - 16/05/11 - CR1212 - #3627
		AND		stocklocn = @location
		AND		contractno = @contractno
	END
	ELSE
	BEGIN
		SELECT	@delivered = isnull(sum(quantity), 0)
		FROM	delivery
		WHERE	acctno = @acctNo
		AND		agrmtno = @agreementNo
		--AND		itemno = @itemNo
		AND		ItemID = @itemID					--IP - 16/05/11 - CR1212 - #3627
		AND		stocklocn = @location
		AND		contractno = @contractno
		--AND		parentItemNo = @parentItemNo
		AND		ParentItemID = @ParentItemID		--IP - 16/05/11 - CR1212 - #3627	
	END

	


	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

