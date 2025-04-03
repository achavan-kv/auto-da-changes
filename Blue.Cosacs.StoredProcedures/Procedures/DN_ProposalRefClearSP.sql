SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ProposalRefClearSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProposalRefClearSP]
GO




CREATE PROCEDURE        dbo.DN_ProposalRefClearSP
			@acctno varchar(12),
			@return int OUTPUT
AS

	SET 	@return = 0		--initialise return code
	
	DELETE FROM proposalref
	WHERE acctno = @acctno
	
	IF (@@rowcount = 0) SET @return = -1

	IF (@@error != 0) SET @return = @@error





GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

