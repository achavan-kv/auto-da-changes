
-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

DECLARE @Delimiter	CHAR(5) = '<���>'
DECLARE @string		VARCHAR(4000)

DELETE Service.Resolution

SELECT 
	@string = REPLACE(REPLACE((SELECT CONVERT(VarChar(max), ValueText) FROM Config.Setting where id = 'ServiceResolution'), CHAR(13), ''), CHAR(10), @Delimiter)

;WITH Split(stpos,endpos) 
AS(
    SELECT 
		0 AS stpos, CHARINDEX(@Delimiter, @String) AS endpos
    UNION ALL
    SELECT 
		endpos + len(@Delimiter), CHARINDEX(@Delimiter, @String, endpos + len(@Delimiter))
    FROM 
		Split
    WHERE 
		endpos > 0 
)
INSERT INTO Service.Resolution
SELECT 
    SUBSTRING(@String, stpos, COALESCE(NULLIF(endpos, 0), LEN(@String) + 1) - stpos) AS Description,
	1 AS Fail
FROM 
	Split
WHERE
	ISNULL(endpos, 0) > 0

IF @@ROWCOUNT = 0
BEGIN
	INSERT INTO Service.Resolution
		(Description, Fail)
	VALUES
		('Beyond Economic Repair', 1),
		('Cosmetic Defect', 1),
		('Damage On Delivery', 1),
		('Electrical Defect', 1),
		('Event or terms NOT covered', 1),
		('Hardware', 1),
		('Installation of New Product', 1),
		('Mechanical Defect', 1),
		('Misuse by the customer', 1),
		('No Fault Found', 1),
		('Rusting', 1),
		('Save a Call', 1),
		('Software', 1),
		('Structure', 1),
		('Upholstery Fabric Defect', 1)
END
GO

UPDATE Service.Resolution
	SET Fail = 0
WHERE
	[description] IN ('Save a Call', 'No Fault Found', 'Installation of New Product')
GO

DELETE Config.Setting 
WHERE 
	id = 'ServiceResolution'