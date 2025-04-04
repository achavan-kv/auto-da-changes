if exists (select * from dbo.sysobjects where id = object_id('[dbo].[VE_CountryMaintenance]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[VE_CountryMaintenance]
END
GO

CREATE PROCEDURE [dbo].[VE_CountryMaintenance]
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
	ELSE
	BEGIN
		SELECT * FROM [Service].[CountryView] 

		SELECT * FROM [Financial].[CountryMaintenanceView]
		SET @Message = 'Country Maintenance details found'
		SET @Status = '200'
	END
END