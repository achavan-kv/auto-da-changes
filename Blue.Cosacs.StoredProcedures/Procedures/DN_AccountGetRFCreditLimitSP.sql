/* Credit 67589 - Correcting max spend limit issue 
19-sep-2005 AA removing comment*/
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_AccountGetRFCreditLimitSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountGetRFCreditLimitSP]
GO

CREATE PROCEDURE dbo.DN_AccountGetRFCreditLimitSP
            @country char(2),
            @custid varchar(20),
            @dateprop smalldatetime,
            @score int,
            @region varchar(3),
            @creditlimit money OUT,
            @scorecard CHAR(1),
            @return int OUTPUT

AS
    /* 25/05/2004 jason duff change to reduce credit limit if 
    ** automatically rescored and customer in good status */

    SET     @return = 0            --initialise return code

    DECLARE @category varchar(12),
            @monthlygross money,
            @monthlyrent money, 
            @expenses money,
            @ratio smallint,    
            @income money,
            @oldlimit money,
            @currstatus char(1),
		     @maxRFIncomeRatio money,
           @rfscore smallint

    --change for rescore to prevent customer getting lower limit letter if null passed in.
    if @creditlimit is null set @creditlimit = 0


    SELECT  @monthlygross = isnull(P.mthlyincome, 0) + isnull(P.addincome, 0) , /* isnull (P.A2MthlyIncome,0) + isnull (P.A2AddIncome,0) ,Joint  income should be used to determine credit limit  but not sole with spouse UAT 144*/
            @category = ISNULL(C.category,'PCF'),
            @expenses = isnull(P.commitments1, 0) + isnull(P.commitments2, 0) + isnull(P.commitments3, 0) + isnull(P.otherpmnts, 0) +
            isnull(additionalexpenditure2,0) + isnull(additionalexpenditure1,0)
    FROM    proposal P 
    LEFT JOIN code C ON CAST(P.rfcategory as varchar(4)) = C.code 
    AND     C.category in ('PCF', 'PCO', 'PCE', 'PCW')
    WHERE   P.custid = @custid
    AND     P.dateprop = @dateprop
    
    -- increase monthly gross by second applicant only if joint account, but not if sole with spouse... 
	SELECT @monthlygross = @monthlygross + isnull (P.A2MthlyIncome,0) + isnull (P.A2AddIncome,0) 
	FROM proposal p
	JOIN custacct ca ON p.acctno = ca.acctno 
	WHERE p.custid = @custid AND p.dateprop = @dateprop AND ca.hldorjnt = 'J'

    SELECT  @monthlyrent = isnull (mthlyrent,0)
    FROM    custaddress 
    WHERE   (datemoved is null or datemoved ='1-Jan-1900')                                           
    AND     custid=@custid 
    AND     addtype= 'H'

    SET @income = @monthlygross - @expenses - @monthlyrent
--    select  @income as income
    SELECT    @ratio = maxRFIncomeRatio
    FROM      country
    WHERE     countrycode = @country

    if @ratio = 1000 -- not implementing
			set @maxRFIncomeRatio = 10000000			
    else
			set @maxRFIncomeRatio = @income * @ratio

    
    IF(@@rowcount = 0)
    BEGIN
        SET @category = 'PCF'    --default to furniture
    END        

    IF ((@category = 'PCF') or (@category = 'PCO'))
    BEGIN
        SELECT  TOP 1
                @creditlimit = furnlimit
        FROM    rfcreditlimit rf
        WHERE   countrycode = @country
        AND     region = @region
        AND     score = (SELECT MAX(Score) FROM rfcreditlimit rf1
                        WHERE rf1.countrycode = rf.countrycode
                        AND rf1.ScoreType = rf.ScoreType
						AND rf1.dateimported = rf.dateimported
                        AND rf1.score <=@score)
        AND     income = (SELECT MAX(income) FROM rfcreditlimit rfi
                        WHERE rfi.countrycode = rf.countrycode
                        AND rfi.ScoreType = rf.ScoreType
                        AND rfi.dateimported = rf.dateimported
                        AND rfi.income <=@income)               /* redo CR462 */
        AND     dateimported =      -- DSR 5/10/04 Ignore history rcords
                (SELECT MAX(dateimported)
                 FROM   rfcreditlimit
                 WHERE  countrycode = @country
                 AND    region = @region AND scoretype = @scorecard )
        AND     ScoreType = @scorecard          
        ORDER BY furnlimit DESC  -- DSR 5/10/04 Order on income as well
        if @creditlimit > @maxRFIncomeRatio and @maxRFIncomeRatio !=10000000       /* redo CR503 */
        begin
          SELECT  TOP 1
                  @creditlimit = furnlimit
          FROM    rfcreditlimit rf
          WHERE   countrycode = @country
          AND     region = @region
           AND     score = (SELECT MAX(Score) FROM rfcreditlimit rf1
                        WHERE rf1.countrycode = rf.countrycode
                        AND rf1.ScoreType = rf.ScoreType
						AND rf1.dateimported = rf.dateimported
                        AND rf1.score <=@score)
          AND     furnlimit <= @maxRFIncomeRatio                 /* redo CR462 */
          AND     dateimported =      -- DSR 5/10/04 Ignore history rcords
                (SELECT MAX(dateimported)
                 FROM   rfcreditlimit
                 WHERE  countrycode = @country
                 AND    region = @region AND scoretype = @scorecard )
           AND     ScoreType = @scorecard       
          ORDER BY furnlimit DESC  -- DSR 5/10/04 Order on income as well
        end
     --select  @income * @ratio as incomeratio
    END

    IF ((@category = 'PCE') or (@category = 'PCW'))
    BEGIN
        SELECT  TOP 1
                @creditlimit = isnull(eleclimit,0)
        FROM    rfcreditlimit rf
        WHERE   countrycode = @country
        AND     region = @region
        AND     score = (SELECT MAX(Score) FROM rfcreditlimit rf1
                        WHERE rf1.countrycode = rf.countrycode
                        AND rf1.ScoreType = rf.ScoreType
						AND rf1.dateimported = rf.dateimported
                        AND rf1.score <=@score)
        AND     income = (SELECT MAX(income) FROM rfcreditlimit rfi
                        WHERE rfi.countrycode = rf.countrycode
                        AND rfi.ScoreType = rf.ScoreType
						AND rfi.dateimported = rf.dateimported
                        AND rfi.income <=@income)                     /* redo CR462 */
        AND     dateimported =      -- DSR 5/10/04 Ignore history rcords
                (SELECT MAX(dateimported)
                 FROM   rfcreditlimit
                 WHERE  countrycode = @country
                 AND    region = @region AND scoretype = @scorecard )
        AND     ScoreType = @scorecard          
        ORDER BY eleclimit desc  -- 
        if @creditlimit > @maxRFIncomeRatio and @maxRFIncomeRatio !=10000000       /* redo CR503 */
        begin
          SELECT  TOP 1
                  @creditlimit = eleclimit
          FROM    rfcreditlimit rf
          WHERE   countrycode = @country
          AND     region = @region
          AND     score = (SELECT MAX(Score) FROM rfcreditlimit rf1
                        WHERE rf1.countrycode = rf.countrycode
                        AND rf1.ScoreType = rf.ScoreType
						AND rf1.dateimported = rf.dateimported
                        AND rf1.score <=@score)
          AND     eleclimit <= @maxRFIncomeRatio                 /* redo CR462 */
          AND     dateimported =      -- DSR 5/10/04 Ignore history rcords
                (SELECT MAX(dateimported)
                 FROM   rfcreditlimit
                 WHERE  countrycode = @country
                 AND    region = @region AND scoretype = @scorecard )
          AND     ScoreType = @scorecard        
          ORDER BY eleclimit DESC  -- DSR 5/10/04 Order on income as well
        end
     --select  @income * @ratio as incomeratio

    END


        
    -- if this is a automated eod re-score then do not change credit limit. UAT 76 5184 removing
    /*select  @oldlimit =isnull(c.rfcreditlimit, 0)
    from    customer c,customer_rescore r 
    where   c.custid =r.custid and r.date_rescore  > dateadd (day, - 1, getdate())
    and     c.custid =@custid
	
    if @oldlimit > 0 and @creditlimit >= 0
    begin
        select  @currstatus = isnull (Max (acct.currstatus),'')
        from    acct, custacct
    *    where   acct.acctno = custacct.acctno
        and     custacct.hldorjnt= 'H' 
        and     custacct.acctno = acct.acctno 
        and     acct.currstatus not in ('S','U')
        and     custacct.custid = @custid

        if  @currstatus not in ('4', '5', '6', '7', '8') 
    *  and @creditlimit<@oldlimit --do not reduce credit limit if customer is in good status
            set @creditlimit=@oldlimit
        else 
            SELECT    @creditlimit = isnull(@creditlimit, 0)
    end       
    else
        SELECT @creditlimit = isnull(@creditlimit, 0)
     */   

    -- now checking whether limit has been changed manually 
    declare @manuallimit money 
    select  @manuallimit = isnull (newlimit,0)
    from    customerRFlimitoverride 
    where   custid = @custid
    and     datechanged =
                (select Max(datechanged) from customerRFlimitoverride f
                 where f.custid = customerRFlimitoverride.custid)
    and     datechanged > dateadd (month, - 4, getdate()) -- recently as want limit to be reduced if customer goes bad

    if @manuallimit != @creditlimit and @manuallimit > 0
           select  @creditlimit = @manuallimit
      
    SET @return = @@error
GO 

GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


