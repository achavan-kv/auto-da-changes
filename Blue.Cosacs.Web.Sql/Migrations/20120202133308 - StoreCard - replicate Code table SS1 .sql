-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

if not exists (select * from codecat where category='SS2')
BEGIN
	
	insert into codecat (origbr, category, catdescript, codelgth, forcenum, forcenumdesc, usermaint, CodeHeaderText, 
	DescriptionHeaderText, SortOrderHeaderText, ReferenceHeaderText, AdditionalHeaderText, ToolTipText, Additional2HeaderText)
	
	select c.origbr, 'SS2', c.catdescript, c.codelgth, c.forcenum, c.forcenumdesc, c.usermaint, c.CodeHeaderText, 
		c.DescriptionHeaderText, c.SortOrderHeaderText, c.ReferenceHeaderText, c.AdditionalHeaderText, c.ToolTipText, c.Additional2HeaderText
	from codecat c where c.category='SS1'
	
	insert into code (origbr, category, code, codedescript, statusflag, sortorder, reference, additional, Additional2)
	
	select origbr, 'SS2', 'SC'+c.code, 'StoreCard '+ c.codedescript, c.statusflag, c.sortorder, c.reference, c.additional, c.Additional2 
	from code c
	where c.category='SS1' and c.code not in('INS','DFA','SER','NSL','REP')
	and exists (select * from CMStrategy s where c.code=s.strategy)
	
	
END

