-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
INSERT INTO Code
	(origbr, category, Code, CodeDescript, StatusFlag, sortorder, reference)
SELECT 
	NULL AS origbr,
	'EDC' AS category,
	'FIN.TRAN' AS Code,
	'Export Financial Transaction to Oracle' AS CodeDescript,
	'L' AS StatusFlag,
	ISNULL(MAX(SortOrder), 0) + 1 AS sortorder,
	'0' AS reference
FROM
	code 
WHERE 
	category='EDC'