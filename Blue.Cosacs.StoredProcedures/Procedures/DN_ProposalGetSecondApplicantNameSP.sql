SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ProposalGetSecondApplicantNameSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProposalGetSecondApplicantNameSP]
GO

CREATE PROCEDURE 	dbo.DN_ProposalGetSecondApplicantNameSP
			@custid varchar(20),
			@acctno varchar(12),
			@name varchar(50) OUT,
			@jointid varchar(20) OUT,
			@relationship char(2) OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

SELECT	TOP 1 
			@name = C.title + ' ' + C.firstname + ' ' + C.name,
			@jointid = C.custid,
			@relationship = CA.hldorjnt
	FROM Custacct CA 
	INNER JOIN	customer C ON CA.custid = C.custid
	WHERE ca.acctno = @acctno 
	AND hldorjnt = 'J'
		

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

