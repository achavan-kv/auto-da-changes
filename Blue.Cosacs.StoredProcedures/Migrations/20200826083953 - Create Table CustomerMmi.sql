-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF  NOT EXISTS (SELECT	1 
				FROM	sys.objects WITH (NOLOCK)
				WHERE	object_id = OBJECT_ID(N'[CustomerMmi]') 
						AND type in (N'U')
				)
BEGIN

	CREATE TABLE dbo.CustomerMmi
	(
		CustId VARCHAR(20) NOT NULL,
		MmiLimit MONEY NOT NULL
	)  ON [PRIMARY]
	ALTER TABLE dbo.CustomerMmi ADD CONSTRAINT
		DF_CustomerMmi_MmiLimit DEFAULT 0 FOR MmiLimit
	ALTER TABLE dbo.CustomerMmi ADD CONSTRAINT
	PK_CustomerMmi PRIMARY KEY CLUSTERED 
	(
		CustId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

END