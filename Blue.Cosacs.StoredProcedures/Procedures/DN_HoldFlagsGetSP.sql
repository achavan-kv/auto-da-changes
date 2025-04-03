--
-- Database procedure to load proposalflag details for Delivery Authorisation by account
--
--
-- Modified On		BY		Comment
-- 23-Mar-2005		Rupal Desai	Procedure modified to add missing link to custacct table 66215
--
--

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


IF EXISTS (SELECT * FROM dbo.sysobjects 
	   WHERE id = object_id('DN_HoldFlagsGetSP') 
	   AND OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE DN_HoldFlagsGetSP
Go

/****** Object:  StoredProcedure [dbo].[DN_HoldFlagsGetSP]    Script Date: 11/05/2007 11:04:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE 	[dbo].[DN_HoldFlagsGetSP]
			@acctno varchar(12),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	PF.checktype as 'Flag',
		convert(varchar(20), PF.datecleared, 6) as 'Date Cleared',
		PF.empeenopflg as 'By'
	FROM	proposalflag PF,
		proposal P,
		custacct C
	WHERE	C.acctno = @acctno
	AND	C.custid = P.custid
	AND	P.acctno = C.acctno
	AND	C.hldorjnt = 'H'		-- RD 66215 added to enusre only holder record is loaded
	AND	P.acctno = PF.acctno

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

