-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
CREATE TABLE Communication.BlackEmailList
(
	Email		VarChar(128) NOT NULL,
	Reason		VarChar(32) NULL,
	CreatedOn	SmallDateTime NOT NULL,
	Provider	VarChar(32) NOT NULL
)

ALTER TABLE Communication.BlackEmailList 
ADD  CONSTRAINT PK_BlackEmailList PRIMARY KEY CLUSTERED 
(
	Email ASC
)
WITH 
(
	PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON
) 

