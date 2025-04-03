SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_TermsTypeFreeInstallmentSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TermsTypeFreeInstallmentSaveSP]
GO


CREATE PROCEDURE 	dbo.DN_TermsTypeFreeInstallmentSaveSP
			@termstype varchar(4),
			@intratefrom datetime,
			@intrateto datetime,
			@datefrom datetime,
			@dateto datetime,
			@month int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	INSERT 
	INTO		TermsTypeFreeInstallments
			(termstype, intratefrom, intrateto, datefrom, dateto, month)
	VALUES	(@termstype, @intratefrom, @intrateto, @datefrom, @dateto, @month)

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

