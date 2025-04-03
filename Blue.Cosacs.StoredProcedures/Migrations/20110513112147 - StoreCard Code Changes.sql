-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

delete FROM code WHERE category = 'stpm' AND code !='CSH'

IF NOT EXISTS (SELECT * FROM codecat WHERE category = 'stcm')
BEGIN
	INSERT INTO codecat (
		origbr,		category,		catdescript,
		codelgth,		forcenum,		forcenumdesc,
		usermaint,		CodeHeaderText,		DescriptionHeaderText,
		SortOrderHeaderText,		ReferenceHeaderText,		AdditionalHeaderText,
		ToolTipText
	) 
	SELECT 	origbr,
		'STCM',		'StoreCard Contact Method',		5,
		forcenum,		forcenumdesc,		usermaint,
		CodeHeaderText,		DescriptionHeaderText,		SortOrderHeaderText,
		ReferenceHeaderText,		AdditionalHeaderText,		'Store Card Contact Method'
	FROM codecat WHERE category = 'stpm'


	INSERT INTO code (
		origbr,		category,		code,
		codedescript,		statusflag,		sortorder,
		reference,		additional
	) VALUES ( 0,'STCM',  'POST',
		/* codedescript - nvarchar(64) */ N'Post','',0,
		'','' ) 
		
		
	INSERT INTO code (
		origbr,		category,		code,
		codedescript,		statusflag,		sortorder,
		reference,		additional
	) VALUES ( 0,'STCM',  'EMAIL',
		/* codedescript - nvarchar(64) */ N'E-Mail','',0,
		'','' ) 
END 


