-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

INSERT INTO dbo.codecat (
	origbr,	category,	catdescript,
	codelgth,	forcenum,	forcenumdesc,
	usermaint,	CodeHeaderText,	DescriptionHeaderText,
	SortOrderHeaderText,	ReferenceHeaderText,	AdditionalHeaderText,
	ToolTipText
) VALUES ( 
	/* origbr - smallint */ 0,
	/* category - varchar(12) */ 'STQ',
	/* catdescript - nvarchar(64) */ N'Store Card Security Question',
	/* codelgth - int */ 4,
	/* forcenum - char(1) */ 'N',
	/* forcenumdesc - char(1) */ 'N',
	/* usermaint - char(1) */ 'Y',
	/* CodeHeaderText - varchar(30) */ '',
	/* DescriptionHeaderText - varchar(30) */ 'Security Question',
	/* SortOrderHeaderText - varchar(30) */ '',
	/* ReferenceHeaderText - varchar(30) */ '',
	/* AdditionalHeaderText - varchar(30) */ '',
	/* ToolTipText - varchar(300) */ 'This will appear in the Store Card Maintenance Screen' ) 
	
	
	INSERT INTO dbo.code (
		origbr,
		category,
		code,
		codedescript,
		statusflag,
		sortorder,
		reference,
		additional
	) VALUES ( 
		/* origbr - smallint */ 0,
		/* category - varchar(12) */ 'STQ',
		/* code - varchar(12) */ 'MFM',
		/* codedescript - nvarchar(64) */ N'Mothers First Name',
		/* statusflag - char(1) */ 'L',
		/* sortorder - smallint */ 0,
		/* reference - varchar(12) */ '',
		/* additional - varchar(15) */ '' ) 
		
			
	INSERT INTO dbo.code (
		origbr,
		category,
		code,
		codedescript,
		statusflag,
		sortorder,
		reference,
		additional
	) VALUES ( 
		/* origbr - smallint */ 0,
		/* category - varchar(12) */ 'STQ',
		/* code - varchar(12) */ 'SN',
		/* codedescript - nvarchar(64) */ N'First School Name',
		/* statusflag - char(1) */ 'L',
		/* sortorder - smallint */ 0,
		/* reference - varchar(12) */ '',
		/* additional - varchar(15) */ '' ) 
		
		