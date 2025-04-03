SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemDropTempSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemDropTempSP]
GO




CREATE PROCEDURE 	dbo.DN_LineItemDropTempSP
			@acctno varchar(12),
			@agreementno int,
			@return int OUTPUT
         
AS

	SET 	@return = 0			--initialise return code

	delete 
	from 	lineitem_amend 
	where 	acctno = @acctno
	and	agrmtno = @agreementno

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

