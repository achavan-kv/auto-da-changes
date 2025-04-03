SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'DeleteTestTechBookings'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE DeleteTestTechBookings
END
GO

CREATE PROCEDURE DeleteTestTechBookings

AS
BEGIN

	DELETE FROM Service.TechnicianBooking WHERE UserId IN (SELECT Id FROM Admin.[User] WHERE FirstName like '%Selenium%')

END

GO

EXEC DeleteTestTechBookings