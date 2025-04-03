SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemGetOldQtySP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemGetOldQtySP]
GO

CREATE PROCEDURE 	dbo.DN_LineItemGetOldQtySP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_LineItemGetOldQtySP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Line Item Get Old Quantity
-- Author       : ??
-- Date         : ??
--
-- This procedure will retrieve the old quantity from the lineitem_amend table.
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 27/04/11 jec CR1212 RI Integration - Use ItemID instead of ItemNo
-- ================================================
			@acctno varchar(12),
			--@itemno varchar(8),
			@itemID int,		-- RI
			@stocklocn smallint,
			@contractno varchar(10),
			@agreementno int,
			@ParentItemID int,
			@quantity float OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	@quantity = quantity 
	FROM		lineitem_amend
	WHERE	acctno = @acctno
	--AND		itemno = @itemno
	AND		ItemID= @itemID			-- RI 
	AND		stocklocn = @stocklocn
	AND		contractno = @contractno
	AND		agrmtno = @agreementno
	and		ParentItemID = @ParentItemID
	
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End
