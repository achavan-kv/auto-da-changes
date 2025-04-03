SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_IntRateHistorySaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_IntRateHistorySaveSP]
GO

CREATE PROCEDURE dbo.DN_IntRateHistorySaveSP
    @termstype varchar(4),
    @datefrom datetime,
    @dateto datetime,
    @intrate float,
    @inspcent float,
    @adminpcent float, 
    @insincluded smallint,
    @includewarranty smallint,
    @ratetype varchar(2),
    @band varchar(32),
    @pointsFrom smallint,
    @pointsTo smallint,
    @user int = 0,
    @adminValue float,
    @return int OUTPUT

AS DECLARE

    @Pdateto varchar(500),
    @Pintrate varchar(500),
    @Pinspcent varchar(500),
    @Padminpcent varchar(500), 
    @Padminvalue varchar(500), 
    @Pinsincluded varchar(500),
    @Pincludewarranty varchar(500),
    @Pratetype varchar(2),
    @genvar varchar(500)

BEGIN

    SET NOCOUNT ON
    SET @return = 0
    
    -- A start date is always at midnight
    SET @DateFrom = CONVERT(DATETIME,CONVERT(CHAR(10),@DateFrom,105),105)

    if @dateto != CONVERT(DATETIME,'1-jan-1900',105)
    begin
        -- An end date is always one minute before midnight
        SET @DateTo = CONVERT(DATETIME,CONVERT(CHAR(10),@DateTo,105),105)
        -- Note: SMALLDATETIME would round 23:59:59 up to 00:00:00 next day
        SET @DateTo = DATEADD(Day,1,DATEADD(Minute,-1,@DateTo))
    end

            
    SELECT
        @Pdateto = isnull(convert(varchar,dateto),''),
        @Pintrate = isnull(convert(varchar,intrate),'') ,
        @Pinspcent = isnull(convert(varchar,inspcent),''),
        @Padminpcent = isnull(convert(varchar,adminpcent),''), 
        @Padminvalue = isnull(convert(varchar,adminvalue),''), 
        @Pinsincluded = isnull(convert(varchar,insincluded),''),
        @Pincludewarranty = isnull(convert(varchar,includewarranty),''),
        @Pratetype = isnull(convert(varchar,ratetype),'')
    FROM intratehistorydeletedtemp
    WHERE datefrom = @datefrom 
    AND termstype = @termstype

    --
    -- Audit Changes
    --
    -- Band, PointsFrom and PointsTo have been added for Terms Type Bands.
    -- These cannot be changed from the Terms Type screen and so are not audited here.
    -- Changes on the Terms Type Matrix are audited in the TermsTypeBand table.
    --
    
    set @genvar = convert(varchar, @dateto)
    if @genvar != @Pdateto  
        exec DN_TermsTypeHistorySaveSP @termstype = @termstype, @empeenochange = @user, @changedfield='dateto',
                                        @origvalue = @Pdateto , @newvalue = @genvar
    
    set @genvar = convert(varchar, @intrate)
    if @genvar != @Pintrate  
        exec DN_TermsTypeHistorySaveSP @termstype = @termstype, @empeenochange = @user, @changedfield='intrate',
                                        @origvalue = @Pintrate , @newvalue = @genvar
    set @genvar = convert(varchar, @inspcent)
    if @genvar != @Pinspcent  
        exec DN_TermsTypeHistorySaveSP @termstype = @termstype, @empeenochange = @user, @changedfield='inspcent',
                                        @origvalue = @Pinspcent , @newvalue = @genvar

    set @genvar = convert(varchar, @adminpcent)
    if @genvar != @Padminpcent  
        exec DN_TermsTypeHistorySaveSP @termstype = @termstype, @empeenochange = @user, @changedfield='adminpcent',
                                    @origvalue = @Padminpcent , @newvalue = @genvar

    set @genvar = convert(varchar, @adminValue)
    if @genvar != @Padminvalue  
        exec DN_TermsTypeHistorySaveSP @termstype = @termstype, @empeenochange = @user, @changedfield='adminvalue',
                                    @origvalue = @Padminvalue , @newvalue = @genvar

    set @genvar = convert(varchar, @insincluded)
    if @genvar != @Pinsincluded  
        exec DN_TermsTypeHistorySaveSP @termstype = @termstype, @empeenochange = @user, @changedfield='insincluded',
                                    @origvalue = @Pinsincluded, @newvalue = @genvar

    set @genvar = convert(varchar, @includewarranty)
    if @genvar != @Pincludewarranty  
        exec DN_TermsTypeHistorySaveSP @termstype = @termstype, @empeenochange = @user, @changedfield='includewarranty',
                                        @origvalue = @Pincludewarranty , @newvalue = @genvar

    set @genvar = convert(varchar, @ratetype)
    if @genvar != @Pratetype  
        exec DN_TermsTypeHistorySaveSP @termstype = @termstype, @empeenochange = @user, @changedfield='ratetype',
                                    @origvalue = @Pratetype, @newvalue = @genvar


    UPDATE  intratehistory
    SET     intrate = @intrate,
            inspcent = @inspcent,
            adminpcent = @adminpcent,
            adminValue = @adminValue,
            insincluded = @insincluded,
            includewarranty = @includewarranty,
            ratetype = @ratetype,
            band = @band,
            pointsFrom = @pointsFrom,
            pointsTo = @pointsTo,
            empeenochange = @user,
            datechange =getdate()
    WHERE   termstype = @termstype
    AND     DATEDIFF(Day, datefrom, @datefrom) = 0
    AND     DATEDIFF(Day, dateto, @dateto) = 0
    AND     band = @band

    IF (@@rowcount = 0)
    BEGIN
        INSERT INTO intratehistory
            (termstype, datefrom, dateto, intrate, 
             inspcent, adminpcent, insincluded, includewarranty, 
             ratetype, band, pointsFrom, pointsTo, empeenochange,datechange, adminvalue)
        VALUES
            (@termstype, @datefrom, @dateto, @intrate, 
             @inspcent, @adminpcent, @insincluded, @includewarranty, 
             @ratetype, @band, @pointsFrom, @pointsTo, @user, getdate(), @adminValue)
    END


    SET @return = @@error
    SET NOCOUNT OFF
    RETURN @Return
END
GO

