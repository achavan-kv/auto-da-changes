-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('ProductHeirarchy') AND type in (N'U'))
BEGIN	
	CREATE TABLE ProductHeirarchy
	(
		RISource			CHAR(2)		default '' not null,
	    CatalogType			CHAR(2)		default '' not null,
	    PrimaryCode			VARCHAR(10)	default '' not null,
	    CodeDescription		VARCHAR(60)	,
	    ParentCode			VARCHAR(10)	default '' not null,
	    CodeStatus			CHAR(1)		default '' not null,
	    CONSTRAINT [PK_ProductHeirarchy] PRIMARY KEY CLUSTERED 
(
	RISource ASC,
	CatalogType ASC,
	PrimaryCode ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
	
			
END