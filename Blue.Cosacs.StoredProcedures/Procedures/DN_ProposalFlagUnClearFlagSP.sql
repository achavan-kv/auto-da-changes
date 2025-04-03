
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_ProposalFlagUnClearFlagSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProposalFlagUnClearFlagSP]
GO


/****** Object:  StoredProcedure [dbo].[DN_ProposalFlagUnClearFlagSP]    Script Date: 11/05/2007 12:11:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE  [dbo].[DN_ProposalFlagUnClearFlagSP]
            @acctno char(12),
            @checkType varchar(5),
            @changeStatus smallint,
            @user int = 0,
            @return int OUTPUT

AS

    SET     @return = 0            --initialise return code

    DECLARE @custid varchar(20)
    DECLARE @dateprop datetime
    DECLARE @outsbal money
    DECLARE @propresult char(1)
    DECLARE @reason varchar(20)

    /* get an update lock on the proposal table */
    UPDATE  proposal
    SET     acctno = @acctno
    WHERE   acctno = @acctno

    /* get the holding customer and the latest dateprop for this account */
    SELECT  TOP 1
            @dateprop = dateprop,
            @custid = custid
    FROM    proposal
    WHERE   acctno = @acctno
    ORDER BY dateprop DESC

    IF (@@rowcount > 0)
    BEGIN
        /* if we're clearing Referral due to revised agreement, reset status */
        -- this is duplicating what happens to referal when stage 1 re-opened
        -- jec 68538  26/09/06

        IF (@checkType = 'R')    
        BEGIN
        
            -- If this was referred only because it exceeded the limit then
            -- it can now be set to approved
            SELECT @propResult = PropResult,
                   @reason = ISNULL(reason,'') + 
						     ISNULL(reason2,'') + 
						     ISNULL(reason3,'') + 
						     ISNULL(reason4,'') + 
						     ISNULL(reason5,'') + 
						     ISNULL(reason6,'')
            FROM   Proposal
            WHERE  custid = @custid 
            AND    dateprop = @dateprop
            AND    acctno = @acctno
            
            IF (@propResult = 'R' AND LTRIM(RTRIM(@reason)) = 'LX')
            BEGIN
				 /* Delete any referral flag */
				DELETE
				FROM    proposalflag
				WHERE   acctno = @acctno
				AND     checktype = 'R'    
								    
				/* Reset the decision */
				UPDATE  proposal 
				SET     propresult = 'A',
				        reason = '',
				        reason2 = '',
				        reason3 = '',
				        reason4 = '',
				        reason5 = '',
				        reason6 = ''
				WHERE   custid = @custid 
				AND     dateprop = @dateprop
				AND     acctno = @acctno
				AND     propresult = 'R'
            END    
        END
        -- 68538 ends here

        ELSE
        BEGIN
            UPDATE  proposalflag 
            SET     datecleared = null, unclearedby =@user
            WHERE   acctno = @acctno
            AND     checktype = @checkType    

            /* if we're clearing stage 1, reset status and clear referral flags */
            IF (@checkType = 'S1')    
            BEGIN
                /* Make sure the other proposal flags exist.           */
                /* This was a problem with proposals for old accounts. */
                IF NOT EXISTS (SELECT 1 FROM ProposalFlag
                               WHERE acctno = @acctno AND CheckType = 'S1')
                BEGIN
                    INSERT INTO proposalflag
                            (origbr, custid, dateprop, checktype, datecleared, empeenopflg,unclearedby, acctno)
                    VALUES  (0, @custid, @dateprop, 'S1', null, null,@user, @acctno)
                END

                IF NOT EXISTS (SELECT 1 FROM ProposalFlag
                               WHERE acctno = @acctno AND CheckType = 'S2')
                BEGIN
                    INSERT INTO proposalflag
                            (origbr, custid, dateprop, checktype, datecleared, empeenopflg,unclearedby, acctno)
                    VALUES  (0, @custid, @dateprop, 'S2', null, null,@user, @acctno)
                END

                IF NOT EXISTS (SELECT 1 FROM ProposalFlag
                               WHERE acctno = @acctno AND CheckType = 'DC')
                BEGIN
                    INSERT INTO proposalflag
                            (origbr, custid, dateprop, checktype, datecleared, empeenopflg,unclearedby, acctno)
                    VALUES  (0, @custid, @dateprop, 'DC', null, null,@user, @acctno)
                END

                /* Delete any referral flag */
                DELETE
                FROM    proposalflag
                WHERE   acctno = @acctno
                AND     checktype = 'R'    

                /* Reset the decision */
                UPDATE  proposal 
                SET     propresult = ''
                WHERE   custid = @custid 
                AND     dateprop = @dateprop
                AND     acctno = @acctno

                IF (@changeStatus = 1)
                BEGIN
                    --SELECT @outsbal = outstbal
                    --FROM   acct 
                    --WHERE  acctno = @acctno        

                    --IF (abs(@outsbal) < 0.01)
                    --BEGIN

                    UPDATE acct
                    SET    currstatus = '0',
                    lastupdatedby =@user
                    WHERE  acctno = @acctno

                    DELETE    
                    FROM   cancellation 
                    WHERE  acctno = @acctno

                    --END    
                END        
            END
        END
    END

    IF (@@error != 0)
    BEGIN
        SET @return = @@error
    END

