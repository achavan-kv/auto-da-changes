if exists (select * from dbo.sysobjects where id = object_id('[dbo].[VE_CustomerDetailsUpdate]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[VE_CustomerDetailsUpdate]
END
GO

CREATE PROCEDURE [dbo].[VE_CustomerDetailsUpdate]
	 @UserJson XML 
	,@Message VARCHAR(MAX) OUTPUT
	,@StatusCode INT OUTPUT    
AS  
BEGIN 

BEGIN TRY
	SET NOCOUNT ON;   
	SET @Message = '';
	SET @StatusCode = 0
	
	IF NOT EXISTS(SELECT 1 FROM customer cust, @UserJson.nodes('/UserJson') n(x) WHERE n.x.value('CustomerID[1]','varchar(20)') = cust.custid )
	BEGIN  
		SET @Message = 'User not found.';
		SET @StatusCode = 404
	END
	IF (LEN(@Message) > 0)
	BEGIN
		RETURN
	END

	BEGIN TRANSACTION

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
	
	SELECT 
	@PhoneNumber = n.x.value('ContactNumber[1]' ,'[varchar](20)')
		FROM @UserJson.nodes('/UserJson/Contact/Contact') n(x) 

	SELECT
	@EmailId=	n.x.value('Email[1]' ,'[varchar](60)')
		FROM @UserJson.nodes('/UserJson/Address/Address') n(x) 

	IF(@CustId <> '' AND @CustId IS NOT NULL)
	BEGIN
		--added for validate
		UPDATE customer
		SET customer.branchnohdle = T.c.value('BranchNo[1]','varchar(20)'),
			customer.name = T.c.value('LastName[1]','varchar(20)'),
			customer.firstname = T.c.value('FirstName[1]','varchar(20)'),
			-- customer.title = T.c.value('Title[1]','varchar(20)'),
			customer.title = UPPER( REPLACE( T.c.value('Title[1]','varchar(20)'), '.','')),
			customer.dateborn = T.c.value('DOB[1]','[varchar](20)'),
			customer.idnumber = @IDnumber,
			customer.idtype = T.c.value('IdType[1]','varchar(20)')
		FROM @UserJson.nodes('/UserJson') T(c)
		WHERE customer.custid = @CustId


		declare @AddressTable TABLE
			(
				AddressID INT
			)
			
		INSERT INTO @AddressTable
		(
			AddressID
		)
		SELECT  
			 MAX(custAddress.AddressID) as addressID
		FROM @UserJson.nodes('/UserJson/Address/Address') n(x) 
		INNER JOIN custAddress ON custAddress.addtype = n.x.value('AddressType[1]' ,'[varchar](20)') 
		WHERE custAddress.custid = @CustId
		GROUP BY custAddress.addtype


		UPDATE custAddress
		SET custAddress.addtype = n.x.value('AddressType[1]' ,'[varchar](20)'),
			custAddress.datein = n.x.value('DateIn[1]','varchar(20)'),
			custAddress.cusaddr1 = n.x.value('Address1[1]' ,'[varchar](50)'),
			custAddress.cusaddr2 = n.x.value('Address2[1]' ,'[varchar](50)'),
			custAddress.cusaddr3 = n.x.value('Address3[1]' ,'[varchar](50)'),
			custAddress.cuspocode = n.x.value('Postalcode[1]' ,'[varchar](10)'),
			custAddress.deliveryarea = n.x.value('DeliveryArea[1]','varchar(20)') ,
			custAddress.custlocn = @BranchNohdle,
			--custAddress.notes = n.x.value('Notes[1]','varchar(2000)'),
			custAddress.Email = @EmailID,    
			custAddress.datechange = GETDATE()
		FROM @UserJson.nodes('/UserJson/Address/Address') n(x) 
		WHERE custAddress.custid = @CustId 
			AND 
			custAddress.AddressID  IN (SELECT AddressID FROM @AddressTable)
			AND custAddress.addtype = n.x.value('AddressType[1]' ,'[varchar](20)')
		
		INSERT INTO custAddress
		(
			--origbr,
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
			--notes,
			Email,
			--empeenochange,
			--zone,
			datechange
		)
			SELECT 
			--@origbr,
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
			--n.x.value('Notes[1]','varchar(20)') AS Notes,
			n.x.value('Email[1]' ,'[varchar](60)') AS Email, 
			--@user,
			--@zone,
			getdate()	
		FROM @UserJson.nodes('/UserJson/Address/Address') n(x) 
		WHERE n.x.value('AddressType[1]' ,'[varchar](20)') NOT IN ( SELECT DISTINCT addtype FROM custAddress WHERE custid = @CustId)


		UPDATE Custtel
		SET Custtel.tellocn = n.x.value('ContactLocation[1]' ,'[varchar](20)'),

				Custtel.telno=case when n.x.value('ContactLocation[1]' ,'[varchar](20)') ='M' 
				then n.x.value('ContactNumber[1]' ,'[varchar](20)') else 
				SUBSTRING(n.x.value('ContactNumber[1]' ,'[varchar](20)'), 4, 17) END ,
				
				Custtel.DialCode=case when n.x.value('ContactLocation[1]' ,'[varchar](20)') ='M' then 
				' ' else SUBSTRING(n.x.value('ContactNumber[1]' ,'[varchar](20)'), 1, 3)  end,

			Custtel.extnno = n.x.value('Ext[1]' ,'[varchar](20)'),
			Custtel.datechange = GETDATE() 
		FROM @UserJson.nodes('/UserJson/Contact/Contact') n(x) 
		WHERE Custtel.custid = @CustId
			AND Custtel.tellocn = n.x.value('ContactLocation[1]' ,'[varchar](20)')
					
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
		null AS origbr,
		@CustId AS custid,
		n.x.value('ContactLocation[1]' ,'[varchar](20)') AS tellocn ,
		N'' AS dateteladd,
		null AS datediscon,
		case when n.x.value('ContactLocation[1]' ,'[varchar](20)') ='M' 
				then n.x.value('ContactNumber[1]' ,'[varchar](20)') else 
				SUBSTRING(n.x.value('ContactNumber[1]' ,'[varchar](20)'), 4, 17) END AS telno ,
		n.x.value('Ext[1]' ,'[varchar](20)') AS ExtnNo,
		case when n.x.value('ContactLocation[1]' ,'[varchar](20)') ='M' then ' ' else SUBSTRING(n.x.value('ContactNumber[1]' ,'[varchar](20)'), 1, 3)  end AS DialCode,
		213465 AS empeenochange,
		GETDATE()  AS datechange
	FROM @UserJson.nodes('/UserJson/Contact/Contact') n(x) 
	WHERE n.x.value('ContactLocation[1]' ,'[varchar](20)') NOT IN ( SELECT DISTINCT tellocn FROM custtel WHERE custid = @CustId)
	
		IF (@@error != 0)
		BEGIN
			ROLLBACK
			print'Transaction rolled back'
			SET @StatusCode = 500;		
		END
		ELSE
		BEGIN
			SET @StatusCode = 200;		
			SET @Message = 'Customer information updated succesfully.'		
			print CONVERT(VARCHAR(3), @StatusCode)
			COMMIT
			print'Transaction committed'
		END 
	END 
	--END 
	END TRY  
		BEGIN CATCH  
			IF (@@error > 0)
		SET @StatusCode = 500;		
		SET @Message = ERROR_Message()
       ROLLBACK TRAN  
	END CATCH 
END