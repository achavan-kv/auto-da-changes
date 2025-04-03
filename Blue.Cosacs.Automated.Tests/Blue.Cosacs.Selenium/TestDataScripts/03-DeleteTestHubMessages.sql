SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'DeleteTestHubMessages'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE DeleteTestHubMessages
END
GO

CREATE PROCEDURE DeleteTestHubMessages

AS
BEGIN
    DELETE FROM HUB.Message WHERE IsRouted = 0
    DELETE FROM HUB.QueueMessage WHERE State IN ('P', 'I')

END

GO

EXEC DeleteTestHubMessages