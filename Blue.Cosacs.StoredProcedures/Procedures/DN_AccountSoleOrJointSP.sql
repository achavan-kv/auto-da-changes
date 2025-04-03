SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountSoleOrJointSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountSoleOrJointSP]
GO






CREATE PROCEDURE 	dbo.DN_AccountSoleOrJointSP
			@acctno varchar(12),
			@jntcustid varchar(20) OUT,
			@hldorjnt varchar(1) ,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	TOP 1
			@jntcustid = custid,
			@hldorjnt = hldorjnt
	FROM		custacct
	WHERE	hldorjnt = @hldorjnt
	AND		acctno = @acctno

	IF(@@rowcount = 0)
		SET	@return = -1

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END





GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

