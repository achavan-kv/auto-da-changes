SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ReplacementDeliveryNoteForWarranty]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ReplacementDeliveryNoteForWarranty]
GO

CREATE PROCEDURE [dbo].[DN_ReplacementDeliveryNoteForWarranty] 
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_ReplacementDeliveryNoteForWarranty.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : 
-- Date         : ??
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 09/06/11  IP  CR1212 - RI use ItemID
-- ================================================
			@acctno CHAR(12), 
			@agrmtno INTEGER, 
			--@itemno VARCHAR(8), 
			@itemID INTEGER,							--IP - 09/06/11 - CR1212 - RI
			@stocklocn SMALLINT, 
			@contractno VARCHAR(10),
			@quantity FLOAT,
			@user  CHAR(10),
			@retstocklocn SMALLINT,
			@return INTEGER OUTPUT
AS

	SET 	@return = 0		

	UPDATE 	lineitem
	SET 	qtydiff = 'Y',
			notes ='Replacement:' + CONVERT(varchar, GETDATE(),109) + ':' + @user
	WHERE 	acctno = @acctno
	AND 	agrmtno = @agrmtno
	--AND 		itemno = @itemno
	AND		ItemID = @itemID							--IP - 09/06/11 - CR1212 - RI
	AND 	stocklocn = @stocklocn
	AND		contractno = @contractno
	
	SELECT @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO



