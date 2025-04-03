-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
CREATE TABLE SalesManagement.InactiveCustomersInteraction
(
	Id									tinyint IDENTITY(1,1),
	MailchimpTemplateID					smallint NULL,
	SendAlternativeContactOnFlushCall	tinyint NULL,

	CONSTRAINT PK_InactiveCustomersInteraction PRIMARY KEY CLUSTERED 
	(
		Id ASC
	)
	WITH 
	(
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON
	)
)

INSERT INTO SalesManagement.InactiveCustomersInteraction
	(MailchimpTemplateID, SendAlternativeContactOnFlushCall)
VALUES
	(NULL, NULL)

