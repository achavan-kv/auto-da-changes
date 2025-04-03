SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects 
where id = object_id('[dbo].[DN_SaveCommissionBasisSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SaveCommissionBasisSP]
GO
CREATE PROCEDURE dbo.DN_SaveCommissionBasisSP
    @allocpercent float,
    @collecttype  char(1),
    @collectionpercent  float,
    @commnpercent  float,
    @countrycode char(1),
    @debitaccount smallint,
    @empeetype varchar(4),
    @maxvalue float,
    @minvalue float,
    @reposspercent float,
    @reppercent float,
    @statuscode char(1),

    @return int OUTPUT

AS
    SET     @return = 0

    DECLARE @rowCount int
    SET     @rowCount =  0

    SELECT  @rowCount = count(*)
    FROM    commnbasis
    WHERE   countrycode = @countrycode
    AND     statuscode = @statuscode
    AND     collecttype = @collecttype
    AND     empeetype = @empeetype

    IF @rowCount > 0
    BEGIN
        UPDATE  commnbasis
        SET     allocpercent        = @allocpercent,
                collectionpercent   = @collectionpercent,
                commnpercent        = @commnpercent,
                debitaccount        = @debitaccount,
                maxvalue            = @maxvalue,
                minvalue            = @minvalue,
                reposspercent       = @reposspercent,
                reppercent          = @reppercent
        WHERE   countrycode = @countrycode
        AND     statuscode  = @statuscode
        AND     collecttype = @collecttype
        AND     empeetype   = @empeetype
    END
    ELSE
    BEGIN
        INSERT INTO commnbasis (
            allocpercent,
            collecttype,
            collectionpercent,
            commnpercent,
            countrycode,
            debitaccount,
            empeetype,
            maxvalue,
            minvalue,
            reposspercent,
            reppercent,
            statuscode)
        VALUES (
            @allocpercent,
            @collecttype,
            @collectionpercent,
            @commnpercent,
            @countrycode,
            @debitaccount,
            @empeetype,
            @maxvalue,
            @minvalue,
            @reposspercent,
            @reppercent,
            @statuscode)
    END

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
