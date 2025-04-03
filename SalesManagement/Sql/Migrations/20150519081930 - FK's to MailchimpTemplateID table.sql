-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/

IF NOT EXISTS(SELECT 1 FROM sys.all_objects WHERE name = 'FK_FollowUpCall_MailchimpTemplateID')
BEGIN
	ALTER TABLE SalesManagement.FollowUpCall ADD CONSTRAINT
		FK_FollowUpCall_MailchimpTemplateID FOREIGN KEY
		(
			MailchimpTemplateID
		) REFERENCES Communication.MailchimpTemplateID
		(
			Id
		) 
		 ON UPDATE  NO ACTION 
		 ON DELETE  NO ACTION 


	ALTER TABLE SalesManagement.MailsToSend ADD CONSTRAINT
		FK_MailsToSend_MailchimpTemplateID FOREIGN KEY
		(
			TemplateId
		) REFERENCES Communication.MailchimpTemplateID
		(
			Id
		) 
		 ON UPDATE  NO ACTION 
		 ON DELETE  NO ACTION 


	ALTER TABLE SalesManagement.InactiveCustomersInteraction ADD CONSTRAINT
		FK_InactiveCustomersInteraction_MailchimpTemplateID FOREIGN KEY
		(
			MailchimpTemplateID
		) REFERENCES Communication.MailchimpTemplateID
		(
			Id
		) 
		 ON UPDATE  NO ACTION 
		 ON DELETE  NO ACTION 
END