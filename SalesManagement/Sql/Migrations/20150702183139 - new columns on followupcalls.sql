-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE SalesManagement.FollowUpCall
	ADD ContactEmailSubject VarChar(32) NULL

ALTER TABLE SalesManagement.FollowUpCall
	ADD FlushedEmailSubject VarChar(32) NULL