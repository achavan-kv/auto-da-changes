-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
CREATE NONCLUSTERED INDEX HubQueueMessage_State
ON [Hub].[QueueMessage] ([State])
INCLUDE ([Id],[QueueId],[CreatedOn],[DispatchedOn],[MessageId],[RunCount])
