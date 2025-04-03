SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemGetAssociatedDiscountsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemGetAssociatedDiscountsSP]
GO

CREATE PROCEDURE 	dbo.DN_LineItemGetAssociatedDiscountsSP
--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_LineItemGetAssociatedDiscountsSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Associated Discounts
-- Author       : ??
-- Date         : ??
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 20/05/11 jec CR1212 RI Integration - Use ItemID instead of ItemNo
--------------------------------------------------------------------------------
    -- Parameters
			@acctno varchar(12),
			--@itemno varchar(8),
			@itemid int,			-- RI
			@stocklocn smallint,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	--SELECT	l.itemno as associateditem,
	SELECT	IUPC as associateditem,				-- RI
			l.stocklocn,
			l.contractno,
			l.ItemID as associateditemid			-- RI
	FROM	lineitem l, stockitem s
	WHERE	l.acctno = @acctno
	--AND	l.parentitemno = @itemno
	AND		ParentItemID = @itemid			-- RI
	AND	l.parentlocation = @stocklocn
	--AND	l.itemno = s.itemno
	AND	l.ItemID = s.itemID					-- RI
	AND	l.stocklocn = s.stocklocn
	AND	s.itemtype = 'N'
	--AND	s.category NOT IN(12, 82)
	--AND	s.category NOT IN(select distinct code from code where category = 'WAR') --IP - 29/10/09 - CoSACS Improvement - Remove hardcoded discounts & warranties
    AND s.category IN(select code from code where category = 'PCDIS')
	AND 	quantity != 0

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- end end end end end end end end end end end end end end end end end end end end end end end
