SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_ScoringBandForAccountSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ScoringBandForAccountSP]
GO

-- 14-Dec-2006 Changed script to put in blank band for loyalty club members
CREATE PROCEDURE dbo.DN_ScoringBandForAccountSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_ScoringBandForAccountSP
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : DN_ScoringBandForAccountSP
-- Author       : ?
-- Date         : ?
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 30/09/10  ip   UAT(43) UAT5.4 - Added isnull check around @CurrentBand and @ScoringBand

-- ====================================================
    @AcctNo         CHAR(12),
    @CurrentBand    VARCHAR(4) OUTPUT,
    @ScoringBand    VARCHAR(4) OUTPUT,
    @points smallint = -1 OUTPUT ,
    @scoretype CHAR(1) = 'X',
    @Return         INTEGER OUTPUT

AS DECLARE

    @custId             VARCHAR(20),
    
    @loyaltyBand        VARCHAR(4),
    @loyaltyDesc        VARCHAR(64),
    @bandSC             FLOAT,
    @loyaltySC          FLOAT

BEGIN
    --
    -- Terms Type Scoring Band
    --

    SET NOCOUNT ON
    SET @Return = 0

    SET @CurrentBand = ''
    SET @ScoringBand = ''
    SET @loyaltyBand = ''
    SET @loyaltyDesc = ''
    SET @bandSC = 0
    SET @loyaltySC = 0


    SELECT  @custId = Custid
    FROM    CustAcct
    WHERE   AcctNo = @AcctNo
    AND     HldOrJnt = 'H'


	IF (@scoretype = 'X')
	BEGIN
	-- No paramter passed in.
		SELECT @CurrentBand = ISNULL(scoringband,'')-- AA  getting this from instalplan for Accounts
		FROM dbo.instalplan WHERE acctno = @acctno	--IP - 30/09/10 - UAT(43) UAT5.4
		SELECT @ScoringBand = ISNULL(ScoringBand,'')	--IP - 30/09/10 - UAT(43) UAT5.4
		FROM customer
		WHERE custid = @custid
	END
		ELSE
	BEGIN
	-- Normal procedure 

    -- If revising a proposal then the same band @CurrentBand should be used again
    if @points = -1 --normal proposal
    begin
		SELECT  @CurrentBand = ISNULL(ScoringBand,''),
				@ScoringBand = ISNULL(ScoringBand,''),
				@points      = Points
		FROM    Proposal
		WHERE   AcctNo = @AcctNo order by dateprop DESC 
	end 
	ELSE -- end of day rescore use 
	BEGIN
	SELECT  @CurrentBand = ISNULL(p.ScoringBand,'') --,
			/*@ScoringBand = ISNULL(B.ScoringBand,''),
				@points      = b.Points*/
		FROM    Proposal p
		WHERE   p.AcctNo = @AcctNo 
		--AND b.acctno= p.acctno 
		order by p.dateprop DESC 
	END 


	-- Calculate what the new band @ScoringBand should now be
    IF (ISNULL(@points,0) > 0)
    BEGIN
        -- This is a proposal that has just been scored,
        -- or an old proposal scored before bands were introduced.
      -- Work out the appropriate band for the score.
	     SELECT  @ScoringBand = b.Band,
	             @bandSC = b.ServiceCharge
        FROM    CountryMaintenance c, TermsTypeBand b,customer
         WHERE   c.CodeName = 'CountryCode'
        AND     b.CountryCode = c.Value
        AND     b.StartDate = (SELECT MAX(b2.StartDate) FROM TermsTypeBand b2
                               WHERE  ISNULL(b2.StartDate,'') > CONVERT(DATETIME,'1-Jan-1900',106)
                               AND    b2.StartDate <= GETDATE()
                               AND b2.scoretype = @scoretype)
        AND     @points BETWEEN b.PointsFrom AND b.PointsTo
        --and isnull(customer.scorecardtype,'A') = b.scoretype 
        AND b.scoretype = @scoretype -- using input parameter for behavioural scoring. 
        AND customer.custid =@custId
    END

    -- Otherwise the band used when this customer was last scored on HP/RF
    IF ISNULL(@ScoringBand,'') = ''
    BEGIN
        SELECT  @ScoringBand = ScoringBand
        FROM    Customer
        WHERE   CustId = @custId

        SELECT  @bandSC = b.ServiceCharge
        FROM    CountryMaintenance c, TermsTypeBand b
        WHERE   c.CodeName = 'CountryCode'
        AND     b.CountryCode = c.Value
        AND     b.StartDate = (SELECT MAX(b2.StartDate) FROM TermsTypeBand b2
                               WHERE  ISNULL(b2.StartDate,'') > CONVERT(DATETIME,'1-Jan-1900',106)
                               AND    b2.StartDate <= GETDATE())
        AND     b.Band = @ScoringBand
    END


    -- Check for a more favourable Loyalty Club band
    EXEC DN_CustomerIsPrivilegeClubMemberSP
        @custid,
        @loyaltyBand OUT,
        @loyaltyDesc OUT,
        @return OUT

    IF (@loyaltyBand = 'TIR1' OR @loyaltyBand = 'TIR2')
    BEGIN
		-- Loyalty bands are not held in the matrix - only on the terms types
		SELECT  @loyaltySC = t.ServPcent
		FROM    Acct a, TermsTypeAllBands t
		WHERE   a.AcctNo = @acctNo
		AND     t.TermsType = a.TermsType
		AND     t.Band = @loyaltyBand

		IF (@loyaltySC > 0 AND @loyaltySC <= ISNULL(@bandSC, @loyaltySC))
			SET @ScoringBand = @loyaltyBand
		ELSE
			SET @ScoringBand = '' --Scoring band should be blank if the customer has loyalty club [Ref: Alex/ Nikki - 2006-12-14] 
    END
    ELSE
		-- Otherwise the country default band
		IF ISNULL(@ScoringBand,'') = ''
		BEGIN
			SELECT @ScoringBand = Value 
			FROM CountryMaintenance
			WHERE CodeName = 'TermsTypeBandDefault'
		END
END


    SET @Return = @@error

    SET NOCOUNT OFF
    RETURN @Return
END
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO


