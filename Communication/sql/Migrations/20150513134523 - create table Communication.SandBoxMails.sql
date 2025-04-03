-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
CREATE TABLE Communication.SandBoxMails
(
	Id			Int Identity(1, 1) NOT NULL,
	CreatedOn	SmallDateTime NOT NULL,
	MailMessage	VarChar(MAX) NOT NULL
)

ALTER TABLE Communication.SandBoxMails ADD  CONSTRAINT PK_SandBoxMails PRIMARY KEY CLUSTERED 
(
	Id ASC
)
WITH 
(
	PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON
) ON [PRIMARY]

