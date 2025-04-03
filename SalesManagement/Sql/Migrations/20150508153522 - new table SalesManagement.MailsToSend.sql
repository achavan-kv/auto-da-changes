-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
CREATE TABLE SalesManagement.MailsToSend
(
	id					int Identity(1, 1)	NOT NULL,
	CustomerId			varchar(20)			NOT NULL,
	TemplateId			SmallInt			NOT NULL,
	DateToSend			date				NOT NULL,
	MailSudject			VarChar(32)			NOT NULL,
	MailAdress			VarChar(128)		NOT NULL,
	OverrideUnsubscribe	Bit					NOT NULL				
)

ALTER TABLE SalesManagement.MailsToSend ADD CONSTRAINT
	PK_MailsToSend PRIMARY KEY CLUSTERED 
	(
		id
	) 
	WITH
	( 
		STATISTICS_NORECOMPUTE = OFF, 
		IGNORE_DUP_KEY = OFF, 
		ALLOW_ROW_LOCKS = ON, 
		ALLOW_PAGE_LOCKS = ON
	)

ALTER TABLE SalesManagement.MailsToSend ADD CONSTRAINT
	FK_MailsToSend_customer FOREIGN KEY
	(
		CustomerId
	) REFERENCES dbo.customer
	(
		custid
	) 
	 ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

ALTER TABLE SalesManagement.MailsToSend ADD CONSTRAINT
	FK_MailsToSend_TemplateId FOREIGN KEY
	(
		TemplateId
	) REFERENCES SalesManagement.MailchimpTemplateID
	(
		Id
	) 
	 ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 