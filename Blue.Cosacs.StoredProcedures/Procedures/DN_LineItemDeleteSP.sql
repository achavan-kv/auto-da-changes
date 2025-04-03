SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemDeleteSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemDeleteSP]
GO

CREATE PROCEDURE 	dbo.DN_LineItemDeleteSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_LineItemDeleteSP.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : 
-- Author       : ??
-- Date         : ??
--
-- This procedure will retrieve the value before collection.
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 18/05/11  IP/NM 	RI Integration changes - CR1212 - #3627 - use itemID
-- ================================================
			@acctNo varchar(12),
			@agreementNo int, 
			--@itemNo varchar(8),				--IP/NM - 18/05/11 -CR1212 - #3627 
			@itemID int,
			@location smallint,
			@return int OUTPUT

AS

	SET 		@return = 0			--initialise return code

	DELETE
	FROM		lineitem
	WHERE	acctno = @acctNo
	AND		agrmtno = @agreementNo
	--AND		itemno = @itemNo
	AND		ItemID = @itemID				--IP/NM - 18/05/11 -CR1212 - #3627 
	AND		stocklocn = @location

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END




GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

