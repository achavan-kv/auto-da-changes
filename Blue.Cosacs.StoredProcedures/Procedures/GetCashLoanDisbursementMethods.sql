SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[GetCashLoanDisbursementMethods]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[GetCashLoanDisbursementMethods]
GO

CREATE PROCEDURE 	dbo.GetCashLoanDisbursementMethods
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	
        fpm.Category,
        fpm.Code,
        fpm.codedescript, 
        fpm.statusflag,
        fpm.sortorder,
        fpm.reference,
        fpm.additional,
        fpm.additional
    FROM
        Code fpm
    INNER JOIN 
        Code cdm on fpm.code = cdm.code
    WHERE
        fpm.category = 'FPM'
        and cdm.category = 'CDM'
            

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
 
