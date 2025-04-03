
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_AccountGetRFPotentialCreditLimitSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountGetRFPotentialCreditLimitSP]
GO

CREATE PROCEDURE dbo.DN_AccountGetRFPotentialCreditLimitSP
            @country char(2),
            @custid varchar(20),
            @dateprop datetime,
            @score int,
            @region varchar(3),
            @creditlimit money OUT,
            @return int OUTPUT

AS
    SET     @return = 0            --initialise return code

    DECLARE @category varchar(12),
            @monthlygross money,
            @monthlyrent money, 
            @expenses money,
            @income money,
            @ratio smallint,@maxrfincomeratio money

            
    SET @monthlygross = 0
    SET @monthlyrent = 0
    SET @expenses = 0
    SET @income = 0            

    if @creditlimit is null set @creditlimit = 0


    SELECT  @monthlygross = isnull(P.mthlyincome, 0) + isnull(P.addincome, 0) + isnull (P.A2MthlyIncome,0) + isnull (P.A2AddIncome,0),
            @category = ISNULL(C.category,'PCF'),
            @expenses = isnull(P.commitments1, 0) + isnull(P.commitments2, 0) + isnull(P.commitments3, 0) + isnull(P.otherpmnts, 0) +
            isnull(additionalexpenditure2,0) + isnull(additionalexpenditure1,0)
    FROM    proposal P LEFT JOIN code C ON CAST(P.rfcategory as varchar(4)) = C.code 
    AND     C.category in ('PCF', 'PCO', 'PCE', 'PCW')
    WHERE   P.custid = @custid
    AND     P.dateprop = @dateprop


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

    
        SELECT  TOP 1
                @creditlimit = isnull(eleclimit,0)
        FROM    rfcreditlimit 
        WHERE   countrycode = @country
        AND     region = @region
        AND     score <= @score
        AND     @income >= income                 /* redo CR462 */
        AND     dateimported =      -- DSR 5/10/04 Ignore history rcords
                (SELECT MAX(dateimported)
                 FROM   rfcreditlimit
                 WHERE  countrycode = @country
                 AND    region = @region)
        ORDER BY eleclimit desc  -- 
        if @creditlimit > @maxRFIncomeRatio and @maxRFIncomeRatio !=10000000       /* redo CR503 */
        begin
          SELECT  TOP 1
                  @creditlimit = eleclimit
          FROM    rfcreditlimit
          WHERE   countrycode = @country
          AND     region = @region
          AND     @score >=score
          AND     eleclimit <= @maxRFIncomeRatio                 /* redo CR462 */
          AND     dateimported =      -- DSR 5/10/04 Ignore history rcords
                (SELECT MAX(dateimported)
                 FROM   rfcreditlimit
                 WHERE  countrycode = @country
                 AND    region = @region)
          ORDER BY eleclimit DESC  -- DSR 5/10/04 Order on income as well
        end

    SELECT @creditlimit = ISNULL(@creditlimit, 0)
      
    SET @return = @@error
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

