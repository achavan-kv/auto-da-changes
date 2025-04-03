SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ProposalGetDatePropSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProposalGetDatePropSP]
GO


CREATE PROCEDURE 	dbo.DN_ProposalGetDatePropSP
			@acctno varchar(12),
			@custid varchar(20),
			@dateprop smalldatetime OUT,	
			@return int OUTPUT

AS

	SET @return = 0

	SELECT	TOP 1
			@dateprop = P.dateprop
	FROM	proposal P
	WHERE	P.acctno = @acctno
	ORDER BY P.dateprop DESC

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

