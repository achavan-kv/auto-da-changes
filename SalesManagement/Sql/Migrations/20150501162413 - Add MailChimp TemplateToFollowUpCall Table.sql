-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/

ALTER TABLE SalesManagement.FollowUpCall ADD
	MailchimpTemplateID smallint NULL

ALTER TABLE SalesManagement.FollowUpCall 
	ADD CONSTRAINT FK_FollowUpCall_MailchimpTemplateID FOREIGN KEY
	(
		MailchimpTemplateID
	) 
	REFERENCES SalesManagement.MailchimpTemplateID
	(
		Id
	) 
	 ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
