SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_SaveBailiffCommissionBasisSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SaveBailiffCommissionBasisSP]
GO
-- CR101 RD 02/12/05 Modified to add changed to BailiffcommnBasAudit table

CREATE PROCEDURE     dbo.DN_SaveBailiffCommissionBasisSP
            @empeeno int,
            @allocpercent float,
            @collecttype  char(1),
            @collectionpercent  float,
            @commnpercent  float,
            @debitaccount smallint,
            @empeetype varchar(4),
            @maxvalue float,
            @minvalue float,
            @reposspercent float,
            @reppercent float,
            @statuscode char(1),
	    @empeenochange int, -- CR101
            @return int OUTPUT

AS
    SET     @return = 0

    IF (@EmpeeNo = -1)
    BEGIN
        -- Apply to all employees of the same type

        UPDATE  bailcommnbas
        SET     allocpercent      = @allocpercent,
                collectionpercent = @collectionpercent,                
                commnpercent      = @commnpercent,
                debitaccount      = @debitaccount,
                maxvalue          = @maxvalue,
                minvalue          = @minvalue,
                reposspercent     = @reposspercent,
                reppercent        = @reppercent
        WHERE   statuscode  = @statuscode
        AND     collecttype = @collecttype
        AND     empeetype   = @empeetype

        -- Insert any employees of this type not already entered

        INSERT INTO bailcommnbas (
            allocpercent,
            collecttype,
            collectionpercent,
            commnpercent,
            debitaccount,
            empeeno,
            empeetype,
            maxvalue,
            minvalue,
            reposspercent,
            reppercent,
            statuscode)
        SELECT
            @allocpercent,
            @collecttype,
            @collectionpercent,
            @commnpercent,
            @debitaccount,
            u.id,
            @empeetype,
            @maxvalue,
            @minvalue,
            @reposspercent,
            @reppercent,
            @statuscode
        FROM Admin.[User] u
        INNER JOIN Admin.UserRole ur ON u.Id = ur.UserId
        WHERE ur.RoleId = @empeetype
        AND NOT EXISTS (SELECT * FROM bailcommnbas bc
                        WHERE bc.EmpeeNo = u.id
                        AND   bc.statuscode  = @statuscode
                        AND   bc.collecttype = @collecttype)

--	CR101 RD 02/12/05 Added user to BailiffcommnbasAudit table store changes to the Bailcommnbas table
	INSERT INTO BailiffcommnBasAudit  (
            allocpercent,
            collecttype,
            collectionpercent,
            commnpercent,
            debitaccount,
            empeeno,
            empeetype,
            maxvalue,
            minvalue,
            reposspercent,
            reppercent,
            statuscode,
	    empeenochange,
	    datechange)
        SELECT
            @allocpercent,
            @collecttype,
            @collectionpercent,
            @commnpercent,
            @debitaccount,
            u.id,
            @empeetype,
            @maxvalue,
            @minvalue,
            @reposspercent,
            @reppercent,
            @statuscode,
	    @empeenochange,
	    getdate()
         FROM Admin.[User] u
        INNER JOIN Admin.UserRole ur ON u.Id = ur.UserId
        WHERE ur.RoleId = @empeetype
        AND NOT EXISTS (SELECT * FROM BailiffcommnBasAudit  bc
                        WHERE bc.EmpeeNo = u.Id
                        AND   bc.statuscode  = @statuscode
                        AND   bc.collecttype = @collecttype
			AND   bc.empeenochange = @empeenochange)
    END
    ELSE -- empeeno not -1 so employee does exist
    BEGIN
        DECLARE @rowCount int
        SET     @rowCount =  0

        SELECT  @rowCount = count(*)
        FROM    bailcommnbas
        WHERE   empeeno = @empeeno
        AND     statuscode = @statuscode
        AND     collecttype = @collecttype
        AND     empeetype = @empeetype

	     IF @rowCount >= 1 -- record exists so update surely? was = 0
        BEGIN

            UPDATE  bailcommnbas
               SET  allocpercent      = @allocpercent,
                    collectionpercent = @collectionpercent,
                    commnpercent      = @commnpercent,
                    debitaccount      = @debitaccount,
                    maxvalue          = @maxvalue,
                    minvalue          = @minvalue,
                    reposspercent     = @reposspercent,
                    reppercent        = @reppercent
             WHERE  empeeno = @empeeno
               AND  statuscode = @statuscode
               AND  collecttype = @collecttype
               AND  empeetype = @empeetype

-- CR101 RD 02/12/05 Added user to BailiffcommnbasAudit table store changes to the Bailcommnbas table
            INSERT INTO BailiffcommnBasAudit  (
                allocpercent, collecttype,  collectionpercent, commnpercent,
                debitaccount, empeeno,empeetype, maxvalue,
                minvalue,reposspercent, reppercent,
                statuscode,empeenochange, datechange)
            VALUES (
                @allocpercent,@collecttype,@collectionpercent, @commnpercent,
                @debitaccount,@empeeno,@empeetype,@maxvalue,
                @minvalue,@reposspercent,@reppercent,@statuscode,
		          @empeenochange,getdate())

        END
        ELSE -- there is no record so insert
        BEGIN

            INSERT INTO bailcommnbas (
                allocpercent,
                collecttype,
                collectionpercent,
                commnpercent,
                debitaccount,
                empeeno,
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
                @debitaccount,
                @empeeno,
                @empeetype,
                @maxvalue,
                @minvalue,
                @reposspercent,
                @reppercent,
                @statuscode)

        END
    END

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
