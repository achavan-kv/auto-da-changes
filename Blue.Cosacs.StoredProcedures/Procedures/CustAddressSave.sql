/****** Object:  StoredProcedure [dbo].[CustAddressSave]    Script Date: 07/07/2008 11:21:55 ******/
IF  EXISTS (SELECT * FROM sysobjects WHERE name = 'CustAddressSave' AND type in ('P', 'PC'))
DROP PROCEDURE [dbo].[CustAddressSave]

GO

-- ================================================
-- Project      : CoSACS .NET
-- File Name    : CustAddressSave.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Save Customer Address details
-- Author       : ??
-- Date         : ??
--
-- This procedure will will save the Customer address details.
-- 
-- Change Control
-- --------------
--  Version	Date      By  Description
-- ----      --  -----------
-- 1.0		07/10/2008 jec UAT552 email cannot be null.
-- 2.0		23/08/10  jec CR1084 Save address Zone
-- 3.0		06/11/18  KM  CR Saving Delivery address with Title,FirstName,LastName
-- 4.0		06/03/2020	Address Standardization CR Latitude and Longitude columns added --Script for Address Standardization CR2019 - 025
-- =================================================================================
CREATE PROCEDURE [dbo].[CustAddressSave]
	@custID         varchar(20),
    @addressType    char(2),
    @address1       varchar(50),
    @address2       varchar(50),
    @address3       varchar(50),
    @postcode       varchar(10),
    @DeliveryArea   varchar(8),
    @notes          varchar(1000),
    @email          varchar(60),
    @datein         datetime,
    @user           int, 
    @newRecord		bit,
    @Zone		    varchar(4),		--CR1084 
	@DELTitleC          VARCHAR(50),
	@DELFirstname		varchar(50),
	@DELLastname		varchar(50),
	@Latitude		FLOAT,
	@Longitude		FLOAT,
    @return         int out

AS

	SET @return = 0

	SET @address1 = REPLACE(@address1,',',' ')
    SET @address2 = REPLACE(@address2,',',' ')
    SET @address3 = REPLACE(@address3,',',' ')

	DECLARE @oldDatein          datetime
    DECLARE @oldResStatus       char(1)
    DECLARE @oldMthlyRent       float
    DECLARE @oldPropType        char(4)

	-- Get the CURRENT address (if any)
        SELECT  @oldDatein          = datein,
                @oldResStatus       = resstatus,
                @oldMthlyRent       = mthlyrent,
                @oldPropType        = proptype
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
        END

-- if @newRecord
--	 update datemoved for previous record (if any) and insert new
-- else
--	update previous record(if none insert new)

IF @newRecord = 1
begin
	UPDATE custaddress
		SET datemoved = @datein,
			datechange = GETDATE(),
			empeenochange = @user
			WHERE custid = @custid AND
				addType = @addressType AND
				ISNULL(datemoved,'')=''

            -- Insert the amended address
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
                 zone,		-- CR1084
                 datechange,
				 DELTitleC,
				 DELFirstname,
				 DELLastname,
				 Latitude,
				 Longitude)
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
                 ISNULL(@email,' '),		-- jec UAT552 jec 07/10/08
                 @oldPropType,
                 @user,
                 @zone,		-- CR1084
                 getdate(),
				  @DELTitleC,
				 @DELFirstname,
				 @DELLastname,
				 @Latitude,
				 @Longitude)
end
ELSE
BEGIN
	-- update, if no record count insert
    -- Insert the amended address
    update custaddress
		SET datein = @datein,
			cusaddr1 = @address1,
			cusaddr2 = @address2,
			cusaddr3 = @address3,
			cuspocode = @postCode,
			deliveryarea = @DeliveryArea,
			resstatus = @oldResStatus,
			mthlyrent = @oldMthlyRent,
			notes = @notes,
			Email = ISNULL(@email,' '),		-- jec UAT552 jec 07/10/08
			PropType = @oldPropType,
			empeenochange = @user,
			Zone = @zone,		-- CR1084
			DELTitleC=@DELTitleC,
			DELFirstname=@DELFirstname,
			DELLastname=@DELLastname,
			datechange =  getdate(),
			Latitude = @Latitude,
			Longitude = @Longitude
	WHERE custid = @custid AND 
			addType = @addressType AND 
			ISNULL(datemoved,'')=''

IF @@ROWCOUNT = 0
BEGIN
	            -- Insert the amended address
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
                 zone,		-- CR1084
                 datechange,
				 DELTitleC,
				 DELFirstname,
				 DELLastname,
				 Latitude,
				 Longitude)
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
                 ISNULL(@email,' '),		-- jec UAT552 jec 07/10/08
                 @oldPropType,
                 @user,
                 @zone,		-- CR1084
                 getdate(),
				 @DELTitleC,
				 @DELFirstname,
				 @DELLastname,
				 @Latitude,
				 @Longitude)
END

END

	IF @addressType = 'H'
	BEGIN
		--NM to fix the LW 70517
		UPDATE CustomerAdditionalDetails
			SET DateInCurrentAddress = (select top 1 CA.datein from custaddress CA where 
										CA.custid = @custID
										AND CA.addtype = 'H'
										AND CA.datemoved IS NULL 
										order by CA.datein DESC) ,
			DateinPreviousAddress = (select top 1 CA.datein from custaddress CA where 
									 CA.custid = @custID
									 AND CA.addtype = 'H'
									 AND CA.datemoved IS NOT NULL 
									 order by CA.datein DESC) 
		WHERE custid = @custID
	END


	IF (@@error != 0)
    BEGIN
        SET @return = @@error
    END	
GO


-- End End End End End End End End End End End End End End End End End End End End End End End End 