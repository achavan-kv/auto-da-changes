SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DeliveryNoteSchdulePrintedSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DeliveryNoteSchdulePrintedSP]
GO

CREATE PROCEDURE 	dbo.DN_DeliveryNoteSchdulePrintedSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_DeliveryNoteSchdulePrintedSP.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : 
-- Author       : ??
-- Date         : ??
--
-- This procedure will retrieve the value before collection.
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 18/05/11  IP/NM 	RI Integration changes - CR1212 - #3627 - use itemID
-- ================================================
			@acctNo varchar(12),
			@agrmtno int,
			--@itemNo varchar(8), 
			@itemID int,							--IP/NM - 18/05/11 -CR1212 - #3627 	
			@stocklocn smallint,
			@delnoteprinted smallint OUT, 
			@onpicklist smallint OUT, 
			@onload smallint OUT,
			@return int OUTPUT
AS

	SET 	@return = 0			--initialise return code

	DECLARE	@picklistno int,
			@datepickprinted datetime,
			@dateprinted datetime,
			@load smallint

	SELECT	@picklistno = picklistnumber, 
			@datepickprinted = ISNULL(datepicklistprinted, '1/1/1900'),
			@dateprinted = ISNULL(dateprinted, '1/1/1900'),
			@load = ISNULL(loadno, 0)
	FROM	schedule s 
	WHERE	s.acctno = @acctno
	--AND		s.itemno = @itemno
	AND		s.ItemID = @itemID							--IP/NM - 18/05/11 -CR1212 - #3627 		
	AND		s.stocklocn = @stocklocn
	AND		s.agrmtno = @agrmtno

	IF(@picklistno > 0 OR @datepickprinted > '1/1/1900')
		SET @onpicklist = 1
	ELSE
		SET @onpicklist = 0

	IF(@load > 0)
		SET @onload = 1
	ELSE
		SET @onload = 0

	IF(@dateprinted > '1/1/1900')
		SET @delnoteprinted = 1
	ELSE
		SET @delnoteprinted = 0

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
