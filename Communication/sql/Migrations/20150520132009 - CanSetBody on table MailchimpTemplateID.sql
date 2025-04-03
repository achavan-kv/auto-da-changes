-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE Communication.MailchimpTemplateID
	ADD CanSetBody Bit NULL
GO

UPDATE Communication.MailchimpTemplateID
SET CanSetBody =  0
GO

ALTER TABLE Communication.MailchimpTemplateID
	ALTER COLUMN CanSetBody Bit NOT NULL