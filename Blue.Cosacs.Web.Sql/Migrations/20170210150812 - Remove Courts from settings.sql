UPDATE c
SET valuetext = REPLACE(CAST(ValueText AS VARCHAR(MAX)), 'Courts' + CHAR(10), '')
FROM Config.setting c
WHERE id = 'Fascia' 
AND Namespace = 'Blue.Cosacs.Merchandising'