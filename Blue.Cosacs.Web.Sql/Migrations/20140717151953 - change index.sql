-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF EXISTS(SELECT * FROM sys.indexes i where name = 'IX_HubMessage_IsRouted' AND i.object_id = OBJECT_ID('Hub.Message'))
	DROP INDEX IX_HubMessage_IsRouted ON hub.Message

CREATE NONCLUSTERED INDEX IX_HubMessage_IsRouted 
ON Hub.Message (IsRouted) 
INCLUDE (Id, Routing) 
