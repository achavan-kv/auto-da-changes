SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ScheduleSetDeliveryPrintedSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ScheduleSetDeliveryPrintedSP]
GO


CREATE PROCEDURE dbo.DN_ScheduleSetDeliveryPrintedSP
			@acctno varchar(12),
			@buffno int,
			@buffbranchno smallint,
			@user int,
			@return int OUTPUT

AS
	SET 	@return = 0			--initialise return code

	UPDATE	schedule 
	SET	printedby = @user,
		dateprinted = GETDATE()
	WHERE	acctno = @acctno
	AND		buffno = @buffno
	AND		@buffbranchno = (CASE WHEN ISNULL(retstocklocn,0) = 0 THEN stocklocn ELSE retstocklocn END)
	
	UPDATE	order_removed 
	SET		dateprinted = GETDATE()
	WHERE	acctno = @acctno
	AND		buffno = @buffno
	AND		stocklocn = @buffbranchno

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

