SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DeliveryGetValueSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DeliveryGetValueSP]
GO

CREATE PROCEDURE 	dbo.DN_DeliveryGetValueSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_DeliveryGetValueSP.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Retrieve the delivered value for an item
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
			@itemID int,					--IP - 16/05/11 - CR1212 - #3627
			@location smallint,
			@contractno varchar(10),
			--@ParentItemNo varchar(8),		--IP - 16/05/11 - CR1212 - #3627
			@parentItemID int,
			@delivered money OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	SET	@delivered = 0

	SELECT	@delivered = isnull(sum(transvalue), 0)
	FROM		delivery
	WHERE	acctno = @acctNo
	AND		agrmtno = @agreementNo
	--AND		itemno = @itemNo			--IP - 16/05/11 - CR1212 - #3627
	AND		ItemID = @itemID
	AND		stocklocn = @location
	AND		contractno = @contractno
	--AND		parentItemNo = @parentItemNo
	AND		ParentItemID = @parentItemID	--IP - 16/05/11 - CR1212 - #3627

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

