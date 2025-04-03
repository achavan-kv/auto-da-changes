-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

CREATE TABLE Config.Setting
	(
	Id varchar(100) NOT NULL,
	ValueBit bit NULL,
	ValueInt int NULL,
	ValueDateTime smalldatetime NULL,
	ValueDecimal decimal(38, 19) NULL,
	ValueString nvarchar(256) NULL,
	ValueText ntext NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE Config.Setting ADD CONSTRAINT
	PK_Setting PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO