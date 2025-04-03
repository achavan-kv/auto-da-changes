SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ProposalFlagDeleteSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProposalFlagDeleteSP]
GO


/****** Object:  StoredProcedure [dbo].[DN_ProposalFlagDeleteSP]    Script Date: 11/05/2007 11:53:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE  [dbo].[DN_ProposalFlagDeleteSP]
			@checktype varchar(4),
			@acctno char(12),
   			@return int OUTPUT
 
AS
 
 	SET  @return = 0   --initialise return code

	DELETE
	FROM		proposalflag 
	WHERE	acctno = @acctno
	AND		CheckType	= @checktype

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END


