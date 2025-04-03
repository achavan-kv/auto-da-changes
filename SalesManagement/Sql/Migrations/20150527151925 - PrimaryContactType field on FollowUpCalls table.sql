-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
CREATE TABLE SalesManagement.ContactMean
(
	Id	TinyInt NOT NULL,
	Name VarChar(32) NOT NULL,
)

ALTER TABLE SalesManagement.ContactMean ADD CONSTRAINT PK_ContactMean PRIMARY KEY CLUSTERED 
(
	Id ASC
)
GO

INSERT INTO SalesManagement.ContactMean
	(Id, Name)
VALUES
	(1, 'Phone Call'),
	(2, 'Email'),
	(3, 'Sms')

ALTER TABLE SalesManagement.FollowUpCall
	ADD ContactMeansId TinyInt NULL
GO

UPDATE SalesManagement.FollowUpCall
SET ContactMeansId = 1 
GO

ALTER TABLE SalesManagement.FollowUpCall
	ALTER COLUMN ContactMeansId TinyInt NOT NULL
GO

ALTER TABLE SalesManagement.FollowUpCall  WITH CHECK ADD  CONSTRAINT FK_FollowUpCall_ContactMeansId FOREIGN KEY(ContactMeansId)
REFERENCES SalesManagement.ContactMean (Id)
GO

ALTER TABLE SalesManagement.FollowUpCall CHECK CONSTRAINT FK_FollowUpCall_ContactMeansId
GO

EXEC sp_rename 'SalesManagement.FollowUpCall.SendAlternativeContactOnFlushCall', 'AlternativeContactMeanId', 'COLUMN'
GO

ALTER TABLE SalesManagement.FollowUpCall  WITH CHECK ADD  CONSTRAINT FK_FollowUpCall_AlternativeContactMeanId FOREIGN KEY(AlternativeContactMeanId)
REFERENCES SalesManagement.ContactMean (Id)
GO

ALTER TABLE SalesManagement.FollowUpCall CHECK CONSTRAINT FK_FollowUpCall_AlternativeContactMeanId
GO

ALTER TABLE SalesManagement.FollowUpCall CHECK CONSTRAINT FK_FollowUpCall_ContactMeansId
GO

EXEC sp_rename 'SalesManagement.Call.SendAlternativeContactOnFlushCall', 'AlternativeContactMeanId', 'COLUMN'
GO

ALTER TABLE SalesManagement.Call  WITH CHECK ADD  CONSTRAINT FK_Call_AlternativeContactMeanId FOREIGN KEY(AlternativeContactMeanId)
REFERENCES SalesManagement.ContactMean (Id)
GO

ALTER TABLE SalesManagement.Call CHECK CONSTRAINT FK_Call_AlternativeContactMeanId
GO
