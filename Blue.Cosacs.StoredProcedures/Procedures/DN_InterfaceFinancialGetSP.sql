SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_InterfaceFinancialGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_InterfaceFinancialGetSP]
GO

CREATE PROCEDURE 	dbo.DN_InterfaceFinancialGetSP
			@runno int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	runno,
			sum(transvalue) as transvalue,
			interfaceaccount,
			branchno 
	FROM		interface_financial
	WHERE	runno = @runno
	GROUP BY 	interfaceaccount, branchno, runno 


	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

