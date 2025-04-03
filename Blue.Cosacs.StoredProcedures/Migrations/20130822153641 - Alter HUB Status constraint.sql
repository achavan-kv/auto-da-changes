-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE [Hub].[Message]  drop  CONSTRAINT [HubMessageState]
        
ALTER TABLE [Hub].[Message]  WITH CHECK ADD  CONSTRAINT [HubMessageState] CHECK  (([State]='P' OR [State]='S' OR [State]='R' OR [State]='I' or [State] = 'X'))

ALTER TABLE [Hub].[Message] CHECK CONSTRAINT [HubMessageState]