-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: CR12949 #13715 & 13718


IF NOT EXISTS(select * from CodeCat where category = 'RDYAST')
BEGIN

	INSERT INTO codecat(origbr, category, catdescript, codelgth, forcenum, forcenumdesc, usermaint, CodeHeaderText, DescriptionHeaderText, 
	SortOrderHeaderText, ReferenceHeaderText, AdditionalHeaderText, ToolTipText, Additional2HeaderText)
	SELECT 0, 'RDYAST', 'Ready Assist Items', 18, 'N', 'N', 'N', NULL, NULL, NULL,
	NULL, NULL, NULL, NULL

END

IF NOT EXISTS(select * from code where category = 'RDYAST')
BEGIN
	
	INSERT INTO Code(origbr, category, code, codedescript, statusflag, sortorder, reference, additional, Additional2)
	SELECT 0,  'RDYAST', 'READY1', 'Ready Assist 1', 'L', 0, 0, null, null
	UNION 
	SELECT 0,  'RDYAST', 'READY2', 'Ready Assist 2', 'L', 0, 0, null, null
	UNION 
	SELECT 0,  'RDYAST', 'READY3', 'Ready Assist 3', 'L', 0, 0, null, null

END


IF NOT EXISTS(select * from CodeCat where category = 'RDYCON')
BEGIN

	INSERT INTO codecat(origbr, category, catdescript, codelgth, forcenum, forcenumdesc, usermaint, CodeHeaderText, DescriptionHeaderText, 
	SortOrderHeaderText, ReferenceHeaderText, AdditionalHeaderText, ToolTipText, Additional2HeaderText)
	SELECT 0, 'RDYCON', 'Ready Assist Contracts', 4, 'N', 'N', 'N', NULL, NULL, NULL,
	NULL, NULL, NULL, NULL

END

IF NOT EXISTS(select * from code where category = 'RDYCON')
BEGIN
	
	INSERT INTO Code(origbr, category, code, codedescript, statusflag, sortorder, reference, additional, Additional2)
	SELECT 0,  'RDYCON', 'RDY1', 'ReadyAssistContract', 'L', 0, 0, null, null

END
