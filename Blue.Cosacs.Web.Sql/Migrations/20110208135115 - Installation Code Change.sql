UPDATE dbo.codecat
SET category = 'INSTPRMCHRG',
	catdescript = 'Installation Primary Charge'
WHERE category = 'INSTCHARGE'

UPDATE dbo.code
SET category = 'INSTPRMCHRG'
WHERE category = 'INSTCHARGE'