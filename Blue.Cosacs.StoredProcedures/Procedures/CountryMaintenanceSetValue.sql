IF EXISTS (SELECT * FROM sysobjects 
		   WHERE NAME = 'CountryMaintenanceSetValue'
		   AND xtype = 'P')
BEGIN
	DROP PROCEDURE CountryMaintenanceSetValue
END
GO

CREATE PROCEDURE CountryMaintenanceSetValue
@codename VARCHAR(30),
@value varchar(1500)
AS
BEGIN
	UPDATE CountryMaintenance
	SET Value = @value
	WHERE codename = @codename
END
GO
