-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from CodeCat where category = 'ASSY')
BEGIN
    insert into CodeCat(origbr, category, catdescript, codelgth, forcenum, forcenumdesc, usermaint, CodeHeaderText, DescriptionHeaderText, SortOrderHeaderText, ReferenceHeaderText, AdditionalHeaderText, ToolTipText, Additional2HeaderText)
    select 0, 'ASSY', 'Assembly Cost Items', 8, 'N', 'N', 'Y', 'Item', 'Description', NULL, 'Category', NULL, 'Items added to this category will need to also have be associated with the products.', NULL
END
GO
