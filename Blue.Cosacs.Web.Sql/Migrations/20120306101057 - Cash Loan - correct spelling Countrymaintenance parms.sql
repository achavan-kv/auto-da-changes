-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


UPDATE CountryMaintenance
set [Description]=REPLACE([Description],'customer?s','customer''s')
where CodeName in('CL_ReferralRescored','CL_ReferralResidence','CL_ReferralEmployment','CL_ReferralPercentage')

UPDATE CountryMaintenance
set [Description]=REPLACE([Description],'customers','customer''s')
where CodeName ='CL_CashLoanLetterPrevSettMths'

