-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM dbo.codecat WHERE  category = 'INST')
INSERT INTO dbo.codecat (
	Origbr,	category,	catdescript,	codelgth,
	forcenum,	forcenumdesc,	usermaint,	CodeHeaderText,
	DescriptionHeaderText,	SortOrderHeaderText,	ReferenceHeaderText,	AdditionalHeaderText,
	ToolTipText
) VALUES ( 0, 'INST', N'Installation Items',8,
	'N', 'N','Y', 'Item', 
	'Description',NULL,NULL,NULL,
'Items added to this category will need to also have be associated with the products. Then those products which do have installation codes ' + 
'associated with them will appear in the pending installation screen' )
 