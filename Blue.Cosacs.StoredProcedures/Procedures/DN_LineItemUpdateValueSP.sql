SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemUpdateValueSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemUpdateValueSP]
GO


CREATE PROCEDURE 	dbo.DN_LineItemUpdateValueSP
--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_LineItemUpdateValueSP.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        :
-- Description  : 
-- Author       : ??
-- Date         : ??
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 17/05/11  ip  RI Integration changes - CR1212 - #3627 - Changed joins to join on ItemID
--------------------------------------------------------------------------------
			@acctNo varchar(12),
			--@itemNo varchar(8),
			@itemID int,					--IP - 17/05/11 - CR1212 - #3627 
			@location smallint,
			@newvalue money,
			@agreementno int,
			@return int OUTPUT

AS

	SET 		@return = 0			--initialise return code

	UPDATE	lineitem
	SET		ordval = @newvalue,
			price = @newvalue / quantity
	WHERE	acctno = @acctNo
	AND		agrmtno = @agreementno
	--AND		itemno = @itemNo
	AND		itemID = @itemID				--IP - 17/05/11 - CR1212 - #3627
	AND		stocklocn = @location
	AND		quantity != 0			--make sure we don't divide by 0
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

