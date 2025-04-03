-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE SalesManagement.InactiveCustomersInteraction
	ADD SmsText VarChar(160) NULL
GO
	
ALTER TABLE SalesManagement.InactiveCustomersInteraction
	ADD ContactMeansId TinyInt NULL
GO
	
UPDATE SalesManagement.InactiveCustomersInteraction
SET ContactMeansId = 1 
GO

ALTER TABLE SalesManagement.InactiveCustomersInteraction
	ALTER COLUMN ContactMeansId TinyInt NOT NULL
GO

ALTER TABLE SalesManagement.InactiveCustomersInteraction
	ADD AlternativeContactMeanId TinyInt NULL
GO

ALTER TABLE SalesManagement.InactiveCustomersInteraction  WITH CHECK ADD  CONSTRAINT FK_InactiveCustomersInteraction_ContactMeansId FOREIGN KEY(ContactMeansId)
REFERENCES SalesManagement.ContactMean (Id)
GO

ALTER TABLE SalesManagement.InactiveCustomersInteraction CHECK CONSTRAINT FK_InactiveCustomersInteraction_ContactMeansId
GO

ALTER TABLE SalesManagement.InactiveCustomersInteraction  WITH CHECK ADD  CONSTRAINT FK_InactiveCustomersInteraction_AlternativeContactMeanId FOREIGN KEY(AlternativeContactMeanId)
REFERENCES SalesManagement.InactiveCustomersInteraction (Id)
GO

ALTER TABLE SalesManagement.InactiveCustomersInteraction CHECK CONSTRAINT FK_InactiveCustomersInteraction_AlternativeContactMeanId
GO