IF NOT EXISTS ( SELECT 1 FROM dbo.sysindexes
			WHERE OBJECT_ID('[lineitem]') = id AND name = 'IX_LineItemForInstallation')
	CREATE NONCLUSTERED INDEX IX_LineItemForInstallation
	ON [dbo].[lineitem] ([itemno],[quantity])
	INCLUDE ([acctno],[parentitemno],[parentlocation])
GO

IF NOT EXISTS ( SELECT 1 FROM dbo.sysindexes
			WHERE OBJECT_ID('[SR_TechnicianDiary]') = id AND name = 'IX_SR_TechnicianDiary_ForInstallation')			
	CREATE NONCLUSTERED INDEX IX_SR_TechnicianDiary_ForInstallation
	ON [dbo].[SR_TechnicianDiary] ([BookingType])
	INCLUDE ([TechnicianId],[SlotDate],[SlotNo],[InstallationNo])
GO