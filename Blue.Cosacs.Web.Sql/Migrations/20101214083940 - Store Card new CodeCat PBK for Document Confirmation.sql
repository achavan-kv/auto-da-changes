-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

--Add new category for Store Card - Document Confirmation

IF NOT EXISTS(select * from codecat where category ='PBK')
INSERT INTO codecat (origbr, 
					 category, 
					 catdescript, 
					 codelgth, 
					 forcenum, 
					 forcenumdesc, 
					 usermaint, 
					 CodeHeaderText, 
					 DescriptionHeaderText, 
					 SortOrderHeaderText, 
					 ReferenceHeaderText,
					 AdditionalHeaderText, ToolTipText)
VALUES (0,
		'PBK',
		'Proof Bank (CRef)',
		1,
		'N',
		'N',
		'Y',
		NULL,
		NULL,
		NULL,
		NULL,
		NULL,
		'Enter proof of Bank Details displayed under Document Confirmation')
		
		
		
