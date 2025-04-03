SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_SpaGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SpaGetSP]
GO

CREATE PROCEDURE 	dbo.DN_SpaGetSP
			@acctno varchar(12),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	S.AcctNo,
			S.AllocNo,
			S.ActionNo,
			S.DateAdded,
			S.Code,
			ISNULL(C.CodeDescript,' ') as ReasonCodeDesc,
			S.DateExpiry,
			S.SpaInstal,
		    S.EmpeeNo,
			S.EmpeeNoSpa
	FROM 		Spa S
			LEFT OUTER JOIN Code C
			     ON S.Code = C.Code
			     AND C.Category = 'SP2'
	WHERE 	S.AcctNo = @acctno
	ORDER BY	S.DateAdded DESC

	SET @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

