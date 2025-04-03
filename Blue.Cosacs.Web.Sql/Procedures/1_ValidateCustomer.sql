
/****** Object:  StoredProcedure [dbo].[ValidateCustomer]    Script Date: 11/19/2018 1:28:35 PM ******/
IF EXISTS (SELECT * FROM sysobjects 
   WHERE NAME = 'ValidateCustomer'
   )
BEGIN
DROP PROCEDURE [dbo].[ValidateCustomer]
END
GO

/****** Object:  StoredProcedure [dbo].[ValidateCustomer]    Script Date: 11/19/2018 1:28:35 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ValidateCustomer]
	@IdNumber char(30),
	@IdType char(4),
	@PhoneNumber VARCHAR(20)
AS
BEGIN

	DECLARE @ReturnIdNumber char(30) = '';
	DECLARE @ReturnIdType char(4) = '';

	DECLARE @Message VARCHAR(MAX) = '';
	DECLARE @custid varchar(20) = '';
	DECLARE @LastName varchar(60) = '';
	DECLARE @FirstName varchar(30) = '';
	DECLARE @EmailId varchar(60) = '';
	DECLARE @DOB datetime = '';
	DECLARE @Status INT = 0;

	IF (@PhoneNumber != '' AND @PhoneNumber IS NOT NULL) 
	BEGIN
		PRINT 'First IF'
		IF ( 
				(
					SELECT DISTINCT COUNT(tel.custId)  FROM [dbo].[custtel] tel 
					WHERE REPLACE(REPLACE(REPLACE(LTRIM(RTRIM(tel.telno)) , '-', ''),' ',''),'+','') = REPLACE(REPLACE( REPLACE(LTRIM(RTRIM(@PhoneNumber)), '-', ''),' ',''),'+','')
						AND tel.tellocn = 'M' 
						and datediscon is null --#7224649 Add delete condition in Custtel table query
				) > 1
			)
		BEGIN
			PRINT 'Multiple for PhoneNumber'
			SET @Message += 'Multiple users found';		
			SET @Status = 406;
		END
		ELSE IF(
			ISNULL(@IdNumber,'') <> '' AND 
			EXISTS(
						SELECT 1 FROM [dbo].[customer] cust 
						WHERE (LTRIM(RTRIM(cust.IdNumber)) = @IdNumber) 
						GROUP BY cust.IdNumber 
						HAVING COUNT(LTRIM(RTRIM(cust.IdNumber))
					) > 1)
				)
		BEGIN
			PRINT 'Multiple for IdNumber'
			SET @Message += 'Multiple users found';		
			SET @Status = 406;
		END
		ELSE
		BEGIN
			SELECT @custid = cust.[custid]
				  ,@LastName = cust.[name]
				  ,@FirstName = cust.[firstname]
				  ,@DOB = cust.[dateborn]
				  ,@EmailId = adr.Email
				  ,@ReturnIdNumber = cust.[IdNumber]
				  ,@ReturnIdType = cust.[IdType]
				  --,tel.telno
			  FROM [dbo].[customer] cust
			  INNER JOIN [dbo].[custtel] tel ON tel.custid = cust.custid
			  INNER JOIN [dbo].[custaddress] adr ON adr.custid = cust.custid
			  WHERE
			  (LTRIM(RTRIM(cust.IdNumber)) = @IdNumber OR ISNULL(LTRIM(RTRIM(cust.IdNumber)), '') = '')
			  AND (REPLACE(REPLACE(REPLACE(LTRIM(RTRIM(tel.telno)) , '-', ''),' ',''),'+','')			
				= REPLACE(REPLACE( REPLACE(LTRIM(RTRIM(@PhoneNumber)), '-', ''),' ',''),'+',''))
			  AND adr.addtype = 'H'

			IF(@custid != '' AND @custid != '0')
			BEGIN
				SET @Message += 'User Exists with customer : ' + @custid;
				SET @Status = 200;
			END
			ELSE 
			BEGIN
				SELECT @custid = cust.[custid]
					  ,@LastName = cust.[name]
					  ,@FirstName = cust.[firstname]
					  ,@DOB = cust.[dateborn]
					  ,@EmailId = adr.Email
					  ,@ReturnIdNumber = cust.[IdNumber]
					  ,@ReturnIdType = cust.[IdType]
					  --,tel.telno
				  FROM [dbo].[customer] cust
				  INNER JOIN [dbo].[custtel] tel ON tel.custid = cust.custid
				  INNER JOIN [dbo].[custaddress] adr ON adr.custid = cust.custid
				  WHERE					
					REPLACE(REPLACE(REPLACE(LTRIM(RTRIM(tel.telno)) , '-', ''),' ',''),'+','')			
					= REPLACE(REPLACE( REPLACE(LTRIM(RTRIM(@PhoneNumber)), '-', ''),' ',''),'+','')					
					AND adr.addtype = 'H'

				IF(@custid != '' AND @custid != '0')
				BEGIN
					SET @Message += 'User Exists with customer : ' + @custid;
					SET @Status = 200;					
				END
				ELSE 
				BEGIN
					SET @Message = 'No user found';
					SET @Status = 404;
				END
			END
		END	
		 	
	END
	ELSE
	BEGIN
		PRINT 'First ELSE'
		IF(
				EXISTS(SELECT 1 FROM [dbo].[custtel] tel WHERE  
						REPLACE(REPLACE(REPLACE(LTRIM(RTRIM(tel.telno)) , '-', ''),' ',''),'+','')			
			= REPLACE(REPLACE( REPLACE(LTRIM(RTRIM(@PhoneNumber)), '-', ''),' ',''),'+','')	
				) OR
				EXISTS(SELECT 1 FROM [dbo].[customer] cust WHERE (LTRIM(RTRIM(cust.IdNumber)) = @IdNumber))
			)
		BEGIN	
			SET @Message += 'Multiple users found';		
			SET @Status = 406;
		END
		ELSE
		BEGIN
			SET @Message = 'No user found';
			SET @Status = 404;
		END
	END	 

	SELECT 
		@ReturnIdNumber 'IdNumber'
		, CASE 
			WHEN ISNULL(LTRIM(RTRIM(@ReturnIdType)), '') <> '' 
			THEN 
				CASE WHEN LTRIM(RTRIM(@ReturnIdType)) = 'P' THEN 'PP' ELSE
					CASE WHEN LTRIM(RTRIM(@ReturnIdType)) = 'D' THEN 'DP' ELSE
						CASE WHEN LTRIM(RTRIM(@ReturnIdType)) = 'I' THEN 'NI' ELSE LTRIM(RTRIM(@ReturnIdType)) 
						END 
					END 
				END
			ELSE '' 
		  END'IdType'
		, @custid 'CustomerId'
		, @LastName 'LastName'
		, @FirstName 'FirstName'
		, @DOB 'DateOfBirth'
		, @EmailId 'EmailId'
		, @Message 'Message'
		, @Status 'StatusCode' 

END	 

GO

