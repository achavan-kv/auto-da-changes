SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ProposalNotesSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProposalNotesSaveSP]
GO



CREATE PROCEDURE 	dbo.DN_ProposalNotesSaveSP
			@custid varchar(20),
			@acctno varchar(12),
			@dateprop smalldatetime,
			@notes varchar(1000),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	proposal
	SET		propnotes = @notes
	WHERE	custid = @custid
	AND		acctno = @acctno
	AND		dateprop = @dateprop

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

