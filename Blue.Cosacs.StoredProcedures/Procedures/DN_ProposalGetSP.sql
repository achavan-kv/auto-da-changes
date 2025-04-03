SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ProposalGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProposalGetSP]
GO




--[27/Nov/2006] CR 866 Added 6 fields to this SP
CREATE PROCEDURE 	dbo.DN_ProposalGetSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_ProposalGetSP
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Return proposal details 
-- Author       : ??
-- Date         : ??
--
-- This procedure will return proposal details for a customers account
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 02/03/11  IP  Sprint 5.11 - #3226 - DBNULL error encountered. Added ISNULL check.
-- 12/08/13  IP  #10477 - LW75062 - Prevent users from re-opening S1 if not most recent RF account
-- 09/10/20 Dipsosable Income CR change : Saved / added IspouseWorking Flag
-- ================================================
			@origbr smallint OUT,
			@custid varchar(20),
			@dateprop datetime OUT,
			@origbranchno smallint OUT,
			@sanctserno int OUT,
			@empeenoprop int OUT,
			@maritalstat varchar(1) OUT,
			@dependants int OUT,			
			@yrscuremplmt float OUT,
			@mthlyincome float OUT,
			@jobslstyrs int OUT,
			@health char(1) OUT,
			@otherpmnts float OUT,
			@scorecardno smallint OUT,
			@points smallint OUT,
			@propresult char(1) OUT,
			@reason char(2) OUT,
			@yrscurraddr smallint OUT,
			@yrsprevaddr smallint OUT,
			@bankaccttype char(1) OUT,
			@yrsbankachld smallint OUT,
			@acctno varchar(12),
			@propnotes varchar(1000) OUT,
			@hasstring smallint OUT,
			@addincome money OUT,
			@appstatus char(4) OUT,
			@ccardno1 varchar(4) OUT,
			@ccardno2 varchar(4) OUT,
			@ccardno3 varchar(4) OUT,
			@ccardno4 varchar(4) OUT,
			@commitments1 money OUT,
			@commitments2 money OUT,
			@commitments3 money OUT,
			@empaddr1 varchar(50) OUT,
			@empaddr2 varchar(50) OUT,
			@empcity varchar(50) OUT,
			@emppostcode varchar(26) OUT,
			@empdept varchar(26) OUT,
			@empname varchar(26) OUT,
			@location varchar(4) OUT,
			@nationality varchar(4) OUT,
			@noofref int OUT,
			@pempmm int OUT,
			@pempyy int OUT,			
			@proofaddress varchar(4) OUT,
			@proofid varchar(4) OUT,
			@proofincome varchar(4) OUT,
			@s1comment varchar(1000) OUT,
			@s2comment varchar(1000) OUT,
			@specialpromo varchar(1) OUT,
			@a2mthlyincome money OUT,
			@a2addincome money OUT,
			@a2maritalstat varchar(4) OUT,
			@transactidno char(10) OUT,
			@paddress1 varchar(50) OUT,
			@paddress2 varchar(50) OUT,
			@pcity varchar(50) OUT,
			@ppostcode varchar(26) OUT,
			@paddyy int OUT,
			@paddmm int OUT,
			@presstatus varchar(4) OUT,
			@empeenochange int OUT,
			@datechange datetime OUT,		
			@A2Relation char(1) OUT,	
			@RFCategory smallint OUT,
			@outAcctno varchar(12) OUT,
			
			@EmploymentStatus char(1) OUT,
			@Occupation varchar(2) OUT,
			@PayFrequency char(1) OUT,
			@DateEmpStart datetime OUT,
			@DatePEmpStart datetime OUT,
			@EmploymentTelNo char(13) OUT,
			@EmploymentDialCode char(8) OUT,
			@BankCode varchar(6) OUT,
			@BankAccountNo varchar(20) OUT,	
			@BankAccountOpened datetime OUT,
			
			@additionalexpenditure1 money OUT,
			@additionalexpenditure2 money OUT,
			@vehicleregistration varchar(15) OUT,
			@scoringBand varchar(4) OUT,
			--CR 866 New fields
			@TransportType TTransportType OUT,
			@EducationLevel TEducationLevel OUT,
			@DistanceFromStore smallint OUT,
			@Industry TIndustry OUT,
			@JobTitle TJobTitle OUT,
			@Organisation TOrganisation OUT,
			@DateCurrAddress DATETIME OUT,
			@CurrentResStatus varchar(4) OUT,
			@PropertyType varchar(4) OUT,
			@AllowReopenS1 bit OUT,					--#10477
            @PurchaseCashLoan BIT OUT,
			@IsSpouseWorking BIT OUT,
			--End CR 866

			@return int OUTPUT
AS

	SET 	@return = 0			--initialise return code

	DECLARE @secondapp varchar(20)
	DECLARE @type char(1), 
			@origAcctno varchar(12), @reopenS1 bit	--#10477

	select @origAcctno = @acctno					--#10477

	/* 	if this is an RF account we should return the latest proposal, not 
		necessarily the proposal they have asked for		*/
	SELECT	@type = AT.accttype
	FROM		accttype AT INNER JOIN
			acct A ON A.accttype = AT.genaccttype
	WHERE	A.acctno = @acctno

	IF(@type = 'R')
	BEGIN
		SELECT	TOP 1
				@acctno = P.acctno
		FROM		proposal P INNER JOIN
				acct A ON P.acctno = A.acctno INNER JOIN
				accttype AT ON A.accttype = AT.genaccttype
		WHERE	P.custid = @custid
		AND		AT.accttype = 'R'
		ORDER BY	P.dateprop DESC
	END	

	if(@origAcctno != @acctno)					--#10477
	begin
		set @reopenS1 = 0
	end
	else
	begin
		set @reopenS1 = 1
	end

	SELECT	top 1 
			@origbr = origbr, 
			@dateprop = dateprop ,
			@origbranchno = origbranchno ,
			@sanctserno  = sanctserno ,
			@empeenoprop = empeenoprop ,
			@maritalstat = maritalstat ,
			@dependants = dependants ,			
			@yrscuremplmt = yrscuremplmt ,
			@mthlyincome = mthlyincome ,
			@jobslstyrs  = jobslstyrs ,
			@health = health ,
			@otherpmnts = otherpmnts ,
			@scorecardno = scorecardno ,
			@points = points ,
			@propresult = propresult ,
			@reason = reason ,
			@yrscurraddr = yrscurraddr ,
			@yrsprevaddr = yrsprevaddr ,
			@bankaccttype = bankaccttype ,
			@yrsbankachld = yrsbankachld ,
			@propnotes = propnotes ,
			@hasstring = hasstring ,
			@addincome = addincome ,
			@appstatus = appstatus ,
			@ccardno1  = ccardno1 ,
			@ccardno2 = ccardno2 ,
			@ccardno3 = ccardno3 ,
			@ccardno4 = ccardno4 ,
			@commitments1  = commitments1 ,
			@commitments2 = commitments2 ,
			@commitments3 = commitments3 ,
			@empaddr1 = empaddr1 ,
			@empaddr2 = empaddr2 ,
			@empcity = empcity ,
			@emppostcode = emppostcode ,
			@empdept = empdept ,
			@empname = empname ,
			@location = location,
			@nationality = nationality ,
			@noofref = noofref ,
			@pempmm = pempmm ,
			@pempyy = pempyy ,			
			@proofaddress = proofaddress ,
			@proofid = proofid ,
			@proofincome = proofincome ,
			@s1comment = s1comment ,
			@s2comment = s2comment ,
			@specialpromo = specialpromo ,
			@a2mthlyincome = a2mthlyincome ,
			@a2addincome = a2addincome ,
			@a2maritalstat = a2maritalstat ,
			@transactidno = transactidno,
			@paddress1 = paddress1 ,
			@paddress2 = paddress2 ,
			@pcity = pcity ,
			@ppostcode = ppostcode ,
			@paddyy = paddyy ,
			@paddmm = paddmm ,
			@presstatus =presstatus ,
			@empeenochange = empeenochange ,
			@datechange = datechange ,
			@A2Relation = A2Relation,
			@RFCategory = RFCategory,
			@outAcctno = acctno,
			@EmploymentStatus = EmploymentStatus,
			@Occupation = Occupation,
			@PayFrequency = PayFrequency,
			@DateEmpStart = DateEmpStart,
			@DatePEmpStart = DatePEmpStart,
			@EmploymentTelNo = EmploymentTelNo,
			@EmploymentDialCode = EmploymentDialCode,
			@BankCode = BankCode,
			@BankAccountNo = BankAccountNo,
			@BankAccountOpened = BankAccountOpened,
			@additionalexpenditure1 = additionalexpenditure1,
			@additionalexpenditure2 = additionalexpenditure2,
			@vehicleregistration = vehicleregistration,
			@scoringBand = scoringBand,
			--CR 866 New fields
			@TransportType = TransportType,
			@EducationLevel =  EducationLevel,
			@DistanceFromStore = DistanceFromStore,
			@Industry  = Industry,
			@JobTitle = JobTitle,
			@Organisation = Organisation,
			@DateCurrAddress = isnull(DateCurrAddress,'1-Jan-1900'), 
			@CurrentResStatus = isnull(CurrentResStatus,''), --	IP - 02/03/11 - #3226
			@PropertyType = isnull(PropertyType,''), --	IP - 02/03/11 - #3226
			@AllowReopenS1 = @reopenS1,					 --#10477
            @PurchaseCashLoan = isnull(PurchaseCashLoan,0),
			@IsSpouseWorking = ISNULL(IsSpouseWorking,0)
	FROM		proposal
	WHERE	acctno = @acctno
	AND		custid = @custid
	ORDER BY	dateprop desc

	IF(@@rowcount=0)
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


