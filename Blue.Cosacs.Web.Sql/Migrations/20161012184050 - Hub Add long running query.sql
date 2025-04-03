IF NOT EXISTS(
    SELECT 1
    FROM sys.columns 
    WHERE Name      = N'LongRunningTimeOut'
      AND Object_ID = Object_ID(N'Hub.Queue'))
BEGIN
	ALTER TABLE Hub.Queue
	ADD LongRunningTimeOut BIT NULL
END
GO

UPDATE Hub.Queue
Set LongRunningTimeout = 0

UPDATE Hub.Queue
Set LongRunningTimeout = 1
WHERE Id IN (200, 201)
