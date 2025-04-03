if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerUpdateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
	drop procedure [dbo].[DN_CustomerUpdateSP]
GO

CREATE PROCEDURE     dbo.DN_CustomerUpdateSP
            @custid varchar (20),
            @origbr smallint ,
            @otherid varchar(20) ,     
            @branchnohdle smallint ,
            @name varchar(60) ,
            @firstname varchar(30) ,
            @title varchar(25) ,
            @alias varchar(25) ,
            @addrsort varchar(20) ,
            @namesort varchar(20) ,
            @dateborn datetime ,
            @sex char(1) ,
            @ethnicity char(1) ,
            @morerewardsno varchar(16) ,
            @effectivedate smalldatetime ,
            @idtype char(4) ,
            @idnumber char(30) ,
            @userno int,
            @datechanged datetime,
            @maidenname varchar(30), 
            @storetype varchar(2),
            @dependants int,
            @maritalStat char(1),
            @nationality char(4),
            @resieveSms bit,
            @return int OUTPUT

AS

    SET     @return = 0

    UPDATE  customer
    SET     origbr        =    @origbr,
            otherid        =    @otherid,
            branchnohdle    =    @branchnohdle,
            name        =    @name,
            firstname    =    @firstname,
            title        =    @title,
            alias        =    @alias,
            addrsort        =    @addrsort,
            namesort    =    @namesort,
            dateborn    =    @dateborn,
            sex        =    @sex,
            ethnicity        =    @ethnicity,
            morerewardsno    =    @morerewardsno,
            effectivedate    =    @effectivedate,    
            IdNumber    =    @idnumber,
            IdType        =    @idtype,
            datechange     =    @datechanged,
            empeenochange =     @userno,    
            maidenname    =    @maidenname,
            dependants = @dependants,
            maritalStat = @maritalStat,
            nationality = @nationality,
			resieveSms = @resieveSms
    WHERE   custid = @custid

    IF(@@rowcount=0 and @@error = 0)
    BEGIN
        INSERT
        INTO    customer     (origbr,
                    custid,
                    otherid,
                    branchnohdle,
                    name,
                    firstname,
                    title,
                    alias,
                    addrsort,
                    namesort,
                    dateborn,
                    sex,
                    ethnicity,
                    morerewardsno,
                    effectivedate,
                    IdNumber,
                    IdType,
                    datechange,
                    empeenochange,
                    maidenname,
                    dependants,
                    maritalStat,
                    nationality,storetype )
        VALUES        (@origbr,
                    @custid ,
                    @otherid ,
                    @branchnohdle,
                    @name,
                    @firstname,
                    @title,
                    @alias,
                    @addrsort,
                    @namesort,
                    @dateborn,
                    @sex,
                    @ethnicity,    
                    @morerewardsno,
                    @effectivedate,
                    @idnumber,
                    @idtype,
                    @datechanged,
                    @userno,
                    @maidenname,
                    @dependants,
                    @maritalStat,
                    @nationality ,@storetype )
    END    


    IF (@@error != 0)
    BEGIN
        SET @return = @@error
    END
GO