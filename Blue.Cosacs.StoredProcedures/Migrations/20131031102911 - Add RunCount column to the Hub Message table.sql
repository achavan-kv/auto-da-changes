-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS(SELECT * FROM sys.columns
              WHERE Name = N'RunCount' AND
                    Object_ID = Object_ID(N'Hub.Message'))
BEGIN
    -- Create the new column
    ALTER TABLE [Hub].[Message]
    ADD [RunCount] INT 
    CONSTRAINT CK_Hub_Message_RunCount_Default DEFAULT 0 NULL;

END

GO

UPDATE [Hub].[Message]
SET [RunCount] = 0
WHERE [RunCount] IS NULL

GO 

ALTER TABLE [Hub].[Message]
ALTER COLUMN [RunCount] INT NOT NULL

GO
