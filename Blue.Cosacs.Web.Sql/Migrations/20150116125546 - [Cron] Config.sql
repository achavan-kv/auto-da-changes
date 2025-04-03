-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
CREATE TABLE Cron.Config
	(
	UserName varchar(50) NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE Cron.Config ADD CONSTRAINT
	PK_Config PRIMARY KEY CLUSTERED 
	(
	UserName
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
insert into Cron.Config (UserName) values ('99999')
go