-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS(select * from CodeCat where category = 'CDM')
BEGIN
    insert into 
        CodeCat (origbr, category, catdescript, codelgth, 
                 forcenum, forcenumdesc, usermaint, 
                 CodeHeaderText, DescriptionHeaderText, SortOrderHeaderText, 
                 ReferenceHeaderText, AdditionalHeaderText, ToolTipText, 
                 Additional2HeaderText)
        select 0, 'CDM', 'Cash Loan Disbursement Method', 2,
               'N', 'N', 'Y', 
               null, null, null, 
               null, null, 'These Disbursement Methods will appear in the Cash Loan Disbursement screen',
               null

END
GO

IF NOT EXISTS(select * from code where category = 'FPM' and Code = '84')
BEGIN
    insert into code(origbr, category, code, codedescript, statusflag, sortorder, reference, additional, Additional2)
    select 0, 'FPM', '84', 'Electronic Bank Transfer', 'L', 0, 0, null, '11'
END
GO

IF NOT EXISTS(select * from code where category = 'CDM')
BEGIN
    insert into code(origbr, category, code, codedescript, statusflag, sortorder, reference, additional, Additional2)
    select 0,'CDM', '1', 'Cash', 'L', 0, 0, null, null
    union
    select 0,'CDM', '2', 'Cheque', 'L', 0, 0, null, null
    union
    select 0,'CDM', '3', 'Credit Card', 'L', 0, 0, null, null
    union 
    select 0,'CDM', '4', 'Debit Card', 'L', 0, 0, null, null
    union
    select 0, 'CDM', '84', 'Electronic Bank Transfer', 'L', 0, 0, null, null
END
GO

