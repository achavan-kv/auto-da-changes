IF EXISTS (SELECT * FROM sysobjects 
		   WHERE NAME = 'CountryMaintenanceGetValue'
		   AND xtype = 'P')
BEGIN
DROP PROCEDURE CountryMaintenanceGetValue
END
GO

CREATE PROCEDURE CountryMaintenanceGetValue
@codename VARCHAR(30)
AS 
BEGIN

	SELECT VALUE
	FROM CountryMaintenance
	WHERE codename = @codename
END
GO