
/****** Object:  StoredProcedure [dbo].[GetCountryMaintenance]    Script Date: 11/19/2018 1:49:57 PM ******/
IF EXISTS (SELECT * FROM sysobjects 
   WHERE NAME = 'GetCountryMaintenance'
   )
BEGIN
DROP PROCEDURE [dbo].[GetCountryMaintenance]
END
GO

/****** Object:  StoredProcedure [dbo].[GetCountryMaintenance]    Script Date: 11/19/2018 1:49:57 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
/*
	DECLARE @Message varchar(MAX)
	DECLARE @Status varchar(5)
	EXEC GetCountryMaintenance @CustId = '98655334SKrHV', @Message = @Message OUT, @Status = @Status OUT
	SELECT @Message Message, @Status Status
*/
CREATE PROCEDURE [dbo].[GetCountryMaintenance]
	@CustId VARCHAR(20),
	@Message varchar(MAX) output,
	@Status varchar(5) output
AS
BEGIN
	SET @Message = ''
	SET @Status = ''
	IF NOT EXISTS (select 1 from Customer where Custid=@CustId)
	BEGIN
		SET @Message = 'User not found'
		SET @Status = '404'
		RETURN		
	END
	--ELSE IF NOT EXISTS (select 1 from custacct where custid=@CustId)
	--BEGIN
	--	SET @Message = 'No accounts for user'
	--	SET @Status = '404'
	--	RETURN		
	--END
	--ELSE IF EXISTS (SELECT 1 FROM customer cust INNER JOIN custacct ca ON ca.CustId = cust.CustId WHERE RFCreditLimit = 0.00 AND cust.CustId=@CustId)
	--BEGIN
	--	SET @Message = 'No transactions for user'
	--	RETURN		
	--END
	ELSE
	BEGIN
		--SELECT * FROM [Service].[CountryView] 
		--SELECT origbr = 816, countrycode FROM [Service].[CountryView] 

		SELECT branchno as origbr,countrycode from branch where branchname='EMMA' and StoreType in('C','N')

		SELECT * FROM [Financial].[CountryMaintenanceView]

		SELECT * FROM mailtemplate
		SET @Message = 'Country Maintenance details found'
		SET @Status = '200'
	END
END
GO

