IF EXISTS (SELECT * FROM sysobjects
           WHERE xtype = 'p'
           AND NAME = 'BCPpathGet')
BEGIN 
DROP PROCEDURE BCPpathGet
END
GO


CREATE PROCEDURE BCPpathGet
--@return int OUTPUT
AS
BEGIN
	 
	SELECT value FROM CountryMaintenance
	WHERE CodeName = 'BCPpath'
	
	 
END
GO