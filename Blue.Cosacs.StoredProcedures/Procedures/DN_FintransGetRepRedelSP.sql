SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FintransGetRepRedelSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FintransGetRepRedelSP]
GO


CREATE PROCEDURE  dbo.DN_FintransGetRepRedelSP
   			@acctno VARCHAR(12),
   			@return int OUTPUT
 
AS
 
 	SET  @return = 0   --initialise return code

	SELECT	AcctNo,
        			TransRefNo,
        			DateTrans,
			TransTypeCode,
	             		empeeno,
			TransUpdated,
	             		TransPrinted,
			TransValue,
        			ChequeNo,
        			Bankacctno,
        			Bankcode,
        			FTNotes,
        			PayMethod,
        			Source,
        			runno
	FROM 		fintrans
	WHERE 	AcctNo = @acctNo 
	AND 		(transtypecode=N'REP'or transtypecode = 'RDL')

 	IF (@@error != 0)
 	BEGIN
  		SET @return = @@error
 	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

