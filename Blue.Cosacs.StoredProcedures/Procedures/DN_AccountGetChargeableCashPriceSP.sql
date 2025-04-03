SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_AccountGetChargeableCashPriceSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountGetChargeableCashPriceSP]
GO


CREATE PROCEDURE  dbo.DN_AccountGetChargeableCashPriceSP
    @acctno varchar(12),
    @price money OUT,
    @adminprice money OUT,
    @return int OUTPUT

AS

    SET @return = 0            --initialise return code

    DECLARE @noninterest        varchar(2)
    DECLARE @chargeabletax      money
    DECLARE @agrmttaxtype       char(1)
    DECLARE @includewarranty    smallint
    DECLARE @adminitem          varchar(10)
    DECLARE @insitem            varchar(10)
    DECLARE @chargeableadmintax money

    SET @chargeabletax = 0
    SET @chargeableadmintax = 0
    SET @adminprice = 0

    SELECT  @noninterest = noninterestitem,
            @agrmttaxtype = agrmttaxtype,
            @adminitem = adminitemno,
            @insitem = insitemno
    FROM    country

    SELECT  @includewarranty = tt.includewarranty
    FROM    acct A
    INNER JOIN termstype tt
    ON      A.termstype = tt.termstype
    WHERE   A.acctno = @acctno

    SELECT  @price = isnull(sum(l.ordval), 0),
            @chargeabletax = isnull(sum(l.taxamt),0)
    FROM    lineitem l
    INNER JOIN dbo.StockInfo s ON l.ItemID = s.ID
    WHERE   l.acctno = @acctno
    AND     l.iskit  = 0
    AND     s.IUPC != 'DT'
    AND     s.IUPC != 'STAX'
    AND     s.IUPC != @adminitem
    AND     s.IUPC != @insitem
            /* issue 346 cater for countries with blank noninterestitem */
    AND     (s.IUPC NOT LIKE @noninterest + '%' OR @noninterest = '')
	AND		CONVERT(VARCHAR,s.category) not in (SELECT code FROM dbo.code WHERE category = 'PCDIS')		-- #18554

    IF (@includewarranty = 1)
    BEGIN
        SET @adminprice = @price
    END
    ELSE
    BEGIN
        SELECT  @adminprice = isnull(sum(LI.OrdVal), 0),
                @chargeableadmintax = isnull(sum(LI.taxamt),0)
        FROM    lineitem LI
        --INNER JOIN stockitem SI ON LI.itemno = SI.itemno AND LI.stocklocn = SI.stocklocn
        INNER JOIN stockitem SI ON LI.ItemID = SI.ItemID AND LI.stocklocn = SI.stocklocn				--IP - 12/01/12 
        WHERE   LI.acctno = @acctno
        AND     LI.IsKit  = 0
        AND     SI.IUPC != 'DT'
        AND     SI.IUPC != 'STAX'
        AND     SI.IUPC != @adminitem
        AND     SI.IUPC != @insitem
                /* issue 346 cater for countries with blank noninterestitem */
        AND     (SI.IUPC NOT LIKE @noninterest + '%' OR @noninterest = '')
        AND     CONVERT(VARCHAR,SI.category) not in (SELECT code FROM dbo.code WHERE category = 'WAR') 
		AND		CONVERT(VARCHAR,SI.category) not in (SELECT code FROM dbo.code WHERE category = 'PCDIS')  -- #18554
    END

    IF (@agrmttaxtype = 'E')        /* we need to add on chargeable sales tax */
    BEGIN
        SET  @price = @price + @chargeabletax
        SET  @adminprice = @adminprice + @chargeableadmintax
    END

    IF (@@error != 0)
    BEGIN
        SET @return = @@error
    END
GO

SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
