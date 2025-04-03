-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS(select * from syscolumns where name = 'ReferralReasons' and object_name(id) = 'CashLoan')
BEGIN
	alter table CashLoan add ReferralReasons varchar(4000) null
END
GO