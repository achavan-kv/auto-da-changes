SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'DeleteTestDriversAndTrucks'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE DeleteTestDriversAndTrucks
END
GO

CREATE PROCEDURE DeleteTestDriversAndTrucks

AS
BEGIN

    DELETE FROM Warehouse.Driver WHERE NAME LIKE 'SelDriver%'
    DELETE FROM Warehouse.Driver WHERE Name = 'NewSeleniumDriver22'
    DELETE FROM Warehouse.Truck WHERE Name IN ('SeleniumTruck17', 'SeleniumTruck18')

END

GO

EXEC DeleteTestDriversAndTrucks