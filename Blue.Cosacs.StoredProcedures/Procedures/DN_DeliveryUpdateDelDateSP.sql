SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DeliveryUpdateDelDateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DeliveryUpdateDelDateSP]
GO

CREATE PROCEDURE 	dbo.DN_DeliveryUpdateDelDateSP
			@acctno varchar(12),
			@agreementno int,
			@locn smallint,
			@itemId int,
			@contractno varchar(10),
			@deldate datetime,
			@return int OUTPUT
AS

	SET 	@return = 0			--initialise return code

	SET @deldate = CONVERT(SMALLDATETIME, CONVERT(VARCHAR(10), @deldate, 105), 105)

	UPDATE	delivery
	SET		datedel = @deldate
	WHERE	acctno = @acctno
	AND		agrmtno = @agreementno
	AND		stocklocn = @locn
	AND		itemId = @itemId
	AND		contractno = @contractno
	AND		delorcoll = 'D'
	
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

