-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS(SELECT * FROM sys.indexes i where name = 'IX_HubMessage_IsRouted' AND i.object_id = OBJECT_ID('Hub.Message'))
BEGIN 
	SET ARITHABORT ON;

	CREATE NONCLUSTERED INDEX IX_HubMessage_IsRouted 
	ON Hub.Message (IsRouted) 
	INCLUDE (Id, Routing) 
	WHERE IsRouted = CONVERT(BIT, 0)
END