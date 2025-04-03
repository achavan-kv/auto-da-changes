-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS(select * from codecat where category ='STC')
INSERT INTO codecat (origbr, 					 category, 					 catdescript, 
					 codelgth, 					 forcenum, 					 forcenumdesc, 
					 usermaint, 					 CodeHeaderText, 					 DescriptionHeaderText, 
					 SortOrderHeaderText, 					 ReferenceHeaderText,					 AdditionalHeaderText, ToolTipText)
VALUES (0,		'STC',		'Store Card Cancellation',
		1,		'N',		'N',
		'Y',		NULL,		NULL,
		NULL,		NULL,		NULL,
		'Store Card Cancellation Reasons')
IF NOT EXISTS(select * from code where category ='STC' AND code = 'CNI')
	
		INSERT INTO code (
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
			/* category - varchar(12) */ 'STC',
			/* code - varchar(12) */ 'CNI',
			/* codedescript - nvarchar(64) */ N'Customer NOT Interested',
			/* statusflag - char(1) */ 'L',
			/* sortorder - smallint */ 0,
			/* reference - varchar(12) */ '',
			/* additional - varchar(15) */ '' ) 
		
GO 		
		
