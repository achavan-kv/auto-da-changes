SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DeleteBailiffCommissionBasisSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DeleteBailiffCommissionBasisSP]
GO
CREATE PROCEDURE 	dbo.DN_DeleteBailiffCommissionBasisSP
			@empeeno int,
			@statuscode char,
			@collecttype char,
			
			@return int OUTPUT

AS


	SET 	@return = 0			--initialise return code

	DELETE  
          FROM  bailcommnbas
         WHERE  empeeno = @empeeno
           AND  statuscode = @statuscode
           AND  collecttype = @collecttype
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO 