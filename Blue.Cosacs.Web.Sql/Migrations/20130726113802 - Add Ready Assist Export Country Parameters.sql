-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #13719 - CR12949

IF NOT EXISTS(select * from countrymaintenance where CodeName = 'RdyAstAcctId')
BEGIN
	insert into CountryMaintenance
	Select c.CountryCode, 13, 'Ready Assist Extract Account Id', 'UNI', 'text', 0, '', '', 'This is the Account Id to be shown in the Ready Assist Extract', 'RdyAstAcctId'
	from country c
END

IF NOT EXISTS(select * from countrymaintenance where CodeName = 'RdyAstPlanId')
BEGIN
	insert into CountryMaintenance
	Select c.CountryCode, 13, 'Ready Assist Extract Plan Id', 'CRA', 'text', 0, '', '', 'This is the Plan Id to be shown in the Ready Assist Extract', 'RdyAstPlanId'
	from country c
END