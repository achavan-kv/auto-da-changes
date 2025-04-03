SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountUpdateBalStatusSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountUpdateBalStatusSP]
GO



CREATE PROCEDURE 	dbo.DN_AccountUpdateBalStatusSP
			@acctno varchar(12),
			@status char(1),
			@outstbal money,
                                        @user integer,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	acct
	SET		currstatus = @status,
			outstbal = @outstbal,
                                        lastupdatedby =@user
	WHERE	acctno = @acctno

	SET @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

