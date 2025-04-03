-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here



IF NOT EXISTS (SELECT * FROM codecat WHERE category = 'STCR')
BEGIN
	INSERT INTO codecat (
		origbr,		category,		catdescript,
		codelgth,		forcenum,		forcenumdesc,
		usermaint,		CodeHeaderText,		DescriptionHeaderText,
		SortOrderHeaderText,		ReferenceHeaderText,		AdditionalHeaderText,
		ToolTipText
	) 
	SELECT 	origbr,
		'STCR',		'StoreCard Cancellation Reason',		5,
		forcenum,		forcenumdesc,		usermaint,
		CodeHeaderText,		DescriptionHeaderText,		SortOrderHeaderText,
		ReferenceHeaderText,		AdditionalHeaderText,		'StoreCard Cancellation Reason'
	FROM codecat WHERE category = 'STCM'


	INSERT INTO code (
		origbr,		category,		code,
		codedescript,		statusflag,		sortorder,
		reference,		additional
	) VALUES ( 0,'STCR',  'Lost',
		/* codedescript - nvarchar(64) */ N'Lost','L',0,
		'','' ) 
		
		
	INSERT INTO code (
		origbr,		category,		code,
		codedescript,		statusflag,		sortorder,
		reference,		additional
	) VALUES ( 0,'STCR',  'Stol',
		/* codedescript - nvarchar(64) */ N'Stolen','L',0,
		'','' ) 
		
		
	INSERT INTO code (
		origbr,		category,		code,
		codedescript,		statusflag,		sortorder,
		reference,		additional
	) VALUES ( 0,'STCR',  'CANC',
		/* codedescript - nvarchar(64) */ N'Cancelled - customer not interested ','L',0,
		'','' ) 
		
		
	INSERT INTO code (
		origbr,		category,		code,
		codedescript,		statusflag,		sortorder,
		reference,		additional
	) VALUES ( 0,'STCR',  'Stol',
		/* codedescript - nvarchar(64) */ N'Cancelled - duplicate card','L',0,
		'','' ) 
END 


