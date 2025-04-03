-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

SET IDENTITY_INSERT SalesManagement.InactiveCustomersInteraction ON 
INSERT INTO SalesManagement.InactiveCustomersInteraction
(
	Id, MailchimpTemplateID, SmsText, ContactMeansId, AlternativeContactMeanId, ContactEmailSubject, FlushedEmailSubject
)
VALUES 
	(2, NULL, NULL, 1, NULL, NULL, NULL)

SET IDENTITY_INSERT SalesManagement.InactiveCustomersInteraction OFF