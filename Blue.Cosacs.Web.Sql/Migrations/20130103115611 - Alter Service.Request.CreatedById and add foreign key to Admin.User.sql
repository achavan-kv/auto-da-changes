-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #11881

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'Request' AND  Column_Name = 'CreatedById'
           AND TABLE_SCHEMA = 'Service')
BEGIN
	ALTER TABLE Service.Request ALTER COLUMN CreatedById INT NOT NULL
END


IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Service].[FK_Request_CreatedById]') AND parent_object_id = OBJECT_ID(N'[Service].[Request]'))
BEGIN
	ALTER TABLE [Service].[Request]  WITH CHECK ADD  CONSTRAINT [FK_Request_CreatedById] FOREIGN KEY([CreatedById])
	REFERENCES [Admin].[User] ([Id])

	ALTER TABLE [Service].[Request] CHECK CONSTRAINT [FK_Request_CreatedById]

END
