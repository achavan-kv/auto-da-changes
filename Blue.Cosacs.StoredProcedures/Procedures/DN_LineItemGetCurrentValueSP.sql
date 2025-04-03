SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemGetCurrentValueSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemGetCurrentValueSP]
GO

CREATE PROCEDURE 	dbo.DN_LineItemGetCurrentValueSP
--------------------------------------------------------------------------------
-- Project      : eCoSACS r 2002 Strategic Thought Ltd.
-- File Name    : DN_LineItemGetCurrentValueSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Lineitem Current values
-- Author       : ??
-- Date         : ??
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 07/06/11 jec  CR1212 RI Integration use itemid instead of itemno
--------------------------------------------------------------------------------

    -- Parameters
			@acctno varchar(12),
			--@itemno varchar(8),
			@itemid int,
			@stocklocn smallint,
			@contractno varchar(10),
			@agreementno int,
			@ParentItemID int,
			@value money OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	@value = ordval
	FROM		lineitem
	WHERE	acctno = @acctno
	--AND		itemno = @itemno
	AND		itemid = @itemid		-- RI
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

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End