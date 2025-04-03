-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/

CREATE TABLE Communication.CustomerInteraction
	(
	CustomerId varchar(20) NOT NULL,
	LastEmailSentOn date NULL,
	LastSmsSentOn date NULL
	)  ON [PRIMARY]
GO
ALTER TABLE Communication.CustomerInteraction ADD CONSTRAINT
	PK_CustomerInteraction PRIMARY KEY CLUSTERED 
(
	CustomerId
) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE Communication.CustomerInteraction ADD CONSTRAINT
	FK_CustomerInteraction_customer FOREIGN KEY
	(
	CustomerId
	) REFERENCES dbo.customer
	(
	custid
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
