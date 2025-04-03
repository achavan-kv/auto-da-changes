SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ProposalSaveReferralNotesSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProposalSaveReferralNotesSP]
GO



CREATE PROCEDURE 	dbo.DN_ProposalSaveReferralNotesSP
			@custid varchar(20),
			@acctno varchar(12),
			@dateprop smalldatetime,
			@notes varchar(1000),
			@creditLimit money,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	proposal
	SET		propnotes = @notes
	WHERE	custid = @custid
	AND		acctno = @acctno
	AND		dateprop = @dateprop

	UPDATE	customer
	SET		RFCreditLimit = @creditLimit
	WHERE	custid = @custid

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

