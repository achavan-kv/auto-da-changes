SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerAddressUpdateHomeSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerAddressUpdateHomeSP]
GO

CREATE PROCEDURE 	dbo.DN_CustomerAddressUpdateHomeSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_CustomerAddressUpdateHomeSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Update Customer address
-- Author       : ??
-- Date         : ??
--
-- This SP will never create new custaddress records that is done through the BasicCustomerDetails screen
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 13/07/08  jec CR907 - update date changed
-- 19/02/10  jec CR1072 Malaysia merge UAT153
-- ================================================
	-- Add the parameters for the stored procedure here

			@custid varchar(20),
			@datein datetime OUT,
			@resstatus char(1) OUT,
			@proptype char(4) OUT,
			@pdatein datetime OUT,
			@presstatus char(1) OUT,
			@pproptype char(4) OUT,
			@mthlyrent float OUT,
			@return int OUTPUT
AS

	SET 	@return = 0			--initialise return code
	DECLARE @prevdate DATETIME
	Declare @prevDateTimestamp DATETIME		-- uat(4.3) - 153
	DECLARE @datechange datetime			--CR907 jec
	DECLARE @olddatechange datetime			--CR907 jec
	DECLARE @oldDatein      datetime		--CR907 jec
    DECLARE @oldResStatus   char(1)		--CR907 jec
    DECLARE @oldMthlyRent   float			--CR907 jec
    DECLARE @oldPropType    char(4)		--CR907 jec

	/*	this SP will never create new custaddress records
		that is done through the BasicCustomerDetails screen
		Here - if the datein chages it is always carried out as an update
		to the custaddress record. */

	-- CR907 - get current values
	SELECT  @oldDatein    = datein,
                @oldResStatus = resstatus,
                @oldMthlyRent = mthlyrent,
                @oldPropType  = proptype,
				@olddatechange = datechange						

        FROM    custaddress
        WHERE	custid = @custid
			AND		addtype = 'H'
			AND		datemoved is null

        IF (@@ROWCOUNT = 0)
        BEGIN
            SET @oldDatein    = null
            SET @oldResStatus = null
            SET @oldMthlyRent = null
			SET @olddatechange = null
            SET @oldPropType  = ''				
        END
			-- CR907 update datechange
			-- if none of the following have changed;
			-- (proptype, resstatus, mthlyrent, datemoved)
			-- the datechange will be set back to the previous datechange 

			if	@oldDatein    = @datein				
                and @oldResStatus = @resstatus
                and @oldMthlyRent = @mthlyrent
                and @oldPropType  = @proptype				
			
				set @datechange = @olddatechange
			else
				set @datechange = getdate()

	--update the current address data
	UPDATE	custaddress
	SET		datein = @datein,
			resstatus = @resstatus,
			proptype = @proptype,
			mthlyrent = @mthlyrent,
			datechange=@datechange					--CR907 jec 13/08/07
	WHERE	custid = @custid
	AND		addtype = 'H'
	AND		datemoved is null

	IF(@@rowcount>0)
	BEGIN
		--update the previous address
		SELECT	TOP 1
				@prevdate = datemoved,
				@prevDateTimestamp = datechange  -- uat(4.3) - 153
		FROM		custaddress
		WHERE	custid = @custid
		AND		addtype = 'H'	
		AND		datemoved is not null
		ORDER BY	datein DESC, datechange DESC	--71106 RM - multiple address with same datein
		
		IF(@@rowcount>0)
		BEGIN
			UPDATE TOP(1) custaddress	-- uat(4.3) - 153
			SET		datein = @pdatein,
					resstatus = @presstatus,
					proptype = @pproptype
			WHERE	custid = @custid
			AND		addtype = 'H'
			AND		datemoved = @prevdate
			AND		datechange = @prevDateTimestamp  -- uat(4.3) - 153	
			--ORDER BY	datein DESC, datechange DESC	--71106 RM - multiple address with same datein			
		END
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

-- End End End End End End End End End End End End End End End End End End End End End End End End End End 
