SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemGetOldValueSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemGetOldValueSP]
GO

-- 67977 RD 22/02/06 Added to get value for taxamt
CREATE PROCEDURE 	dbo.DN_LineItemGetOldValueSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_LineItemGetOldValueSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Line Item Get Old Value
-- Author       : ??
-- Date         : ??
--
-- This procedure will retrieve the old value from the lineitem_amend table.
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
			@parentitemid int,
			@value money OUT,
			@taxamt money OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	@value = ordval,
			@taxamt = taxamt
	FROM	lineitem_amend
	WHERE	acctno = @acctno
	--AND		itemno = @itemno
	AND		ItemID= @itemID			-- RI 
	AND		stocklocn = @stocklocn
	AND		agrmtno = @agreementno
	AND		contractno = @contractno
	AND		parentitemid = @parentitemid
	
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
