-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/

ALTER TABLE SalesManagement.Call ALTER COLUMN AlternativeContactMeanId
	ADD SPARSE
GO
ALTER TABLE SalesManagement.Call ADD
	MailchimpTemplateID smallint SPARSE  NULL,
	EmailSubject varchar(32) SPARSE  NULL,
	SmsText varchar(160) SPARSE  NULL
GO


ALTER TABLE SalesManagement.Call  WITH CHECK ADD  CONSTRAINT FK_Call_MailchimpTemplateID FOREIGN KEY(MailchimpTemplateID)
REFERENCES Communication.MailchimpTemplateID (Id)
GO

ALTER TABLE SalesManagement.Call CHECK CONSTRAINT FK_Call_MailchimpTemplateID
GO

