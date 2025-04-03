SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemGetWarrantiesOnCreditValueSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemGetWarrantiesOnCreditValueSP]
GO

CREATE PROCEDURE 	dbo.DN_LineItemGetWarrantiesOnCreditValueSP
			@acctno varchar(12),
			@value money OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	@value = isnull(sum(LI.ordval),0)
	FROM		lineitem LI INNER JOIN stockitem SI
	ON		LI.itemno = SI.itemno
	AND		LI.stocklocn = SI.stocklocn INNER JOIN acct A
	ON		LI.acctno = A.acctno
	WHERE	LI.acctno = @acctno 
	AND		SI.category in (select distinct code from code where category = 'WAR')
	AND		A.termstype = 'WC'

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET ANSI_NULLS ON 
GO

