SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustAddressSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustAddressSaveSP]
GO


CREATE PROCEDURE dbo.DN_CustAddressSaveSP
/* PC-09/10/06 Added a check for additional mobile numbers (M2, M3, M4)  */
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_CustAddressSaveSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Save Customer address
-- Author       : ??
-- Date         : ??
--
-- Save ADDRESS
    -- but only if this is not a 'mobile address type'
    --
    -- The following steps must be followed to maintain the history records:
    -- 1. Some columns are not updated by this routine so they are temporarily saved in variables
    -- 2. If a history record is NOT being created then the current record is deleted
    -- 3. If a history record is being created then an end date is placed on the current record
    -- 4. An exception will be raised if the new start date is earlier than a history record
    -- 5. The new record is inserted
    -- 6. The previous history record is updated to end the same day as the new start date
    -- 7. For a work address the employer address is updated as well
    --
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 13/07/08  jec CR907 - update date changed
-- ================================================
	-- Add the parameters for the stored procedure here
/* AA-25/09/02 adding notes */
    @custID         varchar(20),
    @addressType    char(2),
    @address1       varchar(50),
    @address2       varchar(50),
    @address3       varchar(50),
    @postcode       varchar(10),
    @DeliveryArea   varchar(8),
    @notes          varchar(1000),
    @email          varchar(60),
    @dialcode       varchar(8),
    @phone          varchar(20),
    @ext            varchar(6),
    @datein         datetime,
    @user           int,
    @newRecord      int,
    @return         int OUTPUT

AS

    SET @return = 0

    DECLARE @oldDatein      datetime
    DECLARE @oldResStatus   char(1)
    DECLARE @oldMthlyRent   float
    DECLARE @oldPropType    char(4)
    DECLARE @pDateIn        datetime
    DECLARE @dateMsg        varchar(10)
    DECLARE @oldDialcode    varchar(8)
    DECLARE @oldPhone       varchar(20)
    DECLARE @oldExt         varchar(6)
	DECLARE @oldAdd1        varchar(50),			--CR907 jec
			@oldAdd2		varchar(50),
			@oldpocode		varchar(10),
			@olddatechange	datetime,
			@olddatemoved	datetime,
			@datechange		datetime


    --
    -- Save ADDRESS
    -- but only if this is not a 'mobile address type'
    --
    -- The following steps must be followed to maintain the history records:
    -- 1. Some columns are not updated by this routine so they are temporarily saved in variables
    -- 2. If a history record is NOT being created then the current record is deleted
    -- 3. If a history record is being created then an end date is placed on the current record
    -- 4. An exception will be raised if the new start date is earlier than a history record
    -- 5. The new record is inserted
    -- 6. The previous history record is updated to end the same day as the new start date
    -- 7. For a work address the employer address is updated as well
    --

    IF (@addressType NOT IN('M', 'M2', 'M3', 'M4'))
    BEGIN
        -- 1. Some columns are not updated by this routine so they are temporarily saved in variables
        SELECT  @oldDatein    = datein,
                @oldResStatus = resstatus,
                @oldMthlyRent = mthlyrent,
                @oldPropType  = proptype,
				@oldAdd1 = cusaddr1,			--CR907 jec
				@oldAdd2 = cusaddr2,			--CR907 jec
				@oldpocode = cuspocode,			--CR907 jec
				@olddatemoved = datemoved,		--CR907 jec	
				@olddatechange = datechange		--CR907 jec

        FROM    custaddress
        WHERE   custid = @custID
        AND     addtype = @addressType
        AND     ISNULL(datemoved,'') = ''

        IF (@@ROWCOUNT = 0)
        BEGIN
            SET @oldDatein    = null
            SET @oldResStatus = null
            SET @oldMthlyRent = null
            SET @oldPropType  = ''
			SET @oldAdd1 = null				--CR907 jec
			SET @oldAdd2 = null				--CR907 jec	
			SET @oldpocode = null			--CR907 jec	
			SET @olddatemoved = null		--CR907 jec	
			SET @olddatechange = null		--CR907 jec		
        END

        IF (@newRecord = 0)
        BEGIN
            -- 2. If a history record is NOT being created then the current record is deleted
            DELETE  FROM custaddress
            WHERE   custid = @custID
            AND     addtype = @addressType
            AND     ISNULL(datemoved,'') = ''
        END
        ELSE
        BEGIN
            -- 3. If a history record is being created then an end date is placed on the current record
            UPDATE  custaddress
            SET     datemoved = @datein,
                    datechange = getdate(),
                    empeenochange = @user
            WHERE   custid = @custID
            AND     addtype = @addressType
            AND     ISNULL(datemoved,'') = ''
        END

        -- 4. An exception will be raised if the new start date is earlier than a history record
        SELECT  @pDateIn = MAX(datein)
        FROM    custaddress
        WHERE   custid = @custid
        AND     addtype = @addressType

        IF (@@ROWCOUNT = 0)
        BEGIN
            SET @pDateIn = null
        END

					-- CR907 update datechange
			-- if none of the following have changed;
			-- (custid, datein, cusaddr1, cusaddr2, cuspocode, resstatus, mthlyrent, datemoved)
			-- the datechange will be set back to the previous datechange 

			if	@oldDatein    = @datein				
                --@oldResStatus = resstatus
               -- @oldMthlyRent = mthlyrent
               -- @oldPropType  = proptype
				and @oldAdd1 = @address1			
				and @oldAdd2 = @address2			
				and @oldpocode = @postCode			
				--and @olddatemoved = @datein		
			
				set @datechange = @olddatechange
			else
				set @datechange = getdate()

        -- This expression must resolve to false if there is no history record
        IF (@datein <= @pDateIn)
        BEGIN
            -- Must not set the date into the current address BEFORE the date into the history address
            SET @dateMsg = CONVERT(varchar(10), @pDateIn, 105)
            -- This should rollback any updates done above
            RAISERROR('Date into current address must be after %s (date into the previous address) for address type %s', 16, 1, @dateMsg, @addressType)
        END
        ELSE
        BEGIN
            -- 5. The new record is inserted
            INSERT INTO custaddress
                (origbr,
                 custid,
                 addtype,
                 datein,
                 cusaddr1,
                 cusaddr2,
                 cusaddr3,
                 cuspocode,
                 deliveryarea,
                 custlocn,
                 resstatus,
                 mthlyrent,
                 datemoved,
                 hasstring,
                 notes,
                 Email,
                 PropType,
                 empeenochange,
                 datechange)
            VALUES
                (null,
                 @custID,
                 @addressType,
                 @datein,
                 @address1,
                 @address2,
                 @address3,
                 @postCode,
                 @DeliveryArea,
                 null,          -- custlocn
                 @oldResStatus,
                 @oldMthlyRent,
                 null,          -- datemoved
                 0,             -- hasstring
                 @notes,
                 @email,
                 @oldPropType,
                 @user,
                 @datechange)				--CR907 jec
			
			-- CR907 update datechange
			-- if none of the following have changed;
			-- (custid, datein, cusaddr1, cusaddr2, cuspocode, resstatus, mthlyrent, datemoved)
			-- the datechange will be set back to the previous datechange 
			if	@oldDatein    = @datein				
                --@oldResStatus = resstatus
               -- @oldMthlyRent = mthlyrent
               -- @oldPropType  = proptype
				and @oldAdd1 = @address1			
				and @oldAdd2 = @address2			
				and @oldpocode = @postCode			
				and @olddatemoved = @datein		
			Begin
				update custaddress
					set datechange = @olddatechange
			End
					

            -- 6. The previous history record is updated to end the same day as the new start date
            -- (If a history record was just created this field should already be set correctly)
            UPDATE  custaddress
            SET     datemoved = @datein
            WHERE   custid = @custid
            AND     addtype = @addressType
            AND     datein = @pDateIn

            IF (@addressType = 'W')
            BEGIN
                -- 7. For a work address the employer address is updated as well
                EXEC DN_ProposalEmployerAddressUpdateSP
                    @custid   = @custID,
                    @address1 = @address1,
                    @address2 = @address2,
                    @address3 = @address3,
                    @postcode = @postcode,
                    @return   = @return OUT
            END
        END
    END

    -- END OF ADDRESS processing



    --
    -- Save TELEPHONE
    --
    -- The following steps must be followed to maintain the history records:
    -- 1. If a history record is NOT being created then the current record is deleted
    -- 2. If a history record is being created then an end date is placed on the current record
    -- 3. An exception will be raised if the new start date is earlier than a history record
    -- 4. The new record is inserted
    -- 5. The previous history record is updated to end the same day as the new start date
    --

    IF (@newRecord = 0)
    BEGIN
        -- 1. If a history record is NOT being created then the current record is deleted
        DELETE  FROM CustTel
        WHERE   custid = @custID
        AND     tellocn = @addressType
        AND     ISNULL(datediscon,'') = ''
    END
    ELSE
    BEGIN
        -- 2. If a history record is being created then an end date is placed on the current record
        UPDATE  CustTel
        SET     datediscon = @datein,
                datechange = getdate(),
                empeenochange = @user
        WHERE   custid = @custID
        AND     tellocn = @addressType
        AND     ISNULL(datediscon,'') = ''
    END

    -- 3. An exception will be raised if the new start date is earlier than a history record
    SELECT  @pDateIn = MAX(dateteladd)
    FROM    CustTel
    WHERE   custid = @custid
    AND     tellocn = @addressType

    IF (@@ROWCOUNT = 0)
    BEGIN
      SET @pdatein = null
    END

    -- This expression must resolve to false if there is no history record
    IF (@datein <= @pdatein)
    BEGIN
        -- Must not set the date into the current Telephone BEFORE the date into the history Telephone
        SET @dateMsg = CONVERT(varchar(10), @pdatein, 105)
        -- This should rollback any updates done above
        RAISERROR('Date of current telephone must be after %s (date of previous telephone) for telephone type %s', 16, 1, @dateMsg, @addressType)
    END
    ELSE
    BEGIN
        -- 4. The new record is inserted
        INSERT INTO CustTel
            (origbr,
             custid,
             tellocn,
             dateteladd,
             datediscon,
             telno,
             extnno,
             DialCode,
             empeenochange,
             datechange)
        VALUES
            (null,
             @custid,
             @addressType,
             @datein,
             null,
             @phone,
             @ext,
             @dialcode,
             @user,
             GETDATE())

        -- 5. The previous history record is updated to end the same day as the new start date
        -- (If a history record was just created this field should already be set correctly)
        UPDATE  CustTel
        SET     datediscon = @datein
        WHERE   custid = @custid
        AND     tellocn = @addressType
        AND     dateteladd = @pdatein

    END

    -- END OF Telephone processing

    SET @Return = @@ERROR
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

