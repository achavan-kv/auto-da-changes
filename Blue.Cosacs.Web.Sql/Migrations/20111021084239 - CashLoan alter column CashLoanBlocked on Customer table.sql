-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


DECLARE @cmd NVARCHAR(1000)

IF EXISTS(select * from sysobjects where name like 'DF__Customer__CashLo%' and type = 'D')
BEGIN

	declare @constraintName varchar(50)
	set @constraintName = (select name from sysobjects where name like 'DF__Customer__CashLo%' and type = 'D')
	
	SET @cmd = 'ALTER TABLE customer DROP CONSTRAINT ' + @constraintName

	EXEC sp_executesql @cmd
END
GO

IF EXISTS(select * from syscolumns where name = 'CashLoanBlocked' and object_name(id) = 'Customer')
BEGIN
	ALTER TABLE Customer DROP COLUMN CashLoanBlocked
END	
GO

IF NOT EXISTS(select * from syscolumns where name = 'CashLoanBlocked' and object_name(id) = 'Customer')
BEGIN
	ALTER TABLE Customer ADD CashLoanBlocked CHAR(1) DEFAULT ' '
END 
GO