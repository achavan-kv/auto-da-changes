SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerAddressUpdateCurrentWorkSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerAddressUpdateCurrentWorkSP]
GO

-- =============================================
-- CREATED BY:	   Sachin Wandhare
-- CREATE DATE:	   14/08/2020
-- SCRIPT Name:    DN_CustomerAddressUpdateCurrentWorkSP.sql
-- SCRIPT COMMENT: Added @companyName parameter to save as Work company name on Proposal creation
-- Discription:   Address Standardization CR 
-- Version: <002>
-- Version	Date      By  Description
-- ----      --  -----------
-- 1.0			
-- 2.0		14/08/2020 Sachin Wandhare	Added @companyName parameter to save as Work company name on Proposal creation
-- =============================================
CREATE PROCEDURE 	dbo.DN_CustomerAddressUpdateCurrentWorkSP
			@custid varchar(20),
			@address1 varchar(50),
			@address2 varchar(50),
			@address3 varchar(50),
			@postcode  varchar(10),
			@companyName varchar(50),
			@user int,
			@return int OUTPUT

AS
SET NOCOUNT ON
-- New DateIn is truncated to exclude time part
DECLARE @DateIn SMALLDATETIME
SET     @DateIn = CONVERT(DATETIME,CONVERT(VARCHAR(10),GETDATE(),105),105)

	SET 	@return = 0			--initialise return code

	IF( 	@address1 != '' or
		@address2 != '' or
		@address3 != '' )
	BEGIN
	
		UPDATE	custaddress
		SET		cusaddr1 = @address1,
				cusaddr2 = @address2,
				cusaddr3 = @address3,
				cuspocode = @postcode,
				DELTitleC = '',		
				DELFirstName = CASE WHEN ISNULL(@companyName, '') = ''
							THEN DELFirstName
							ELSE @companyName
							END, -- Address Standardization CR
				DELLastname = '',
				empeenochange = @user
		WHERE	custid = @custid 
		AND		addtype = 'W'
		AND		(datemoved IS NULL OR datein = @DateIn)
	
		IF(@@rowcount = 0)
		BEGIN
			INSERT
			INTO		custaddress
					(origbr,	custid,	addtype, datein,	cusaddr1,
					cusaddr2, cusaddr3, cuspocode, custlocn,
					resstatus, mthlyrent, datemoved, hasstring,
					notes, Email, PropType, empeenochange,
					datechange,DELTitleC, DELFirstName,DELLastname )
			VALUES	(null,
					@custid, 'W', 
					-- Remove the time part from the DateIn,
					-- otherwise this can error as being after the DateProp.
					@DateIn,
					@address1, @address2, @address3,
					@postCode, 
					null,			
					null,		
					null,		
					null,			
					0,			
					N'',			
					N'',
					N'', 		
					@user,	
					getdate(),
					N'',
					@companyName,
					N'')
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


