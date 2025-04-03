-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
DROP TABLE Communication.SmsUnsubcription
GO

CREATE TABLE Communication.SmsUnsubcription(
	Id Int Identity (1,1) NOT NULL,
	CustomerId varchar(20) NOT NULL,
	PhoneNumber varchar(26) NULL,
	CreatedOn smalldatetime NULL,
	CreatedBy int NULL,
 CONSTRAINT PK_SmsUnsubcription PRIMARY KEY CLUSTERED 
(
	Id ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
) 

