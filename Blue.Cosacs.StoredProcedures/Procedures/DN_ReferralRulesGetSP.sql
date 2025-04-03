SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ReferralRulesGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ReferralRulesGetSP]
GO

CREATE PROCEDURE 	dbo.DN_ReferralRulesGetSP
			@custid varchar(20),
			@dateprop datetime,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
   create table #refer (codedescript varchar(128))

   insert into #refer select C.codedescript
	FROM		proposal RR INNER JOIN code C
	ON		RR.reason = C.code
	WHERE	RR.custid = @custid
	AND		RR.dateprop = @dateprop
   and rr.reason !=''
	AND		C.category = 'SN1'

   insert into #refer select C.codedescript
	FROM		proposal RR INNER JOIN code C
	ON		RR.reason2 = C.code
	WHERE	RR.custid = @custid
	AND		RR.dateprop = @dateprop
   and rr.reason2 !=''
	AND		C.category = 'SN1'

   insert into #refer select C.codedescript
	FROM		proposal RR INNER JOIN code C
	ON		RR.reason3 = C.code
	WHERE	RR.custid = @custid
	AND		RR.dateprop = @dateprop
   and rr.reason3 !=''
	AND		C.category = 'SN1'

   insert into #refer select C.codedescript
	FROM		proposal RR INNER JOIN code C
	ON		RR.reason4 = C.code
	WHERE	RR.custid = @custid
	AND		RR.dateprop = @dateprop
   and rr.reason4 !=''
	AND		C.category = 'SN1'

   insert into #refer select C.codedescript
	FROM		proposal RR INNER JOIN code C
	ON		RR.reason5 = C.code
	WHERE	RR.custid = @custid
	AND		RR.dateprop = @dateprop
   and rr.reason5 !=''
	AND		C.category = 'SN1'

   insert into #refer select C.codedescript
	FROM		proposal RR INNER JOIN code C
	ON		RR.reason6 = C.code
	WHERE	RR.custid = @custid
	AND		RR.dateprop = @dateprop
   and rr.reason6 !=''
	AND		C.category = 'SN1'

   select codedescript from #refer

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

