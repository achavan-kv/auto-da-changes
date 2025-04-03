-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from syscolumns where name = 'Additional2' and object_name(id) = 'Code')
BEGIN
	ALTER TABLE Code ADD Additional2 VARCHAR(15)
END 


IF NOT EXISTS(select * from syscolumns where name = 'Additional2HeaderText' and object_name(id) = 'CodeCat')
BEGIN
	ALTER TABLE CodeCat ADD Additional2HeaderText VARCHAR(30)
END 
GO

UPDATE codecat
SET Additional2HeaderText = 'Shortage/Overage'
WHERE category = 'FPM'


Update codecat
SET ToolTipText = ToolTipText + ' ' + 'Shortage/Overage should be entered as: 1 to allow and 0 to disallow.'
where category = 'FPM'
and ToolTipText not like '%shortage%'


UPDATE code
SET Additional2 = '0'
WHERE Additional2 is null
AND category = 'FPM'




