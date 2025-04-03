SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DeliveryUpdateReturnItemNoSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DeliveryUpdateReturnItemNoSP]
GO

CREATE PROCEDURE 	dbo.DN_DeliveryUpdateReturnItemNoSP
			@acctno varchar(12),
			@agreementno int,
			@locn smallint,
			@itemId int,
			@contractno varchar(10),
			@returnitemId int,
			@retlocn smallint,
			--uat363 ADD parentitemno
			@parentItemId int,
			@return int OUTPUT
AS

	SET 	@return = 0			--initialise return code

	UPDATE	delivery
	SET		RetItemID = @returnitemId,
			retstocklocn = @retlocn
	WHERE	acctno = @acctno
	AND		agrmtno = @agreementno
	AND		stocklocn = @locn
	AND		ItemID = @itemId
	AND		contractno = @contractno
	AND		quantity < 0
	AND		ParentItemID = @parentItemId
	
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

