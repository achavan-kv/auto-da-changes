-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
DROP TABLE SalesManagement.ChainCall
CREATE TABLE SalesManagement.FollowUpCall
(
	Id				smallint NOT NULL IDENTITY (1, 1),
	TimePeriod		tinyint NOT NULL,
	Quantity		smallint NOT NULL,
	ReasonToCall	varchar(32) NOT NULL
)  ON [PRIMARY]
GO

DECLARE @v sql_variant 
SET @v = N'1-> Days | 2-> Weeks | 3-> Months'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'SalesManagement', N'TABLE', N'FollowUpCall', N'COLUMN', N'TimePeriod'
GO

ALTER TABLE SalesManagement.FollowUpCall ADD CONSTRAINT
	PK_FollowUpCall PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
