SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemBfCollectionDeleteSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemBfCollectionDeleteSP]
GO

CREATE PROCEDURE 	dbo.DN_LineItemBfCollectionDeleteSP
/***********************************************************************************************************
--
-- Project      : CoSACS .NET
-- File Name    : DN_LineItemBfCollectionDeleteSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Description  : 
--				
-- Author       : ?
-- Date         : ?
--
--  
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 06/06/11  IP  CR1212 - RI - #3806 - Use ItemID
************************************************************************************************************/
			@acctNo varchar(12),
			@agreementNo int, 
			--@itemNo varchar(8),
			@itemID int,				--IP - CR1212 - RI - #3806
			@contractNo varchar(10),
			@return int OUTPUT

AS

	SET 		@return = 0			--initialise return code

	DELETE
	FROM	LineItemBfCollection
	WHERE	acctno = @acctNo
	AND		agrmtno = @agreementNo
	--AND		itemno = @itemNo
	AND		ItemID = @itemID			--IP - CR1212 - RI - #3806
	AND		contractno = @contractNo
	
	DELETE
	FROM	agreementbfcollection
	WHERE	acctno = @acctNo

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

