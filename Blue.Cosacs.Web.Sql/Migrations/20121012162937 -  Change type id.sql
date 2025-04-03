EXECUTE sp_rename N'Service.RequestContact.Type', N'Tmp_TypeId_1', 'COLUMN' 
GO
EXECUTE sp_rename N'Service.RequestContact.Tmp_TypeId_1', N'TypeId', 'COLUMN' 
GO
ALTER TABLE Service.RequestContact SET (LOCK_ESCALATION = TABLE)
GO