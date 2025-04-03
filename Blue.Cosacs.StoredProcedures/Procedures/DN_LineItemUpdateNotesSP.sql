SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemUpdateNotesSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemUpdateNotesSP]
GO

CREATE PROCEDURE [dbo].[DN_LineItemUpdateNotesSP] 
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_LineItemUpdateNotesSP.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : 
-- Author       : ??
-- Date         : ??
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 17/05/11 ip  RI Integration changes - CR1212 - #3627 - Changed joins to use ItemID rather than ItemNo
-- ================================================
			@acctno char(12), 
			@agrmtno int, 
			--@itemno varchar(8), 
			@itemID int,				--IP - 17/05/11 - CR1212 - #3627
			@stocklocn smallint, 
			@contractno varchar(10),
			--@notes varchar(128),
			@notes varchar(300),		--CR1048 jec 14/10/09 -- IP - 10/02/10 - CR1048 (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)
			@overwrite bit,
			@return int OUTPUT
AS

	SET 	@return = 0		

	IF(@overwrite = 1)
	BEGIN
		UPDATE 	lineitem
		SET 	notes = @notes
		WHERE 	acctno = @acctno
		AND 	agrmtno = @agrmtno
		--AND 	itemno = @itemno
		AND 	ItemID = @itemID		--IP - 17/05/11 - CR1212 - #3627
		AND 	stocklocn = @stocklocn
		AND		contractno = @contractno	
	END
	ELSE
	BEGIN	
		UPDATE 	lineitem
		SET 	notes = notes + @notes
		WHERE 	acctno = @acctno
		AND 	agrmtno = @agrmtno
		--AND 	itemno = @itemno
		AND 	ItemID = @itemID		--IP - 17/05/11 - CR1212 - #3627
		AND 	stocklocn = @stocklocn
		AND		contractno = @contractno
	END	
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

