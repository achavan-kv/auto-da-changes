SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO


if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_ProposalWriteResultSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProposalWriteResultSP]
GO


CREATE PROCEDURE dbo.DN_ProposalWriteResultSP
--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_ProposalWriteResultSP.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        :
-- Author       : ??
-- Date         : ??
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 28/06/11  IP  5.13 - LW73619 - #3751 - If holdprop = 'N' (accounts sanction stage 1 was re-opened and not revised
--				 need to remove entry from delauthorise which would mean it will no longer appear in Incomplete Credit screen
--------------------------------------------------------------------------------
    @custid VARCHAR(20),
    @acctno VARCHAR(12),
    @dateprop smalldatetime,
    @points smallint,
    @propresult char(1),
    @reason char(2),
    @reason2 char(2),
    @reason3 char(2),
    @reason4 char(2),
    @reason5 char(2),
    @reason6 char(2),
    @notes VARCHAR(1000),
    @user int,
    @newBand varchar(4),
	@scorecard CHAR(1),
	@AccountBand varchar(4),
    @return int OUTPUT

AS
    declare @dlength integer,
            @currentBand       VARCHAR(4),
            @holdprop char(1)		--IP - 5.13 - LW73619 - #3751


    SET NOCOUNT ON
    SET @return = 0

    select @dlength = len (@notes)


    UPDATE  proposal
    SET     points = @points,
            ScoringBand = @AccountBand,        -- required to calculate a new band
            propresult = @propresult,
            reason = @reason,
            reason2 = @reason2,
            reason3 = @reason3,
            reason4 = @reason4,
            reason5 = @reason5,
            reason6 = @reason6,
            systemrecommendation = @propresult,
            propnotes = @notes + left (propnotes, 1000 -@dlength),
            scorecard =ISNULL(@scorecard,'A')
    WHERE   custid = @custid
    AND     dateprop = @dateprop
    AND     acctno = @acctno
    
	-- Added logic to only set ScoringBand to NULL when an agreement
	-- does not have a ServiceChg, preventing the problem of a customers
	-- band changing and their ServiceCharge being recalculated
	--UPDATE proposal  Taking out as determining this in Bproposal
	--SET ScoringBand = null
	--	FROM proposal
	--	INNER JOIN agreement
	--		ON proposal.acctno =  agreement.acctno
	--		WHERE ServiceChg = 0 
	--		and proposal.custid = @custid
	--		AND     proposal.dateprop = @dateprop
	--		AND     proposal.acctno = @acctno
	
	
    -- Don't Calculate the new scoring band as doing through the new application
    --EXEC DN_ScoringBandForAccountSP @AcctNo= @AcctNo, @currentBand = @CurrentBand out, @ScoringBand = @newBand out, @points = @points,@scoretype = @scorecard,
   --- @return = @Return out
    
    --69442 Should only be setting a scoring band for an account that has been opened since terms types were introduced
    DECLARE @startDate DATETIME
    SET @startDate = (SELECT MIN(datefrom) FROM intratehistory WHERE Band <> '')
    
 --   UPDATE  Proposal
 --   SET     ScoringBand = @newBand
 --   WHERE   custid = @custid
 --   AND     dateprop = @dateprop
 --   AND     acctno = @acctno
	--AND		ScoringBand IS NULL
	--AND     dateprop >= @startDate
	--and     @propresult not in ('X') leave bands in in case customer reopened

    
    -- The last band awarded is stored on the customer
    UPDATE  Customer
    SET     ScoringBand = @newBand
    WHERE   custid = @custid
	--and     @propresult not in ('X') leave bands in in case customer reopened
    

   delete from proposalaudit where custid =@custid and datescored =getdate()

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
        reason4,reason5,reason6, ScoringBand, DateScoreBandChanged, ScoreBandChangedBy -- IP -CR916 - 10/04/08  - Added 'ScoringBand', 'DateScoreBandChanged', 'ScoreBandChangedBy'
        ) 
    select top 1 custid,dateprop,empeenoprop,maritalstat,
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
        reason4,reason5,reason6,@accountband, GETDATE(), @user --IP - CR916 - 10/04/08 - new values inserted into ProposalAudit for ScoreBand
    from proposal where custid =@custid and dateprop=@dateprop
    and not exists (select * from proposalaudit p where p.custid = @custid
    and p.acctno =@acctno and p.datescored =getdate())

	--IP - 5.13 - LW73619 - #3751 
	select @holdprop = holdprop from agreement where acctno = @acctno
	
	if(@propresult = 'A' and @holdprop = 'N')
	begin
		exec dbremoveauth @acctno
	end
	
    SET @return = @@error

    SET NOCOUNT OFF
    RETURN @Return


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

