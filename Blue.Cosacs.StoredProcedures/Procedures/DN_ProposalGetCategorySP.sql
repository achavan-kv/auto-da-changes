SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ProposalGetCategorySP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProposalGetCategorySP]
GO




CREATE PROCEDURE dbo.DN_ProposalGetCategorySP
			@acctno varchar(12),
			@category char(1) OUT,
			@return int OUTPUT

AS
		SET 	@return = 0			--initialise return code

		SELECT	@category = rfcategory
		FROM 		proposal x
		WHERE 	acctno = @acctno
		AND 		dateprop = (SELECT MAX(dateprop)
					      FROM proposal y
					      WHERE x.acctno = y.acctno)

		IF (@@error != 0)
		BEGIN
			SET @return = @@error
		END



GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

