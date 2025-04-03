SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetBailiffCommissionBasisSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetBailiffCommissionBasisSP]
GO
CREATE PROCEDURE 	dbo.DN_GetBailiffCommissionBasisSP
			@empeeno int,
			@return int OUTPUT

AS


	SET 	@return = 0			--initialise return code

	SELECT 	empeeno,
		statuscode,
		collecttype,
		collectionpercent,
		commnpercent,
        reppercent,
		allocpercent,
		reposspercent,
                minvalue,
                maxvalue,
                debitaccount
	FROM    bailcommnbas
        WHERE   empeeNo = @empeeno
       

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO