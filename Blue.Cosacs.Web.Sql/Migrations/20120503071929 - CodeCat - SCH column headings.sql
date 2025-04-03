-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE codecat
	set CodeHeaderText='AcctType',DescriptionHeaderText='Item Number',ReferenceHeaderText='Item Value',AdditionalHeaderText='N/A',Additional2HeaderText='N/A',
	ToolTipText='Charges automatically added when new accounts are created'
where category='SCH'
