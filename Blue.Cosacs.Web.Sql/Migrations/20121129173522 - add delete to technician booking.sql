IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Service].[FK_TechnicianBooking_Request]') AND parent_object_id = OBJECT_ID(N'[Service].[TechnicianBooking]'))
ALTER TABLE [Service].[TechnicianBooking] DROP CONSTRAINT [FK_TechnicianBooking_Request]
GO


ALTER TABLE [Service].[TechnicianBooking]  WITH CHECK ADD  CONSTRAINT [FK_TechnicianBooking_Request] FOREIGN KEY([RequestId])
REFERENCES [Service].[Request] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [Service].[TechnicianBooking] CHECK CONSTRAINT [FK_TechnicianBooking_Request]
GO


