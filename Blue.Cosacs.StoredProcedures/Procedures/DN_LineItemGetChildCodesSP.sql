SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemGetChildCodesSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemGetChildCodesSP]
GO

CREATE PROCEDURE 	dbo.DN_LineItemGetChildCodesSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_LineItemGetChildCodesSP.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        :  
-- Author       : ??
-- Date         : ??
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 16/05/11 ip  RI Integration changes - CR1212 - #3627 - joined onto StockInfo to substitute itemno for IUPC
-- ================================================
			@acctNo varchar(12),
			@itemId int,
			@location smallint,
			@agreementNo int,
			@return int OUTPUT
AS

	SET 	@return = 0	

	SELECT	s.IUPC as itemno, l.stocklocn, l.contractno, l.itemId
	FROM	lineitem l 
	INNER JOIN stockinfo s ON l.ItemId = s.ID
	WHERE	acctno = @acctNo
	AND		ParentItemID = @itemId
	AND		parentLocation = @location
	AND		agrmtno = @agreementNo
	order by s.itemtype desc
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

