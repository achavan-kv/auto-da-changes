SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemLinkWarrantySP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemLinkWarrantySP]
GO

CREATE PROCEDURE 	dbo.DN_LineItemLinkWarrantySP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_LineItemLinkWarrantySP.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Delivery and Schedule details  
-- Author       : ??
-- Date         : ??
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 18/05/11  IP/NM 	RI Integration changes - CR1212 - #3627 - use itemID
-- ================================================
			@acctno varchar(12),
			--@itemno varchar(10),
			@itemID int,				--IP/NM - 18/05/11 -CR1212 - #3627 
			@locn smallint,
			--@warrantyno varchar(10),
			@warrantyID int,			--IP/NM - 18/05/11 -CR1212 - #3627 
			@warrantylocn smallint,
			@contractno varchar(10),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	lineitem
	--SET	parentitemno = @itemno,
	SET ParentItemID = @itemID,			--IP/NM - 18/05/11 -CR1212 - #3627 
		parentlocation = @locn
	WHERE	acctno = @acctno
	--AND	itemno = @warrantyno
	AND ItemID = @warrantyID			--IP/NM - 18/05/11 -CR1212 - #3627 
	AND	stocklocn = @warrantylocn
	AND	contractno = @contractno
	
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


