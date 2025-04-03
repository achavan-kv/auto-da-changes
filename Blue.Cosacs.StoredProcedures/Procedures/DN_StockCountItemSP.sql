SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_StockCountItemSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_StockCountItemSP]
GO

CREATE PROCEDURE 	dbo.DN_StockCountItemSP
--------------------------------------------------------------------------------
--
-- Project      : eCoSACS r 2002 Strategic Thought Ltd.
-- File Name    : DN_StockCountItemSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Count of Stock Items at location
-- Author       : ??
-- Date         : ??
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 19/05/11 jec  CR1212 RI Integration use ItemID
--------------------------------------------------------------------------------

    -- Parameters
			--@productcode varchar(8),
			@itemId		INT,		-- RI
			@stocklocn smallint,
			@rowcount int OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT 	@rowcount = COUNT(*)
	FROM 		STOCKITEM S, BRANCH B
	--WHERE 	S.ItemNo	= @productCode
	WHERE 	S.ItemID	= @ItemID				-- RI
        	AND 		S.StockLocn 	= @stocklocn
        	AND 		B.BranchNo  	= S.StockLocn

	SET	@return = @@error

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End
