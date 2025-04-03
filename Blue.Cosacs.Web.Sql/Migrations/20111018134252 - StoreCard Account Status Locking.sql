-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE StoreCardAccountStatus_Lookup SET Description= 'Offer Expired' WHERE Status='S'


IF EXISTS (SELECT * FROM sysindexes WHERE NAME= 'ix_status_acctno')
	DROP INDEX status.ix_status_acctno