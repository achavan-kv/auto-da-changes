-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from codecat where category = 'WDS')
BEGIN
	INSERT INTO CodeCat (origbr, category, catdescript, codelgth, forcenum, forcenumdesc, usermaint, CodeHeaderText, 
						 DescriptionHeaderText,  SortOrderHeaderText, ReferenceHeaderText,  AdditionalHeaderText, ToolTipText)
						 
	SELECT	0, 'WDS', 'Wave Deposit Product Sub Class', 5, 0, 0, 'Y', 'Product Sub Class', 
			'Scoring Band', null, null, null, null
	
END
GO

IF EXISTS(select * from codecat where category = 'WDL')
BEGIN
	UPDATE codecat set CodeHeaderText = 'Product Class'
	WHERE category = 'WDL'
END
GO
	