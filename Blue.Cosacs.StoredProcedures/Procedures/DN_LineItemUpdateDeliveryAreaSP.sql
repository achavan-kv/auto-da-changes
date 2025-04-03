SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemUpdateDeliveryAreaSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemUpdateDeliveryAreaSP]
GO


CREATE PROCEDURE 	dbo.DN_LineItemUpdateDeliveryAreaSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_LineItemUpdateDeliveryAreaSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Lock Account
-- Date         : ??
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 08/06/11  IP  CR1212 - RI use ItemID
-- ================================================
			@acctNo varchar(12),
			--@itemNo varchar(8),
			@itemID int,										--IP - 08/06/11 - CR1212 - RI 
			@location smallint,
			@deliveryArea varchar(16),
			@deliveryProcess varchar(2),
			@agreementno int,
			@contractno varchar(10),
			@return int OUTPUT

AS

	SET 		@return = 0			--initialise return code

	UPDATE	lineitem
	SET		deliveryarea = @deliveryArea,
			deliveryprocess = @deliveryProcess
	WHERE	acctno = @acctNo
	AND		agrmtno = @agreementno
	--AND		itemno = @itemNo
	AND		ItemID = @itemID									--IP - 08/06/11 - CR1212 - RI
	AND		stocklocn = @location
	AND		contractno = @contractno

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

