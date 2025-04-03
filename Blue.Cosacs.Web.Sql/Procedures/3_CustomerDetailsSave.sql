
/****** Object:  StoredProcedure [dbo].[CustomerDetailsSave]    Script Date: 11/19/2018 1:43:18 PM ******/
IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'CustomerDetailsSave')
BEGIN
DROP PROCEDURE [dbo].[CustomerDetailsSave]
END
GO

/****** Object:  StoredProcedure [dbo].[CustomerDetailsSave]    Script Date: 11/19/2018 1:43:18 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--*********************************************************************** 
-- Script Name : CustomerDetailsSave.sql 
-- Created For  : Unipay (T) 
-- Created By   : Sagar Kute
-- Created On   : 21/06/2018 
--*********************************************************************** 
/*
-- Change Control 
-- -------------- 
-- Date(DD/MM/YYYY)				Changed By(FName LName)			Description 
-- ModifiedOn(DD/MM/YYYY)		ModifiedBy(FName LName)			Comment
-- DATE(DD/MM/YYYY)				Modify_By(FName LName)			Remark
-- -------------------------------------------------------------------------------------------------------


--*********************************************************************************************************
-- Insert

DECLARE @ReturnCustId VARCHAR(20) = '';
DECLARE @Message VARCHAR(MAX) = '';
DECLARE @StatusCode INT = 0;

EXEC CustomerDetailsSave @UnipayId = '34343', @Title = 'Mr.', @FirstName = 'First33', @LastName = 'Last44', @EmailId = '22334455@gmail.com', 
							@PhoneNumber = '4433225123', @DateBorn = '1966-6-6', @CusAddr1 = 'Pune', 
							@DeliveryArea = 'H', @IDType = 'S', @IDNumber = '3666329123', @ReturnCustId = @ReturnCustId OUTPUT, 
							@Message = @Message OUTPUT, @StatusCode = @StatusCode OUTPUT , @CustId = '', @NewRecord = 1 --, @Return = 0
SELECT @ReturnCustId as ReturnCustId, @Message as Message, @StatusCode as StatusCode

-- UPDATE

--DECLARE @ReturnCustId VARCHAR(20) = '';
--DECLARE @Message VARCHAR(MAX) = '';

EXEC CustomerDetailsSave @UnipayId = '3434', @Title = 'Mr.', @FirstName = '33RtFirst33', @LastName = '44EmLast44', @EmailId = '223344@gmail.com', 
							@PhoneNumber = '4433225566', @DateBorn = '1966-6-6', @CusAddr1 = 'Pune', 
							@DeliveryArea = 'H', @IDType = 'S', @IDNumber = '3666329129', @ReturnCustId = @ReturnCustId OUTPUT, 
							@Message = @Message OUTPUT , @CustId = @ReturnCustId, @NewRecord = 0 --, @Return = 0
SELECT @ReturnCustId as ReturnCustId, @Message as Message

*/

CREATE PROCEDURE [dbo].[CustomerDetailsSave]
	-- Below params send by MobileApp(Unipay)
	@UnipayId NVARCHAR(200), --Required 
	@FirstName VARCHAR(30), --Required
	@LastName VARCHAR(60), --Required
	@EmailId VARCHAR(60), --Required
	@PhoneNumber VARCHAR(20) = '', --Required
	-- Customer Table
	@CustId VARCHAR(20) = N'',
	@DateBorn DATETIME = NULL,  --Required 
	@Origbr smallint = NULL,
	@OtherId varchar(20) = NULL,     
	@BranchNohdle smallint = NULL ,
	@Title varchar(25) = N'',  --Required
	@Alias varchar(25) = NULL,
	@AddrSort varchar(20) = NULL,
	@NameSort varchar(20) = NULL,
	@Sex char(1) = NULL,
	@EthniCity char(1) = NULL,
	@MoreRewardsNo varchar(16) = NULL,
	@EffectiveDate smalldatetime = NULL ,
	@IDType char(4), --Required
	@IDNumber char(30), --Required
	@UserNo int  = NULL,
	@DateChanged datetime = NULL,
	@MaidenName varchar(30) = NULL, 
	@StoreType varchar(2) = NULL,
	@Dependants int = NULL,
	@MaritalStat char(1) = NULL,
	@Nationality char(4) = NULL,
	@ResieveSms bit = NULL,
	-- CustTel Table
	@AddressType CHAR(2) = NULL,
	@CusAddr1	VARCHAR(50) = N'', --Required -M
	@CusAddr2	VARCHAR(50) = NULL,
	@CusAddr3	VARCHAR(50) = NULL,
	@NewRecord bit = 0,
	@DeliveryArea NVARCHAR(16) = N'', --Required - M
	@PostCode varchar(10) = NULL,
	@Notes varchar(1000) = NULL,
	@DateIn datetime = NULL,
	@User int = NULL, 
	@Zone varchar(4) = NULL,
	-- CustTel Table
	@DateTelAdd DATETIME = NULL,
	@ExtnNo VARCHAR(6) = NULL,
	@TelLocn CHAR(2) = NULL,
	@DialCode VARCHAR(8) = NULL,
	@EmpeeNoChange INT = NULL
	-- Output Param
	,@ReturnCustId VARCHAR(20) OUTPUT
	,@Message VARCHAR(MAX) OUTPUT
	,@StatusCode INT OUTPUT
AS
BEGIN
    SET @Message = ''
    SET @StatusCode = 0
	--IF(@PhoneNumber <> '' AND @PhoneNumber IS NOT NULL)
	--BEGIN
	--IF( @NewRecord = 1 AND 
	--		(
	--			EXISTS(SELECT TOP 1 1 FROM [dbo].[custtel] tel WHERE (LTRIM(RTRIM(tel.telno)) = @PhoneNumber))) 
	--			OR 
	--			EXISTS(SELECT TOP 1 1 FROM [dbo].[customer] cust WHERE (LTRIM(RTRIM(cust.IdNumber)) = @IdNumber))
	--			OR
	--			EXISTS(SELECT 1 FROM dbo.CustUnipay WHERE UnipayId = @UnipayId)
	--	)
	--	BEGIN
	--		PRINT 'Exists'
	--		--SET @Message = 'User Exists with phone number (' + LTRIM(RTRIM(@PhoneNumber)) + ')';		
	--		SET @Message = 'User already exists';		
	--		SET @StatusCode = 406;		
	--	END
	--END

	--IF( @NewRecord = 1 AND EXISTS(SELECT TOP 1 1 FROM [dbo].[customer] cust WHERE (LTRIM(RTRIM(cust.IdNumber)) = @IdNumber)))
	--BEGIN
	--	IF(LTRIM(RTRIM(@Message)) = '')
	--		SET @Message = 'User Exists with Id Number (' + LTRIM(RTRIM(@IdNumber)) + ')';
	--	ELSE
	--		SET @Message += ' and Id Number (' + LTRIM(RTRIM(@IdNumber)) + ')';
	--END

	--IF(EXISTS(SELECT 1 FROM dbo.CustUnipay WHERE UnipayId = @UnipayId))
	--BEGIN
	--	IF(LTRIM(RTRIM(@Message)) = '')
	--		SET @Message = 'User Exists with Unipay Number (ExtUId) (' + LTRIM(RTRIM(@UnipayId)) + ')';
	--	ELSE
	--		SET @Message += ' and Unipay Number (ExtUId) (' + LTRIM(RTRIM(@UnipayId)) + ')';
	--END
	IF(LEN(LTRIM(RTRIM(@Message))) > 0)
	BEGIN
		SET @ReturnCustId = '';
		RETURN;
	END

	IF(@NewRecord=0)   --Only for Update Case
	BEGIN
		IF(NOT EXISTS (SELECT 1 FROM dbo.customer WHERE custid = @custid))
		BEGIN
			SET @ReturnCustId = '';
			SET @Message='User does not exists'
			print @Message
			RETURN;
		END
	END

	BEGIN TRANSACTION 
	IF(@DateBorn IS NULL)
		SET @DateBorn  = N''

	IF(@Origbr IS NULL)
		SET @Origbr = 0;
	
	IF(@OtherId IS NULL)
		SET @OtherId = N'';
		    
	IF(@BranchNohdle IS NULL)
		SET @BranchNohdle = (SELECT TOP 1 hobranchno FROM country);

	IF(@Alias IS NULL)
		SET @Alias = N''

	IF(@AddrSort IS NULL)
		SET @AddrSort  = N''

	IF(@NameSort IS NULL)
		SET @NameSort  = N'' 

	IF(@Sex IS NULL)
		SET @Sex  = N''

	IF(@EthniCity IS NULL)
		SET @EthniCity  = N''

	IF(@MoreRewardsNo IS NULL)
		SET @MoreRewardsNo  = N''
	
	IF(@UserNo IS NULL)
		SET @UserNo   = 0

	IF(@MaidenName IS NULL)
		SET @MaidenName  = N'' 

	IF(@StoreType IS NULL)
		SET @StoreType  = N''

	IF(@Dependants IS NULL)
		SET @Dependants = 0

	IF(@MaritalStat IS NULL)
		SET @MaritalStat  = N''

	IF(@Nationality IS NULL)
		SET @Nationality  = N''

	IF(@ResieveSms IS NULL)
		SET @ResieveSms  = 0

	IF(@AddressType IS NULL)
		SET @AddressType = N'H'

	IF(@CusAddr2 IS NULL)
		SET @CusAddr2	 = N''

	IF(@CusAddr3 IS NULL)
		SET @CusAddr3	 = N''

	IF(@NewRecord IS NULL)
		SET @NewRecord  = 0

	IF(@PostCode IS NULL)
		SET @PostCode  = N''

	IF(@Notes IS NULL)
		SET @Notes  = N''

	IF(@DateIn IS NULL)
		SET @DateIn  = N''

	IF(@User IS NULL)
		SET @User  = 0 

	IF(@Zone IS NULL)
		SET @Zone  = N''

	IF(@DateTelAdd IS NULL)
		SET @DateTelAdd  = N''

	IF(@TelLocn IS NULL)
		SET @TelLocn  = N'M'

	IF(@DialCode IS NULL)
		SET @DialCode  = N''

	IF(@EmpeeNoChange IS NULL)
		SET @EmpeeNoChange = 213465

	IF(@CustId = '')
		SET @CustId = NULL

	-- Logic for creating CustId needs to be implement here. 
	-- Since CustId is provided from UI in CoSaCs Web and Windons, but in Unipay it should be created on DB end.
	-- Code to get branch code from DeliveryArea.
	DECLARE @Return int = 0;
	--DECLARE @CustId VARCHAR(20) = '';
	IF(@CustId IS NULL OR @CustId = '' OR @NewRecord = 1)
	BEGIN
		PRINT 'WHILE started.'
		WHILE(1 = 1)
		BEGIN
			SET @CustId = '';
			EXEC GenerateCustID @IDNumber = @IDNumber, @FirstName = @FirstName, @LastName = @LastName, @len = 3, 
							@exclude = '0123456789:;<=>?@O[]`^\/_-', @CustId = @CustId OUTPUT
			IF(NOT EXISTS(SELECT 1 FROM dbo.customer WHERE custid = @CustId))
				BREAK;
		END
		PRINT 'WHILE Ended.'
	END
	PRINT 'CustId:- ' + @CustId

	IF(ISNULL(@IDType, '') <> '')
	BEGIN
		SET @IDType = (	CASE 
						WHEN ISNULL(LTRIM(RTRIM(@IDType)), '') <> '' 
						THEN 
							CASE WHEN LTRIM(RTRIM(@IDType)) = 'PP' THEN 'P' ELSE
								CASE WHEN LTRIM(RTRIM(@IDType)) = 'DP' THEN 'D' ELSE
									CASE WHEN LTRIM(RTRIM(@IDType)) = 'NI' THEN 'I' ELSE LTRIM(RTRIM(@IDType)) 
									END 
								END 
							END
						ELSE '' 
					  END)
	END

	
	IF(@NewRecord = 0 AND EXISTS(SELECT 1 FROM dbo.customer WHERE custid = @CustId))
	BEGIN
		SELECT
			 @origbr		= origbr,
            @otherid		= otherid ,
            @branchnohdle	= branchnohdle,
            --@name			= name,
            --@firstname	= firstname,
            @title			= title,
            @alias			= alias,
            @addrsort		= addrsort,
            @namesort		= namesort,
            @dateborn		= dateborn,
            @sex			= sex,
            @ethnicity		= ethnicity,
            @morerewardsno	= morerewardsno,
            @effectivedate	= effectivedate,    
            @idnumber		= IdNumber,
            @idtype			= IdType,
            @datechanged	= getdate(),
            @userno			= empeenochange,    
            @maidenname		= maidenname,
            @dependants		= dependants,
            @maritalStat	= maritalStat,
            @nationality	= nationality,
			@resieveSms		= resieveSms
		FROM dbo.customer
		WHERE custid = @custid
	END

	EXEC DN_CustomerUpdateSP @custid = @CustId, @origbr = @Origbr, @otherid = @OtherId, @branchnohdle = @BranchNohdle, @name = @LastName, 
							@firstname = @FirstName, @title=@Title,@alias = @Alias,@addrsort = @AddrSort, @namesort = @AddrSort,
							@dateborn = @DateBorn, @sex = @Sex, @ethnicity = @EthniCity, @morerewardsno = @MoreRewardsNo, 
							@effectivedate = @EffectiveDate, @idtype = @IDType,@idnumber = @IDNumber, @userno = @UserNo, 
							@datechanged = @DateChanged, @maidenname = @MaidenName, @storetype = @StoreType, @dependants = @Dependants, 
							@maritalStat = @MaritalStat, @Nationality = @Nationality, @ResieveSms = @ResieveSms, @return = @Return OUTPUT
	PRINT 'DN_CustomerUpdateSP executed.'

	IF(@NewRecord = 0 AND EXISTS(SELECT 1 FROM dbo.custaddress WHERE custid = @CustId AND addtype='H'))
	BEGIN
		SELECT  
			@DateIn			= datein,
			@CusAddr1		= cusaddr1,
			@CusAddr2		= cusaddr2,
			@CusAddr3		= cusaddr3,
			@PostCode		= cuspocode,
			@DeliveryArea	= deliveryarea,
			--@oldResStatus = resstatus,
			--@oldMthlyRent = mthlyrent,
			@Notes			= notes,
			--@EmailId		= Email,
			--@oldPropType	= PropType,
			@user			= empeenochange,
			@zone			= Zone	
			--,datechange	=  getdate() 
		FROM dbo.custaddress 
		WHERE custid = @CustId AND addtype='H'
	END

	EXEC CustAddressSave @custid = @CustId, @addressType = @AddressType, @address1 = @CusAddr1, @address2 = @CusAddr2, 
						@address3 = @CusAddr3, @postcode = @PostCode, @DeliveryArea = @DeliveryArea,@notes = @Notes, @email = @EmailId,
						@datein = @DateIn, @user = @User,@newRecord = @NewRecord,@Zone = @Zone,
						@DELTitleC=@Title,
						@DELFirstname=@FirstName,
						@DELLastname=@LastName,
						@Latitude = NULL,
						@Longitude= NULL,
						@return = @Return OUTPUT
	PRINT 'CustAddressSave executed.'

	
	IF(@NewRecord = 0 AND EXISTS(SELECT 1 FROM dbo.custtel WHERE custid = @CustId AND tellocn='M'))
	BEGIN
		SELECT 
			@DateTelAdd = dateteladd,
			@TelLocn = tellocn,
			@ExtnNo = extnno,
			@DialCode = DialCode, 
			@EmpeeNoChange = empeenochange
			--,datechange =  getdate()
		FROM dbo.custtel
		WHERE custid = @CustId AND tellocn='M'
		ORDER BY datechange DESC
	END

	
	EXEC CustTelSave @custid = @CustId, @tellocn = @TelLocn, @dateteladd = @DateTelAdd,@telno = @PhoneNumber, @extnno = @ExtnNo, 
					@dialcode = @DialCode, @empeenochange = @EmpeeNoChange, @newRecord = @NewRecord, @return = @Return OUTPUT

	PRINT 'CustTelSave executed.'
	
	IF(ISNULL(@UnipayId, '')<> '' AND @NewRecord = 1 AND (NOT EXISTS(SELECT 1 FROM dbo.CustUnipay WHERE custid = @CustId AND UnipayId = @UnipayId)))
	BEGIN
		INSERT INTO CustUnipay(CustId, UnipayId, CreatedOn, UpdatedOn) VALUES (@CustId, @UnipayId, GETDATE(), GETDATE());
		PRINT 'Inserted values into CustUnipay table.'
	END
	
	IF (@@error != 0)
	BEGIN
		ROLLBACK
		print'Transaction rolled back'
		SET @Return = @@error
		SET @StatusCode = 500;		
	END
	ELSE
	BEGIN
		SET @ReturnCustId = LTRIM(RTRIM(@CustId))
		SET @StatusCode = 201;		
		print CONVERT(VARCHAR(3), @StatusCode)
		COMMIT
		print'Transaction committed'
	END
END
GO