-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 

IF EXISTS(SELECT * FROM sys.columns 
          WHERE [name] = N'Level_1' AND [object_id] = OBJECT_ID(N'Sales.OrderItem'))
BEGIN

	EXECUTE sp_rename N'Sales.OrderItem.Level_1', N'Tmp_Level_1', 'COLUMN' 

	EXECUTE sp_rename N'Sales.OrderItem.Tmp_Level_1', N'Department', 'COLUMN' 

END
GO

IF EXISTS(SELECT * FROM sys.columns 
          WHERE [name] = N'Level_2' AND [object_id] = OBJECT_ID(N'Sales.OrderItem'))		  
BEGIN

	EXECUTE sp_rename N'Sales.OrderItem.Level_2', N'Tmp_Level_2', 'COLUMN'

	EXECUTE sp_rename N'Sales.OrderItem.Tmp_Level_2', N'Category', 'COLUMN'

END
GO

IF EXISTS(SELECT * FROM sys.columns 
		   WHERE [name] = N'Level_3' AND [object_id] = OBJECT_ID(N'Sales.OrderItem'))

BEGIN

	EXECUTE sp_rename N'Sales.OrderItem.Level_3', N'Tmp_Level_3', 'COLUMN'

	EXECUTE sp_rename N'Sales.OrderItem.Tmp_Level_3', N'Class', 'COLUMN'

END

GO