SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AcctNoCtrlSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AcctNoCtrlSaveSP]
GO


CREATE PROCEDURE 	dbo.DN_AcctNoCtrlSaveSP
			@branchno smallint,
			@acctcat varchar(3),		
			@acctcatdesc varchar(25),
			@hiallocated int,
			@hiallowed int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	AcctNoCtrl
	SET		acctcatdesc = @acctcatdesc,
			hiallocated = @hiallocated,
			hiallowed = @hiallowed
	WHERE	branchno = @branchno
	  AND   acctcat = @acctcat

	IF(@@rowcount = 0)
	BEGIN
		INSERT
		INTO	AcctNoCtrl
				(origbr, branchno, acctcat, acctcatdesc, hiallocated, hiallowed)
		VALUES	(0,@branchno, @acctcat, @acctcatdesc, @hiallocated, @hiallowed)
	END	

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

