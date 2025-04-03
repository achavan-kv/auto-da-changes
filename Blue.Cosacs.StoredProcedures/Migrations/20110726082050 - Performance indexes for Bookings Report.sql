-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
if not exists (SELECT * FROM sysindexes WHERE NAME = 'ix_agreementAudit_datechange')
create index ix_agreementAudit_datechange on agreementaudit(datechange,acctno)

if not exists (SELECT * FROM sysindexes WHERE NAME = 'ix_acct_dateacctopen')
create index ix_acct_dateacctopen on acct(dateacctopen)

