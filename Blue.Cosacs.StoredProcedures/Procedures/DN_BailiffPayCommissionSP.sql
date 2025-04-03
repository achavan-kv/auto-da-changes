SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_BailiffPayCommissionSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_BailiffPayCommissionSP]
GO

CREATE PROCEDURE 	dbo.DN_BailiffPayCommissionSP
			@empeeno int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	
    INSERT INTO dbo.bailiffCommnPaid
        ( empeeno ,
          transrefno ,
          acctno ,
          datetrans ,
          transvalue ,
          chequecolln ,
          DatePaid
        )

    select 
          empeeno ,
          transrefno ,
          acctno ,
          datetrans ,
          transvalue ,
          chequecolln ,
          getdate()
    FROM 	bailiffcommn
	WHERE	empeeno = @empeeno
	AND	status  = 'P'



	DELETE 
	FROM 	bailiffcommn
	WHERE	empeeno = @empeeno
	AND	status  = 'P'

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO	