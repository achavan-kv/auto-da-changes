SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FactTransDeleteSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FactTransDeleteSP]
GO

CREATE PROCEDURE 	dbo.DN_FactTransDeleteSP
			@acctno varchar(12),
			@itemno varchar(18),
			@stocklocn smallint,
			@tccode varchar(2),
			@trantype varchar(3),
			@rowcount int OUTPUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DELETE
	FROM		facttrans
	WHERE	acctno	= @acctno
	and		trantype =  @trantype
	and		itemno =  @itemno
	and		stocklocn =  @stocklocn
	and		TCCode = @tccode

	SET @rowcount = @@rowcount

	SET @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

