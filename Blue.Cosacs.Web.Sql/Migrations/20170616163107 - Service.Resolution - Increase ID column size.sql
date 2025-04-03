-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.

ALTER TABLE  Service.Resolution 
DROP CONSTRAINT PK_ServiceResolution;   

GO

alter table Service.Resolution
alter column ID INT

GO

ALTER TABLE [Service].[Resolution] ADD CONSTRAINT [PK_ServiceResolution] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)