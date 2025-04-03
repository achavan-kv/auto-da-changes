-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('StockInfoAssociated') AND type in (N'U'))
BEGIN	
	CREATE TABLE StockInfoAssociated
	(
		ProductGroup	VARCHAR(3)	default '' not null,
	    Category		INT			default 0 not null,
	    Class			VARCHAR(3)	default '' not null,
	    SubClass		VARCHAR(5)	default '' not null,
	    AssocItemId		INT			default 0 not null,
	    CONSTRAINT [PK_StockInfoAssociated] PRIMARY KEY CLUSTERED 
(
	ProductGroup ASC,
	Category ASC,
	Class ASC,
	SubClass ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
	
			
END

