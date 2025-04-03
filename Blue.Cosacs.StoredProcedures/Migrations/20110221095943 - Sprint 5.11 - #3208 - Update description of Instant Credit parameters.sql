-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
--Update existing Instant Credit Country Parameter descriptions to only refer to Instant Credit and not Cash Loan
--as Cash Loans now have their own section for Country Parameters.

if exists(select * from countrymaintenance where codename = 'IC_EmployChgManApprove')
update countrymaintenance set description = 'If checked will mean that any changes to employment details will prevent instant credit approval'
where codename = 'IC_EmployChgManApprove'

if exists(select * from countrymaintenance where codename = 'IC_ResidenceChgManApprove')
update countrymaintenance set description = 'If checked will mean that any changes to address details will prevent instant credit approval'
where codename = 'IC_ResidenceChgManApprove'

if exists(select * from countrymaintenance where codename = 'IC_ExistAccountLength')
update countrymaintenance set description = 'On an existing account, the minimum time period since delivery for the customer to qualify for Instant Credit.'
where codename = 'IC_ExistAccountLength'

if exists(select * from countrymaintenance where codename = 'IC_HighStatusTimeFrame')
update countrymaintenance set description = 'The maximum status of any current or settled account where settled within the Highest status of any account time frame. A status higher than this will prevent instant credit approval.'
where codename = 'IC_HighStatusTimeFrame'

if exists(select * from countrymaintenance where codename = 'IC_MaxArrearsLevel')
update countrymaintenance set description = 'Maximum number of instalments in arrears. Any account more than this in arrears will prevent instant credit approval'
where codename = 'IC_MaxArrearsLevel'

if exists(select * from countrymaintenance where codename = 'IC_SettledCashMonths')
update countrymaintenance set description = 'If a customer has no current accounts, this is the maximum number of months since the most recent Cash account has been settled for the customer to qualify for Instant Credit.'
where codename = 'IC_SettledCashMonths'

if exists(select * from countrymaintenance where codename = 'IC_SettledCredMonths')
update countrymaintenance set description = 'If a customer has no current accounts, this is the maximum number of months since the most recent Credit account has been settled for the customer to qualify for Instant Credit.'
where codename = 'IC_SettledCredMonths'
