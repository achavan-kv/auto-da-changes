-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF  EXISTS (SELECT * FROM sys.check_constraints WHERE object_id = OBJECT_ID(N'[Warranty].[CK_Warranty_TypeCode]') AND parent_object_id = OBJECT_ID(N'[Warranty].[Warranty]'))
	ALTER TABLE [Warranty].[Warranty] DROP CONSTRAINT [CK_Warranty_TypeCode]
GO

IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[Warranty].[TypeCodeToUpperTrig]'))
	DROP TRIGGER [Warranty].[TypeCodeToUpperTrig]
GO

IF NOT EXISTS(SELECT * FROM sys.columns 
        WHERE [name] = N'TypeCode' AND [object_id] = OBJECT_ID(N'[Warranty].[Warranty]')) BEGIN
	ALTER TABLE [Warranty].[Warranty] ADD TypeCode char(1) NOT NULL CONSTRAINT DF_Warranty_TypeCode DEFAULT 'E'
END
GO

ALTER TABLE [Warranty].[Warranty] ADD CONSTRAINT
	CK_Warranty_TypeCode CHECK (NOT [TypeCode] LIKE '%[^EFI]%')
GO

CREATE TRIGGER [Warranty].TypeCodeToUpperTrig
   ON  [Warranty].[Warranty]
   AFTER INSERT,UPDATE
AS BEGIN
	UPDATE [Warranty].[Warranty] 
	SET TypeCode = UPPER(TypeCode)
	WHERE Id IN (SELECT Id FROM inserted) 

END
GO

IF EXISTS(SELECT * FROM sys.columns 
        WHERE [name] = N'Free' AND [object_id] = OBJECT_ID(N'[Warranty].[Warranty]')) BEGIN
	UPDATE [Warranty].[Warranty] 
	SET TypeCode = (CASE WHEN [Free] = 1 THEN 'F' ELSE 'E' END)
	
	ALTER TABLE [Warranty].[Warranty] DROP COLUMN Free
END
GO