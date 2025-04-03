
IF EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'ParentItemId' AND [object_id] = OBJECT_ID(N'Sales.OrderItem')) BEGIN
	EXECUTE sp_rename N'Sales.OrderItem.ParentItemId', N'Tmp_ParentId', 'COLUMN'
 
	EXECUTE sp_rename N'Sales.OrderItem.Tmp_ParentId', N'ParentId', 'COLUMN' 


END
GO
