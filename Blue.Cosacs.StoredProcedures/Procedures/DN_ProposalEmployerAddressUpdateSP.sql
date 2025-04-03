SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ProposalEmployerAddressUpdateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProposalEmployerAddressUpdateSP]
GO

CREATE PROCEDURE 	dbo.DN_ProposalEmployerAddressUpdateSP
			@custid varchar(20),
			@address1 varchar(50),
			@address2 varchar(50),
			@address3 varchar(50),
			@postcode varchar(10),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	/* the employment address is recorded at sanction stage 2 therefore the 
	    proposal record should only be updated if stage 2 is not yet complete 
	    for reach proposal */	
	UPDATE	proposal 
	SET		empaddr1 = @address1,
			empaddr2 = @address2,
			empcity = @address3,
			emppostcode = @postcode
	WHERE	custid = @custid
	AND		NOT EXISTS (	SELECT	1 
					FROM		proposalflag PF 
					WHERE	PF.custid = @custid 
					AND		PF.dateprop = proposal.dateprop
					AND		PF.checktype = 'S2'
					AND		PF.datecleared is not null )

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

