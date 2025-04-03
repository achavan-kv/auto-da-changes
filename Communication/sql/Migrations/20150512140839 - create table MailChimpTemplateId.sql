-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
CREATE SCHEMA Communication 
GO

CREATE TABLE Communication.MailchimpTemplateID
(
	Id smallint		IDENTITY(1,1) NOT NULL,
	Name			varchar(32) NOT NULL,
	TemplateId		varchar(32) NOT NULL,
	CreatedOn		smalldatetime NOT NULL,
	CreatedBy		int NOT NULL,

	CONSTRAINT PK_MailchimpTemplateID PRIMARY KEY CLUSTERED 
	(
		Id ASC
	)
	WITH 
	(
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON
	)
) 