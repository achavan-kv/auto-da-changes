SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_ProposalOverrideBandSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProposalOverrideBandSP]
GO

CREATE PROCEDURE dbo.DN_ProposalOverrideBandSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_ProposalOverrideBandSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : ??
-- Author       : ??
-- Date         : ??
--
-- This procedure will ??
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 17/09/10 jec Additional parameters not supplied to DN_ScoringBandForAccountSP
-- ================================================
	-- Add the parameters for the stored procedure here
    @acctno char(12),
    @newBand varchar(4),
	@user INT, --IP - CR916 - 10/04/08 - Added @user as this is needed when writing record to ProposalAudit table.
    @return int OUTPUT

AS DECLARE
    @currentBand varchar(4)
    
    SET NOCOUNT ON
    SET @return = 0
    
    IF (LTRIM(ISNULL(@newBand,'')) = '')
    BEGIN
        -- The new band is blank so calculate it from the current score
        EXEC DN_ScoringBandForAccountSP @AcctNo=@AcctNo, @currentBand =@CurrentBand out,
         @ScoringBand =@newBand out, @points=null, @scoretype=null, @return=@Return out		
    END
    
    UPDATE  proposal
    SET     scoringBand = @newBand
    WHERE   acctno = @acctno
    
    -- 69544 Customer table to be updated with new band as well
--    UPDATE  customer
--    SET     scoringBand = @newBand
--    WHERE   custid = (SELECT custid FROM proposal WHERE acctno = @acctno)

	--IP - 10/11/08 - (70362)- If an account was created for a customer
	--and then the account was linked to a different custid
	--there are two records in the 'Proposal' table for the account for the different custid's
	--therefore previously there was no join to custacct and would 
	--return more than one result from the sub-query.
	--(IP - 11/11/08 - I have commented this fix out for the time being and should be 
	--re-instated if this scenario re-ocurrs).
	UPDATE  customer
    SET     scoringBand = @newBand
    WHERE   custid = (SELECT distinct ca.custid FROM proposal p inner join custacct ca
						ON p.acctno = ca.acctno
						and ca.hldorjnt = 'H' WHERE p.acctno = @acctno)

	--IP - 10/11/08 - (70362)- If an account was created for a customer
	--and then the account was linked to a different custid
	--there are two records in the 'Proposal' table for the account for the different custid's
	--therefore previously there was no join to custacct and would 
	--return more than one result from the sub-query.
	--(IP - 11/11/08 - I have commented this fix out for the time being and should be 
	--re-instated if this scenario re-ocurrs).
--	UPDATE  customer
--    SET     scoringBand = @newBand
--    WHERE   custid = (SELECT distinct ca.custid FROM proposal p inner join custacct ca
--						ON p.acctno = ca.acctno
--						and ca.hldorjnt = 'H' WHERE p.acctno = @acctno)

	--IP - 10/04/08 - CR916 - A record would need to be inserted into ProposlAudit table for the account
	-- as the Score Band has been overriden with a new Score Band.

	 insert into proposalaudit --- 
        (custid,dateprop,empeenoprop,maritalstat,
        dependants,yrscuremplmt,mthlyincome,jobslstyrs,
        otherpmnts,scorecardno,points,propresult,
        yrscurraddr,yrsprevaddr,bankaccttype,yrsbankachld,
        acctno,empeenoscored,datescored,AddIncome,
        Commitments1,Commitments2,Commitments3,A2MthlyIncome,
        A2AddIncome, A2MaritalStat,PResStatus,A2Relation,
        RFCategory,Bankruptcies,TimeSinceLawsuit,NumberLawsuits,
        MobilePhone,additionalexpenditure1,additionalexpenditure2,PayFrequency,
        BankAccountOpened,EmploymentStatus,BankCode,Occupation,
        SystemRecommendation,reason1,reason2,reason3,
        reason4,reason5,reason6, ScoringBand, DateScoreBandChanged, ScoreBandChangedBy --IP - CR916 - 10/04/08 - Added 'ScoringBand', 'DateScoreBandChanged', 'ScoreBandChangedBy'
        ) 
    select custid,dateprop,empeenoprop,maritalstat,
        dependants,yrscuremplmt,mthlyincome,jobslstyrs,
        otherpmnts,scorecardno,points,propresult,
        yrscurraddr,yrsprevaddr,bankaccttype,yrsbankachld,
        acctno,empeenochange,getdate(),AddIncome,
        Commitments1,Commitments2,Commitments3,A2MthlyIncome,
        A2AddIncome, A2MaritalStat,PResStatus,A2Relation,
        RFCategory,Bankruptcies,TimeSinceLawsuit,NumberLawsuits,
        MobilePhone,additionalexpenditure1,additionalexpenditure2,PayFrequency,
        BankAccountOpened,EmploymentStatus,BankCode,Occupation,
        SystemRecommendation,reason,reason2,reason3,
        reason4,reason5,reason6, @newBand, GETDATE(), @user
    from proposal where acctno =@acctno

		--IP - 10/11/08 - (70362)- Need to select the record
		--from proposal table for the custid currently linked to the account
		--whom is the holder of the account.
		--(IP - 11/11/08 - I have commented this fix out for the time being and should be 
		--re-instated if this scenario re-ocurrs).
--	  select p.custid,dateprop,empeenoprop,maritalstat,
--        dependants,yrscuremplmt,mthlyincome,jobslstyrs,
--        otherpmnts,scorecardno,points,propresult,
--        yrscurraddr,yrsprevaddr,bankaccttype,yrsbankachld,
--        p.acctno,empeenochange,getdate(),AddIncome,
--        Commitments1,Commitments2,Commitments3,A2MthlyIncome,
--        A2AddIncome, A2MaritalStat,PResStatus,A2Relation,
--        RFCategory,Bankruptcies,TimeSinceLawsuit,NumberLawsuits,
--        MobilePhone,additionalexpenditure1,additionalexpenditure2,PayFrequency,
--        BankAccountOpened,EmploymentStatus,BankCode,Occupation,
--        SystemRecommendation,reason,reason2,reason3,
--        reason4,reason5,reason6, @newBand, GETDATE(), @user
--    from proposal p inner join custacct ca
--	on p.acctno = ca.acctno
--	and ca.hldorjnt = 'H'
--	and p.custid = ca.custid 
--	where p.acctno =@acctno
    
    SET @return = @@error

    SET NOCOUNT OFF
    RETURN @Return

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End 