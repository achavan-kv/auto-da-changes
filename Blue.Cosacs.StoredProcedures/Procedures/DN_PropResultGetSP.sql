SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_PropResultGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_PropResultGetSP]
GO





CREATE PROCEDURE 	dbo.DN_PropResultGetSP
			@acctno varchar(12),
			@adcomment varchar(200) OUT,
			@adreqd  varchar(1) OUT,
			@decision varchar(4) OUT,
			@finaldec varchar(4) OUT,
			@manualrefer varchar(1) OUT,
			@override varchar(4) OUT,
			@policyrule1 varchar(4) OUT,
			@policyrule2 varchar(4) OUT,
			@policyrule3 varchar(4) OUT,
			@policyrule4 varchar(4) OUT,
			@policyrule5 varchar(4) OUT,
			@policyrule6 varchar(4) OUT,
			@riskcat varchar(1) OUT,
			@score int OUT,
			@sysrecommend varchar(1) OUT,
			@uwcomment varchar(200) OUT,
			@warning varchar(50) OUT,
			@curnumber int OUT,
			@curworst varchar(1) OUT,
			@setnumber int OUT,
			@setworst varchar(1) OUT,
			@prodcat varchar(2) OUT,
			@prodcode varchar(8) OUT,
			@summarydata varchar(999) OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	@adcomment = adcomment,
			@adreqd = adreqd,
			@decision = decision,
			@finaldec = finaldec,
			@manualrefer = manualrefer,
			@override = override,
			@policyrule1 = policyrule1,
			@policyrule2 = policyrule2,
			@policyrule3 = policyrule3,
			@policyrule4 = policyrule4,
			@policyrule5 = policyrule5,
			@policyrule6 = policyrule6,
			@riskcat = riskcat,
			@score = score,
			@sysrecommend = sysrecommend,
			@uwcomment = uwcomment,
			@warning = warning,
			@curnumber = curnumber,
			@curworst = curworst,	
			@setnumber = setnumber,
			@setworst = setworst,
			@prodcat = prodcat,
			@prodcode = prodcode,
			@summarydata = summarydata
	FROM		propresult
	WHERE	acctno = @acctno

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END




GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

