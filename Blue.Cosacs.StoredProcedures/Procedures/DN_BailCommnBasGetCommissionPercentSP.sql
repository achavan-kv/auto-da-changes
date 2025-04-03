
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_BailCommnBasGetCommissionPercentSP]')
and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_BailCommnBasGetCommissionPercentSP]
GO

CREATE PROCEDURE     dbo.DN_BailCommnBasGetCommissionPercentSP
            @empeeno int,
            @acctno varchar(12),
            @collType varchar(1),
            @return int OUTPUT

AS
    DECLARE @statusCode char(1)
    DECLARE @datealloc datetime
    DECLARE @status char(1)
    DECLARE @rowcount int

    SET @return = 0            --initialise return code
    SET @rowcount = 0

    -- The account status to be used can be at the time of allocation
    -- or the current status

    IF NOT EXISTS (SELECT Value FROM CountryMaintenance
                   WHERE  CodeName = 'CurStatusCommission'
                   AND    Value    = 'True')
    BEGIN
        -- Get the status at the time of allocation
        SELECT  TOP 1
                @status = statuscode
        FROM    status
        WHERE   acctno = @acctno
        AND     datestatchge <=
            (SELECT  max(datealloc)
             FROM    follupalloc
             WHERE   isnull(datealloc, '1/1/1900') != '1/1/1900'
             AND     isnull(datealloc, '1/1/1900') <= getdate()
             AND     (isnull(datedealloc, '1/1/1900') = '1/1/1900' OR isnull(datedealloc, '1/1/1900') >= getdate())
             AND     acctno = @acctno
             AND     empeeno = @empeeno)
        ORDER BY    datestatchge desc

        SET @rowcount = @@ROWCOUNT
    END


    if @rowcount = 0
        -- Get the current status
        select @status = currstatus from acct where acctno = @acctno


    --Get the collection rates
    SELECT  collectionpercent,
            commnpercent,
            reposspercent,
            allocpercent,
            reppercent,
            minvalue,
            maxvalue,
            isnull(debitaccount, 0) as debitaccount
    FROM    bailcommnbas
    WHERE   empeeno = @empeeno
    AND     statuscode = @status
    AND     collecttype = @colltype

    SET @return = @@error

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

