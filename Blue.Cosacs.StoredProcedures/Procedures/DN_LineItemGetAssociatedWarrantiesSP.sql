SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemGetAssociatedWarrantiesSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemGetAssociatedWarrantiesSP]
GO

CREATE PROCEDURE 	dbo.DN_LineItemGetAssociatedWarrantiesSP
--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_LineItemGetAssociatedWarrantiesSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Associated Warranties
-- Author       : ??
-- Date         : ??
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 20/05/11 jec CR1212 RI Integration - Use ItemID instead of ItemNo
-- 14/02/13 ip #17444 - Specified cast error 
--------------------------------------------------------------------------------
    -- Parameters
			@acctno varchar(12),
			--@itemno varchar(8),
			@itemid int,			-- RI
			@stocklocn smallint,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

		--SELECT	itemno as associateditem,
	SELECT	IUPC as associateditem,
			contractno,
			stocklocn,
			ItemID as associateditemid,			-- RI
			s.WarrantyType,			--#17883 -- #15993
			isnull(l.WarrantyGroupId,0)	as WarrantyGroupId	  -- #18038
	FROM lineitem l 
	INNER JOIN stockinfo s on l.ItemID=s.ID		-- RI
	WHERE	acctno = @acctno
	--AND		parentitemno = @itemno
	AND		ParentItemID = @itemid			-- RI
	AND		parentlocation = @stocklocn
	AND		contractno != ''
	AND 		quantity != 0  

UNION ALL
	
SELECT	IUPC as associateditem,
			contractno,
			stocklocn,
			ItemID as associateditemid,
			s.WarrantyType,		--#17883 -- #15993
			isnull(l.WarrantyGroupId,0)	as WarrantyGroupId	  -- #18038
FROM lineitem l 
INNER JOIN stockinfo s on l.ItemID=s.ID		
WHERE acctno = @acctno
AND contractno != ''
AND EXISTS  (SELECT * 
			 FROM lineitem l2
			 WHERE l2.acctno = l.acctno
			 AND l2.ItemID = l.ParentItemID
			 AND l2.stocklocn = l.Parentlocation
			 AND l2.isKit = 1
			 AND EXISTS (SELECT * FROM lineitem l3
						 WHERE l3.acctno = l2.acctno
						 AND l3.parentitemid = l2.itemid
						 AND l3.parentlocation = l2.stocklocn
						 AND l3.itemid = @itemid
						 AND l3.stocklocn = @stocklocn))
Order by isnull(l.WarrantyGroupId,0)			-- #15993

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
