UPDATE Service.RequestPart
SET [Source] = 'External'
WHERE
	[Source] IS NULL
	AND PartNumber IS NULL

UPDATE Service.RequestPart
SET [Source] = 'Internal'
WHERE
	[Source] IS NULL
	AND PartNumber IS NOT NULL
