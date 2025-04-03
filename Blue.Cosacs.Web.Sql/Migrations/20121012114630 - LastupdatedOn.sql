EXECUTE sp_rename N'Service.Request.LastUpdatedDate', N'Tmp_LastUpdatedOn', 'COLUMN' 
GO
EXECUTE sp_rename N'Service.Request.Tmp_LastUpdatedOn', N'LastUpdatedOn', 'COLUMN' 
GO
ALTER TABLE Service.Request SET (LOCK_ESCALATION = TABLE)