update Config.Setting
set ValueText = REPLACE(CAST(ValueText AS VARCHAR(max)), 'Cosacs' + Char(10), '')
WHERE id = 'Fascia'