-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

if exists(select * from INFORMATION_SCHEMA.Columns
			WHERE  Table_Name = 'agreement'
           AND Column_Name = 'oldagrmtbal'
           AND DATA_TYPE = 'float'
           )
BEGIN
	alter table agreement alter column oldagrmtbal money
END
GO

if exists(select * from INFORMATION_SCHEMA.Columns
			WHERE  Table_Name = 'agreement'
           AND Column_Name = 'discount'
           AND DATA_TYPE = 'float'
           )
BEGIN
	alter table agreement alter column discount money
END
GO

if exists(select * from INFORMATION_SCHEMA.Columns
			WHERE  Table_Name = 'agreement'
           AND Column_Name = 'pxallowed'
           AND DATA_TYPE = 'float'
           )
BEGIN
	alter table agreement alter column pxallowed money
END
GO

if exists(select * from INFORMATION_SCHEMA.Columns
			WHERE  Table_Name = 'agreement'
           AND Column_Name = 'sdrychgtot'
           AND DATA_TYPE = 'float'
           )
BEGIN
	alter table agreement alter column sdrychgtot money
END
GO