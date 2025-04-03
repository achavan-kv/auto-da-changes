-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS(select * from CodeCat where category = 'ANNSERVCONT')
BEGIN
    insert into CodeCat(origbr, category, catdescript, codelgth, forcenum, forcenumdesc, usermaint, CodeHeaderText, DescriptionHeaderText, SortOrderHeaderText, ReferenceHeaderText, AdditionalHeaderText, ToolTipText, Additional2HeaderText)
    select 0, 'ANNSERVCONT', 'Annual Service Contract Items', 8, 'N', 'N', 'Y', 'Item', 'Description', NULL, 'Contract Length', 'Is Full Refund', 'Items added to this category will need to be associated with products.', NULL
END
GO


IF EXISTS(select * from CodeCat where category = 'ASSY')
BEGIN
    update
        codecat
    set
        ToolTipText = 'Items added to this category will need to be associated with products.'
    where
        category = 'ASSY'
END
GO