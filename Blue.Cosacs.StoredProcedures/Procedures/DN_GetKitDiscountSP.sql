SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_GetKitDiscountSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetKitDiscountSP]
GO

CREATE PROCEDURE  dbo.DN_GetKitDiscountSP
    @itemId			int,
    @branchCode     smallint,
    @accountType    varchar(3),
    @country        varchar(1),
    @dutyfree       smallint,
    @taxExempt      smallint,
    @discount       float OUT,
    @return         int OUTPUT

AS

    SET @return = 0            --initialise return code

    declare @hpprice        float
    declare @cashprice      float
    declare @dutyfreeprice  float
    declare @DateToday      datetime
    declare @AgTaxType      varchar(1)
    declare @TaxType        varchar(1)
    declare @taxrate        float

    SET @discount = 0

    SELECT  @cashprice = unitpricecash,
            @hpprice = unitpricehp,
            @dutyfreeprice = unitpricedutyfree,
            @taxrate = taxrate
    FROM    stockitem
    WHERE   ItemID = @itemId
    AND     stocklocn = @branchCode

    SET @return = @@error

    IF (@return = 0)
    BEGIN
        -- Price type (Cash, HP or Duty Free)
        IF (@accountType IN ('C','S'))      /* Acct Type Translation DSR 29/9/03 */
            SET @discount = @CashPrice
        ELSE
            SET @discount = @HPPrice

        IF (@dutyfree=1)
            SET @discount = @DutyFreePrice
    END

    IF (@return = 0)
    BEGIN
        SET @DateToday = getdate()

        SELECT  @discount = unitprice
        FROM    promoprice
        WHERE   ItemID = @itemId
        AND     stocklocn = @branchCode
        AND     ((hporcash = 'H' AND @accountType NOT IN ('C','S'))
                 OR (hporcash = 'C' AND @accountType IN ('C','S')))    /* Acct Type Translation DSR 29/9/03 */
        AND     (fromdate < @DateToday AND todate > @DateToday)
    END

    SET @return = @@error


    -- At this point we have all the relevant price values for the item, but we haven't
    -- taken into account whether that price is inclusive or exclusive of tax and
    -- whether it should be displayed and stored inclusive or exclusive of tax

    IF (@return = 0)
    BEGIN
        SELECT  @AgTaxType = agrmttaxtype,
                @TaxType = taxtype
        FROM    country
        WHERE   countrycode = @country

        IF(@AgTaxType = 'E' and @TaxType = 'I')
        BEGIN
            --need to remove tax
            SET @discount = ROUND(@discount / (1 + (@taxrate/100)),2)
        END

        IF(@AgTaxType = 'I' and @TaxType = 'E' and @taxExempt = 0)
        BEGIN
            --need to add tax on
            SET @discount = ROUND(@discount * (1 + (@taxrate/100)),2)
        END

        IF(@AgTaxType = 'I' and @TaxType = 'I' and @taxExempt = 1)
        BEGIN
            --need to remove tax
            SET @discount = ROUND(@discount / (1 + (@taxrate/100)),2)
        END
    END


    SET @return = @@error

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
