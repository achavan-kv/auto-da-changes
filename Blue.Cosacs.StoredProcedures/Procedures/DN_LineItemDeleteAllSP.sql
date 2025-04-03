
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemDeleteAllSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemDeleteAllSP]
GO

CREATE PROCEDURE 	dbo.DN_LineItemDeleteAllSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_LineItemDeleteAllSP.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : 
-- Author       : ??
-- Date         : ??
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 20/05/11  IP  CR1212 - RI - #3665 - Added ItemID, ParentItemID, and SalesBrnNo. Taking left(itemno, 8) when inserting into 
--				 Lineitem_Amed from Lineitem as previously received truncation error.
-- 27/05/11 jec  CR1212 RI ItemNo/ParentItemNo not required
-- ================================================
			@acctNo varchar(12),
			@agreementNo int, 
			@return int OUTPUT

AS

	SET 		@return = 0			--initialise return code
 	delete from	lineitem_amend
	WHERE	acctno = @acctNo and agrmtno = @agreementNo 

	insert INTO	lineitem_amend 
			(origbr,
			acctno,
			agrmtno,
			itemno,
			itemsupptext,
			quantity,
			delqty,
			stocklocn,
			price,
			ordval,
			datereqdel,
			timereqdel,
			dateplandel,
			delnotebranch,
			qtydiff,
			itemtype,
			--hasstring, --IP - 09/03/11 - Removed hasstring
			notes,
			taxamt,
			parentItemNo,
			parentLocation,
			isKit,
			deliveryAddress,
			contractno,
			expectedreturndate,
			ItemID,					--IP - 20/05/11 - CR1212 - #3665
			ParentItemID,			--IP - 20/05/11 - CR1212 - #3665
			SalesBrnNo)				--IP - 20/05/11 - CR1212 - #3665				
			
			select origbr,
			acctno,
			agrmtno,
			'',			-- RI
			itemsupptext,
			quantity,
			delqty,
			stocklocn,
			price,
			ordval,
			datereqdel,
			timereqdel,
			dateplandel,
			delnotebranch,
			qtydiff,
			itemtype,
			--hasstring, --IP - 09/03/11 - Removed hasstring
			notes,
			taxamt,
			'',		-- RI	--parentItemNo,
			parentLocation,
			isKit,
			deliveryAddress,
			contractno,
			expectedreturndate,
			ItemID,					--IP - 20/05/11 - CR1212 - #3665
			ParentItemID,			--IP - 20/05/11 - CR1212 - #3665
			SalesBrnNo				--IP - 20/05/11 - CR1212 - #3665
	FROM		lineitem
	WHERE	acctno = @acctNo
	AND		agrmtno = @agreementNo
-- setting quantity and ordval to 0 rather than removing...
-- this to allow the Mauritius Oracle export order numbers to stay on the database
	update
			lineitem
    set quantity = 0,
    ordval =0,taxamt = 0			
	WHERE	acctno = @acctNo
	AND		agrmtno = @agreementNo

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End