--
-- Database procedure to load instant credit flags for Instant Credit Authorisation by account
--
--
-- Modified On		BY				Comment
-- 30-Jan-2011		Ruth McQueeney	Created from dn_holdflagsgetsp
--
--

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


IF EXISTS (SELECT * FROM dbo.sysobjects 
	   WHERE id = object_id('DN_ICFlagsGetSP') 
	   AND OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE DN_ICFlagsGetSP
Go

/****** Object:  StoredProcedure [dbo].[DN_ICFlagsGetSP]    Script Date: 11/05/2007 11:04:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE 	[dbo].[DN_ICFlagsGetSP]
			@acctno varchar(12),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	F.checktype as 'Flag',
		convert(varchar(20), F.datecleared, 6) as 'Date Cleared',
		F.empeenopflg as 'By',
		c.codedescript AS 'Description'
	FROM	InstantCreditFlag F, code c 
	WHERE	F.acctno = @acctno
	AND f.checktype = c.code 
	AND c.category = 'ICF' 
	UNION 
	SELECT C.code,P.datecleared, p.empeenopflg,c.codedescript
	FROM code c, proposalflag P 
	WHERE p.Acctno = @acctno
	AND p.checktype = 'DC' AND p.checktype= c.code 
	AND C.CATEGORY='PH1' AND p.datecleared IS NULL 

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO 

 