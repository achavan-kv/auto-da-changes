/*
    Author:    John Croft
    Date:      May 2006

    This procedure Deletes the Payment file definitions
    The table is maintained in the Payment File Definition screen

*/

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_PaymentFileDefnDeleteSP]') 
        and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_PaymentFileDefnDeleteSP]
GO



CREATE PROCEDURE 	dbo.DN_PaymentFileDefnDeleteSP

    @bankname varchar(16),
    @return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	Delete StorderControl
		where bankname = @bankname


	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO



