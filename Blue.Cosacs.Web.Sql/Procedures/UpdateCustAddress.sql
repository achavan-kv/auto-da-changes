

/****** Object:  StoredProcedure [dbo].[UpdateCustAddress]    Script Date: 11/19/2018 1:57:19 PM ******/
IF EXISTS (SELECT * FROM sysobjects 
   WHERE NAME = 'UpdateCustAddress'
   )
BEGIN
DROP PROCEDURE [dbo].[UpdateCustAddress]
END
GO

/****** Object:  StoredProcedure [dbo].[UpdateCustAddress]    Script Date: 11/19/2018 1:57:19 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:	<Author,Bhupesh Badwaik>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateCustAddress]
			@custId varchar(20),
			@addressType varchar(20),
			@address varchar(500),
			@resStatus varchar(500),
			@workAddress varchar(500),
			@WorkPhone VARCHAR(20)
			
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Update Home address.
    -- Insert statements for procedure here
	update CustAddress set cusaddr1=@address, resstatus=@resStatus where custid=@custId and addtype=@addressType

	Declare @Return int = 0,
		@tempDate datetime

	Set @tempDate = getdate()


	if exists (select * from custaddress where custid = @custId and addtype = 'W' )		
	--update Work Address 
		 update CustAddress set cusaddr1=@address where custid=@custId and addtype='W'
		 ELSE 
		 	--Insert work address
		Declare 
			@DELTitleC          VARCHAR(50),
			@DELFirstname		varchar(50),
			@DELLastname		varchar(50),
			@Title          VARCHAR(50),
			@FirstName		varchar(50),
			@LastName		varchar(50)


			Select @Title=title,@FirstName=firstname,@LastName=name from Customer where custid=@custId
		 EXEC CustAddressSave @custid = @custId, @addressType = 'W', @address1 = @workAddress, @address2 = '', 
						@address3 = '', @postcode = '', @DeliveryArea = '',@notes = '', @email = '',
						@datein = @tempDate, @user = '',@newRecord = 1, 
						@DELTitleC=@Title,
						@DELFirstname=@FirstName,
						@DELLastname=@LastName,
						@Latitude = NULL,
						@Longitude= NULL,
						@Zone = '', @return = @Return OUTPUT


		if exists (Select * from Custtel where custid = @custId and tellocn='W' )		
	--update Work Tel
		 update Custtel set telno = @workPhone where custid = @custId and tellocn='W'
		 ELSE 
		 	--Insert work tel
		 EXEC CustTelSave @custid = @custId, @tellocn = 'W', @dateteladd = @tempDate,@telno = @workPhone, @extnno = '', 
						@dialcode = '', @empeenochange = 213465, @newRecord = 1, @return = @Return OUTPUT


	

END



GO

