SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DeleteCommissionBasisSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DeleteCommissionBasisSP]
GO
CREATE PROCEDURE 	dbo.DN_DeleteCommissionBasisSP
			@countrycode char,
			@statuscode char,
			@collecttype char,
			@empeetype varchar (4),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DELETE 
          FROM  commnbasis
         WHERE  countrycode = @countrycode
           AND  statuscode = @statuscode
           AND  collecttype = @collecttype
           AND  empeetype = @empeetype

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
