SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ProposalSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProposalSaveSP]
GO




-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_ProposalSaveSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Save Proposal Details
-- Author       : ??
-- Date         : ??
--
-- This procedure will save the Proposal Details
-- 
-- Change Control
-- --------------
-- Version	Date			By					Description
-- ----      --				------				-----------
-- 1.0		29/Nov/2006							//CR 866 Added additional proposal fields [PC] 
-- 2.0		20/08/07		jec					68994  - don't change the proposal date
-- 3.0		14/08/2020		Sachin Wandhare		passed @companyName parameter to DN_CustomerAddressUpdateCurrentWorkSP
-- 4.0      11/09/2020		Shubham Gaikwad		Disposable Income CR - made changes to save IsSpouseWorking Flag on S1 stage screen
-- ================================================
	-- Add the parameters for the stored procedure here
CREATE PROCEDURE 	dbo.DN_ProposalSaveSP

			@origbr smallint ,
			@custid varchar(20),
			@dateprop smalldatetime ,
			@origbranchno smallint ,
			@sanctserno int ,
			@empeenoprop int ,
			@maritalstat varchar(1) ,
			@dependants int ,			
			@yrscuremplmt float ,
			@mthlyincome float ,
			@jobslstyrs int ,
			@health char(1) ,
			@otherpmnts float ,
			@scorecardno smallint ,
			@points smallint ,
			@propresult char(1) ,
			@reason char(2) ,
			@yrscurraddr smallint ,
			@yrsprevaddr smallint ,
			@bankaccttype char(1) ,
			@yrsbankachld smallint ,
			@acctno varchar(12),
			@propnotes varchar(1000) ,
			@hasstring smallint ,
			@addincome money ,
			@appstatus char(4) ,
			@ccardno1 varchar(4) ,
			@ccardno2 varchar(4) ,
			@ccardno3 varchar(4) ,
			@ccardno4 varchar(4) ,
			@commitments1 money ,
			@commitments2 money ,
			@commitments3 money ,
			@empaddr1 varchar(50) ,
			@empaddr2 varchar(50) ,
			@empcity varchar(50) ,
			@emppostcode varchar(26),
			@empdept varchar(26) ,
			@empname varchar(26) ,
			@location varchar(4) ,
			@nationality varchar(4) ,
			@noofref int ,
			@pempmm int ,
			@pempyy int ,			
			@proofaddress varchar(4) ,
			@proofid varchar(4) ,
			@proofincome varchar(4) ,
			@s1comment varchar(1000) ,
			@s2comment varchar(1000) ,
			@specialpromo varchar(1) ,
			@a2mthlyincome money ,
			@a2addincome money ,
			@a2maritalstat varchar(4) ,
			@transactidno char(10),
			@paddress1 varchar(50) ,
			@paddress2 varchar(50) ,
			@pcity varchar(50) ,
			@ppostcode varchar(26) ,
			@paddyy int ,
			@paddmm int ,
			@presstatus varchar(4) ,
			@empeenochange int ,
			@datechange datetime ,	
			@A2Relation char(1) OUT,
			@RFCategory smallint,

			@EmploymentStatus char(1),
			@Occupation varchar(2),
			@PayFrequency char(1),
			@DateEmpStart datetime,
			@DatePEmpStart datetime,
			@EmploymentTelNo char(13),
			@EmploymentDialCode char(8),
			@BankCode varchar(6),
			@BankAccountNo varchar(20),	
			@BankAccountOpened datetime,

			@additionalexpenditure1 money,
			@additionalexpenditure2 money,
			@vehicleregistration varchar(15),
			@scoringBand varchar(4),
			--CR 866 New fields
			@TransportType TTransportType ,
			@EducationLevel TEducationLevel ,
			@DistanceFromStore smallint ,
			@Industry TIndustry ,
			@JobTitle TJobTitle ,
			@Organisation TOrganisation ,
			--End CR 866
			@DateCurrAddress DATETIME,
			@CurrentResStatus varchar(4),
			@PropertyType varchar(4),
            @PurchaseCashLoan bit,
			@IsSpouseWorking BIT,
			@return int OUTPUT
AS
	SET NOCOUNT ON
	SET 	@return = 0			--initialise return code
	
	DECLARE	@latest smalldatetime

	SELECT	@latest = MAX(dateprop)
	FROM		proposal
	WHERE	acctno = @acctno 
	AND		custid = @custid

	--CR1034
	UPDATE instalplan
	SET scoringband = @scoringBand
	WHERE acctno = @acctno
	

	IF(@latest is not null)
	BEGIN
		UPDATE	proposal
		SET		origbr = @origbr,
				custid = @custid, 
		--		dateprop = @dateprop,			68994 jec 20/08/07
				origbranchno = @origbranchno,
				sanctserno = @sanctserno,
				empeenoprop = @empeenoprop,
				maritalstat = @maritalstat,
				dependants = @dependants,
				yrscuremplmt = @yrscuremplmt,
				mthlyincome = @mthlyincome,
				jobslstyrs = @jobslstyrs,
				health = @health,
				otherpmnts = @otherpmnts,
				scorecardno = @scorecardno,
				points = @points,
				propresult = @propresult,
				reason = @reason,
				yrscurraddr	 = @yrscurraddr,
				yrsprevaddr = @yrsprevaddr,
				bankaccttype = @bankaccttype,
				yrsbankachld = @yrsbankachld,
				acctno = @acctno,
				propnotes = @propnotes,
				hasstring = @hasstring,
				AddIncome = @addincome,
				AppStatus = @appstatus,
				CCardNo1 = @ccardno1,
				CCardNo2 = @ccardno2,
				CCardNo3 = @ccardno3,
				CCardNo4 = @ccardno4,
				Commitments1 = @commitments1,
				Commitments2	= @commitments2,
				Commitments3 = @commitments3,
				EmpAddr1 = @empaddr1,
				EmpAddr2 = @empaddr2,
				EmpCity = @empcity,
				EmpDept = @empdept,
				EmpPostCode = @emppostcode,
				EmpName = @empname,
				Location = @location,
				Nationality = @nationality,				
				NoOfRef = @noofref,
				PEmpMM = @pempmm,
				PEmpYY = @pempyy,
				ProofAddress = @proofaddress,
				ProofId = @proofid,
				ProofIncome = @proofincome,
				S1Comment = @s1comment,
				S2Comment = @s2comment,
				SpecialPromo = @specialpromo,
				A2MthlyIncome = @a2mthlyincome,
				A2AddIncome = @a2addincome,
				A2MaritalStat = @a2maritalstat,
				TransactIdNo = @transactidno,
				PAddress1 = @paddress1,
				PAddress2 = @paddress2,
				PCity = @pcity,		
				PPostCode = @ppostcode,
				PAddYY = @paddyy,
				PAddMM = @paddmm,
				PResStatus = @presstatus,
				empeenochange = @empeenochange,
				datechange = getdate(),
				A2Relation = @A2Relation,
				RFCategory = @RFCategory,
				EmploymentStatus = @EmploymentStatus ,
				Occupation = @Occupation ,
				PayFrequency = @PayFrequency ,
				DateEmpStart = @DateEmpStart ,
				DatePEmpStart = @DatePEmpStart ,
				EmploymentTelNo = @EmploymentTelNo ,
				EmploymentDialCode = @EmploymentDialCode ,
				BankCode = @BankCode ,
				BankAccountNo = @BankAccountNo ,	
				BankAccountOpened = @BankAccountOpened ,
				additionalexpenditure1 = @additionalexpenditure1,
				additionalexpenditure2 = @additionalexpenditure2,
				vehicleregistration = @vehicleregistration,
				scoringBand = 	@scoringBand ,
				--CR 866 New fields
				TransportType = @TransportType,
				EducationLevel =  @EducationLevel,
				DistanceFromStore = @DistanceFromStore,
				Industry  = @Industry,
				JobTitle = @JobTitle,
				Organisation = @Organisation,
				DateCurrAddress = @DateCurrAddress, 
				CurrentResStatus = @CurrentResStatus,
				PropertyType = @PropertyType,
                PurchaseCashLoan = @PurchaseCashLoan,
				IsSpouseWorking = @IsSpouseWorking
			
		WHERE	acctno 		=	@acctno
		AND		custid 		=	@custid
		AND		dateprop	=	@latest

		EXEC DN_CustomerAddressUpdateCurrentWorkSP	@custid = @custid,
									@address1 = @empaddr1,
									@address2 = @empaddr2,
									@address3 = @empcity,
									@postcode = @emppostcode,
									@companyName = @empname,
									@user = @empeenochange,
									@return  = @return OUT
	END
	ELSE
	BEGIN
		INSERT	
		INTO		proposal
				(origbr,
				custid,
				dateprop,
				origbranchno,
				sanctserno,
				empeenoprop,
				maritalstat,
				dependants,
				yrscuremplmt,
				mthlyincome,
				jobslstyrs,
				health,
				otherpmnts,
				scorecardno,
				points,
				propresult,
				reason,
				yrscurraddr,
				yrsprevaddr,
				bankaccttype,
				yrsbankachld,
				acctno,
				propnotes,
				hasstring,
				AddIncome,
				AppStatus,
				CCardNo1,
				CCardNo2,
				CCardNo3,
				CCardNo4,
				Commitments1,
				Commitments2,
				Commitments3,
				EmpAddr1,
				EmpAddr2,
				EmpCity,
				EmpDept,
				EmpPostCode,
				EmpName,
				Location,
				Nationality,
				NoOfRef,
				PEmpMM,
				PEmpYY,
				ProofAddress,
				ProofId,
				ProofIncome,
				S1Comment,
				S2Comment,
				SpecialPromo,
				A2MthlyIncome,
				A2AddIncome,
				A2MaritalStat,
				TransactIdNo,
				PAddress1,
				PAddress2,
				PCity,
				PPostCode,
				PAddYY,
				PAddMM,
				PResStatus,
				empeenochange,
				datechange,
				A2Relation,
				RFCategory,
				EmploymentStatus  ,
				Occupation  ,
				PayFrequency  ,
				DateEmpStart  ,
				DatePEmpStart  ,
				EmploymentTelNo  ,
				EmploymentDialCode  ,
				BankCode  ,
				BankAccountNo  ,	
				BankAccountOpened ,
				additionalexpenditure1,
				additionalexpenditure2,
				vehicleregistration,
				scoringBand ,
				TransportType , --CR 866
				EducationLevel, --CR 866
				DistanceFromStore, --CR 866
				Industry , --CR 866
				JobTitle , --CR 866
				Organisation,  --CR 866
				DateCurrAddress,
				CurrentResStatus,
				PropertyType,
                PurchaseCashLoan,
				IsSpouseWorking
				)
		VALUES	(@origbr  ,
				@custid ,
				@dateprop  ,
				@origbranchno  ,
				@sanctserno  ,
				@empeenoprop  ,
				@maritalstat  ,
				@dependants  ,			
				@yrscuremplmt  ,
				@mthlyincome  ,
				@jobslstyrs  ,
				@health  ,
				@otherpmnts  ,
				@scorecardno  ,
				@points  ,
				@propresult  ,
				@reason  ,
				@yrscurraddr  ,
				@yrsprevaddr  ,
				@bankaccttype  ,
				@yrsbankachld  ,
				@acctno ,
				@propnotes  ,
				@hasstring  ,
				@addincome  ,
				@appstatus  ,
				@ccardno1  ,
				@ccardno2  ,
				@ccardno3  ,
				@ccardno4  ,
				@commitments1 ,
				@commitments2  ,
				@commitments3  ,
				@empaddr1  ,
				@empaddr2  ,
				@empcity  ,
				@empdept  ,
				@emppostcode ,
				@empname  ,
				@location  ,
				@nationality  ,
				@noofref  ,
				@pempmm  ,
				@pempyy  ,			
				@proofaddress  ,
				@proofid  ,
				@proofincome  ,
				@s1comment  ,
				@s2comment  ,
				@specialpromo  ,
				@a2mthlyincome  ,
				@a2addincome  ,
				@a2maritalstat  ,
				@transactidno,
				@paddress1  ,
				@paddress2  ,
				@pcity  ,
				@ppostcode  ,
				@paddyy  ,
				@paddmm  ,
				@presstatus  ,
				@empeenochange  ,
				getdate(),
				@A2Relation,
				@RFCategory,
				@EmploymentStatus ,
				@Occupation ,
				@PayFrequency ,
				@DateEmpStart ,
				@DatePEmpStart ,
				@EmploymentTelNo ,
				@EmploymentDialCode ,
				@BankCode ,
				@BankAccountNo ,	
				@BankAccountOpened,
				@additionalexpenditure1,
				@additionalexpenditure2,
				@vehicleregistration,
				@scoringBand,
				@TransportType, --CR 866
				@EducationLevel, --CR 866
				@DistanceFromStore, --CR 866
				@Industry, --CR 866
				@JobTitle, --CR 866
				@Organisation,
				@DateCurrAddress,
				@CurrentResStatus,
				@PropertyType,
                @PurchaseCashLoan,
				@IsSpouseWorking)
	END

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


