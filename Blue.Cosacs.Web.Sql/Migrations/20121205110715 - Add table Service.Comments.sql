-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #11670

IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES  WHERE TABLE_SCHEMA = 'Service' AND TABLE_NAME = 'Comments')
BEGIN
	CREATE TABLE [Service].[Comments]
	(
		Id int IDENTITY (1, 1),
		[RequestId] int NOT NULL,
		[Date] datetime NOT NULL,
		[AddedBy] varchar(50),
		[Comment] varchar(4000),
	 CONSTRAINT [PK_ServiceComment] PRIMARY KEY CLUSTERED 
	(
	[RequestId] ASC,
	[Date] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
	
END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Columns
           WHERE  Table_Name = 'Request'
           AND    Column_Name = 'Comments'
           AND TABLE_SCHEMA = 'Service')
BEGIN

	ALTER TABLE Service.Request DROP COLUMN Comments
	
END