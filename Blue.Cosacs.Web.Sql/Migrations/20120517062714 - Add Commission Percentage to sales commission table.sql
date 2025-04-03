-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from syscolumns where name = 'CommissionPcent' and object_name(id) = 'SalesCommission')
BEGIN
	alter TABLE SalesCommission add CommissionPcent MONEY not null default 0
	
End