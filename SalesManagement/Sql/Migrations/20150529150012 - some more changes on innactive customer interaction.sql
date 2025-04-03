-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE SalesManagement.InactiveCustomersInteraction
	DROP CONSTRAINT FK_InactiveCustomersInteraction_AlternativeContactMeanId


ALTER TABLE SalesManagement.InactiveCustomersInteraction  WITH CHECK ADD  CONSTRAINT FK_InactiveCustomersInteraction_AlternativeContactMeanId FOREIGN KEY(AlternativeContactMeanId)
REFERENCES SalesManagement.ContactMean (Id)
GO
