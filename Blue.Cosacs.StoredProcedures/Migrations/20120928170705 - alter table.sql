EXECUTE sp_rename N'Service.Request.Quantity', N'Tmp_ItemQuantity', 'COLUMN' 
GO
EXECUTE sp_rename N'Service.Request.Tmp_ItemQuantity', N'ItemQuantity', 'COLUMN' 
GO
ALTER TABLE Service.Request ADD
	TransitNotes varchar(4000) NULL
GO
ALTER TABLE Service.Request SET (LOCK_ESCALATION = TABLE)
GO