-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
CREATE TABLE Communication.SmsUnsubcription
(
	CustomerId	VarChar(20),
	PhoneNumber	VarChar(26),
	CreatedOn	SmallDateTime,
	CreatedBy	Int,

	CONSTRAINT PK_SmsUnsubcription PRIMARY KEY CLUSTERED 
	(
		[CustomerId] ASC
	)
	WITH 
	(
		PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON
	) 
)

