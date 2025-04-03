SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AgreementUpdateHoldPropSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AgreementUpdateHoldPropSP]
GO

CREATE PROCEDURE 	dbo.DN_AgreementUpdateHoldPropSP
			@acctNo varchar(12),
			@agreementNo int,
			@holdProp char(1),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE 	agreement
	SET		holdprop = @holdProp
	WHERE	acctno = @acctNo
	AND		agrmtno = @agreementNo

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

