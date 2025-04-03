ALTER TABLE Hub.Message 
ADD	[Index] varchar(50) NULL
GO

ALTER TABLE Hub.Message SET (LOCK_ESCALATION = TABLE)
GO

CREATE NONCLUSTERED INDEX IX_Hub_Message_Index
ON Hub.Message ([Index]); 
GO
