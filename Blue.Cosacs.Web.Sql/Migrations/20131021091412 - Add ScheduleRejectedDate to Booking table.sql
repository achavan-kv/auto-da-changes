-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #15512

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'Booking' AND  Column_Name = 'ScheduleRejectedDate'
           AND TABLE_SCHEMA = 'Warehouse')
BEGIN
	ALTER TABLE Warehouse.Booking ADD ScheduleRejectedDate date NULL
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'Booking' AND  Column_Name = 'ScheduleRejectedBy'
           AND TABLE_SCHEMA = 'Warehouse')
BEGIN
	ALTER TABLE Warehouse.Booking ADD ScheduleRejectedBy int NULL

	ALTER TABLE [Warehouse].[Booking]  WITH CHECK ADD  CONSTRAINT [FK_Schedule_Rejected_By] FOREIGN KEY([ScheduleRejectedBy])
	REFERENCES [Admin].[User] ([Id])

	ALTER TABLE [Warehouse].[Booking] CHECK CONSTRAINT [FK_Schedule_Rejected_By]


END
GO
