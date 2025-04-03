SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO
if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ProposalCloneSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProposalCloneSP]
GO

/****** Object:  StoredProcedure [dbo].[DN_ProposalCloneSP]    Script Date: 11/05/2007 11:37:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE PROCEDURE 	[dbo].[DN_ProposalCloneSP]

--------------------------------------------------------------------------------
--
-- Project      : eCoSACS ? 2003 Strategic Thought Ltd.
-- File Name    : DN_ProposalCloneSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Clone a new Proposal 
-- Author       : ??
-- Date         : ??
--
--
-- Change Control
-- --------------
-- Date    ByDescription
-- ----    -------------
-- 19/11/08 jec 70435 Set date changed to proposal date and clear propresult.
-- 20/05/10 ip UAT(176) UAT5.2.1.0 Log - If the previous RF account has been referred then copy the propreason to the new cloned proposal.
--									   - Only insert the 'R' ProposalFlag record if the previous RF accounts propresult is 'R'.
-- 02/03/11 ip #2807 - CR1090 - Set the InstalmentWaived flag for an RF sub agreement if it qualifies based on points.
-- 08/03/11 ip Sprint 5.11 - #3285 - Points must meet the minimum in country parameter (>=) to qualify for first instalment waiver
-- 10/03/11 ip Sprint 5.11 - #3291 - For sub RF accounts, when cloning the proposal select the scorecard type from customer table as we do currently for score band
-- 17/08/11 ip #4562 - UAT51 - selecting and inserting DateCurrAddress for the cloned proposal. Caused a SQL Overflow error when completing Sanction Stage 2 as this column was null
-- 19/08/11 ip #4562 - UAT51 - if the DateCurrAddress from most recent proposal is null, then set this from custaddress
-- 17/01/13 ip #12038 - UAT181 - Linking existing account to a new Customer incorrectly
--					 created new proposal record. Now updating Customer ID to the new 
--					 Customer ID on all relevant tables.
--------------------------------------------------------------------------------

    -- Parameters
			@custid varchar(20),
			@acctno varchar(12),
			@rescore bit OUT,
			@cloned bit out,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	set @cloned = 0

	DECLARE	@oldDate datetime
	DECLARE	@newDate SMALLDATETIME
	DECLARE @propResult CHAR(1)			--IP - 20/05/10 - UAT(176) UAT5.2.1.0 Log

	SET	@newDate = getdate()

	DECLARE @scoringband VARCHAR(2), @scoreCard char(1) --IP - 10/03/11 - #3291 - Added @scoreCardType
	SELECT @scoringband = scoringband FROM customer 
	WHERE custid = @custid
	
	--IP - 10/03/11 - #3291 
	SELECT @scoreCard = ScoreCardType FROM customer
	WHERE custid = @custid

	--IP - 17/01/13 - #12038
	-- If linking the account to a different customer
	IF EXISTS(SELECT * FROM custacct 
				WHERE acctno = @acctno
				AND hldorjnt = 'H'
				AND custid != @custid)
	BEGIN
		
		DECLARE @prevCustId VARCHAR(20)
		SELECT @prevCustId = custid FROM custacct WHERE acctno = @acctno
									AND hldorjnt = 'H'
		UPDATE proposal 
		SET custid = @custid
		WHERE acctno = @acctno
		AND custid = @prevCustId
		
		UPDATE proposalflag
		SET custid = @custid
		WHERE Acctno = @acctno
		AND custid = @prevCustId
		
	END
	ELSE
	BEGIN
		-- get most recent account for customer but not this one
			SELECT	TOP 1 @oldDate = P.dateprop,
						  @propResult = p.propresult
			FROM		proposal P INNER JOIN
					acct A ON P.acctno = A.acctno	INNER JOIN
					accttype AT ON A.accttype = AT.accttype 
			WHERE	AT.genaccttype = 'R'
			AND		P.custid = @custid AND p.acctno !=@acctno 
			ORDER BY	P.dateprop DESC

			IF(@@rowcount>0)
			BEGIN
				set @cloned =1
				/*Copy the most recent proposal record	*/
				INSERT	
				INTO		proposal
						(origbr,custid,dateprop,origbranchno,sanctserno,empeenoprop,
						maritalstat,dependants,yrscuremplmt,mthlyincome,jobslstyrs,health,
						otherpmnts,scorecardno,points,scoringband,propresult,reason,yrscurraddr,yrsprevaddr,
						bankaccttype,yrsbankachld,acctno,propnotes,hasstring,notes,empeenochange,
						datechange,AddIncome,AppStatus,CCardNo1,CCardNo2,CCardNo3,CCardNo4,
						Commitments1,Commitments2,Commitments3,EmpAddr1,EmpAddr2,EmpCity,
						EmpDept,EmpName,Location,Nationality,NoOfRef,PEmpMM,PEmpYY,
						S1Comment,S2Comment,SpecialPromo,A2MthlyIncome,A2AddIncome,
						A2MaritalStat,TransactIdNo,PAddress1,PAddress2,PCity,PPostCode,PAddYY,	
						PAddMM,PResStatus,A2Relation,RFCategory,EmpPostCode,PayFrequency,
						Occupation, EmploymentStatus, DateEmpStart, DatePEmpStart, 
						EmploymentDialCode, EmploymentTelNo, BankAccountOpened, scorecard, DateCurrAddress) --IP - 17/08/11 - #4562 - UAT51 - Added DateCurrAddress --IP - 10/03/11 - #3291
				SELECT	TOP 1
						P.origbr,P.custid,@newDate,P.origbranchno,P.sanctserno,P.empeenoprop,
						P.maritalstat,P.dependants,P.yrscuremplmt,P.mthlyincome,P.jobslstyrs,P.health,
						--P.otherpmnts,P.scorecardno,P.points,P.scoringband,P.propresult,P.reason,P.yrscurraddr,P.yrsprevaddr,
						--P.otherpmnts,P.scorecardno,P.points,P.scoringband,P.propresult,' ',P.yrscurraddr,P.yrsprevaddr,	-- 70435 jec 19/11/08
						P.otherpmnts,P.scorecardno,P.points,@scoringband,P.propresult,CASE WHEN p.propresult = 'R' THEN p.reason ELSE '' END ,P.yrscurraddr,P.yrsprevaddr,	-- 70435 jec 19/11/08 --IP - 20/05/10 - UAT(176) UAT5.2.1.0 Log
						P.bankaccttype,P.yrsbankachld,@acctno,P.propnotes,P.hasstring,P.notes,P.empeenochange,
						--P.datechange,P.AddIncome,P.AppStatus,P.CCardNo1,P.CCardNo2,P.CCardNo3,P.CCardNo4,
						@newDate,P.AddIncome,P.AppStatus,P.CCardNo1,P.CCardNo2,P.CCardNo3,P.CCardNo4,	-- 70435 jec 19/11/08
						P.Commitments1,P.Commitments2,P.Commitments3,P.EmpAddr1,P.EmpAddr2,P.EmpCity,
						P.EmpDept,P.EmpName,P.Location,P.Nationality,P.NoOfRef,P.PEmpMM,P.PEmpYY,
						P.S1Comment,P.S2Comment,P.SpecialPromo,P.A2MthlyIncome,P.A2AddIncome,
						P.A2MaritalStat,P.TransactIdNo,P.PAddress1,P.PAddress2,P.PCity,P.PPostCode,P.PAddYY,	
						P.PAddMM,P.PResStatus,P.A2Relation,P.RFCategory,P.EmpPostCode, P.PayFrequency,
						P.Occupation, P.EmploymentStatus, P.DateEmpStart, P.DatePEmpStart,
						P.EmploymentDialCode, P.EmploymentTelNo, P.BankAccountOpened, @scoreCard, 
						case when p.DateCurrAddress is null																			--IP - 19/08/11 - #4562
							then (select datein from custaddress where custid = @custid and addtype = 'H' and datemoved is null) 
							else  p.DateCurrAddress end--IP - 17/08/11 - #4562 - UAT51 - Added DateCurrAddress --IP - 10/03/11 - #3291
				FROM		proposal P INNER JOIN
						acct A ON P.acctno = A.acctno	INNER JOIN
						accttype AT ON A.accttype = AT.accttype 
				WHERE	AT.genaccttype = 'R'
				AND		P.custid = @custid
				AND NOT EXISTS (SELECT * FROM dbo.proposal pp WHERE pp.acctno = @acctno AND pp.custid =@custid) -- prevent duplicate key on insert

				ORDER BY	P.dateprop DESC

				/*Copy the proposal flags from the old proposal	*/
				INSERT
				INTO		proposalflag
						(origbr, custid, dateprop, checktype, datecleared, empeenopflg, acctno)
				SELECT	MAX(origbr), custid, @newDate, checktype, MAX(datecleared), MAX(empeenopflg), @acctno
				FROM		proposalflag
				WHERE	custid = @custid 
				AND		dateprop = @oldDate
				AND		NOT EXISTS(Select * FROM proposalflag dup				--RM - check for duplicate flags
									where dup.custid = proposalflag.custid
									and dup.acctno = @acctno
									and dup.checktype = proposalflag.checktype
									and dup.dateprop = @newdate)
				AND		NOT EXISTS(SELECT * FROM proposalflag f1				--IP - 20/05/10 - UAT(176) UAT5.2.1.0 Log
									WHERE f1.custid = proposalflag.custid
									AND f1.dateprop = proposalflag.dateprop
									AND f1.checktype = 'R'
									AND f1.checktype = proposalflag.checktype
									AND @propResult != 'R')
				GROUP BY custid,checktype
				/* check the customer's RF accounts to see if they have been inactive for the last year.
					if so,  set stage 1 and 2 of the cloned proposal to incomplete as well as DC */
				EXEC	DN_CustomerRFInactiveSP 	@custid = @custid, 
									@inactive = @rescore OUT,
									@return = @return OUT

				IF(@rescore = 1)
				BEGIN
					EXEC	DN_ProposalFlagUnClearFlagSP 	@acctno = @acctno,
										@checkType = 'S1',
										@changeStatus = 1,
										@return = @return OUT
				END	
				ELSE
				BEGIN
					--IP - 02/03/11 - #2807 - CR1090 -- If a re-score is not required then need to set the InstalmentWaived flag
					--based on the points of the cloned proposal
					DECLARE @firstInstWaiverPoints int
					select @firstInstWaiverPoints = value from countrymaintenance where codename = 'InstalWaiverMinScore'
					
					IF(@firstInstWaiverPoints!=0)
					BEGIN
						DECLARE @points int
						select @points = points from proposal where acctno = @acctno
						IF(@points >= @firstInstWaiverPoints) --IP - 08/03/11 - #3285 points must be >=
						BEGIN
							UPDATE instalplan 
							SET InstalmentWaived = 1
							WHERE acctno = @acctno
						END
					END
				END	
			
				/*Update some of them to be not cleared	*/
				UPDATE	proposalflag
				SET		datecleared = null
				WHERE	acctno = @acctno
				AND		checktype IN ('DC')

				/* Copy the referral records from the old proposal	*/
				IF NOT EXISTS (SELECT * FROM referral WHERE custid = @custid AND dateprop = @olddate)
				INSERT
				INTO		referral
						(origbr, custid, dateprop, reflresult, empeeno, datereferral)
				SELECT	TOP 1origbr, custid, @newDate, reflresult, empeeno, datereferral
				FROM		referral
				WHERE	custid = @custid
				AND		dateprop = @oldDate

				/*Update the status of the new account to 1 unless it's S1 stage has been re-opened -- unless new account UAT from 5184*/
				IF(@rescore = 0 AND @propResult !='' )
				BEGIN
					UPDATE	acct
					SET		currstatus = 1
					WHERE	acctno = @acctno
				END

			
			END

	END

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END



GO 

