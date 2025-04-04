if exists (select * from dbo.sysobjects where id = object_id('[dbo].[VE_CustomerDetailsSave]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[VE_CustomerDetailsSave]
END
GO

CREATE PROCEDURE [dbo].[VE_CustomerDetailsSave]
	 @UserJson XML 
	,@Message VARCHAR(MAX) OUTPUT
	,@StatusCode INT OUTPUT    
AS  
BEGIN 

BEGIN TRY
	SET NOCOUNT ON;   
	SET @Message = '';
	SET @StatusCode = 0
	
	DECLARE @CustId VARCHAR(20) = N'',
	@BranchNohdle smallint = NULL,
	@PhoneNumber VARCHAR(20) = '',
	@IDNumber char(30),
	@EmailId VARCHAR(60)

	SELECT 
		@CustId = T.c.value('CustomerID[1]','varchar(20)'),
		@BranchNohdle = T.c.value('BranchNo[1]','varchar(20)') ,
		
		@IDNumber =		T.c.value('ID[1]','varchar(30)')

	FROM @UserJson.nodes('/UserJson') T(c)
	select 
	@PhoneNumber = n.x.value('ContactNumber[1]' ,'[varchar](20)')
			FROM @UserJson.nodes('/UserJson/Contact/Contact') n(x) 

	select  

	@EmailId=	n.x.value('Email[1]' ,'[varchar](60)')
	FROM @UserJson.nodes('/UserJson/Address/Address') n(x) 

	IF(@CustId <> '' AND @CustId IS NOT NULL)
	BEGIN
		IF(EXISTS(SELECT 1 FROM [dbo].[customer] cust WHERE (LTRIM(RTRIM(cust.CustId)) = @CustId)))
		BEGIN
			SET @Message = 'Customer found with details: ' + @CustId;
			SET @StatusCode = 406
		END
		ELSE
		IF(		
			(
				SELECT CASE WHEN COUNT(1) > 1 THEN 1 ELSE NULL END FROM 
				(
					SELECT DISTINCT tel.custId AS CustId FROM [dbo].[custtel] tel 
					WHERE REPLACE(REPLACE(REPLACE(LTRIM(RTRIM(tel.telno)) , '-', ''),' ',''),'+','') = REPLACE(REPLACE( REPLACE(LTRIM(RTRIM(@PhoneNumber)), '-', ''),' ',''),'+','')	
					GROUP BY tel.custId,REPLACE(REPLACE(REPLACE(LTRIM(RTRIM(tel.telno)) , '-', ''),' ',''),'+','')
					HAVING COUNT(REPLACE(REPLACE(REPLACE(LTRIM(RTRIM(tel.custId)) , '-', ''),' ',''),'+','')) > 0 
				) AS Temp
			) IS NOT NULL
		)
		BEGIN
			SET @Message += 'One/Multiple users found for PhoneNumber: ' + @PhoneNumber;		
			SET @StatusCode = 406;
		END
		ELSE IF( 
			EXISTS(SELECT 1 FROM [dbo].[customer] cust WHERE (LTRIM(RTRIM(cust.IdNumber)) = @IdNumber) GROUP BY cust.IdNumber HAVING COUNT(LTRIM(RTRIM(cust.IdNumber))) > 0)
			)
		BEGIN
			SET @Message += 'One/Multiple users found for IdNumber: ' + @IdNumber;		
			SET @StatusCode = 406;
		END
		ELSE
		BEGIN
			  Declare @UnipayId NVARCHAR(200), 
				@FirstName VARCHAR(30),
				@LastName VARCHAR(60),
				-- Customer Table
				@DateBorn DATETIME = NULL, 
				@Origbr smallint = NULL,
				@OtherId varchar(20) = NULL,     
				@Title varchar(25) = N'',
				@Alias varchar(25) = NULL,
				@AddrSort varchar(20) = NULL,
				@NameSort varchar(20) = NULL,
				@Sex char(1) = NULL,
				@EthniCity char(1) = NULL,
				@MoreRewardsNo varchar(16) = NULL,
				@EffectiveDate smalldatetime = NULL ,
				@IDType char(4), --Required
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
				@CusAddr1	VARCHAR(50) = N'',
				@CusAddr2	VARCHAR(50) = NULL,
				@CusAddr3	VARCHAR(50) = NULL,
				@NewRecord bit = 0,
				@DeliveryArea NVARCHAR(16) = N'',
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

				insert into customer
				(
								origbr,
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
								idnumber,
								idtype,
								datechange,
								empeenochange,
								maidenname,
								dependants,
								maritalstat,
								nationality,
								storetype
					)
			SELECT 
				@origbr,
				@CustId AS 'custid',
				  @otherid ,
				T.c.value('BranchNo[1]','varchar(20)') AS 'branchnohdle',
				T.c.value('LastName[1]','varchar(20)') AS 'Name',
				T.c.value('FirstName[1]','varchar(20)') AS 'FirstName',
				UPPER( REPLACE( T.c.value('Title[1]','varchar(20)'), '.','')) AS 'Title',
				 @alias,
				 @addrsort,
				 @namesort,
				T.c.value('DOB[1]','varchar(20)') AS 'DOB',
				 @sex,
				@ethnicity,    
				@morerewardsno,
				@effectivedate,
				@IDnumber,
				T.c.value('IdType[1]','varchar(20)') AS 'IdType',
				@datechanged,
				@userno,
				@maidenname,
				@dependants,
				@maritalStat,
				@nationality ,
				@storetype
	
			FROM @UserJson.nodes('/UserJson') T(c)


			INSERT INTO custAddress
			(
				 origbr,
				 custid,
				 addtype,
				 datein,
				 cusaddr1,
				 cusaddr2,
				 cusaddr3,
				 cuspocode,
				 deliveryarea,
				 custlocn,
				 datemoved,
				 hasstring,
				 notes,
				 Email,
				 empeenochange,
				 zone,
				 datechange
			)
				SELECT 
				@origbr,
				@CustId,
				n.x.value('AddressType[1]' ,'[varchar](20)') AS AddType ,
				n.x.value('DateIn[1]','varchar(20)') AS DateIn,
				n.x.value('Address1[1]' ,'[varchar](50)') AS cusAddr1 ,
				n.x.value('Address2[1]' ,'[varchar](50)') AS cusAddr2 ,
				n.x.value('Address3[1]' ,'[varchar](50)') AS cusAddr3 ,
				n.x.value('Postalcode[1]' ,'[varchar](20)') AS cuspocode ,
				n.x.value('DeliveryArea[1]','varchar(20)') AS DeliveryArea,
				@BranchNohdle,
				null,
				0,
				n.x.value('Notes[1]','varchar(20)') AS Notes,
				n.x.value('Email[1]' ,'[varchar](60)') AS Email, 
				@user,
				@zone,
				getdate()	
			FROM @UserJson.nodes('/UserJson/Address/Address') n(x) 

			INSERT INTO Custtel
			(
				 origbr,
				custid,
				tellocn,
				dateteladd,
				datediscon,
				telno,
				extnno,
				DialCode,
				empeenochange,
				datechange
			)
				SELECT 
				null,
				@CustId,
				n.x.value('ContactLocation[1]' ,'[varchar](20)') AS tellocn ,
				@dateteladd,
				null,
				case when n.x.value('ContactLocation[1]' ,'[varchar](20)') ='M' 
				then n.x.value('ContactNumber[1]' ,'[varchar](20)') else 
				SUBSTRING(n.x.value('ContactNumber[1]' ,'[varchar](20)'), 4, 17) END AS telno ,
				n.x.value('Ext[1]' ,'[varchar](20)') AS ExtnNo,
				case when n.x.value('ContactLocation[1]' ,'[varchar](20)') ='M' then ' ' else SUBSTRING(n.x.value('ContactNumber[1]' ,'[varchar](20)'), 1, 3)  end AS DialCode, -- IP - 26/10/09 - UAT(113) previously was attempting to insert null.
				@empeenochange,
				GETDATE() 
			FROM @UserJson.nodes('/UserJson/Contact/Contact') n(x) 

			IF (@@error != 0)
				BEGIN
					ROLLBACK
					print'Transaction rolled back'
					SET @StatusCode = 500;		
				END
				ELSE
				BEGIN
					SET @StatusCode = 201;		
					SET @Message = 'Customer Information Saved Succesfully.'		
					print CONVERT(VARCHAR(3), @StatusCode)
					COMMIT
					print'Transaction committed'
				END 
		END
	END
	END TRY  
		BEGIN CATCH  
			IF (@@error > 0)
		SET @StatusCode = 500;		
		SET @Message = ERROR_Message()
       ROLLBACK TRAN  
	END CATCH 
END