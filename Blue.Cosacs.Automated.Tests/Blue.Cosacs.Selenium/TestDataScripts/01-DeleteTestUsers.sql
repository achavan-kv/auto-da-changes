SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'DeleteTestUsers'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE DeleteTestUsers
END
GO

CREATE PROCEDURE DeleteTestUsers

AS
BEGIN

    UPDATE Warehouse.Booking SET PickingAssignedBy = (SELECT id FROM Admin.[User] WHERE FirstName = 'Selenium' AND LastName = 'Tester1') WHERE PickingAssignedBy IN (SELECT id FROM Admin.[User] WHERE FirstName = 'SelPerm')
    UPDATE Warehouse.Picking SET Createdby = (SELECT id FROM Admin.[User] WHERE FirstName = 'Selenium' AND LastName = 'Tester1') WHERE Createdby IN (SELECT id FROM Admin.[User] WHERE FirstName = 'SelPerm')
    DELETE FROM CourtsPersonTable WHERE UserId IN (SELECT id FROM Admin.[User] WHERE FirstName = 'SelPerm')
    DELETE FROM Admin.[UserRole] WHERE UserId IN (SELECT id FROM Admin.[User] WHERE FirstName = 'SelPerm')
    DELETE FROM Admin.[User] WHERE FirstName = 'SelPerm'

END

GO

EXEC DeleteTestUsers
