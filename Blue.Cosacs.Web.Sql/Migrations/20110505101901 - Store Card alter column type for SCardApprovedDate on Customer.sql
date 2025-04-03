-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS(select * from information_schema.columns where table_name = 'Customer' and Column_Name = 'SCardApprovedDate')
BEGIN
	alter table customer alter column SCardApprovedDate smalldatetime null
END

--This parameter is a duplicate of parameter with the codename: StoreCardDefaultCardMonths, therefore I am deleting the below. Doesn't 
--seem to be referenced in the code.
If EXISTS(select * from countrymaintenance where codename = 'StoreCardExpMnths')
BEGIN
	delete from countrymaintenance where codename = 'StoreCardExpMnths'
END