-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM sysindexes WHERE NAME LIKE 'ix_CollectionReason')
CREATE INDEX  ix_CollectionReason ON collectionReason(Acctno,Itemid,stocklocn,dateauthorised)